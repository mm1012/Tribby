# Database Implementation Plan

## 🎯 Overview

This document outlines the complete database implementation for Tribby Budgeting App using SQLite with Entity Framework Core migrations.

### Key Design Decisions

1. **Transaction-Envelope Relationship**: Nullable `EnvelopeId` + Type-based validation (supports all 4 transaction types)
2. **SystemCategory Taxonomy**: Dual naming system (user-facing `Name` + system taxonomizer `SystemCategory`)
3. **Balance Reconciliation**: View for verifying account balance integrity
4. **Validation Layer**: Both application (C#) and database (CHECK constraints) validation

---

## 📊 Complete Database Schema

### 1. Users Table

```sql
CREATE TABLE Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    CreatedAt DATE DEFAULT CURRENT_DATE
);
```

**Purpose**: User identity management  
**Indexes**: Auto-index on `Id` (primary key)  
**Constraints**: 
- `Name` is NOT NULL and UNIQUE (prevents duplicate user names)

---

### 2. Categories Table

```sql
CREATE TABLE Categories (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    SystemCategory TEXT NOT NULL CHECK(
        SystemCategory IN (
            'Food & Dining',
            'Groceries',
            'Rent/Mortgage',
            'Utilities',
            'Transportation',
            'Entertainment',
            'Health',
            'Education',
            'Clothing',
            'Insurance',
            'Savings/Investments',
            'Uncategorized'
        )
    ),
    CreatedAt DATE DEFAULT CURRENT_DATE
);
```

**Purpose**: Taxonomy for budget categorization  
**Indexes**: Auto-index on `Id` (primary key)  
**Constraints**: 
- `Name` is NOT NULL and UNIQUE (prevents duplicate user-facing names)
- `SystemCategory` has CHECK constraint with predefined values only

#### SystemCategory Usage

| Field | Purpose | Example Values | Who Sees It? |
|-------|---------|----------------|--------------|
| `Name` | User-facing label for budgets | "Weekly Groceries", "Monthly Rent" | User interface |
| `SystemCategory` | System-level classification | "Groceries", "Rent/Mortgage", "Utilities" | Backend logic, reporting, analytics |

**Migration Pattern**: Users can create variations under same system category:
- "Weekly Groceries" → `SystemCategory = "Groceries"`
- "Monthly Groceries" → `SystemCategory = "Groceries"`
- Both allowed because `Name` is UNIQUE but `SystemCategory` can be shared

---

### 3. Accounts Table

```sql
CREATE TABLE Accounts (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL,
    Name TEXT NOT NULL,
    UntrackedBalance REAL DEFAULT 0,
    CreatedAt DATE DEFAULT CURRENT_DATE,
    
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    INDEX idx_accounts_user ON Accounts(UserId)
);
```

**Purpose**: Financial ledger (source of truth for balances)  
**Indexes**: 
- `idx_accounts_user` - Fast lookup of all accounts for a specific user

**Relationships**:
- **One-to-Many**: One User → Many Accounts
- Each Account belongs to exactly one User (via UserId string FK)

---

### 4. Envelopes Table

```sql
CREATE TABLE Envelopes (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL,
    AccountId INTEGER NOT NULL,
    CategoryId INTEGER NOT NULL,
    
    Name TEXT NOT NULL,
    Budget REAL DEFAULT 0,
    SpentAmount REAL DEFAULT 0,
    
    DateCreated DATE DEFAULT CURRENT_DATE,
    IsActive BOOLEAN DEFAULT TRUE,
    
    FOREIGN KEY (AccountId) REFERENCES Accounts(Id),
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
    
    INDEX idx_envelopes_account ON Envelopes(AccountId),
    INDEX idx_envelopes_user ON Envelopes(UserId),
    INDEX idx_envelopes_active ON Envelopes(IsActive, DateCreated DESC)
);
```

**Purpose**: Budget containers with compound growth model  
**Indexes**: 
- `idx_envelopes_account` - Find all envelopes per account
- `idx_envelopes_user` - Find all envelopes for user (across all accounts)
- `idx_envelopes_active` - Active budgets sorted by creation date

**Relationships**:
- **One-to-Many**: One Account → Many Envelopes (one envelope per category)
- **One-to-Many**: One Category → Many Envelopes (same category reused)
- Each Envelope belongs to exactly one Account and one Category

**Design Principles**:
- Budgets never reset - compound growth model
- `IsActive` flag tracks current active budgets vs superseded ones
- `UserId` string column for user relationship (consistent with Accounts)

---

### 5. Transactions Table

```sql
CREATE TABLE Transactions (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    
    AccountId INTEGER NOT NULL,
    
    Amount REAL NOT NULL,
    Type TEXT NOT NULL CHECK(
        Type IN ('AddIncome', 'AddExpense', 
                 'TransferEnvelope', 'TransferAccount')
    ),
    
    Date DATE DEFAULT CURRENT_DATE,
    Description TEXT,
    
    EnvelopeId INTEGER,  -- Nullable!
    
    FOREIGN KEY (AccountId) REFERENCES Accounts(Id),
    FOREIGN KEY (EnvelopeId) REFERENCES Envelopes(Id),
    
    INDEX idx_transactions_account ON Transactions(AccountId),
    INDEX idx_transactions_date ON Transactions(Date DESC),
    INDEX idx_transactions_envelope ON Transactions(EnvelopeId)
);
```

**Purpose**: Audit log for all money movements  
**Indexes**: 
- `idx_transactions_account` - Find all transactions per account
- `idx_transactions_date` - Recent transactions, sorted by date (DESC)
- `idx_transactions_envelope` - Find transactions per envelope

#### Transaction Types & Envelope Requirements

| Type | Requires EnvelopeId? | Purpose | Example |
|------|---------------------|---------|---------|
| `AddIncome` | ✅ YES | Credit to account + add to envelope budget | Add $200 to Groceries envelope |
| `AddExpense` | ✅ YES | Debit from envelope, credit to account | Spend $45.99 from Groceries |
| `TransferEnvelope` | ❌ NO | Internal transfer between envelopes | Transfer -$100 from Food to Dining |
| `TransferAccount` | ❌ NO | Direct account-to-account movement | Transfer -$2,000 from Checking to Savings |

#### Validation Constraint (Database Layer)

```sql
-- Optional: Add additional constraint for envelope requirement
CHECK(
    (Type IN ('AddIncome', 'AddExpense') AND EnvelopeId IS NOT NULL) OR
    (Type IN ('TransferEnvelope', 'TransferAccount') AND EnvelopeId IS NULL)
)
```

**Note**: This CHECK constraint enforces that:
- AddIncome and AddExpense MUST have a valid EnvelopeId
- TransferEnvelope and TransferAccount MUST NOT have an EnvelopeId

---

## 📊 Balance Reconciliation View

```sql
CREATE VIEW AccountBalanceView AS
SELECT 
    a.Id,
    a.UserId,
    a.Name AS AccountName,
    a.UntrackedBalance,
    
    -- Sum of all budget additions (AddIncome with envelope)
    COALESCE(SUM(
        CASE WHEN t.Type = 'AddIncome' AND t.EnvelopeId IS NOT NULL
             THEN t.Amount ELSE 0 END
    ), 0) AS TotalBudgetAdded,
    
    -- Sum of all expenses (AddExpense with envelope)
    COALESCE(SUM(
        CASE WHEN t.Type = 'AddExpense' AND t.EnvelopeId IS NOT NULL
             THEN ABS(t.Amount) ELSE 0 END
    ), 0) AS TotalBudgetSpent,
    
    -- Current active envelope budgets for this account
    COALESCE((SELECT SUM(e.Budget) 
               FROM Envelopes e 
               WHERE e.AccountId = a.Id AND e.IsActive = 1), 0) AS ActiveEnvelopeBudgets,
    
    -- Expected total budget (formula from architecture)
    (a.UntrackedBalance + 
     COALESCE((SELECT SUM(e.Budget) 
                FROM Envelopes e 
                WHERE e.AccountId = a.Id AND e.IsActive = 1), 0)) AS ExpectedTotalBudget,
    
    -- Balance integrity check
    CASE 
        WHEN ABS(
            (a.UntrackedBalance + 
             COALESCE((SELECT SUM(e.Budget) 
                       FROM Envelopes e 
                       WHERE e.AccountId = a.Id AND e.IsActive = 1), 0)) -
            (COALESCE(SUM(CASE WHEN t.Type = 'AddIncome' AND t.EnvelopeId IS NOT NULL THEN t.Amount ELSE 0 END), 0) +
             COALESCE(SUM(CASE WHEN t.Type = 'AddExpense' AND t.EnvelopeId IS NOT NULL THEN ABS(t.Amount) ELSE 0 END), 0))
        ) < 0.01 THEN 'BALANCED'
        ELSE 'IMBALANCED'
    END AS BalanceStatus
    
FROM Accounts a
LEFT JOIN Transactions t ON a.Id = t.AccountId
GROUP BY a.Id;

CREATE INDEX idx_accountbalanceview_status ON AccountBalanceView(BalanceStatus);
```

**Purpose**: Verify that `Account.UntrackedBalance + Envelope.Budgets = TotalBudget`  
**Usage**:
- Query all imbalanced accounts: `SELECT * FROM AccountBalanceView WHERE BalanceStatus = 'IMBALANCED'`
- Get healthy accounts only: `SELECT * FROM AccountBalanceView WHERE BalanceStatus = 'BALANCED'`

---

## 📁 Entity Models (C#)

### 1. User.cs

```csharp
public class User
{
    public int Id { get; set; }
    public string Name { get; set; }  // Unique user identifier
}
```

**Purpose**: Simple user identity (no authentication in MVP)  
**Notes**: `Name` is the unique identifier for this MVP (string-based)

---

### 2. Category.cs

```csharp
public class Category
{
    public int Id { get; set; }
    
    public string Name { get; set; }              // User name: "Weekly Groceries"
    public string SystemCategory { get; set; }    // System: "Groceries", "Rent/Mortgage"
    
    public DateTime CreatedAt { get; set; }
}
```

**Purpose**: Taxonomy with dual naming system  
**Notes**: 
- `Name` is user-facing (e.g., "Weekly Groceries")
- `SystemCategory` is for grouping/reporting (e.g., "Groceries")

---

### 3. Account.cs

```csharp
public class Account
{
    public int Id { get; set; }
    public string UserId { get; set; }           // User name
    
    public decimal UntrackedBalance { get; set; }
    
    public DateTime CreatedAt { get; set; }
}
```

**Purpose**: Financial ledger (source of truth)  
**Computed Property**: `TotalBudget = UntrackedBalance + Sum(Active Envelope Budgets)`

---

### 4. Envelope.cs

```csharp
public class Envelope
{
    public int Id { get; set; }
    public string UserId { get; set; }
    
    public int AccountId { get; set; }            // Links to ONE account
    public int CategoryId { get; set; }
    
    public string Name { get; set; }              // "Weekly Groceries"
    public decimal Budget { get; set; }           // Total budget (compound)
    public decimal SpentAmount { get; set; }      // Cumulative spending
    
    public DateTime DateCreated { get; set; }     // When this budget was added
    public bool IsActive { get; set; } = true;    // Current active budgets
}
```

**Purpose**: Budget container with compound growth model  
**Notes**: 
- `IsActive` flag tracks current active budgets
- Compound model: budgets never reset, only grow

---

### 5. Transaction.cs

```csharp
public class Transaction
{
    public int Id { get; set; }
    
    public int AccountId { get; set; }
    public decimal Amount { get; set; }           // Positive = out, Negative = in
    
    public string Type { get; set; }              // Enum values below
    
    public DateTime Date { get; set; }            // Date only (no time)
    public string Description { get; set; }
    
    public int? EnvelopeId { get; set; }          // Nullable - required for AddIncome/AddExpense
}

public enum TransactionType
{
    AddIncome,              // Requires EnvelopeId
    AddExpense,             // Requires EnvelopeId
    TransferEnvelope,       // Does NOT require EnvelopeId
    TransferAccount         // Does NOT require EnvelopeId
}
```

**Purpose**: Audit log for all money movements  
**Notes**: 
- `Amount` sign is explicit (positive = out, negative = in)
- `EnvelopeId` is nullable but constrained by Type validation

---

## 🛠️ Validation Layer Strategy

### Application Layer (C# Code)

```csharp
public static class TransactionBuilder
{
    public static async Task<Transaction> CreateAddIncomeAsync(
        DbContext context,
        Account account,
        Envelope envelope,
        decimal amount,
        DateTime date,
        string description)
    {
        if (envelope == null)
            throw new InvalidOperationException("AddIncome requires an EnvelopeId");
        
        return CreateTransaction(
            account.Id, "AddIncome", amount, date, description, envelope.Id);
    }

    public static async Task<Transaction> CreateAddExpenseAsync(
        DbContext context,
        Account account,
        Envelope envelope,
        decimal amount,
        DateTime date,
        string description)
    {
        if (envelope == null)
            throw new InvalidOperationException("AddExpense requires an EnvelopeId");
        
        return CreateTransaction(
            account.Id, "AddExpense", amount, date, description, envelope.Id);
    }

    public static async Task<Transaction> CreateTransferEnvelopeAsync(
        DbContext context,
        Account account,
        decimal amount,
        DateTime date,
        string description)
    {
        return CreateTransaction(account.Id, "TransferEnvelope", amount, date, description, null);
    }

    public static async Task<Transaction> CreateTransferAccountAsync(
        DbContext context,
        Account account,
        decimal amount,
        DateTime date,
        string description)
    {
        return CreateTransaction(account.Id, "TransferAccount", amount, date, description, null);
    }

    private static Transaction CreateTransaction(
        int accountId, string type, decimal amount, DateTime date, 
        string description, int? envelopeId = null)
    {
        var transaction = new Transaction
        {
            AccountId = accountId,
            Type = type,
            Amount = amount,
            Date = date,
            Description = description,
            EnvelopeId = envelopeId
        };
        
        return transaction;
    }
}
```

### Database Layer (CHECK Constraints)

```sql
-- In Transactions table:
CREATE TABLE Transactions (
    ...
    Type TEXT NOT NULL CHECK(
        Type IN ('AddIncome', 'AddExpense', 
                 'TransferEnvelope', 'TransferAccount')
    ),
    
    -- EnvelopeId constrained by Type
    EnvelopeId INTEGER REFERENCES Envelopes(Id),
    
    CHECK(
        (Type IN ('AddIncome', 'AddExpense') AND EnvelopeId IS NOT NULL) OR
        (Type IN ('TransferEnvelope', 'TransferAccount') AND EnvelopeId IS NULL)
    ),
    ...
);
```

---

## 🚀 Migration Execution Plan

### Step 1: Clean Up Old Infrastructure

```powershell
# Delete old migration files
Remove-Item "Tribby.Core\Migrations" -Recurse -Force

# Delete any existing .db files
Get-ChildItem -Path "C:\Dev\Personal\Tribby" -Filter "*.db" -Recurse | Remove-Item -Force
```

### Step 2: Update DbContext Files

```csharp
// TribbyContext.cs - Update to use new entities
public class TribbyDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }     // NEW
    public DbSet<Category> Categories { get; set; }  // NEW
    public DbSet<Envelope> Envelopes { get; set; }   // NEW
    public DbSet<Transaction> Transactions { get; set; }
    
    // Remove old DBSet properties (Group, Share, EnumShareType)
    
    public string DbPath { get; } 
    
    public TribbyDbContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = Path.Join(path, "tribby.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Account relationships
        builder.Entity<Account>()
            .HasOne(a => a)
            .WithMany()
            .HasForeignKey(a => a.UserId);

        // Envelope relationships
        builder.Entity<Envelope>()
            .HasOne(e => e.Account)
            .WithMany()
            .HasForeignKey(e => e.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Envelope>()
            .HasOne(e => e.Category)
            .WithMany()
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Transaction relationships
        builder.Entity<Transaction>()
            .HasOne(t => t.Account)
            .WithMany()
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Transaction>()
            .HasOne(t => t.Envelope)
            .WithMany()
            .HasForeignKey(t => t.EnvelopeId)
            .OnDelete(DeleteBehavior.SetNull);  // Allow transfer without envelope

        // User relationship (string-based FK for simplicity)
        builder.Entity<Account>()
            .HasOne(a => a.User)  // Virtual property if needed
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### Step 3: Add Migration for Initial Schema

```powershell
cd C:\Dev\Personal\Tribby\Tribby.Core
dotnet ef migrations add InitialCreate --context TribbyDbContext
```

### Step 4: Add Seed Data Migration

```powershell
dotnet ef migrations add SeedCategories --context TribbyDbContext
```

**Seed Categories SQL:**
```sql
INSERT INTO Categories (Name, SystemCategory) VALUES
('Weekly Groceries', 'Groceries'),
('Monthly Rent', 'Rent/Mortgage'),
('Electric Bill', 'Utilities'),
('Food & Dining Out', 'Dining'),
('Gas & Transport', 'Transportation'),
('Netflix Subscription', 'Entertainment'),
('Doctor Visit', 'Health'),
('Student Tuition', 'Education'),
('Winter Jacket', 'Clothing'),
('Life Insurance', 'Insurance'),
('Emergency Fund', 'Savings/Investments');
```

### Step 5: Apply Migrations to Database

```powershell
dotnet ef database update --context TribbyDbContext
```

### Step 6: Verify Schema Created Correctly

```powershell
dotnet ef database show --context TribbyDbContext
```

---

## 🧪 Testing & Validation

### Unit Tests for Transaction Validation

```csharp
public class TransactionValidationTests
{
    [Fact]
    public async Task AddIncome_WithoutEnvelope_ShouldThrow()
    {
        var context = new TribbyDbContext();
        await context.Database.EnsureCreatedAsync();
        
        var account = await CreateAccountAsync(context);
        
        // Should throw: "AddIncome requires an EnvelopeId"
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await TransactionBuilder.CreateAddIncomeAsync(
                context, account, null, 200m, DateTime.Today, "Test"));
    }

    [Fact]
    public async Task TransferAccount_WithEnvelope_ShouldThrow()
    {
        var context = new TribbyDbContext();
        await context.Database.EnsureCreatedAsync();
        
        var account = await CreateAccountAsync(context);
        var envelope = await CreateEnvelopeAsync(context, account);
        
        // Should throw: "TransferAccount should not have EnvelopeId"
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await TransactionBuilder.CreateTransferAccountAsync(
                context, account, -500m, DateTime.Today, "Transfer", 1));
    }

    [Fact]
    public async Task TransferEnvelope_WithoutEnvelope_ShouldSucceed()
    {
        var context = new TribbyDbContext();
        await context.Database.EnsureCreatedAsync();
        
        var account = await CreateAccountAsync(context);
        
        // Should succeed - no envelope needed for internal transfers
        var transaction = await TransactionBuilder.CreateTransferEnvelopeAsync(
            context, account, 100m, DateTime.Today, "Internal Transfer");
        
        Assert.NotNull(transaction);
    }
}
```

### Database Integrity Tests

```csharp
public class AccountBalanceIntegrityTests
{
    [Fact]
    public async Task Balance_Reconciliation_ShouldBeBalanced()
    {
        var context = new TribbyDbContext();
        await context.Database.EnsureCreatedAsync();
        
        var account = await CreateAccountAsync(context);
        var envelope = await CreateEnvelopeAsync(context, account);
        
        // Add income to envelope
        await TransactionBuilder.CreateAddIncomeAsync(
            context, account, envelope, 200m, DateTime.Today, "Salary");
        
        // Spend from envelope
        await TransactionBuilder.CreateAddExpenseAsync(
            context, account, envelope, -45.99m, DateTime.Today, "Groceries");
        
        // Check balance view
        var balancedAccounts = await context.AccountBalanceView.ToListAsync();
        
        var accountBalance = balancedAccounts.FirstOrDefault(a => a.Id == account.Id);
        Assert.Equal("BALANCED", accountBalance.BalanceStatus);
    }
}
```

---

## 📋 Summary Table

| Table | Primary Key | Foreign Keys | Indexes | Purpose |
|-------|-------------|--------------|---------|---------|
| Users | Id (Auto) | None | Auto | User identity |
| Categories | Id (Auto) | None | Auto | Taxonomy with SystemCategory |
| Accounts | Id (Auto) | UserId → Users | idx_accounts_user | Financial ledger |
| Envelopes | Id (Auto) | AccountId, CategoryId | idx_envelopes_account, idx_envelopes_user, idx_envelopes_active | Budget containers |
| Transactions | Id (Auto) | AccountId, EnvelopeId | idx_transactions_account, idx_transactions_date, idx_transactions_envelope | Audit log |

---

## ⚠️ Key Design Principles

1. **Accounts = Source of Truth** - All balances reconcile to account totals
2. **Envelope Budgets Never Reset** - Compound growth model
3. **Balance Always Reconciles** - Account.TotalBudget always matches sum
4. **Negative Amounts Explicit** - User specifies sign in transactions
5. **One Envelope Per Account Link** - Simplifies tracking and reporting
6. **Nullable + Type Validation** - Transaction-Envelope relationship with rules enforced by Type field

---

## 🔮 Future Enhancements (Not in MVP)

1. **Multi-user Support**: Add authentication, user accounts table
2. **Scheduled Transactions**: Recurring income/expenses
3. **Analytics Dashboard**: Charts for spending by category, budget vs actual
4. **Export Formats**: CSV, JSON, PDF reports
5. **Budget Notifications**: Alerts when approaching budget limit

---

*Last Updated: 2026-08-31*

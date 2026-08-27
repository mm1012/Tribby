# Tribby Budgeting App - Overview

## 🏷️ Naming Convention Note
**Backend (Core/API)**: Keep current names (`Account`, `Envelope`, `UntrackedBalance`, `TotalBudget`) for consistency with database schema and existing code conventions.  
**Frontend (Console/Web)**: Use intuitive user-facing names (`Wallet`, `Bucket`, `FreeBalance`, `AvailableBalance`). Add mapping layer or DTO transformation between backend and frontend.

## 📋 Core Architecture Summary

```
Account (Source of Truth) ↔ Envelope (Budget Container)

1 Account can have → Many Envelopes
1 Envelope links to → 1 Account only

Balance Equation:
Account.Balance = Sum(Untracked Amount + All Active Envelope Budgets)
```

---

## 🗂️ Entity Data Model

### Account (Source of Truth)
```csharp
public class Account
{
    public int Id { get; set; }
    public string UserId { get; set; }           // User name
    
    // The accounting heart:
    public decimal UntrackedBalance { get; set; } // Money not assigned to envelopes
    public decimal TotalBudget => UntrackedBalance + Envelopes.Sum(e => e.Budget);
    
    public DateTime CreatedAt { get; set; }
}
```

### Envelope (Budget Allocation)
```csharp
public class Envelope
{
    public int Id { get; set; }
    public string UserId { get; set; }           // User name for the budget
    
    // Relationships:
    public int AccountId { get; set; }            // Links to ONE account
    
    // Budget tracking:
    public int CategoryId { get; set; }
    public string Name { get; set; }             // "Weekly Groceries"
    
    public decimal Budget { get; set; }           // Total budget (compound)
    public decimal SpentAmount { get; set; }     // Cumulative spending
    
    // Never resets - only grows!
    public DateTime DateCreated { get; set; }     // When this budget was added
}
```

### Transaction (The 4 Core Types)
```csharp
public class Transaction
{
    public int Id { get; set; }
    
    // Which account touched:
    public int AccountId { get; set; }            // Money moves through this account
    
    public decimal Amount { get; set; }           // Positive = out, Negative = in
    
    public string Type { get; set; }              // Enum values below
    
    public DateTime Date { get; set; }            // Date only (no time)
    public string Description { get; set; }       // Merchant/notes
    
    // Optional envelope reference:
    public int? EnvelopeId { get; set; }          // For Income type
}

public enum TransactionType
{
    AddIncome,              // Credit to account + add to envelope budget
    AddExpense,             // Debit from envelope, credit to account
    TransferEnvelope,       // Between two envelopes
    TransferAccount         // Between two accounts (untracked balance)
}
```

### Category System
```csharp
public class Category
{
    public int Id { get; set; }
    
    public string Name { get; set; }              // User name: "Weekly Groceries"
    public string SystemCategory { get; set; }    // "Food", "Groceries"
}

// MVP System Categories:
- Food & Dining (default: "Uncategorized")
- Groceries (default: "Uncategorized")  
- Rent/Mortgage (default: "Uncategorized")
- Utilities (default: "Uncategorized")
- Transportation (default: "Uncategorized")
- Entertainment (default: "Uncategorized")
- Health (default: "Uncategorized")
- Education (default: "Uncategorized")
- Clothing (default: "Uncategorized")
- Insurance (default: "Uncategorized")
- Savings/Investments (default: "Uncategorized")
```

---

## 💰 Balance Flow Examples

### Example 1: Add Income
```
Before:
Account.Balance = $5,000 (all untracked)
Envelope.Groceries.Budget = $0, Spent = $0

User Action: Add Income +$200 to Groceries envelope
─────────────────────────────
Transaction: Type="AddIncome", Amount=+$200

After:
Account.UntrackedBalance = $4,800 (-$200)
Envelope.Groceries.Budget = $200 (+$200), Spent = $0
Account.TotalBudget = $4,800 + $200 = $5,000 ✓ (balanced)
```

### Example 2: Add Expense
```
Before:
Account.UntrackedBalance = $1,000
Envelope.Groceries.Budget = $200, Spent = $0

User Action: Add Expense -$45.99 to Groceries

After:
Account.UntrackedBalance = $1,045.99 (+$45.99)
Envelope.Groceries.SpentAmount = $45.99
Envelope.Groceries.Budget = $200 (unchanged)
RemainingBudget = $200 - $45.99 = $154.01 ✓
```

### Example 3: Transfer Envelope to Envelope
```
Before:
Envelope.Food.Budget = $500, Spent = $350 → Remaining: $150
Envelope.Dining.Budget = $200, Spent = $50 → Remaining: $150

User Action: Transfer -$100 from Food to Dining

After:
Envelope.Food.SpentAmount = $450 (budget still $500)
Envelope.Food.Remaining = $500 - $450 = $50
Envelope.Dining.Budget = $300 (+$100 added), Spent = $50
Envelope.Dining.Remaining = $300 - $50 = $250
```

### Example 4: Transfer Account to Account
```
Before:
Account.Checking.UntrackedBalance = $5,000
Account.Savings.UntrackedBalance = $1,000

User Action: Transfer -$2,000 from Checking to Savings

After:
Account.Checking.UntrackedBalance = $3,000
Account.Savings.UntrackedBalance = $3,000
```

---

## 🗄️ Database Schema (SQLite)

### Users Table
```sql
CREATE TABLE Users (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE
);
```

### Categories Table
```sql
CREATE TABLE Categories (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL,              -- User-generated name
    SystemCategory TEXT NOT NULL,    -- "Food", "Groceries", etc.
    UNIQUE(Name)                     -- Prevent duplicates
);

-- Insert defaults
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

### Accounts Table
```sql
CREATE TABLE Accounts (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL,
    Name TEXT NOT NULL,              -- "Checking", "Cash on Hand"
    UntrackedBalance REAL DEFAULT 0,
    CreatedAt DATE,                  -- Date only
    
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);

-- Index for performance
CREATE INDEX idx_accounts_user ON Accounts(UserId);
```

### Envelopes Table
```sql
CREATE TABLE Envelopes (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId TEXT NOT NULL,
    AccountId INTEGER NOT NULL,      -- Links to ONE account
    CategoryId INTEGER NOT NULL,
    
    Name TEXT NOT NULL,              -- User budget name
    Budget REAL DEFAULT 0,           -- Total accumulated budget
    SpentAmount REAL DEFAULT 0,      -- Cumulative spending
    
    DateCreated DATE,                -- When this budget was added
    IsActive BOOLEAN DEFAULT TRUE,   -- Current active budgets
    
    FOREIGN KEY (AccountId) REFERENCES Accounts(Id),
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);

-- Indexes
CREATE INDEX idx_envelopes_account ON Envelopes(AccountId);
CREATE INDEX idx_envelopes_user ON Envelopes(UserId);
CREATE INDEX idx_envelopes_active ON Envelopes(IsActive, DateCreated DESC);
```

### Transactions Table
```sql
CREATE TABLE Transactions (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    AccountId INTEGER NOT NULL,
    
    Amount REAL NOT NULL,            -- Positive = out, Negative = in
    Type TEXT NOT NULL,              -- Enum: AddIncome, AddExpense, TransferEnvelope, TransferAccount
    
    Date DATE DEFAULT CURRENT_DATE,  -- Date only (no time)
    Description TEXT,                -- Merchant/notes
    
    EnvelopeId INTEGER,              -- Nullable for income type
    
    FOREIGN KEY (AccountId) REFERENCES Accounts(Id),
    FOREIGN KEY (EnvelopeId) REFERENCES Envelopes(Id)
);

-- Indexes
CREATE INDEX idx_transactions_account ON Transactions(AccountId);
CREATE INDEX idx_transactions_date ON Transactions(Date DESC);
CREATE INDEX idx_transactions_envelope ON Transactions(EnvelopeId);
```

---

## 📁 Project Structure (MVP)

```
Tribby.Core/
├── Models/
│   ├── Account.cs
│   ├── Envelope.cs
│   ├── Transaction.cs
│   ├── Category.cs
│   └── User.cs
├── Enums/
│   ├── TransactionType.cs
│   └── CategoryType.cs (optional, if enum needed)
├── Interfaces/
│   └── IDatabaseHandler.cs
└── Handlers/
    └── SqliteDbHandler.cs

Tribby.Api/
├── Controllers/
│   ├── AccountsController.cs
│   ├── EnvelopesController.cs
│   ├── TransactionsController.cs
│   └── BudgetsController.cs (analytics)
├── Models/           # DTOs for API
└── Program.cs        # Web API host

Tribby.Console/
├── ConsoleApp.cs     # Main console application
├── Menu.cs           # Interactive menu
└── Options.cs        # User input handling

Dockerfile
docker-compose.yml    # (for later, optional for MVP)
```

---

## 🛠️ Core CRUD Operations

### AccountsHandler
- `CreateAccount(string userId, string name, decimal initialBalance)`
- `GetAccountsByUserId(string userId)`
- `GetAccountById(int accountId)`
- `UpdateUntrackedBalance(int accountId, decimal change)`

### EnvelopesHandler
- `CreateEnvelope(string userId, int accountId, int categoryId, 
                  string name, decimal budget, DateTime dateCreated)`
- `GetEnvelopesByAccountId(int accountId)`
- `GetEnvelopeById(int envelopeId)`
- `UpdateSpentAmount(int envelopeId, decimal spent)`

### TransactionsHandler
- `CreateTransaction(int accountId, string type, decimal amount, 
                     DateTime date, string description, int? envelopeId = null)`
- `GetTransactionsByAccountId(int accountId)`
- `GetTransactionsByEnvelopeId(int envelopeId)`
- `GetAllTransactions()` (for history view)

---

## 🎯 Console Menu Flow (MVP)

```
Tribby Budgeting App v1.0
─────────────────────────

[Current Accounts]
  [1] Checking - $5,200.00 (Untracked: $5,000 | Envelopes: +$200 budget)
  [2] Cash on Hand - $1,500.50 (Untracked: $1,500.50)

Menu Options:
a) Create Account
b) View All Envelopes & Budgets
c) Add Income (select account + envelope to credit)
d) Add Expense (select envelope)
e) Transfer Envelope to Envelope
f) Transfer Account to Account
g) Transaction History
h) Export Data (JSON/CSV)
exit

─────────────────────────
```

---

## 🔮 Future Iterations (Not in MVP)

1. **Multi-user support** - Add authentication, user accounts table
2. **Scheduled transactions** - Recurring income/expenses
3. **Analytics dashboard** - Charts: spending by category, budget vs actual
4. **Export formats** - CSV, JSON, PDF reports
5. **Web/Mobile apps** - API endpoints, mobile SDKs
6. **Budget notifications** - Alerts when approaching budget limit

---

## 📋 Pre-Implementation Checklist

Before we start coding:

1. ✅ **Compound budget confirmed** (never resets)
2. ✅ **Negative amounts explicit** (user enters -$45.99)
3. ✅ **Income goes TO selected account**
4. ✅ **Accounts created first, then envelopes**
5. ✅ **"Untracked" balance concept understood**
6. ✅ **Date-only transactions (no time)**
7. ✅ **System categories with user-generated names**

---

## ⚠️ Key Design Principles

1. **Accounts = Source of Truth** - All balances reconcile to account totals
2. **Envelope Budgets Never Reset** - Compound growth model
3. **Balance Always Reconciles** - Account.TotalBudget always matches sum
4. **Negative Amounts Explicit** - User specifies sign, no assumptions
5. **One Envelope Per Account Link** - Simplifies tracking and reporting

---

*Last Updated: 2026-07-31*

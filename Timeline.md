# Tribby Development Timeline

## Overview
**Estimated Duration**: 7 weeks  
**Time Commitment**: ~4 hours/week (2 weekday sessions × 2 hours)  
**Approach**: Models → Database → Business Logic  

---

## Phase 1: Core Entity Models & Relationships (Weeks 1-2)

### Week 1, Session 1 (2 hours): Complete Model Structure
**Goal**: Add navigation properties, enums, and DbContext foundation

#### Tasks:
1. **Enhance existing models** with navigation properties
   - Account → Envelopes (collection)
   - User → Accounts, Envelopes, Transactions
   - Envelope → Category reference
   
2. **Create TransactionType enum** in `Tribby.Core/Enums/`
   ```csharp
   public enum TransactionType {
       AddIncome = 1,
       AddExpense = 2,
       TransferEnvelope = 3,
       TransferAccount = 4
   }
   ```

3. **Add computed properties** to models
   - Account.TotalBudget (computed)
   - Envelope.RemainingBudget (computed)
   
4. **Update TribbyContext**
   - Add DbSet<T> for all entities
   - Configure relationships in OnModelCreating()
   - Set up foreign key constraints

#### Deliverable:
- All model classes complete with navigation properties
- TransactionType enum created
- TribbyContext configured with all DbSets and relationships

---

### Week 1, Session 2 (2 hours): Database Infrastructure
**Goal**: Create migration schema and seed data

#### Tasks:
1. **Create initial migration**
   ```powershell
   Add-Migration InitialCreate
   Update-Database
   ```

2. **Seed default categories** from ArchitecturePlans.md
   - Food & Dining, Groceries, Rent/Mortgage, Utilities
   - Transportation, Entertainment, Health, Education
   - Clothing, Insurance, Savings/Investments
   
3. **Configure indexes** for performance:
   - Accounts(UserId)
   - Envelopes(AccountId, UserId, IsActive)
   - Transactions(AccountId, Date, EnvelopeId)

4. **Test database initialization**:
   - Verify connection works
   - Check tables created
   - Confirm categories seeded

#### Deliverable:
- Database schema matches ArchitecturePlans.md
- Default categories loaded
- Connection and queries working

---

### Week 2, Session 1 (2 hours): Category System Enhancement
**Goal**: Add category management and validation

#### Tasks:
1. **Create CategoryService** interface and implementation
   - Get all categories
   - Get category by ID/name
   - Add custom category (user-defined)
   
2. **Add validation logic**
   - Prevent duplicate system categories
   - Enforce unique user-generated names
   
3. **Implement category queries**
   - Get categories by SystemCategory
   - Get active categories only

4. **Update TribbyContext** for category relationships
   - Ensure foreign keys configured correctly

#### Deliverable:
- Category management complete
- Validation rules in place
- CategoryService ready for use

---

### Week 2, Session 2 (2 hours): Repository Pattern Setup
**Goal**: Implement repository abstraction layer

#### Tasks:
1. **Create interfaces** in `Tribby.Core/Interfaces/`
   - IAccountRepository
   - IEnvelopeRepository
   - ITransactionRepository
   - ICategoryRepository
   
2. **Implement repositories** (EF Core based)
   - GetById, GetAll, Add, Update, Delete patterns
   - Query methods with filtering options
   
3. **Create dependency injection setup**
   - Register repositories in Program.cs
   - Wire up to DbContext

#### Deliverable:
- All repository interfaces defined
- Repository implementations complete
- DI configuration ready

---

## Phase 2: Business Logic Implementation (Weeks 3-5)

### Week 3, Session 1 (2 hours): Account Service
**Goal**: Implement account CRUD operations

#### Tasks:
1. **Create IAccountService interface**
   ```csharp
   interface IAccountService {
       Task<Account> CreateAsync(Account account);
       Task<IEnumerable<Account>> GetAccountsByUserIdAsync(string userId);
       Task<Account> GetAccountByIdAsync(int accountId);
       Task UpdateUntrackedBalanceAsync(int accountId, decimal change);
   }
   ```

2. **Implement AccountService**
   - Use IAccountRepository for data access
   - Validate user exists before creating account
   - Handle balance changes
   
3. **Test basic operations**
   - Create new account
   - Retrieve accounts by user
   - Update balance

#### Deliverable:
- AccountService complete with tests
- Account CRUD operations working

---

### Week 3, Session 2 (2 hours): Envelope Service
**Goal**: Implement envelope management logic

#### Tasks:
1. **Create IEnvelopeService interface**
   ```csharp
   interface IEnvelopeService {
       Task<Envelope> CreateAsync(Envelope envelope);
       Task<IEnumerable<Envelope>> GetEnvelopesByAccountIdAsync(int accountId);
       Task<Envelope> GetEnvelopeByIdAsync(int envelopeId);
       Task UpdateSpentAmountAsync(int envelopeId, decimal spent);
       Task<bool> IsBudgetSufficientAsync(int envelopeId, decimal amount);
   }
   ```

2. **Implement EnvelopeService**
   - Validate account exists and is active
   - Enforce one envelope per account rule
   - Compound budget logic (never resets)
   
3. **Test scenarios**
   - Create envelope with budget
   - Check remaining budget
   - Update spent amount

#### Deliverable:
- EnvelopeService complete
- Budget tracking working correctly

---

### Week 4, Session 1 (2 hours): Transaction Service Part 1
**Goal**: Implement income and expense transactions

#### Tasks:
1. **Create ITransactionService interface**
   ```csharp
   interface ITransactionService {
       Task<Transaction> CreateIncomeAsync(
           int accountId, 
           string type, 
           decimal amount,
           DateTime date,
           string description,
           int? envelopeId = null);
       
       Task<Transaction> CreateExpenseAsync(
           int accountId,
           string type,
           decimal amount,
           DateTime date,
           string description,
           int? envelopeId = null);
   }
   ```

2. **Implement transaction creation**
   - AddIncome: Account.UntrackedBalance--, Envelope.Budget++
   - AddExpense: Account.UntrackedBalance++, Envelope.SpentAmount++
   - Validate sufficient balance before allowing
   
3. **Test both types**
   - Create income transaction
   - Verify balance equation maintained

#### Deliverable:
- Income/expense transactions working
- Balance reconciliation verified

---

### Week 4, Session 2 (2 hours): Transfer Operations
**Goal**: Implement envelope and account transfers

#### Tasks:
1. **Add transfer methods to ITransactionService**
   ```csharp
   Task<Transaction> TransferEnvelopeAsync(
       int fromEnvelopeId,
       int toAccountId, // or new envelope
       decimal amount);
       
   Task<Transaction> TransferAccountAsync(
       int fromAccountId,
       int toAccountId,
       decimal amount);
   ```

2. **Implement Envelope-to-Envelope transfer**
   - Debit source envelope (spent increases)
   - Credit destination (budget increases)
   
3. **Implement Account-to-Account transfer**
   - Move untracked balance between accounts
   - Verify sufficient source balance
   
4. **Test both transfer types**

#### Deliverable:
- TransferEnvelope operation complete
- TransferAccount operation complete
- All operations maintain balance equation

---

### Week 5, Session 1 (2 hours): Query & Analytics Methods
**Goal**: Add reporting and analytics capabilities

#### Tasks:
1. **Create IAnalyticsService interface**
   ```csharp
   interface IAnalyticsService {
       Task<Dictionary<string, decimal>> GetRemainingBudgetsAsync(int accountId);
       Task<Dictionary<string, decimal>> GetSpentAmountsAsync(int accountId);
       Task<decimal> GetTotalBudgetAsync(int accountId);
       Task<IEnumerable<Transaction>> GetTransactionHistoryAsync(
           int accountId, 
           DateTime? startDate = null, 
           DateTime? endDate = null);
   }
   ```

2. **Implement analytics calculations**
   - Remaining budget per envelope
   - Total spent vs total budget
   - Aggregate spending by category
   
3. **Add date range filtering** to transaction queries

4. **Test analytics methods**
   - Verify remaining budget calculations
   - Check transaction history queries

#### Deliverable:
- Analytics service complete
- Query methods working
- Reports and history available

---

### Week 5, Session 2 (2 hours): Business Logic Integration & Refinement
**Goal**: Wire everything together and polish

#### Tasks:
1. **Update BusinessLogic class**
   - Inject all services (Account, Envelope, Transaction, Analytics)
   - Remove direct handler dependencies
   
2. **Add validation layers**
   - Input validation in service methods
   - Error handling and user-friendly messages
   
3. **Test complete workflows**
   - Create account → create envelope → add income → spend money
   - Transfer between envelopes
   - Verify balance at each step

4. **Code cleanup**
   - Add XML documentation to services
   - Ensure consistent error handling
   - Remove unused dependencies

#### Deliverable:
- All services wired into BusinessLogic
- Complete CRUD workflows tested
- Clean, documented codebase

---

## Phase 3: Console Integration & Testing (Weeks 6-7)

### Week 6, Session 1 (2 hours): Console Menu Implementation
**Goal**: Create interactive menu for user operations

#### Tasks:
1. **Create Menu.cs class**
   - Display current accounts with balances
   - Show active envelopes and budgets
   - Present operation options
   
2. **Implement menu handlers**
   - Handler for "Create Account"
   - Handler for "Add Income"
   - Handler for "Add Expense"
   - Handler for "Transfer Envelope"
   - Handler for "Transfer Account"
   - Handler for "View History"

3. **Add user input validation**
   - Parse numeric input
   - Handle invalid selections
   - Display errors gracefully

#### Deliverable:
- Interactive console menu working
- All menu options implemented

---

### Week 6, Session 2 (2 hours): Console Display & Formatting
**Goal**: Enhance output and user experience

#### Tasks:
1. **Format balance displays**
   - Show untracked vs envelope budgets clearly
   - Indicate remaining budget per envelope
   
2. **Create transaction history view**
   - List recent transactions
   - Show date, type, amount, description
   
3. **Add confirmation prompts**
   - Ask before destructive operations
   - Display operation results

4. **Test full console flow**
   - Walk through all menu options
   - Verify output is clear and accurate

#### Deliverable:
- Console app fully interactive
- Output formatted and readable

---

### Week 7, Session 1 (2 hours): Edge Cases & Error Handling
**Goal**: Handle unusual scenarios and validate robustness

#### Tasks:
1. **Test edge cases**
   - Adding $0 income/expense
   - Transferring more than budget allows
   - Creating envelope on inactive account
   - Duplicate category names
   
2. **Add error handling**
   - Catch validation errors
   - Display user-friendly messages
   - Log issues for debugging

3. **Handle balance reconciliation**
   - Ensure all operations maintain balance equation
   - Detect and report inconsistencies

4. **Test boundary conditions**
   - Negative amounts (if allowed)
   - Very large transfers
   - Empty collections

#### Deliverable:
- All edge cases handled
- Error messages user-friendly
- Robust error handling in place

---

### Week 7, Session 2 (2 hours): Final Testing & Documentation
**Goal**: Complete testing cycle and add documentation

#### Tasks:
1. **Run comprehensive test scenarios**
   - Full workflow tests
   - Balance equation verification
   - All CRUD operations
   
2. **Add README with usage guide**
   - How to run the app
   - Sample workflows
   - API reference for services
   
3. **Create sample data script** (optional)
   - Pre-populate with test accounts
   - Create demo envelopes and budgets

4. **Final code review**
   - Check for code smells
   - Ensure consistent style
   - Verify all namespaces referenced

#### Deliverable:
- All features tested and working
- Documentation complete
- Ready for MVP release

---

## Success Criteria

### Week 2 Completion (Models & Database)
- [ ] All model classes with navigation properties
- [ ] TransactionType enum defined
- [ ] DbContext configured with all DbSets
- [ ] Database migration successful
- [ ] Default categories seeded
- [ ] Repositories implemented
- [ ] Connection and queries verified

### Week 5 Completion (Business Logic)
- [ ] IAccountService implemented and tested
- [ ] IEnvelopeService implemented and tested
- [ ] ITransactionService with all operations
- [ ] Balance equation maintained in all operations
- [ ] Analytics service functional
- [ ] Error handling in place

### Week 7 Completion (Integration & Testing)
- [ ] Console menu interactive and responsive
- [ ] All CRUD operations working end-to-end
- [ ] Edge cases handled gracefully
- [ ] Documentation complete
- [ ] Ready for MVP release

---

## Risk Mitigation

| Risk | Mitigation Strategy |
|------|---------------------|
| Models too complex | Break into smaller sessions; review after each |
| Database schema doesn't match requirements | Start with simple migration, iterate as needed |
| Business logic becomes bloated | Keep services thin; delegate to repositories |
| Balance equation breaks | Add assertions in tests; verify after each operation |
| Console app too verbose | Format output carefully; test with sample data first |

---

## Notes & Assumptions

1. **Time estimates** are based on average developer speed; may vary for beginners
2. **"Business logic"** primarily means CRUD operations in this context
3. **Balance equation** must be maintained after every operation
4. **Compound budgets** (never reset) is a core requirement
5. **One envelope per account** rule simplifies tracking
6. **EF Core migrations** should be run after each significant schema change

---

## Quick Start Commands

### After Week 1 Session 1:
```powershell
# Run migration
Add-Migration InitialCreate -Project Tribby.Core
Update-Database -MigrationId InitialCreate -Project Tribby.Core

# Test connection
dotnet run --project Tribby.Console
```

### After Week 2:
```powershell
# Verify repositories work
# Check categories seeded
# Run simple query test
```

### After Week 5:
```powershell
# Full CRUD test
# Verify all services wired
# Test balance reconciliation
```

### After Week 7:
```powershell
# Edge case testing
# Documentation review
# Final verification
```

---

## Next Steps After MVP

Once core functionality is complete (Week 7), consider:

1. **API Layer** - Add Web API controllers for web/mobile clients
2. **Authentication** - Multi-user support with JWT tokens
3. **Scheduled Transactions** - Recurring income/expenses
4. **Analytics Dashboard** - Charts and spending trends
5. **Export Features** - CSV, JSON, PDF reports
6. **Notifications** - Budget alerts and reminders

---

*Last Updated: 2026-08-31*

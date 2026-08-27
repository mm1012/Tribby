# Architecture Design Patterns & Dependency Injection Guide

## Core Concepts

### Design Patterns Purpose
Design patterns provide structured solutions for common architectural problems, enabling:
- **Swappable implementations** without code changes
- **Testable components** through loose coupling
- **Consistent object creation** logic
- **Clean event handling** with minimal dependencies

### Dependency Injection Benefits
Even for small projects, DI provides:
1. **Testability**: Easy to mock external dependencies
2. **Loose coupling**: Components don't create their own dependencies
3. **Swappable implementations**: Change databases/services without code changes
4. **Single responsibility**: Classes only know what they use, not how it's created

---

## Pattern 1: Strategy Pattern

**Purpose**: Encapsulate multiple algorithms/behaviors and make them interchangeable.

**When to use**:
- You have similar operations that differ by implementation
- Need to swap behavior at runtime
- Avoiding long conditional chains (`if type == X do A else if Y do B`)

**Example**:
```csharp
public interface IPaymentProcessor {
    Task<bool> ProcessPayment(decimal amount);
}

// Multiple implementations
public class CreditCardPayment : IPaymentProcessor { ... }
public class PayPalPayment : IPaymentProcessor { ... }

// Inject the strategy
public class ShoppingCart {
    private readonly IPaymentProcessor _processor;
    
    public ShoppingCart(IPaymentProcessor processor) {
        _processor = processor;
    }
}
```

**Benefit**: Change payment method without modifying `ShoppingCart` code.

---

## Pattern 2: Factory Pattern

**Purpose**: Centralize object creation logic and hide implementation details.

**When to use**:
- Object creation involves complex configuration
- Need consistent initialization across the app
- Avoiding multiple constructors with many parameters

**Example**:
```csharp
public class EmailFactory {
    public static IEmail Create(string type, string recipient) {
        return type.ToLower() switch {
            "smtp" => new SmtpEmail(recipient),
            "sendgrid" => new SendGridEmail(recipient),
            _ => throw new ArgumentException("Unknown email type")
        };
    }
}
```

**Benefit**: Change email provider by just passing a different string.

---

## Pattern 3: Observer Pattern

**Purpose**: Automatically notify subscribers when state changes.

**When to use**:
- Multiple components need to react to an event
- Decoupling event emitter from handlers
- Building reactive UI or notification systems

**Example**:
```csharp
public class BalanceChanged : INotification {
    public decimal NewBalance { get; init; }
}

// Subscriber registration
_balanceChangedEvent.Subscribe(
    new AccountUpdateHandler(),
    new TransactionLogWriter()
);
```

**Benefit**: Add/remove handlers without modifying the event source.

---

## Pattern 4: Repository Pattern

**Purpose**: Abstract data access and unify CRUD operations.

**When to use**:
- Multiple data sources (DB, cache, external API)
- Want testable business logic independent of DB
- Need consistent data access patterns across entities

**Example**:
```csharp
public interface IAccountRepository {
    Task<IAccount?> GetByIdAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}

// Implementation details hidden
public class AccountService {
    private readonly IAccountRepository _repository;
    
    public AccountService(IAccountRepository repository) {
        _repository = repository;
    }
}
```

**Benefit**: Swap database implementation without touching business logic.

---

## Quick Decision Guide

| Pattern | Use When... |
|---------|-------------|
| **Strategy** | You need interchangeable behaviors |
| **Factory** | Object creation is complex/configurable |
| **Observer** | Components should react to events |
| **Repository** | Abstracting data access from business logic |

---

## Sources for Further Reading

### Core Pattern Books
1. **GoF Design Patterns** (Gang of Four) - The definitive guide to the 23 classic patterns
   - [Martin Fowler's Design Patterns page](https://martinfowler.com/bliki/DesignPattern.html)

2. **Clean Code** by Robert Martin - Excellent sections on when and why to use patterns
   - [Book site](https://www.cleancode.tech/)

### Pattern-Specific Resources

#### Strategy Pattern
- [Martin Fowler - Strategy](https://martinfowler.com/eaaCatalog/strategy.html)
- [Wikipedia - Strategy Pattern](https://en.wikipedia.org/wiki/Strategy_pattern)

#### Factory Pattern
- [Martin Fowler - Factory](https://martinfowler.com/bliki/FactoryMethod.html)
- [Refactoring.Guru - Factory Method](https://refactoring.guru/design-patterns/factory-method)

#### Observer Pattern
- [Martin Fowler - Observer](https://martinfowler.com/eaaCatalog/observer.html)
- [Google's Event Design Patterns](https://google.github.io/styleguide/event-design/)

#### Repository Pattern
- [Martin Fowler - Repository](https://martinfowler.com/bliki/Repository.html)
- [Clean Architecture Repository](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-repository-pattern.html)

### Dependency Injection Resources

#### Core Concepts
- [Martin Fowler - Inversion of Control](https://martinfowler.com/articles/injection.html) (must-read)
- [Dependency Injection Explained](http://blog.ploeh.dk/2014/09/15/dependency-injection/)

#### When to Use DI
- [Martin Fowler - Dependency Inversion](https://martinfowler.com/bliki/DependencyInjection.html)
- [Dependency Injection in .NET](https://learn.microsoft.com/en-us/dotnet/fundamentals/dependency-injection/)

### Pattern Comparison
- [Refactoring.Guru Design Patterns Guide](https://refactoring.guru/design-patterns) (comprehensive visual guide)
- [Head First Design Patterns](https://www.headfirstdesignpatterns.info/) (beginner-friendly)

---

## Recommended Learning Path

1. Start with: **Martin Fowler's DI article** → understand core concepts
2. Then explore individual pattern pages on Refactoring.Guru for concrete examples
3. Read GoF book for deep theoretical understanding
4. Apply patterns gradually to your specific use cases

---

## Implementation Guidelines for Small Projects

- Use a simple DI container (avoid over-engineering)
- Inject only external dependencies (DB, HTTP clients, logging)
- Keep value objects self-contained (no DI needed)
- Use constructor injection over property/method injection
- Start small: add DI gradually to new components rather than refactoring existing code

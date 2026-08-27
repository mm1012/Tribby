### Tribby
## 📋 Core Architecture Summary

```
Account (Source of Truth) ↔ Envelope (Budget Container)

1 Account can have → Many Envelopes
1 Envelope links to → 1 Account only

Balance Equation:
Account.Balance = Sum(Untracked Amount + All Active Envelope Budgets)
```

### Agent Notes
- Use the md files as guide for understanding what the project is. The md files can be found in the Plans folder. 
- When asked to make changes or decisions always explain you rationale behind the decision.

### Coding Standard
- Shared packages or code goes in Tribby.Core project.
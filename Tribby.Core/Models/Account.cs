public class Account
{
    /// <summary>
    /// The unique identifier of the account.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// The Id of the user that owns this account.
    /// </summary>
    public string UserId { get; set; }
    
    /// <summary>
    /// The budget of the account that has not been assigned to any envelopes.
    /// </summary>
    public decimal UntrackedBalance { get; set; }

    /// <summary>
    /// The total budget of the account, which is the sum of the untracked balance
    /// and the budgets of all envelopes.
    /// </summary>
    public decimal TotalBudget => UntrackedBalance + Envelopes.Sum(e => e.Budget);
}

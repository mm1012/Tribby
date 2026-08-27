public class Transaction
{
    /// <summary>
    /// The unique identifier of the transaction.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Which account this money moved through.
    /// </summary>
    public int AccountId { get; set; }
    
    /// <summary>
    /// Amount: Positive = out, Negative = in.
    /// </summary>
    public decimal Amount { get; set; }
    
    /// <summary>
    /// Transaction type: AddIncome, AddExpense, TransferEnvelope, or TransferAccount.
    /// </summary>
    public string Type { get; set; }
    
    /// <summary>
    /// Date of the transaction (date only, no time).
    /// </summary>
    public DateTime Date { get; set; }
    
    /// <summary>
    /// Merchant or notes for this transaction.
    /// </summary>
    public string Description { get; set; } = "";
    
    /// <summary>
    /// Optional envelope reference (for Income type).
    /// </summary>
    public int? EnvelopeId { get; set; }
}

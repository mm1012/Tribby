public class Share
{
    public int Id { get; set; }

    public int ShareType { get; set; }

    public virtual int UserId { get; set; }

    public decimal Amount { get; set; }

    public List<Transaction> Transactions { get; }
}
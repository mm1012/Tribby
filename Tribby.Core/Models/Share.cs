public class Share
{
    public int Id { get; set; }

    public List<Transaction> TransactionsIds { get; } = new ();
}
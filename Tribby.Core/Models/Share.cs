public class Share
{
    public int Id { get; set; }

    public List<Transaction> Transactions { get; set;} = new ();
}
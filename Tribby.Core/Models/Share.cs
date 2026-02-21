public class Share
{
    public int Id { get; set; }

    public List<Transaction> Transactions { get; set;} = new ();

    public int GroupId { get; set; }
}
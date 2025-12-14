using System.Transactions;

public class Transaction
{
    public int Id { get; set; }

    public string Description { get; set; }

    public decimal Amount { get; set; }

    public virtual int UserId { get; set; }

    public virtual int ShareId { get; set; }

    public List<Share> Shares { get; } = new ();

    public bool IsCleared { get; set;}

}

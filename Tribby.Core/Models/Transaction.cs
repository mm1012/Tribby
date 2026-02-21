public class Transaction
{
    public int Id { get; set; }

    public string Description { get; set; } = "";

    public decimal Amount { get; set; }

    public virtual int UserId { get; set; }

    // public int PayerId { get; set; }

    public virtual int ShareType { get; set; }

    public bool IsCleared { get; set;} = false;

    public int GroupId { get; set; }
}

public class Share
{
    public int Id { get; set; }

    public int ShareType { get; set; }

    public virtual int UserId { get; set; }

    public virtual int TransactionId { get; set; }

    public string Operation { get; set; }
}
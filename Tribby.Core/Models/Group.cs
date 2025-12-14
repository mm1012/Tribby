
public class Group
{
    public string Name { get; set; }

    public int Id { get; set; }

    public decimal Balance { get; set; }

    public List<User> Users { get; } = new ();
}

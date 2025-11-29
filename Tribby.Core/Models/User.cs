public class User {
    
    public string Name { get; set; }

    public int Id { get; set; }

    public int GroupId { get; set;}

    public List<Group> Groups { get; } = new ();

}
public class Member {
    
    public string Name { get; set; }

    public double Balance { get; set; } = 0;


    public Member(string name)
    {
        Name = name;
    }
    
    public void UpdateBalance (double amount)
    {
        Balance += amount;
    }
}
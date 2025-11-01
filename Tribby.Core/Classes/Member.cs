public class Member {
    
    public string Name { get; set; }

    public float Balance { get; set; } = 0;


    public Member(string name)
    {
        Name = name;
    }
    
    public void UpdateBalance (float amount)
    {
        Balance += amount;
    }
}
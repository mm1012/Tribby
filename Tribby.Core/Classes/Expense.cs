namespace Tribby.Core.Classes
{
    public class Expense
    {
        public string Name { get; set; }

        public float Amount { get; set; }

        public string Payee { get; set; }

        public Expense(string name, float amount, string payee)
        {
            Name = name;
            Amount = amount;
            Payee = payee;
        }

    }
}

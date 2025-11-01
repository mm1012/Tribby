namespace Tribby.Core.Classes
{
    public class Transaction
    {
        public string Description { get; set; }

        public float Amount { get; set; }

        public string Payer { get; set; }

        public Transaction(string description, float amount, string payer)
        {
            Description = description;
            Amount = amount;
            Payer = payer;
        }

    }
}

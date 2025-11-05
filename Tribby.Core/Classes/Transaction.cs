using System.Transactions;

namespace Tribby.Core.Classes
{
    public class Transaction
    {
        public string Description { get; set; }

        public double Amount { get; set; }

        public string Payer { get; set; }

        public Transaction[] Link { get; private set; }

        public Transaction (int memberCount)
        {
            Description = string.Empty;
            Payer = string.Empty;
            Amount = 0;

            Link = new Transaction[memberCount];
        }

        public Transaction(string description, double amount, string payer, int memberCount)
        {
            Description = description;
            Amount = amount;
            Payer = payer;

            if (Link == null)
            {
                Link = new Transaction[memberCount];
            }
        }

    }
}

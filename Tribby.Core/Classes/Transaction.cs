using System.Transactions;

namespace Tribby.Core.Classes
{
    public class Transaction
    {
        public string Description { get; set; }

        public float Amount { get; set; }

        public string Payer { get; set; }

        public Transaction[] Link { get; private set; }

        public Transaction(string description, float amount, string payer, int memberCount)
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

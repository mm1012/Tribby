using System.Transactions;

namespace Tribby.Core.Classes
{
    public class Transaction
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public double Amount { get; set; }

        public int UserId { get; set; }

        public int ShareId { get; set; }

    }
}

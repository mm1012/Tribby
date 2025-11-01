using System.Security.Cryptography.X509Certificates;

namespace Tribby.Core.Classes
{
    public class Group
    {
        public string Name { get; set; }


        public static List<string> _members { get; } = new List<string>();

        public static List<Transaction> Transactions { get; private set; } = new List<Transaction>();

        public float Balance { get; set; } = 0;

        public Group(string name)
        {
            if (Transactions == null) {
                Transactions = new List<Transaction>();
            }

            Name = name;
        }
        
        public void AddMember(string member)
        {
            _members.Add(member);
        }
    }
}

using System.Security.Cryptography.X509Certificates;

namespace Tribby.Core.Classes
{
    public class Group
    {
        public string Name { get; private set; }

        public List<string> _members { get; private set; }
        
        public List<Transaction> Transactions { get; private set; }

        //Create hashtable/dictionary to hold transactions for each member(?)

        public double Balance { get; private set; } = 0;

        public Group(string name)
        {
            if (Transactions == null)
            {
                Transactions = new List<Transaction>();
            }
            
            if (_members == null)
            {
                _members = new List<string>();
            }

            Name = name;
        }
        
        public void AddMember(string member)
        {
            _members.Add(member);
        }
    }
}

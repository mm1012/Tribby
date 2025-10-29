using System.Security.Cryptography.X509Certificates;

namespace Tribby.Core.Classes
{
    public class Group
    {
        public string Name { get; set; }


        public static List<string> _members { get; } = new List<string>();

        public static List<Expense> Expenses { get; } = new List<Expense>();

        public Group ()
        {
        }

        public Group(string name)
        {
            // if (Expenses == null) {
            //     Expenses = new List<Expense>();
            // }

            Name = name;
        }
        
        public void AddMember(string member)
        {
            
        }
    }
}

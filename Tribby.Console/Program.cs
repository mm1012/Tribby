
using Tribby.Core.Classes;

Console.WriteLine(" Welcome to Tribby!");
Console.WriteLine("How may I help you?");

Console.WriteLine("[a] Add a transaction");
Console.WriteLine("[b] Update a transaction");
Console.WriteLine("[c] Show group transactions");
Console.WriteLine("[d] Settle a transaction");
Console.WriteLine("[e] Exit`n");
string? input = Console.ReadLine();

Group group = new Group("Babebu Budget Board");
group.AddMember("Matt");
group.AddMember("Levine");

while (input != "e")
{
    switch (input)
    {
        case "a":
            Console.WriteLine("Name of the group: ");
            string? groupName = Console.ReadLine();

            if (!string.IsNullOrEmpty(groupName))
            {
                group = new Group(groupName);
            }

            Console.WriteLine("Input your name: ");
            string? memberName = Console.ReadLine();

            group.AddMember(memberName);

            Console.WriteLine("How many members you wish to add: ");
            string? response = Console.ReadLine();
            int max = 1;
            try
            {
                max = int.Parse(response);
            }
            catch (FormatException)
            {
                Console.WriteLine($"'{response}' is and invalid input");
                Console.WriteLine("You must have at least 2 members in a group");
            }

            for (int i = 0; i < max; i++)
            {
                Console.WriteLine($"Input member no. {i + 1}: ");
                string? member = Console.ReadLine();
                if (string.IsNullOrEmpty(member))
                {
                    Console.WriteLine("Invalid input. Try again.");
                    break;
                }
                group.AddMember(member);
            }
            break;
        case "b":

            break;
        case "c":

            break;
    }

    // Show options
    input = Console.ReadLine();
}


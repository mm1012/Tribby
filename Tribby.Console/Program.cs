
using System.Diagnostics.CodeAnalysis;
using Tribby.Core.Handlers;


var sqliteDb = new SqliteDbHandler();
var options = new Options();

sqliteDb.Connect();

Console.WriteLine("--------  Welcome to Tribby! --------\n");

string groupName = "Babebu Budget Board";
sqliteDb.CreateGroup(groupName);

Group group = await sqliteDb.GetGroupByName(groupName);

if (group == null)
{
    Console.WriteLine("Failed to create or retrieve group.");
    // groupName = options.PromptForGroupName();
    return;
}

sqliteDb.CreateUser("Matt", group.Id);

sqliteDb.CreateUser("Levine", group.Id);

User currentUser = await sqliteDb.GetUserByName("Matt", group.Id);

Console.WriteLine($"-------  {group.Name}  -------\n");
Console.WriteLine("How may I help you?");

options.ShowInitialOptions();
options.Choose(options.GetInput()); 

while (options.Current != options.ExitOption)
{
    switch (options.Current)
    {
        case "a":
            List<Transaction> userTransactions = await sqliteDb.GetUserTransactions(currentUser.Id);

            Console.WriteLine("Who paid for the expense?");
            List<User> users = await sqliteDb.GetUsersInAGroup(group.Id);
            options.DisplayUsers(users);
            int payerId = options.GetIntInput();

            Console.WriteLine("Enter a description for the transaction: ");
            string description = options.GetInput();

            Console.WriteLine("Enter the amount of the transaction: ");
            decimal totalAmount = options.GetDecimalInput();

            Console.WriteLine("")
            
            break;
        case "b":
        
            break;
        case "c":

            break;
        case "debug":
            break;
    }
    options.ShowInitialOptions();
    options.Choose(options.GetInput()); 
}

sqliteDb.CloseConnection();

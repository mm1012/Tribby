
using Tribby.Core.Handlers;


var sqliteDb = new SqliteDbHandler();
var options = new Options();

sqliteDb.Connect();

Console.WriteLine("--------  Welcome to Tribby! --------\n");

List<Group> groups = await sqliteDb.GetGroups();

foreach (var grp in groups)
{
    Console.WriteLine($"[{grp.Id}] {grp.Name}");
}

Console.WriteLine("Enter the Id of your group: ");
int groupId = options.GetIntInput();
// sqliteDb.CreateGroup("Test Group");

Group group = groups.Where(g => g.Id == groupId).First();

if (group == null)
{
    Console.WriteLine($"Failed to retrieve group [{group.Name}].");
    // groupName = options.PromptForGroupName();
    return;
}

// sqliteDb.CreateUser("Matt", group.Id);

// sqliteDb.CreateUser("Levine", group.Id);

User currentUser = await sqliteDb.GetUserByName("Matt", group.Id);

Console.WriteLine($"-------  {group.Name}  -------\n");
Console.WriteLine("How may I help you?\n");

Console.WriteLine($"Current Balance: {group.Balance}\n");

options.ShowInitialOptions();
options.Choose(options.GetInput()); 

while (options.Current != options.ExitOption)
{
    switch (options.Current)
    {
        case "a":
            Console.WriteLine("Who paid for the expense?");
            List<User> users = await sqliteDb.GetUsersInAGroup(group.Id);
            options.DisplayUsers(users);
            int payerId = options.GetIntInput();

            Console.WriteLine("Enter a description for the transaction: ");
            string description = options.GetInput();

            Console.WriteLine("Enter the amount of the transaction: ");
            decimal totalAmount = options.GetDecimalInput();
            
            List<EnumShareType> shareTypes = sqliteDb.GetShareTypes();
            foreach (var shareType in shareTypes)
            {
                Console.WriteLine($"[{shareType.ID}] {shareType.Description}");
            }
            Console.WriteLine("Choose a share type: ");
            int shareTypeId = options.GetIntInput();

            // options.DisplayTransaction();

            // Console.WriteLine("Confirm transaction: ");

            // if (options.Confirm(options.GetInput()))
            // {
            //     var transaction = new Transaction
            //     {
            //         Description = description,
            //         Amount = totalAmount,
            //         UserId = payerId,
            //         IsCleared = false
            //     };

            //     await sqliteDb.CreateTransaction(transaction);
            //     Console.WriteLine("Transaction created successfully.");
            // }
            // else
            // {
                
            // }

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

using Tribby.Core.Enums;

var options = new Options();
var businessLogic = new BusinessLogic();

Console.WriteLine("--------  Welcome to Tribby! --------\n");

List<Group> groups = await businessLogic.GetGroups();

foreach (var grp in groups)
{
    Console.WriteLine($"[{grp.Id}] {grp.Name}");
}

Console.WriteLine("Enter the Id of your group: ");
int groupId = options.GetIntInput();

Group? group = await businessLogic.SelectGroup(groupId);

if (group == null)
{
    Console.WriteLine($"Group with Id [{groupId}] not found.");
    return;
}

List<User> users = await businessLogic.GetUsersFromSelectedGroup();
Console.WriteLine("Select your user Id: ");
options.DisplayUsers(users);
int userId = options.GetIntInput();

User? user = users.FirstOrDefault(u => u.Id == userId);
if (user == null)
{
    Console.WriteLine($"User with Id [{userId}] not found.");
    return;
}
businessLogic.SelectUser(user);


Console.WriteLine($"-------  {group.Name}  -------\n");
Console.WriteLine($"Current Balance: {group.Balance}\n");
options.ShowInitialOptions();
options.Choose(options.GetInput()); 

while (options.Current != options.ExitOption)
{
    switch (options.Current)
    {
        case "a":
            // Console.WriteLine("Who paid for the expense?");
            // Console.WriteLine("Select your user Id: ");
            // options.DisplayUsers(users);
            // int payerId = options.GetIntInput();

            Console.WriteLine("Enter a description for the transaction: ");
            string description = options.GetInput();

            Console.WriteLine("Enter the amount of the transaction: ");
            decimal totalAmount = options.GetDecimalInput();
            
            List<EnumShareType> shareTypes = businessLogic.GetShareTypes();
            foreach (var shareType in shareTypes)
            {
                Console.WriteLine($"[{shareType.ID}] {shareType.Description}");
            }
            Console.WriteLine("Choose a share type: ");
            int shareTypeId = options.GetIntInput();

            switch(shareTypeId)
            {
                case (int)ShareTypes.Equal:
                    businessLogic.SplitEqually(description,totalAmount);
                    break;
                case (int)ShareTypes.Exact:
                    // Logic for exact share
                    break;
                case (int)ShareTypes.Percentage:
                    // Logic for percentage share
                    break;
                case (int)ShareTypes.Shares:
                    // Logic for shares
                    break;
                case (int)ShareTypes.ExactAndSplit:
                    // Logic for exact and split remaining amount.
                    break;
                default:
                    throw new ArgumentException("Invalid share type");
            }

            options.DisplayTransactions(await businessLogic.GetUserTransactions(), user.Name);

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

businessLogic.CloseConnection();

using System.Diagnostics;
using Tribby.Core.Enums;
using Tribby.Core.Handlers;

public class BusinessLogic
{
    private User User { get; set; }

    private Group Group { get; set; }

    private SqliteDbHandler sqliteDb { get; set; }

    public BusinessLogic()
    {
        sqliteDb = new SqliteDbHandler();
    }

    public Task<List<Group>> GetGroups()
    {
        return sqliteDb.GetGroups();
    }

    public async Task<Group?> SelectGroup(int groupId)
    {
        var groups = await sqliteDb.GetGroups();

        if (groups == null)
        {
            Debug.WriteLine($"Failed to retrieve Group [{groupId}].");
            // groupName = options.PromptForGroupName();
            return null;
        }

        Group group = groups.FirstOrDefault(g => g.Id == groupId);

        if (group == null)
        {
            Debug.WriteLine($"Group with Id [{groupId}] not found.");
            // groupName = options.PromptForGroupName();
            return null;
        }

        Group = group;
        return Group;
    }

    public Group GetSelectedGroup()
    {
        return Group;
    }

    public User GetSelectedUser()
    {
        return User;
    }

    public async Task<List<User>> GetUsersFromSelectedGroup()
    {
        if (Group == null)
        {
            Debug.WriteLine("Group is not selected.");
            return new List<User>();
        }
        List<User> users = await sqliteDb.GetUsersInAGroup(Group.Id);

        if (users == null)
        {
            Debug.WriteLine($"Failed to retrieve users for Group [{Group.Id}].");
            return new List<User>();
        }

        Group.Users = users;
        return users;
    }

    public void SelectUser(User user)
    {
        if (user == null)
        {
            Debug.WriteLine($"SelectUser() failed. User is null.");
            return;
        }
        User = user;
    }

    public List<EnumShareType> GetShareTypes()
    {
        return sqliteDb.GetShareTypes();
    }

    private async Task<Transaction> SplitTransaction (int userId, decimal amount, int shareTypeId, int groupId)
    {
        Transaction transaction = new Transaction
        {
            UserId = userId,
            Amount = amount,
            ShareType = shareTypeId,
            GroupId = groupId
        };

        await sqliteDb.CreateTransaction(transaction);

        return transaction;
    }
    
    public async void ProcessShares(int payerId, int shareTypeId, decimal totalAmount)
    {
        List<Transaction> shareTransactions = new List<Transaction>();
        switch(shareTypeId)
        {
            case (int)ShareTypes.Equal:
                int operand = Group.Users.Count;
                decimal equalShare = Math.Ceiling(totalAmount / operand);
                // Logic for equal share
                foreach (var user in Group.Users)
                {
                    var shareTransaction = await SplitTransaction(user.Id, equalShare, shareTypeId, Group.Id);
                    shareTransactions.Add(shareTransaction);
                }
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
    }

    public void CloseConnection()
    {
        sqliteDb.CloseConnection();
    }
}
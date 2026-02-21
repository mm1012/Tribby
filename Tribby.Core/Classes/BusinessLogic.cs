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

    private async Task<Transaction> SplitTransaction (int userId, decimal amount, int shareTypeId, int groupId, string description)
    {
        Transaction transaction = new Transaction
        {
            UserId = userId,
            Amount = amount,
            ShareType = shareTypeId,
            GroupId = groupId,
            Description = description
        };

        await sqliteDb.CreateTransaction(transaction);

        return transaction;
    }
    
    public async void SplitEqually(string description, decimal totalAmount)
    {
        List<Transaction> shareTransactions = new List<Transaction>();

        int operand = Group.Users.Count;
        decimal equalShare = Math.Ceiling(totalAmount / operand);
        // Logic for equal share
        foreach (var user in Group.Users)
        {
            var shareTransaction = await SplitTransaction(
                user.Id, equalShare, 
                (int)ShareTypes.Equal, 
                Group.Id, 
                description);
            shareTransactions.Add(shareTransaction);
        }

        await sqliteDb.CreateShare(new Share
        {
            GroupId = Group.Id,
            Transactions = shareTransactions
        });
    }

    public async Task<List<Transaction>> GetUserTransactions()
    {
        if (User == null)
        {
            Debug.WriteLine("GetUserTransactions() failed. User is not selected.");
            return new List<Transaction>();
        }

        var transactions = await sqliteDb.GetUserTransactions(User.Id);
        return transactions;
    }

    public void CloseConnection()
    {
        sqliteDb.CloseConnection();
    }
}
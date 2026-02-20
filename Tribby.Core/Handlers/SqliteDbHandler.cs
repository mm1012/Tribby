using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Tribby.Core.Handlers
{
    public class SqliteDbHandler : IDatabaseHandler
    {
        private static string _connectionString = $"Data Source={_dbPath};foreign keys=true;";

        private static string dbName = "Tribby.db";

        private static string _dbPath = $"{dbName}";

        public static TribbyDbContext DbContext { get; private set; }

        static SqliteDbHandler()
        {
            var path = @"..\..\..\..\";
            _dbPath = Path.Join(path, "Tribby.db");

            if (DbContext == null)
            {
                DbContext = new TribbyDbContext();

                // DbContext.Database.EnsureCreated();
            }
        }

        public async void Connect()
        {
            DbContext.Database.OpenConnection();
            await DbContext.Database.EnsureCreatedAsync();
        }

        public void Insert()
        {
            throw new NotImplementedException();
        }

        public void Query()
        {
            throw new NotImplementedException();
        }

        public void CloseConnection ()
        {
            DbContext.Dispose();
        }

        public async Task<List<Group>> GetGroups()
        {
            if (DbContext == null)
            {
                return new List<Group>();
            }
            
            var groups = await DbContext.Groups
                .Include(g => g.Users)
                .ToListAsync();
            return groups;
        }

        public async Task<Group?> GetGroupByName(string groupName)
        {
            if (DbContext == null)
            {
                return null;
            }

            var group = await DbContext.Groups
                .Include(g => g.Users)
                .FirstOrDefaultAsync(g => g.Name == groupName);

            return group;
        }

        public async Task<Group?> CreateGroup(string groupName)
        {
            if (await GetGroupByName(groupName) != null)
            {
                Debug.WriteLine($"Group {groupName} already exists.");
                return null;
            }

            var group = new Group
            {
                Name = groupName,
                Balance = 0
            };

            DbContext.Groups.Add(group);
            await DbContext.SaveChangesAsync();

            return group;
        }


        public async void CheckIfGroupExists(string groupName)
        {
            var group = await DbContext.Groups
                .FirstOrDefaultAsync(g => g.Name == groupName);

            if (group != null)
            {
                Debug.WriteLine($"Group {groupName} already exists.");
            }
        }

        public async void CreateUser(string userName, int groupId)
        {
            var user = new User
            {
                Name = userName,
                GroupId = groupId
            };

            if (await CheckIfUserExists(userName, groupId) != null)
            {
                Debug.WriteLine($"User {userName} already exists in group {groupId}.");
                return;
            }

            DbContext.Users.Add(user);
            await DbContext.SaveChangesAsync();
        }

        public async Task<User?> GetUserByName(string userName, int groupId)
        {
            var user = await DbContext.Users
                .FirstOrDefaultAsync(u => u.Name == userName && u.GroupId == groupId);

            return user;
        }

        public async Task<User?> CheckIfUserExists(string userName, int groupId)
        {
            var user = await DbContext.Users
                .FirstOrDefaultAsync(u => u.Name == userName && u.GroupId == groupId);

            return user;
        }

        public async Task<List<User>> GetUsersInAGroup(int groupId)
        {
            var users = await DbContext.Users
                .Where(u => u.GroupId == groupId)
                .ToListAsync();

            return users;
        }

        public List<EnumShareType> GetShareTypes()
        {
            var shareTypes = DbContext.EnumShareTypes
                .ToList();

            return shareTypes;
        }
        
        public async Task<int> GetGroupMemberCount(int groupId)
        {

            var count = await DbContext.Users
                .Where(u => u.GroupId == groupId)
                .CountAsync();
            return count;
        }

        public async Task<List<Transaction>> GetUserTransactions(int userId)
        {
            var transactions = await DbContext.Transactions
                .Where(t => t.UserId == userId)
                .ToListAsync();
            return transactions;
        }

        public async Task<Transaction?> CreateTransaction(Transaction transaction)
        {
            DbContext.Transactions.Add(transaction);
            await DbContext.SaveChangesAsync();

            return transaction;
        }
    }
}
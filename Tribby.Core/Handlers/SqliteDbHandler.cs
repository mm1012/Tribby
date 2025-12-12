using System.Collections.Immutable;
using System.Data.Common;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;

namespace Tribby.Core.Handlers
{
    public class SqliteDbHandler : IDatabaseHandler
    {

        private static string _connectionString = $"Data Source={_dbPath};foreign keys=true;";

        private static string dbName = "Tribby.db";

        private static string _dbPath = $"{dbName}";

        public static TribbyDbContext? DbContext { get; private set; }

        public SqliteDbHandler()
        {
            var path = @"..\..\..\..\";
            _dbPath = Path.Join(path, "Tribby.db");
        }

        public async void Connect()
        {
            if (DbContext == null)
            {
                DbContext = new TribbyDbContext();

                //DbContext.Database.EnsureDeleted();
                DbContext.Database.EnsureCreated();

                // DbContext.Add(new User { Id = 1, Name = "Matt", GroupId = 1});
                // DbContext.Add(new User { Id = 2, Name = "Levine", GroupId = 1});
                // DbContext.Add(new Group { Balance = 0, Id = 1, Name = "BBB"});
                // await DbContext.SaveChangesAsync();
            }
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
            if (DbContext == null)
            {
                return;
            }

            DbContext.Dispose();
            DbContext = null;
        }

        public async Task<List<Group>> GetGroups()
        {
            if (DbContext == null)
            {
                return new List<Group>();
            }
            
            var groups = await DbContext.Groups
                .FromSqlRaw(@"select * from Groups where Id == 1")
                .SingleAsync();
                //             left join Groups g on g.Id = gu.GroupsId")
            
            // var users = await DbContext.Entry(groups)
            //     .Collection(b => b.Users)
            //     .LoadAsync();


            return new List<Group>{groups};
        }
    }
}
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
    }
}
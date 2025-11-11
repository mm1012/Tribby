using Microsoft.EntityFrameworkCore;

namespace Tribby.Core.Handlers
{
    public class SqliteDbHandler : DbContext, IDatabaseHandler
    {

        private static string _connectionString = $"Data Source={_dbPath};foreign keys=true;";

        private static string dbName = "Tribby.db";

        private static string _dbPath = $"{dbName}";

        public SqliteDbHandler()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            _dbPath = Path.Join(path, "Tribby.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite(_connectionString);

        public void Connect()
        {
            throw new NotImplementedException();
        }

        public void Insert()
        {
            throw new NotImplementedException();
        }

        public void Query()
        {
            throw new NotImplementedException();
        }
    }
}
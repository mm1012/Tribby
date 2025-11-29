using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Tribby.Core.Handlers
{
    public class SqliteDbHandler : IDatabaseHandler
    {

        private static string _connectionString = $"Data Source={_dbPath};foreign keys=true;";

        private static string dbName = "Tribby.db";

        private static string _dbPath = $"{dbName}";

        public TribbyDbContext? DbContext { get; private set; }

        public SqliteDbHandler()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            _dbPath = Path.Join(path, "Tribby.db");
        }


        public void Connect()
        {
            if (DbContext == null)
            {
                DbContext = new TribbyDbContext();
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
    }
}
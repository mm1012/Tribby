using Microsoft.Data.Sqlite;

class SqliteDbHandler : IDatabaseHandler {

    private static string _connectionString = "Data Source=Tribby.db;foreign keys=true;";
    
    private static SqliteConnection Connection { private get; private set; }

    public SqliteDbHandler()
    {
        if (Connection == null)
        {
            Connection = new SqliteConnection(_connectionString);

            Connection.Open();

            var command = Connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE Users (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    GroupId 
                );

                INSERT INTO user
                VALUES (1, 'Brice'),
                       (2, 'Alexander'),
                       (3, 'Nate');
            """;
            command.ExecuteNonQuery();

        }
    }
    
    
}
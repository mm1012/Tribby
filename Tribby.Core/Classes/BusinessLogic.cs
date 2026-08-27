using System.Diagnostics;
using Tribby.Core.Enums;
using Tribby.Core.Handlers;

public class BusinessLogic
{
    private User User { get; set; }

    private SqliteDbHandler sqliteDb { get; set; }

    public BusinessLogic()
    {
        sqliteDb = new SqliteDbHandler();
    }

    public void CloseConnection()
    {
        sqliteDb.CloseConnection();
    }
}
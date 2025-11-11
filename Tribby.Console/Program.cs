
using Tribby.Core.Classes;
using Tribby.Core.Handlers;


var sqliteDb = new SqliteDbHandler();
var options = new Options();
sqliteDb.Query();


Console.WriteLine("--------  Welcome to Tribby! --------\n");
Console.WriteLine("-------  Babebu Budget Board  -------\n");
Console.WriteLine("How may I help you?");

options.ShowInitialOptions();
options.Choose(options.GetInput()); 

while (options.Current != options.ExitOption)
{
    switch (options.Current)
    {
        case "a":
            
            break;
        case "b":
        
            break;
        case "c":

            break;
    }
    options.ShowInitialOptions();
    options.Choose(options.GetInput()); 
}

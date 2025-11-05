
using Tribby.Core.Classes;


var options = new Options();

Console.WriteLine("--------  Welcome to Tribby! --------\n");
Console.WriteLine("-------  Babebu Budget Board  -------\n");
Console.WriteLine("How may I help you?");

options.ShowInitialOptions();
options.Choose(options.GetInput()); 

var group = new Group("Babebu Budget Board");
group.AddMember("Matt");
group.AddMember("Levine");

while (options.Current != "e")
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

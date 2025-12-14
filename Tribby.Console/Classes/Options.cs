
public class Options
{

    public string Current { get; private set; }

    public string ExitOption { get; private set; } = "d";

    public Options(string input)
    {
        Current = input;
    }

    public Options()
    {
        Current = string.Empty;
    }

    public void ShowInitialOptions()
    {
        Console.WriteLine("[a] Add a transaction");
        Console.WriteLine("[b] Show individual transactions");
        Console.WriteLine("[c] Settle a transaction");
        Console.WriteLine($"[{ExitOption}] Exit\n");
    }

    public void Choose(string input)
    {
        Current = input;
    }

    public void ViewTransactionOptions()
    {
        Console.WriteLine("[a] Add a transaction");
        Console.WriteLine("[b] Update a transaction");
        Console.WriteLine("[c] Settle a transaction");
        Console.WriteLine("[d] Return \n");
    }

    public void DisplayAddTransactionOptions(int groupMemberCount)
    {
        Console.WriteLine("Input a description for the transaction: ");
        string description = GetInput();
        Console.WriteLine("Input the amount of the transaction: ");
        string amntString = GetInput();
        double amount = 0;
        double.TryParse(amntString, out amount);
        Console.WriteLine("Who paid for the transaction: ");
        string payer = GetInput();

        
        Console.WriteLine("Update transaction: ");
        Console.WriteLine("Continue [y/n(cancel and return to main screen)]: ");
        
    }

    public void DisplayUsers (List<User> users)
    {
        Console.WriteLine("-------  Users in Group  -------\n");
        foreach (var user in users)
        {
            Console.WriteLine($"[{user.Id}] {user.Name}");
        }
        Console.WriteLine();
    }

    public string GetInput()
    {
        return Console.ReadLine() ?? "";
    } 

    public int GetIntInput()
    {
        string input = Console.ReadLine() ?? "0";
        int intInput = 0;
        int.TryParse(input, out intInput);
        return intInput;
    }

    public decimal GetDecimalInput()
    {
        string input = Console.ReadLine() ?? "0.0";
        decimal decInput = 0;
        decimal.TryParse(input, out decInput);
        return decInput;
    }
}
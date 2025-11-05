using System.ComponentModel;
using Tribby.Core.Classes;

public class Options
{

    public string Current { get; private set; }

    public Options(string input)
    {
        Current = input;
    }

    public Options()
    {
        Current = "";
    }

    public void ShowInitialOptions()
    {
        Console.WriteLine("[a] Add a transaction");
        Console.WriteLine("[b] Show individual transactions");
        Console.WriteLine("[c] Settle a transaction");
        Console.WriteLine("[d] Exit\n");
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

    public Transaction AddTransactionDisplay(int groupMemberCount)
    {
        var transaction = new Transaction(groupMemberCount);

        Console.WriteLine("Input a description for the transaction: ");
        string description = GetInput();
        Console.WriteLine("Input the amount of the transaction: ");
        string amntString = GetInput();
        double amount = 0;
        double.TryParse(amntString, out amount);
        Console.WriteLine("Who paid for the transaction: ");
        string payer = GetInput();

        
        

        return transaction;
    }

    public string GetInput()
    {
        
        return Console.ReadLine() ?? "";
    } 
}
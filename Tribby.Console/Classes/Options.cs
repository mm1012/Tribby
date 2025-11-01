public class Options
{

    public string Current { get; private set; }

    public Options(string input)
    {
        Current = input;
    }

    public Options ()
    {
        Current = "";
    }

    public void ShowInitialOptions()
    {
        Console.WriteLine("[a] Add a transaction");
        Console.WriteLine("[b] Show group transactions");
        Console.WriteLine("[c] Settle a transaction");
        Console.WriteLine("[e] Exit\n");
    }

    public void Choice (string input)
    {
        Current = input;
    }
}
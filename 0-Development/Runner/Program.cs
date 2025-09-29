namespace Runner;

// =============================================================
/// <summary>
/// Represents this program.
/// </summary>
internal class Program
{
    /// <summary>
    /// Program entry point.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        Ambient.AddConsoleListener();
        Debug.AutoFlush = true;

        Console.WriteEx(true, ConsoleColor.Green, "{0} ", ["Hello"]);
        Console.WriteLineEx(true, ConsoleColor.Yellow, "World!");
        Debug.WriteLineEx(true, "From debug!");

        Console.ReadKey(TimeSpan.FromSeconds(5));
        Console.WriteLine();

        Console.Write("Press ENTER to exit...");
        Console.ReadLine();
    }
}

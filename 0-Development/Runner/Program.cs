namespace Runner;

// ========================================================
internal class Program
{
    /// <summary>
    /// Program entry point.
    /// </summary>
    static void Main()
    {
        Ambient.AddNewConsoleListener();

        DebugEx.Write(true, ConsoleColor.Green, "Enter value: ");
        var source = (string?)null;
        var done = ConsoleEx.EditLine(true, source, out source);

        ConsoleEx.Write(true, ConsoleColor.Green, "You entered: ");
        ConsoleEx.WriteLine(true, source);
        Console.ReadLine();
    }
}

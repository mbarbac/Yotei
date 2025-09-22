namespace Runner;

// ========================================================
internal class Program
{
    /// <summary>
    /// Program entry point.
    /// </summary>
    static void Main()
    {
        Ambient.AddConsoleListener();

        DebugEx.Write(true, ConsoleColor.Green, "Enter value: ");
        var str = ConsoleEx.ReadLine(true);
    }
}

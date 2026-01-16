namespace Runner;

// ========================================================
/// <summary>
/// This executable program.
/// </summary>
internal class Program
{
    /// <summary>
    /// Program entry point.
    /// </summary>
    static void Main()
    {
        Trace.Listeners.EnsureConsoleListener();
        Debug.IndentSize = 2;
        Debug.AutoFlush = true;

        Console.WriteLine("Hello, World!");
    }
}

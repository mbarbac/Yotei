using static System.ConsoleColor;

namespace Runner;

// ========================================================
/// <summary>
/// Represents this program.
/// </summary>
internal class Program
{
    // ----------------------------------------------------

    /// <summary>
    /// The program entry point.
    /// </summary>
    static void Main()
    {
        // Debug environment...
        DebugEx.IndentSize = 2;
        DebugEx.AutoFlush = true;
        Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));

        DebugEx.WriteLine(Green, "Hi\nDear");
        Console.ReadLine();
    }
}
using static Yotei.Tools.Diagnostics.ConsoleEx;
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
        Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        DebugEx.IndentSize = 2;
        DebugEx.AutoFlush = true;

        var r = EditLine(true, White, Green, TimeSpan.FromSeconds(10), null, out var result);
        WriteLine($"[{r}]: {result}");

        WriteLine(true, "Hi\nDear");
        ReadLine(true);
    }
}
using static System.Console;
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
        var value = "Hello world";
        var comp = StringComparison.OrdinalIgnoreCase;
        var iter = value.SplitEx(comp, true, "hello", "world");
        foreach (var item in iter) WriteLine($"- {item}");

        WriteLine("Hello world!");
        ReadLine();
    }
}

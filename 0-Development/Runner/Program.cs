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
        var source = """
            World

            """;

        var iter = new SpanCharSplitter(source, Environment.NewLine) { KeepSeparators = true };
        foreach (var item in iter)
        {
            var temp = item.SequenceEqual(Environment.NewLine) ? "NL" : item;
            WriteLine($"- '{temp}'");
        }

        //WriteLine("Hello world!");
        ReadLine();
    }
}
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
        string? value = """
            Line one

            Line three

            """;

        var array = Wrappers.TokenizeNL(value);

        WriteLine("Hello world!");
        ReadLine();
    }
}

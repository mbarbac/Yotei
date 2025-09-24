using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
internal class Program
{
    static readonly string FatSeparator = new('*', 50);
    static readonly TimeSpan Timeout = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Program entry point.
    /// </summary>
    static void Main()
    {
        // Debug environment...
        Ambient.AddNewConsoleListener();
        DebugEx.IndentSize = 2;
        DebugEx.AutoFlush = true;

        // Main menu...
        var option = 0; do
        {
            WriteLine(true);
            WriteLine(true, Green, FatSeparator);
            WriteLine(true, Green, "Main Menu:");
            option = new ConsoleMenu
            {
                new("Exit"),
                new("First option", () => WriteLine("1st option selected.")),
                new("Examples", () =>
                {
                })
            }
            .Run(true, Green, Timeout);
        }
        while (option > 0);
    }
}

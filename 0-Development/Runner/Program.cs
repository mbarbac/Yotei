using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;
using Debug = Yotei.Tools.Diagnostics.DebugEx;

namespace Runner;

// =============================================================
/// <summary>
/// Represents this program.
/// </summary>
internal class Program
{
    public static readonly string FatSeparator = new('*', 50);
    public static readonly string SlimSeparator = new('-', 30);
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(60);

    /// <summary>
    /// Program entry point.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        Ambient.AddConsoleListener();
        Debug.IndentSize = 2;
        Debug.AutoFlush = true;

        var options = new ConsoleMenuOptions() with { Debug = true, Timeout = Timeout };
        var position = 0; do
        {
            WriteLine(true);
            WriteLine(true, Green, FatSeparator);
            WriteLine(true, Green, "Main Menu");
            WriteLine(true);

            position = new ConsoleMenu
            {
                new("Exit"),
                new("Manage Artifacts"),
                new("Manage Project Packages"),
                new("Examples", () =>
                {
                }),
            }
            .Run(options, position);
        }
        while (position > 0);
    }
}

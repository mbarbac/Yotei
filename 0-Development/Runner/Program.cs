using static Yotei.Tools.ConsoleEx.ConsoleEx;
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
            option = new MenuConsole
            {
                new MenuEntry("Exit"),
                new MenuEntry("First option", () => WriteLine("1st option selected.")),
                new MenuEntry("Second option", () => WriteLine("2nd option selected.")),
                new MenuEntry("Examples", () =>
                {
                    var item = new DateOnly(1, 1, 1);
                    var str = item.ToString();
                    Write(Cyan, "Value: ");
                    WriteLine(str);
                })
            }
            .Run(true, Green, Timeout, option);
        }
        while (option > 0);
    }
}

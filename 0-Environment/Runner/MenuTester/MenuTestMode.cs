using static Yotei.Tools.ConsoleExtensions;
using static System.Console;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class MenuTestMode(bool breakOnError) : ConsoleMenuEntry
{
    bool BreakOnError { get; } = breakOnError;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string Header() => "Execute Tests";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        var menu = new ConsoleMenu { ToDebug = Program.ToDebug, Timeout = Program.Timeout }
        .Add(new("Exit"))
        .Add(new MenuTester(BreakOnError))
        .Add(new MenuTesterFiltered(BreakOnError));

        var position = 0; do
        {
            WriteLineEx(true);
            WriteLineEx(true, Green, Program.FatSeparator);
            WriteLineEx(true, Green, Header());
            WriteLineEx(true);
            position = menu.Run(0);
        }
        while (position > 0);
    }
}
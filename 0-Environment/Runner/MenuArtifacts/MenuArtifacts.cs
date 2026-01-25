using static Yotei.Tools.ConsoleExtensions;
using static System.Console;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class MenuArtifacts : ConsoleMenuEntry
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string Header() => "Manage Artifacts";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        var menu = new ConsoleMenu { ToDebug = Program.ToDebug, Timeout = Program.Timeout }
        .Add(new("Exit"))
        .Add(new ClearLocalPackages())
        .Add(new ClearDiskArtifacts());

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
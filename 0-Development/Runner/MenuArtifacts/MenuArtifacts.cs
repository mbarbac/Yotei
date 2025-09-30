using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// =============================================================
public class MenuArtifacts : ConsoleMenuEntry
{
    /// <inheritdoc/>
    public override string Header() => "Manage Artifacts";

    /// <inheritdoc/>
    public override void Execute()
    {
        var position = 0; do
        {
            WriteLine(true);
            WriteLine(true, Green, Program.FatSeparator);
            WriteLine(true, Green, Header());
            WriteLine(true);

            position = new ConsoleMenu
            {
                new("Exit"),
                new ClearDiskArtifacts(),
                new ClearLocalPackages(),
            }
            .Run(Program.MenuOptions, position);
        }
        while (position > 0);
    }
}
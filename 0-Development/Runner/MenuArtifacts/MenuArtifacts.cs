using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class MenuArtifacts : ConsoleMenuEntry
{
    /// <inheritdoc/>
    public override string Header() => "Manage Artifacts";

    /// <inheritdoc/>
    public override void Execute()
    {
        var option = 0; do
        {
            WriteLine(true);
            WriteLine(true, Green, Program.FatSeparator);
            WriteLine(true, Green, Header());
            option = new ConsoleMenu
            {
                new("Exit"),
                new ClearLocalPackages(),
                new ClearDiskArtifacts(),
            }
            .Run(true, Green, Program.Timeout);
        }
        while (option > 0);
    }
}
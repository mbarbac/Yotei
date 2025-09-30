using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// =============================================================
public class MenuPackages : ConsoleMenuEntry
{
    /// <inheritdoc/>
    public override string Header() => "Manage Packages";

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
                new PackageBuilder(),
                new PackageVersion(),
            }
            .Run(Program.MenuOptions, position);
        }
        while (position > 0);
    }
}
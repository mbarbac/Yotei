using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class MenuBuilder : ConsoleMenuEntry
{
    /// <inheritdoc/>
    public override string Header() => "Build NuGet Packages";

    /// <inheritdoc/>
    public override void Execute()
    {
        //var option = 0; do
        //{
        //    WriteLine(true);
        //    WriteLine(true, Green, Program.FatSeparator);
        //    WriteLine(true, Green, Header());
        //    option = new ConsoleMenu
        //    {
        //        new("Exit"),
        //        new ClearLocalPackages(),
        //        new ClearDiskArtifacts(),
        //    }
        //    .Run(true, Green, Program.Timeout);
        //}
        //while (option > 0);
    }

    // ----------------------------------------------------
}
using static Yotei.Tools.ConsoleEx;
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
        Console.Clear();
        var position = 0; do
        {
            WriteLine(true);
            WriteLine(true, Green, Program.FatSeparator);
            WriteLine(true, Green, Header());
            WriteLine(true);

            position = new ConsoleMenu
            {
                new("Exit"),
                new ClearLocalPackages(),
                new ClearDiskArtifacts(),                
            }
            .Run(Program.MenuOptions);
        }
        while (position > 0);
    }
}
using static System.ConsoleColor;
using static Yotei.Tools.Diagnostics.ConsoleWrapper;

namespace Runner.Artifacts;

// ========================================================
public class MenuArtifacts : MenuEntry
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string Header() => "Manage Artifacts";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        var done = -1; do
        {
            WriteLine(true);
            WriteLine(true, Green, Program.FatSeparator);
            WriteLine(true, Green, $"{Header()}:");
            WriteLine(true);

            done = Menu.Run(
                Green, Program.Timeout,
                new MenuEntry("Previous"),
                new CleanLocalPackages(),
                new CleanDiskArtifacts());
        }
        while (done > 0);
    }
}
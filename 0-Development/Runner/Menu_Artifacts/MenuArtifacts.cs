using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner.Artifacts;

// ========================================================
/// <summary>
/// Menu entry for managing artifacts.
/// </summary>
public class MenuArtifacts : MenuEntry
{
    /// <inheritdoc/>
    public override string Header() => "Manage Artifacts";

    /// <inheritdoc/>
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
                new EntryCleanLocalPackages(),
                new EntryCleanDiskArtifacts());

            WriteLine(true);
        }
        while (done > 0);
    }
}
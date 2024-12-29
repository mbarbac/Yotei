using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class MenuArtifacts : MenuEntry
{
    /// <inheritdoc/>
    public override string Header() => "Manage Artifacts";

    /// <inheritdoc/>
    public override void Execute()
    {
        var done = 0; do
        {
            WriteLine(true);
            WriteLine(true, Green, Program.FatSeparator);
            WriteLine(true, Green, $"{Header()}:");
            WriteLine(true);

            done = new MenuConsole
            {
                new MenuEntry("Exit"),
                new EntryClearLocalPackages(),
                new EntryClearDiskArtifacts(),
            }
            .Run(Green, Program.Timeout, done);
        }
        while (done > 0);
    }
}
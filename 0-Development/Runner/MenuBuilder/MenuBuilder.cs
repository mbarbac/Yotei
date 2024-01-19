using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

namespace Runner.Builder;

// ========================================================
public class MenuBuilder : MenuEntry
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string Header() => "Build Packages";

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
                new BuildOnePackage(),
                new BuildAllPackages(),
                new BuildLocalRepo());
        }
        while (done > 0);
    }
}
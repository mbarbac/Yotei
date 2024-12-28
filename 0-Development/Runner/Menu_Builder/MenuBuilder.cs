using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner.Builder;

// ========================================================
/// <summary>
/// Menu entry for managing artifacts.
/// </summary>
public class MenuBuilder : MenuEntry
{
    /// <inheritdoc/>
    public override string Header() => "Build NuGet Packages";

    /// <inheritdoc/>
    public override void Execute()
    {
        BuildMode mode = BuildMode.Debug;

        WriteLine(true, Green, Program.FatSeparator);
        WriteLine(true, Green, $"{Header()}:");
        WriteLine(true);
        WriteLine(true, "Please select the desired build mode...");
        WriteLine(true);

        if (!Builder.CaptureBuildMode(out mode)) return;

        var done = -1; do
        {
            var root = Program.GetSolutionDirectory();
            var projects = root.FindProjects();
            var packables = projects.SelectPackables();
            packables = packables.OrderByDependencies();
            if (packables.Count == 0) return;

            WriteLine(true);
            WriteLine(true, Green, Program.SlimSeparator);
            WriteLine(true, Green, $"{mode}:");
            WriteLine(true);

            var entries = new List<MenuEntry> {
                new MenuEntry("Previous"),
                new MenuEntry("All projects"),
            };
            foreach (var packable in packables) entries.Add(new PackageBuilder(packable, mode));

            done = Menu.Run(Green, Program.Timeout, entries.ToArray());
        }
        while (done > 0);
    }
}
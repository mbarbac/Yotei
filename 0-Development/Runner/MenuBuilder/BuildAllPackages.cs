using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

namespace Runner.Builder;

// ========================================================
public class BuildAllPackages : MenuEntry
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string Header() => "Build All Packages";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        WriteLine(true);
        WriteLine(true, Green, Program.FatSeparator);
        WriteLine(true, Green, Header());
        WriteLine(true);

        var root = Program.GetSolutionDirectory();
        root = Program.EditDirectory(root, "Root");
        if (root == null) return;

        var projects = root.FindProjects();
        var packables = projects.SelectPackableProjects();

        if (!PackageBuilder.AskForMode(out var mode)) return;

        foreach (var packable in packables)
        {
            var done = mode switch
            {
                BuildMode.Debug => PackageBuilder.BuildDebug(packable),
                BuildMode.Release => PackageBuilder.BuildRelease(packable),
                _ => throw new UnExpectedException()
            };
            if (!done) return;
        }
    }
}
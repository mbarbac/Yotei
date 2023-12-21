using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

namespace Runner.Builder;

// ========================================================
public class BuildOnePackage : MenuEntry
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override string Header() => "Build One Package";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void Execute()
    {
        string? root = null;
        bool first = true;

        var done = -1; do
        {
            WriteLine(true);
            WriteLine(true, Green, Program.FatSeparator);
            WriteLine(true, Green, Header());

            if (first)
            {
                first = false;
                WriteLine(true);

                root = Program.GetSolutionDirectory();
                root = Program.EditDirectory(root, "Root");
                if (root == null) return;
            }

            var projects = root!.FindProjects();
            var packables = projects.SelectPackableProjects();

            var items = new List<MenuEntry> { new("Previous") };
            items.AddRange(packables.Select(x => new Surrogate(x)));

            WriteLine(true);
            done = Menu.Run(Green, Program.Timeout, [.. items]);
        }
        while (done > 0);
    }

    // ====================================================
    public class Surrogate(Project project) : MenuEntry
    {
        /// <summary>
        /// The project this surrogate refers to.
        /// </summary>
        public Project Project { get; } = project;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns></returns>
        public override string Header()
        {
            var version = Project.GetVersion(out var temp) ? temp.ToString() : "Invalid";
            return $"{Project.Name} v:{version}";
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Execute()
        {
            if (!PackageBuilder.AskForMode(out var mode)) return;

            if (mode == BuildMode.Debug) PackageBuilder.BuildDebug(Project);
            if (mode == BuildMode.Release) PackageBuilder.BuildRelease(Project);
        }
    }
}
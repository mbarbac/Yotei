using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
public class MenuBuilder : MenuEntry
{
    /// <inheritdoc/>
    public override string Header() => "Build NuGet Packages";

    /// <inheritdoc/>
    public override void Execute()
    {
        var done = 0; do
        {
            WriteLine(true);
            WriteLine(true, Green, Program.FatSeparator);
            WriteLine(true, Green, $"{Header()}:");
            WriteLine(true);

            var root = Program.GetSolutionDirectory();
            var projects = FindProjects(root);
            projects = projects.Where(x => x.IsPackable()).ToList();
            projects = OrderByDependencies(projects);

            if (projects.Count == 0)
            {
                WriteLine(true);
                Write(true, Red, "No projects found from: "); WriteLine(true, root);
                return;
            }

            var menu = new MenuConsole {
                new MenuEntry("Previous"),
                new EntrySolution(projects),
            };
            foreach (var project in projects) menu.Add(new EntryPackage(project));

            done = menu.Run(Green, Program.Timeout);
        }
        while (done > 0);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the projects found starting at the given directory, and its own
    /// sub-directories, provided they are not part of the exclusion branch.
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="exclusion"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static List<Project> FindProjects(
        string directory,
        string? exclusion = null,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        directory = directory.NotNullNotEmpty();
        exclusion = exclusion.NullWhenEmpty();

        var list = new List<Project>();
        Populate(directory);
        return list;

        // Recursively populates the given path.
        void Populate(string path)
        {
            if (path.Contains(".git", comparison)) return;
            if (path.Contains(".vs", comparison)) return;
            if (path.Contains("\\bin\\", comparison)) return;
            if (path.Contains("\\obj\\", comparison)) return;

            if (exclusion != null &&
                exclusion.Length > 0 &&
                path.StartsWith(exclusion, comparison)) return;

            var dir = new DirectoryInfo(path);
            if (!dir.Exists) return;

            var files = dir.GetFiles("*.csproj");
            foreach (var file in files) list.Add(new(file.FullName));

            var dirs = dir.GetDirectories();
            foreach (var temp in dirs) Populate(temp.FullName);
        }
    }

    /// <summary>
    /// Orders the given collection of projects by their respective dependencies so that the
    /// last ones may depend on the first ones, but not the opposite.
    /// </summary>
    /// <param name="projects"></param>
    /// <returns></returns>
    public static List<Project> OrderByDependencies(IEnumerable<Project> projects)
    {
        projects.ThrowWhenNull();

        var list = new List<Project>();

        foreach (var project in projects)
        {
            if (!project.IsPackable()) continue;

            var pname = project.Name;
            var found = false;

            for (int i = 0; i < list.Count; i++)
            {
                if (found) break;

                var item = list[i];
                var nrefs = item.GetNuReferences();

                foreach (var nref in nrefs)
                {
                    if (string.Compare(pname, nref.Name, ignoreCase: true) == 0)
                    {
                        list.Insert(i, project);
                        found = true;
                        break;
                    }
                }
            }

            if (!found) list.Add(project);
        }

        return list;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture a build mode.
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static bool CaptureMode(out BuildMode mode)
    {
        WriteLine(true);
        WriteLine(true, Green, "Please select the desired build mode:");
        WriteLine(true);

        var done = new MenuConsole {
            new MenuEntry("Previous"),
            new MenuEntry("Debug"),
            new MenuEntry("Local"),
            new MenuEntry("Release"),
        }
        .Run(Green, Program.Timeout);

        switch (done)
        {
            case 1: mode = BuildMode.Debug; return true;
            case 2: mode = BuildMode.Local; return true;
            case 3: mode = BuildMode.Release; return true;
        }

        mode = default;
        return false;
    }

    /// <summary>
    /// Invoked to capture a new value of the semantic version.
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="oldversion"></param>
    /// <param name="newversion"></param>
    /// <returns></returns>
    public static bool CaptureVersion(
        BuildMode mode,
        SemanticVersion oldversion, out SemanticVersion newversion)
    {
        newversion = default!;

        WriteLine(true);
        Write(true, Green, "Please enter the desired version: ");

        if (!EditLine(oldversion, out var result)) return false;
        newversion = new SemanticVersion(result);

        if ((mode == BuildMode.Debug || mode == BuildMode.Local) &&
            newversion.PreRelease.IsEmpty)
        {
            newversion = newversion with { PreRelease = "v001" };
            Write(true, Magenta, "Modified value: ");
            WriteLine(newversion);
        }

        if (mode == BuildMode.Release && !newversion.PreRelease.IsEmpty)
        {
            newversion = newversion with { PreRelease = "" };
            Write(true, Magenta, "Modified value: ");
            WriteLine(newversion);
        }

        if (newversion.CompareTo(oldversion) < 0 ||
            (newversion.CompareTo(oldversion) == 0 && mode != BuildMode.Local))
        {
            WriteLine(true);
            Write(true, Red, "New version value must be greater than the old one.");
            WriteLine(true);
            return false;
        }

        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Increases the given semantic version value using the given build mode.
    /// </summary>
    /// <param name="version"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static SemanticVersion IncreaseVersion(SemanticVersion version, BuildMode mode)
    {
        version.ThrowWhenNull();

        switch (mode)
        {
            case BuildMode.Debug:
                return version.IncreasePreRelease("v001");

            case BuildMode.Local:
                return version.PreRelease.IsEmpty ? version with { PreRelease = "v001" } : version;

            case BuildMode.Release:
                return version.IncreasePatch();
        }

        throw new UnExpectedException("Unknown build mode.").WithData(mode);
    }
}
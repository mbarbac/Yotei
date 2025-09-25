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
        var option = 0; do
        {
            var root = Program.GetSolutionDirectory();
            var projects = FindProjects(root);
            projects = projects.Where(x => x.IsPackable()).ToList();
            OrderByDependencies(projects);

            var items = projects.Select(x => new PackageEntry(x)).ToList();
            if (items.Count == 0)
            {
                WriteLine(true);
                Write(true, Red, "No packable projects found from: "); WriteLine(true, root);
                return;
            }

            WriteLine(true);
            WriteLine(true, Green, Program.FatSeparator);
            WriteLine(true, Green, Header());
            var menu = new ConsoleMenu { new("Exit") };
            menu.AddRange(items);
            option = menu.Run(true, Green, Program.Timeout);
        }
        while (option > 0);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list containing all the projects found starting at the solutions' root directory,
    /// provided they are not part of the exclusion branch.
    /// </summary>
    /// <param name="exclude"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static List<Project> FindProjects(
        string root,
        string? exclude = null,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        root = root.NotNullNotEmpty(true);
        exclude = exclude?.NotNullNotEmpty(true);
        
        var list = new List<Project>();
        Populate(root);
        return list;

        /// <summary>
        /// Recursively populates the list of projects, starting at the given path.
        /// </summary>
        void Populate(string path)
        {
            if (path.Contains(".git", comparison)) return;
            if (path.Contains(".vs", comparison)) return;
            if (path.Contains("\\bin\\", comparison)) return;
            if (path.Contains("\\obj\\", comparison)) return;
            if (exclude is not null && path.StartsWith(exclude, comparison)) return;

            var dir = new DirectoryInfo(path);
            if (!dir.Exists) return;

            var files = dir.GetFiles("*.csproj");
            foreach (var file in files) list.Add(new(file.FullName));

            var dirs = dir.GetDirectories();
            foreach (var temp in dirs) Populate(temp.FullName);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Orders the given collection of projects by their respective cross-dependencies so that
    /// the last ones may depend on the previous ones, but not the opposite.
    /// </summary>
    /// <param name="projects"></param>
    /// <returns></returns>
    public static void OrderByDependencies(List<Project> projects)
    {
        projects.ThrowWhenNull();

        LOOP:
        for (int i = 0; i < projects.Count; i++)
        {
            var project = projects[i];
            var prname = project.Name;
            var nulines = project.Where(x => x.IsNuReference()).ToList();

            foreach (var nuline in nulines)
            {
                if (!nuline.GetNuName(out var nuname)) continue;

                for (int k = 0; k < i; k++)
                {
                    var temp = projects[k];
                    var tname = temp.Name;
                    if (string.Compare(tname, nuname, ignoreCase: true) == 0)
                    {
                        projects.RemoveAt(i);
                        projects.Insert(k, project);
                        goto LOOP;
                    }
                }
            }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to capture the desired build mode.
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static bool CaptureMode(ref BuildMode mode)
    {
        var option = new ConsoleMenu
        {
            new("Debug"),
            new("Local"),
            new("Release"),
        }
        .Run(true, Green, Program.Timeout);

        switch (option)
        {
            case 0: mode = BuildMode.Debug; return true;
            case 1: mode = BuildMode.Local; return true;
            case 2: mode = BuildMode.Release; return true;
        }

        return false;
    }
}
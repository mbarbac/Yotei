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
                new EntryBuildAll(projects),
            };
            foreach (var project in projects) menu.Add(new EntryBuildPackage(project));

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
}
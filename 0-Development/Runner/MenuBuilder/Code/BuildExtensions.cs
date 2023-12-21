namespace Runner.Builder;

// ========================================================
public static class BuildExtensions
{
    /// <summary>
    /// Returns a list with the project files found in the given directory, and its directories,
    /// provided they are not in the given exclusion branch, if any.
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="exclusion"></param>
    /// <returns></returns>
    public static List<Project> FindProjects(this string directory, string? exclusion = null)
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var list = new List<Project>();

        directory = directory.NotNullNotEmpty();
        exclusion = exclusion.NullWhenEmpty();

        Populate(directory);
        return list;

        // Recursively populates the list...
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

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the packable projects of the given collection.
    /// </summary>
    /// <param name="projects"></param>
    /// <returns></returns>
    public static List<Project> SelectPackableProjects(this IEnumerable<Project> projects)
    {
        projects = projects.ThrowWhenNull();
        return projects.Where(x => x.IsPackable()).ToList();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Obtains the lists of regular and symbol NuGet package files found for the given project.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="regulars"></param>
    /// <param name="symbols"></param>
    public static void GetPackageFiles(
        this Project project,
        out List<string> regulars, out List<string> symbols)
    {
        project = project.ThrowWhenNull();

        var dir = new DirectoryInfo(project.Directory);

        regulars = dir.Exists ? dir
            .GetFiles("*.nupkg", SearchOption.AllDirectories)
            .Select(x => x.FullName).ToList()
            : [];

        symbols = dir.Exists ? dir
            .GetFiles("*.snupkg", SearchOption.AllDirectories)
            .Select(x => x.FullName).ToList()
            : [];
    }
}
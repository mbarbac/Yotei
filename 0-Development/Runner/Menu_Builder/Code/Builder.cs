using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner.Builder;

// ========================================================
public static class Builder
{
    /// <summary>
    /// Tries to interactively capture the build mode preferred by the user.
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static bool CaptureBuildMode(out BuildMode mode)
    {
        var done = Menu.Run(
            Green, Program.Timeout,
            new MenuEntry("Previous"),
            new MenuEntry(nameof(BuildMode.Debug)),
            new MenuEntry(nameof(BuildMode.Local)),
            new MenuEntry(nameof(BuildMode.Release)));

        switch (done)
        {
            case 1: mode = BuildMode.Debug; return true;
            case 2: mode = BuildMode.Local; return true;
            case 3: mode = BuildMode.Release; return true;
        }

        mode = default;
        return false;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the projects found starting at the given directory, and in its own
    /// sub-directories, provided they are not in the exclusion branch.
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="exclusion"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public static List<Project> FindProjects(
        this string directory,
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
    /// Returns the packable projects found in the given projects' collection.
    /// </summary>
    /// <param name="projects"></param>
    /// <returns></returns>
    public static List<Project> SelectPackables(this IEnumerable<Project> projects)
    {
        projects.ThrowWhenNull();

        return projects.Where(x => x.IsPackable()).ToList();
    }

    /// <summary>
    /// Orders the given collection of packable project by their respective dependencies, so
    /// that the last ones may depend on the early ones, but not the opposite.
    /// </summary>
    /// <param name="packables"></param>
    /// <returns></returns>
    public static List<Project> OrderByDependencies(this IEnumerable<Project> packables)
    {
        packables.ThrowWhenNull();

        var list = new List<Project>();

        foreach (var packable in packables)
        {
            var pname = packable.Name;
            var found = false;

            for (int i = 0; i < list.Count; i++)
            {
                if (found) break;

                var item = list[i];
                var nrefs = item.GetNuPackageReferences();

                foreach (var nref in nrefs)
                {
                    if (string.Compare(pname, nref.Name, ignoreCase: true) == 0)
                    {
                        list.Insert(i, packable);
                        found = true;
                        break;
                    }
                }
            }

            if (!found) list.Add(packable);
        }

        return list;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Invoked to build the given packable project, using the given build mode. Returns whether
    /// the build process succeeded, or not.
    /// </summary>
    /// <param name="packable"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static bool BuildPackage(Project packable, BuildMode mode)
    {
        packable.ThrowWhenNull();

        switch (mode)
        {
            case BuildMode.Debug: return BuildDebug(packable);
            case BuildMode.Local: return BuildLocal(packable);
            case BuildMode.Release: return BuildRelease(packable);
        }

        return false;
    }

    /// <summary>
    /// Invoked to build the given project in debug mode.
    /// </summary>
    /// <param name="packable"></param>
    /// <returns></returns>
    static bool BuildDebug(Project packable)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to build the given project in local mode.
    /// </summary>
    /// <param name="packable"></param>
    /// <returns></returns>
    static bool BuildLocal(Project packable)
    {
        throw null;
    }

    /// <summary>
    /// Invoked to build the given project in release mode.
    /// </summary>
    /// <param name="packable"></param>
    /// <returns></returns>
    static bool BuildRelease(Project packable)
    {
        throw null;
    }

    // ----------------------------------------------------
}
using static Yotei.Tools.ConsoleExtensions;
using static System.Console;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
/// <summary>
/// This executable program.
/// </summary>
internal class Program
{
    public static readonly bool ToDebug = true;
    public static readonly TimeSpan Timeout = TimeSpan.FromMinutes(10);
    public static readonly string FatSeparator = new('*', 50);
    public static readonly string SlimSeparator = new('-', 30);

    public static RequestList Includes = [];
    public static RequestList Excludes = [];

    public static string LocalRepoPath = @"C:\Dev\Packages";
    public static string NuGetRepoSource = @"https://api.nuget.org/v3/index.json";
    public static string LocalRepoSource = "Local";

    // ----------------------------------------------------

    /// <summary>
    /// Program entry point.
    /// </summary>
    static void Main()
    {
        Trace.Listeners.EnsureConsoleListener();
        Debug.IndentSize = 2;
        Debug.AutoFlush = true;

        var menu = new ConsoleMenu { ToDebug = ToDebug, Timeout = Timeout }
        .Add(new("Exit"))
        .Add(new MenuTestMode(breakOnError: true))
        .Add(new MenuArtifacts())
        .Add(new MenuPackages());

        int position; do
        {
            WriteLineEx(true);
            WriteLineEx(true, Green, FatSeparator);
            WriteLineEx(true, Green, "Main Menu");
            WriteLineEx(true);
            position = menu.Run(0);
        }
        while (position > 0);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the path to the solution's directory.
    /// </summary>
    /// <returns></returns>
    static internal string GetSolutionDirectory()
    {
        var path = AppContext.BaseDirectory;
        var dir = new DirectoryInfo(path);

        while (true)
        {
            var files = dir.GetFiles("*.slnx");
            if (files.Length > 0) return dir.FullName;

            dir = dir.Parent!;
            if (dir == null) Environment.FailFast("Cannot find solution's directory.");
        }
    }

    /// <summary>
    /// Edits the given directory path, returning either a valid one, or null if the edition was
    /// cancelled or the timeout expired.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="description"></param>
    /// <param name="okEmpty"></param>
    /// <returns></returns>
    static internal string? EditDirectory(string? path, string? description = null)
    {
        description = description?.NotNullNotEmpty(true);

        while (true)
        {
            if (description is not null) WriteEx(true, Green, $"{description}: ");

            path = EditLineEx(true, Timeout, path);
            path = path.NullWhenEmpty(true);
            if (path is null || path.Length == 0) return null;

            try
            {
                var dir = new DirectoryInfo(path);
                if (dir.Exists) return dir.FullName;

                WriteLineEx(true, Red, " <Invalid>");
            }
            catch (FileNotFoundException) { }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the projects found starting at the given root directory and its
    /// child ones, provided that are not part of the exclusion branch.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="exclude"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    static internal List<Project> FindProjects(
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
    /// Invoked to capture the desired build mode, if any.
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    static internal bool CaptureBuildMode(ref BuildMode mode)
    {
        var position = mode switch
        {
            BuildMode.Debug => 1,
            BuildMode.Local => 2,
            BuildMode.Release => 3,
            _ => 0,
        };

        position = new ConsoleMenu { ToDebug = ToDebug, Timeout = Timeout }
        .Run(position);

        switch (position)
        {
            case 1: mode = BuildMode.Debug; return true;
            case 2: mode = BuildMode.Local; return true;
            case 3: mode = BuildMode.Release; return true;
        }
        return false;
    }
}

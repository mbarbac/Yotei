using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;
using Debug = Yotei.Tools.Diagnostics.DebugEx;
using System.Diagnostics.Contracts;

namespace Runner;

// ========================================================
/// <summary>
/// Represents this program.
/// </summary>
internal class Program
{
    public static readonly string FatSeparator = new('*', 50);
    public static readonly string SlimSeparator = new('-', 30);
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(60);
    public static readonly ConsoleMenuOptions MenuOptions = new() { Debug = true, Timeout = Timeout };

    public static string LocalRepoPath = @"C:\Dev\Packages";
    public static string NuGetRepoSource = @"https://api.nuget.org/v3/index.json";
    public static string LocalRepoSource = "Local";

    public static RequestList Includes = [];
    public static RequestList Excludes = [];

    /// <summary>
    /// Program entry point.
    /// </summary>
    static void Main()
    {
        Ambient.AddConsoleListener();
        Debug.IndentSize = 2;
        Debug.AutoFlush = true;

        string? path = GetSolutionDirectory();

        var position = 0; do
        {
            WriteLine(true);
            WriteLine(true, Green, FatSeparator);
            WriteLine(true, Green, "Main Menu");
            WriteLine(true);

            position = new ConsoleMenu
            {
                new("Exit"),
                new MenuTester(breakOnError: true),
                new MenuArtifacts(),
                new MenuPackages(),
                //new("Examples", () =>
                //{
                //}),
            }
            .Run(MenuOptions);
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
            if (description is not null) Write(true, Green, $"{description}: ");

            path = EditLine(true, Timeout, path);
            path = path.NullWhenEmpty(true);
            if (path is null || path.Length == 0) return null;

            try
            {
                var dir = new DirectoryInfo(path);
                if (dir.Exists) return dir.FullName;

                WriteLine(true, Red, " <Invalid>");
            }
            catch (FileNotFoundException) { }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the projects found starting at the given root directory, and its
    /// child, provided none if part of the given exclusion branch, if any.
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

        position = new ConsoleMenu
        {
            new("Exit"),
            new("Debug"),
            new("Local"),
            new("Release"),
        }
        .Run(MenuOptions, position);

        switch (position)
        {
            case 1: mode = BuildMode.Debug; return true;
            case 2: mode = BuildMode.Local; return true;
            case 3: mode = BuildMode.Release; return true;
        }
        return false;
    }
}

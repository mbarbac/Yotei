using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;

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
    public static readonly ConsoleMenuOptions MenuOptions = new()
    { Debug = true, Timeout = Timeout };

    public static RequestList Includes = [];
    public static RequestList Excludes = [];

    public static string LocalRepoPath = @"C:\Dev\Packages";
    public static string NuGetRepoSource = @"https://api.nuget.org/v3/index.json";
    public static string LocalRepoSource = "Local";

    /// <summary>
    /// Program entry point.
    /// </summary>
    static void Main()
    {
        Ambient.GetOrAddConsoleListener();
        Debug.IndentSize = 2;
        Debug.AutoFlush = true;

        Includes.Add(new("Yotei.Tools.LambdaParser.*", null, null));

        var position = 0; do
        {
            WriteLine(true, "");
            WriteLine(true, Green, FatSeparator);
            WriteLine(true, Green, "Main Menu");
            WriteLine(true, "");

            position = new ConsoleMenu
            {
                new("Exit"),
                new MenuTester(breakOnError: true),
                new MenuArtifacts(),
                new MenuPackages(),
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
}
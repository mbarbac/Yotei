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
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);
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
        .Add(new MenuTester(breakOnError: true))
        .Add(new MenuArtifacts())
        .Add(new MenuPackages());

        var position = 0; do
        {
            WriteLineEx(true);
            WriteLineEx(true, Green, FatSeparator);
            WriteLineEx(true, Green, "Main Menu");
            WriteLineEx(true);
            position = menu.Run(position);
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
}

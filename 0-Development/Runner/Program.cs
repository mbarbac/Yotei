using static Yotei.Tools.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
internal class Program
{
    public static readonly string FatSeparator = new('*', 50);
    public static readonly string SlimSeparator = new('-', 30);
    public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(60);

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
        // Debug environment...
        Ambient.AddNewConsoleListener();
        DebugEx.IndentSize = 2;
        DebugEx.AutoFlush = true;

        // Explicit includes and excludes...
        //Includes.Add(new("xxx.Tests", null, null));

        // Main menu...
        var option = 0; do
        {
            WriteLine(true);
            WriteLine(true, Green, FatSeparator);
            WriteLine(true, Green, "Main Menu:");
            option = new ConsoleMenu
            {
                new("Exit"),
                new MenuTester(breakOnError: true),
                new MenuArtifacts(),
                new("Examples", () =>
                {
                })
            }
            .Run(true, Green, Timeout);
        }
        while (option > 0);
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
            var files = dir.GetFiles("*.sln");
            if (files.Length > 0) return dir.FullName;

            dir = dir.Parent!;
            if (dir == null) Environment.FailFast("Cannot find solution's directory.");
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Edits the given directory path, returning either a valid one, an empty one if allowed,
    /// or null in case of cancellation by pressing the [Escape] key or by timeout expiration.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="description"></param>
    /// <param name="okEmpty"></param>
    /// <returns></returns>
    static internal string? EditDirectory(
        string path,
        string? description = null, bool okEmpty = false)
    {
        path.ThrowWhenNull();
        description ??= "Directory";

        while (true)
        {
            Write(true, Green, $"{description}: ");

            var valid = EditLine(true, Timeout, path, out path!);
            if (!valid) return null;

            if (path.Length == 0 && okEmpty) return string.Empty;
            try
            {
                var dir = new DirectoryInfo(path);
                if (dir.Exists) return dir.FullName;

                Console.CursorTop -= 1;
                Console.CursorLeft = 0;
                Write(true, Green, $"{description}: ");
                WriteLine(true, Red, "<Invalid>");
            }
            catch (ArgumentException) { }
            catch (FileNotFoundException) { }
        }
    }
}

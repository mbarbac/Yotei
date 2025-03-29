using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Runner;

// ========================================================
/// <summary>
/// Represents this program.
/// </summary>
internal class Program
{
    public static readonly string FatSeparator = "****************************************";
    public static readonly string SlimSeparator = "-------------------------";
    public static TimeSpan Timeout = TimeSpan.FromSeconds(2 * 60);

    public static RequestList Includes = [];
    public static RequestList Excludes = [];

    // Note that "Local" is a label that through configuration maps to a concrete location or
    // path on disk. The later is used when deleting artifacts, the former when building nuget
    // packages.

    public static string NuGetRepoSource = @"https://api.nuget.org/v3/index.json";
    public static string LocalRepoSource = "Local";
    public static string LocalRepoPath = @"C:\Dev\Packages";

    // ----------------------------------------------------

    /// <summary>
    /// The program entry point.
    /// </summary>
    static void Main()
    {
        // Debug environment...
        Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
        DebugEx.IndentSize = 2;
        DebugEx.AutoFlush = true;

        // Explicit includes and excludes, in order...
        //Includes.Add(new("Yotei.Tools.LambdaParser.Tests", "Test_NameParser", null));

        // Main menu...
        var done = 0; do
        {
            WriteLine(true);
            WriteLine(true, Green, FatSeparator);
            WriteLine(true, Green, "Main Menu:");
            WriteLine(true);

            done = new MenuConsole
            {
                new MenuEntry("Exit"),
                new MenuTester(breakOnError: true),
                new MenuArtifacts(),
                new MenuBuilder(),
            }
            .Run(Green, Timeout);
        }
        while (done > 0);
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

            var valid = EditLine(Timeout, path, out path!);
            if (!valid) return null;

            if (path.Length == 0 && okEmpty) return string.Empty;
            try
            {
                var dir = new DirectoryInfo(path);
                if (dir.Exists) return dir.FullName;

                Console.CursorTop -= 1;
                Console.CursorLeft = 0;
                Write(Green, $"{description}: ");
                WriteLine(Red, "<Invalid>");
            }
            catch (ArgumentException) { }
            catch (FileNotFoundException) { }
        }
    }
}
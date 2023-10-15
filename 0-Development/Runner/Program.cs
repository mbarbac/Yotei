using Xunit.Sdk;
using static System.ConsoleColor;
using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using Debug = Yotei.Tools.Diagnostics.DebugWrapper;

namespace Runner;

// ========================================================
/// <summary>
/// Represents this program.
/// </summary>
internal class Program
{
    public static readonly string FatSeparator = "****************************************";
    public static readonly string SlimSeparator = "-------------------------";
    public static TimeSpan Timeout = TimeSpan.FromMinutes(5);

    public static string NuGetRepoSource = @"https://api.nuget.org/v3/index.json";
    public static string LocalRepoSource = "Local";
    public static string LocalRepoPath = @"C:\Dev\Packages";

    public static Tester.RequestList Includes = new();
    public static Tester.RequestList Excludes = new();

    // ----------------------------------------------------

    /// <summary>
    /// The entry point of this program.
    /// </summary>
    static void Main()
    {
        // Debug environment...
        Debug.IndentSize = 2;
        Debug.AutoFlush = true;

        // Customizing the tests...
        // Excludes.Add(new("Experimental.Tests", null, null));
        // Includes.Add(new("Yotei.Tools.Generators.Tests", null, null));
        // Includes.Add(new("Yotei.ORM.Tests", null, null));

        // Main menu...
        var done = -1; do
        {
            WriteLine(true);
            WriteLine(true, Green, FatSeparator);
            WriteLine(true, Green, "Main Menu:");
            WriteLine(true);

            done = Menu.Run(
                Green, Timeout,
                new MenuEntry("Exit"),
                new Tester.MenuTester(breakOnError: true),
                new Artifacts.MenuArtifacts(),
                new Builder.MenuBuilder());
        }
        while (done > 0);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the path of the solution directory.
    /// </summary>
    /// <returns></returns>
    static internal string GetSolutionDirectory()
    {
        var path = AppContext.BaseDirectory;
        var dir = new DirectoryInfo(path);

        while (true)
        {
            var files = dir.GetFiles("*.sln");
            if (files.Length == 1) return dir.FullName;

            dir = dir.Parent!;
            if (dir == null) Environment.FailFast("Cannot find solution directory.");
        }
    }

    /// <summary>
    /// Edits the given directory path. Returns the editted one, or an empty string if allowed,
    /// or <c>null</c> if the [Escape] was pressed.
    /// </summary>
    /// <param name="path"></param>
    /// <param name="description"></param>
    /// <param name="okEmpty"></param>
    /// <returns></returns>
    static internal string? EditDirectory(string path, string description, bool okEmpty = false)
    {
        path ??= string.Empty;
        description ??= "Directory";

        while (true)
        {
            Write(true, Green, $"{description}: ");
            var valid = EditLine(path!, Timeout, out path!);
            if (!valid) return null;

            if (path.Length == 0 && okEmpty) return string.Empty;
            try
            {
                var dir = new DirectoryInfo(path);
                if (dir.Exists) return dir.FullName;

                Console.CursorTop -= 1;
                Console.CursorLeft = 0;
                Write(true, Green, $"{description}: ");
                Write(path);
                WriteLine(true, Red, $" <Invalid>");
            }
            catch (ArgumentException) { }
            catch (FileNotFoundException) { }
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Captures the desired build mode.
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static bool CaptureBuildMode(out Builder.BuildMode mode)
    {
        var done = Menu.Run(
            Green, Timeout,
            new MenuEntry(nameof(Builder.BuildMode.Debug)),
            new MenuEntry(nameof(Builder.BuildMode.Release)));

        switch (done)
        {
            case 0: mode = Builder.BuildMode.Debug; return true;
            case 1: mode = Builder.BuildMode.Release; return true;
        }

        mode = default;
        return false;
    }
}

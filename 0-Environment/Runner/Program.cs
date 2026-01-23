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
        .Add(new("Manage Artifacts"))
        .Add(new("Manage Packages"));

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
}

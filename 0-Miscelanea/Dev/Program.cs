using Dev.Builder;

namespace Dev;

// ========================================================
/// <summary>
/// Represents this program.
/// </summary>
internal class Program : Tools.Program
{
    public static Directory Root { get; private set; } = Directory.Empty;
    public static Directory Exclude { get; private set; } = Directory.Empty;
    public static ImmutableArray<Project> Projects { get; private set; } = ImmutableArray<Project>.Empty;
    public static ImmutableArray<Packable> Packables { get; private set; } = ImmutableArray<Packable>.Empty;
    public static string DotNetExe { get; } = "dotnet";
    public static string LocalRepoPath { get; } = @"C:\Users\mbarb\AppData\Roaming\NuGet\local";
    public static string NuGetRepoUrl { get; } = @"https://api.nuget.org/v3/index.json";

    /// <summary>
    /// Program's entry point.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        Root = Program.CaptureDirectory("Root directory", Program.DefaultRoot(), true, false);
        Exclude = Program.CaptureDirectory("Exclude directory", Program.DefaultExclude(), true, true);
        Projects = Root.FindProjects(Exclude);
        Packables = Projects.SelectPackables().OrderByReferences();

        MenuRun(
            new Janitor.RunJanitor(args),
            new Builder.RunBuilder(),
            new Tester.RunTester());
    }
}

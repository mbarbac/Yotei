namespace Dev;

// ========================================================
/// <summary>
/// Represents this program.
/// </summary>
internal class Program
{
    public static StringComparison Comparison = StringComparison.OrdinalIgnoreCase;
    public static string FatSeparator = "******************************";
    public static string SlimSeparator = "--------------------";
    public static ConsoleColor Color = ConsoleColor.Green;

    public static string DotNetExe = "dotnet";
    public static string LocalRepoPath = @"C:\Users\mbarb\AppData\Roaming\NuGet\local";
    public static string LocalRepoSource = "local";
    public static string NuGetRepoSource = @"https://api.nuget.org/v3/index.json";

    // ----------------------------------------------------

    /// <summary>
    /// Program's entry point.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        var done = -1; do
        {
            WriteLine();
            WriteLine(Color, FatSeparator);
            WriteLine(Color, "Main menu.");
            WriteLine();

            done = Menu.Run(
                Color,
                new MenuEntry(() => WriteLine("Exit")),
                new Tester.Tester(),
                new Builder.LocalBuilder(),
                new Janitor.Janitor());
        }
        while (done > 0);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Returns the solution's root directory.
    /// </summary>
    /// <returns></returns>
    public static Directory SolutionRoot()
    {
        var path = AppContext.BaseDirectory;
        var dir = new Directory(path);

        while (true)
        {
            var files = dir.GetFiles("*.sln");

            if (files.Length == 1) return dir;
            else if (files.Length == 0)
            {
                dir = dir.GetParent();
                if (dir == null) return Directory.Empty;
            }
            else Environment.FailFast($"Too many solution files found at: {dir}");
        }
    }

    /// <summary>
    /// Returns this project's root directory.
    /// </summary>
    /// <returns></returns>
    public static Directory ProjectRoot()
    {
        var path = AppContext.BaseDirectory;
        var dir = new Directory(path);

        while (true)
        {
            var files = dir.GetFiles("*.csproj");

            if (files.Length == 1) return dir;
            else if (files.Length == 0)
            {
                dir = dir.GetParent();
                if (dir == null) return Directory.Empty;
            }
            else Environment.FailFast($"Too many project files found at: {dir}");
        }
    }
}
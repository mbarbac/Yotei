namespace Dev;

// ========================================================
/// <summary>
/// Represents this program.
/// </summary>
internal class Program
{
    public static StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    public static string Separator = "******************************";
    public static ConsoleColor Color = ConsoleColor.Green;

    /// <summary>
    /// Program's entry point.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        new Menu().Run(() =>
        {
            WriteLine();
            WriteLine(Color, Separator);
            WriteLine(Color, "Main menu.");
            WriteLine();
        },
        new Tester.Tester(),
        new Builder.Builder(),
        new Janitor.Janitor());
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
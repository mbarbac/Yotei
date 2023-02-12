namespace Dev.Tools;

// ========================================================
public static class Program
{
    /// <summary>
    /// Returns the solution root.
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
    /// Returns the project root.
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
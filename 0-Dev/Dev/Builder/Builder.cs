namespace Dev.Builder;

// ========================================================
public class Builder : MenuItem
{
    public static string DotNetExe = "dotnet";
    public static string LocalRepoPath = @"C:\Users\mbarb\AppData\Roaming\NuGet\local";
    public static string LocalRepoSource = "local";
    public static string NuGetRepoSource = @"https://api.nuget.org/v3/index.json";

    public Directory Root = Directory.Empty;
    public Directory Exclude = Directory.Empty;
    public ImmutableArray<Project> Projects = ImmutableArray<Project>.Empty;
    public ImmutableArray<Packable> Packables = ImmutableArray<Packable>.Empty;

    /// <inheritdoc>
    /// </inheritdoc>
    public override void Print() => WriteLine("Manage solution packages.");

    /// <inheritdoc>
    /// </inheritdoc>
    public override void Execute()
    {
        var first = true;
        new Menu().Run(() =>
        {
            WriteLine();
            WriteLine(Program.Color, Program.Separator);
            WriteLine(Program.Color, "Manage solution packages.");
            GetRootDirectory(first);
            GetExcludeDirectory(first);
            ResetElements();
            first = false;            

            WriteLine();
        },
        new LocalBuilder(this),
        new CacheManager());
    }

    /// <summary>
    /// Invoked to reset the collection of projects and packages.
    /// </summary>
    public void ResetElements()
    {
        Projects = Root.FindProjects(Exclude);
        Packables = Projects.SelectPackables().OrderByReferences();
    }

    /// <summary>
    /// Gets the root directory,
    /// </summary>
    void GetRootDirectory(bool first)
    {
        var root = first ? Program.SolutionRoot().Path : Root.Path; do
        {
            Write(Color.Green, "Root Directory: "); root = EditLine(root);
            if (root.Length > 0) Root = root;
        }
        while (root.Length == 0);
    }

    /// <summary>
    /// Gets the exclude directory,
    /// </summary>
    void GetExcludeDirectory(bool first)
    {
        var exclude = first ? Program.ProjectRoot().Path : Exclude.Path;

        Write(Color.Green, "Exclude Directory: "); exclude = EditLine(exclude);
        Exclude = exclude.Length > 0 ? exclude : Directory.Empty;
    }
}
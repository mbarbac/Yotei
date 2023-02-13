namespace Dev.Builder;

// ========================================================
/// <summary>
/// Manage packages.
/// </summary>
public class MenuBuilder : MenuItem
{
    public static string DotNetExe = "dotnet";
    public static string LocalRepoPath = @"C:\Users\mbarb\AppData\Roaming\NuGet\local";
    public static string LocalRepoSource = "local";
    public static string NuGetRepoSource = @"https://api.nuget.org/v3/index.json";

    /// <summary>
    /// Print the head title of this menu item, which must end with a new line.
    /// </summary>
    public override void PrintHead() => WriteLine("Manage solution packages.");

    /// <summary>
    /// Executes the actions in this menu item.
    /// </summary>
    public override void Execute()
    {
        WriteLine();
        WriteLine(Color.Green, Menu.SeparatorLine);
        WriteLine(Color.Green, "Managing solutions packages.");
        WriteLine();

        Write(Color.Green, "Root directory: ");
        var root = EditLine(Program.SolutionRoot());
        if (root.Length == 0) return;
        Root = root;

        Write(Color.Green, "Exclude directory: ");
        var exclude = EditLine(Program.ProjectRoot());
        Exclude = exclude.Length == 0 ? Directory.Empty : exclude;

        Projects = Root.FindProjects(Exclude);
        Packables = Projects.SelectPackables().OrderByReferences();

        Menu.Run(
            new MenuBuilderLocal(this),
            new MenuBuilderRemote(this));
    }
    public Directory Root = Directory.Empty;
    public Directory Exclude = Directory.Empty;
    public ImmutableArray<Project> Projects = ImmutableArray<Project>.Empty;
    public ImmutableArray<Packable> Packables = ImmutableArray<Packable>.Empty;
}

// ========================================================
/// <summary>
/// Manage packages.
/// </summary>
public class MenuBuilderLocal : MenuItem
{
    /// <summary>
    /// Initiliazes a new instance.
    /// </summary>
    /// <param name="builder"></param>
    public MenuBuilderLocal(MenuBuilder builder) => Builder = builder.ThrowIfNull();
    MenuBuilder Builder;

    /// <summary>
    /// Print the head title of this menu item, which must end with a new line.
    /// </summary>
    public override void PrintHead() => WriteLine("Manage LOCAL solution packages.");

    /// <summary>
    /// Executes the actions in this menu item.
    /// </summary>
    public override void Execute()
    {
        Menu.Run(
            new LocalPushSelect(Builder),
            new LocalPushAll(Builder),
            new LocalIncreaseSelect(Builder),
            new LocalIncreaseAll(Builder));
    }
}

// ========================================================
/// <summary>
/// Manage packages.
/// </summary>
public class MenuBuilderRemote : MenuItem
{
    /// <summary>
    /// Initiliazes a new instance.
    /// </summary>
    /// <param name="builder"></param>
    public MenuBuilderRemote(MenuBuilder builder) => Builder = builder.ThrowIfNull();
    MenuBuilder Builder;

    /// <summary>
    /// Print the head title of this menu item, which must end with a new line.
    /// </summary>
    public override void PrintHead() => WriteLine("Manage REMOTE solution packages.");

    /// <summary>
    /// Executes the actions in this menu item.
    /// </summary>
    public override void Execute()
    {
        WriteLine();
        WriteLine(Color.Green, "Managing remote solution packages...");
        WriteLine(Color.Magenta, "Not implemented...");
    }
}
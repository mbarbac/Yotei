namespace Dev.Builder;

// ========================================================
/// <summary>
/// Increases version and pushes local package.
/// </summary>
public class LocalIncrease : Runner
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="packable"></param>
    public LocalIncrease(Packable packable) => Packable = packable.ThrowIfNull();
    public Packable Packable = Packable.Empty;

    /// <summary>
    /// Prints the header title of this action, ending in a new line.
    /// </summary>
    public override void PrintHead()
    {
        Write("Increase Local Version, Compile and Push: ");
        WriteLine(Color.Cyan, $"{Packable}.");
    }

    /// <summary>
    /// Executes this action.
    /// </summary>
    public override void Execute()
    {
        WriteLine();
        WriteLine(Color.Green, Program.Separator);
        Write(Color.Green, "Increasing Local Version, Compiling and Pushing package: ");
        Write(Packable.ToString());

        var oldversion = Packable.Version;
        var newversion = oldversion.Beta.Value.Length == 0
            ? oldversion.Increase(SemanticOptions.Patch, out _) with { Beta = "v001" }
            : oldversion.Increase(SemanticOptions.BetaExpand, out _);

        if (ReferenceEquals(newversion, oldversion))
        {
            WriteLine();
            WriteLine(Color.Red, "Cannot increase version.");
            return;
        }
        Write(Color.Green, " To version: ");
        WriteLine(newversion);

        var done = Packable.Project.SetVersion(newversion);
        if (!done)
        {
            WriteLine(Color.Red, "Cannot write new version into packable's file.");
            return;
        }

        WriteLine();
        WriteLine(Color.Green, "Compiling project...");
        WriteLine();
        done = Packable.Project.Compile(CompileMode.Debug);
        if (!done)
        {
            WriteLine(Color.Red, "Cannot compile project.");
            Packable.Project.SetVersion(oldversion);
        }
        else
        {
            var temp = new Packable(Packable.Project, newversion);
            new LocalPush(temp).Execute();
        }
    }
}

// ========================================================
/// <summary>
/// Increases versions and pushes all local packages.
/// </summary>
public class LocalIncreaseAll : Runner
{
    /// <summary>
    /// Prints the header title of this action, ending in a new line.
    /// </summary>
    public override void PrintHead() => WriteLine("Increase Local Version, Compile and Push all packages.");

    /// <summary>
    /// Executes this action.
    /// </summary>
    public override void Execute()
    {
        WriteLine();
        WriteLine(Color.Green, "Pushing all local packages.");

        foreach (var packable in Program.Packables) new LocalIncrease(packable).Execute();
    }
}

// ========================================================
/// <summary>
/// Pushes all local packages.
/// </summary>
public class LocalIncreaseSelect : Runner
{
    /// <summary>
    /// Prints the header title of this action, ending in a new line.
    /// </summary>
    public override void PrintHead() => WriteLine("Increase Local Version, Compile and Push selected packages.");

    /// <summary>
    /// Executes this action.
    /// </summary>
    public override void Execute()
    {
        var list = new List<LocalIncrease>();
        foreach (var packable in Program.Packables) list.Add(new LocalIncrease(packable));
        Program.MenuRun(list.ToArray());
    }
}
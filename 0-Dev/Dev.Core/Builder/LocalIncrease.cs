namespace Dev.Builder;

// ========================================================
/// <summary>
/// Increase version and push a local package.
/// </summary>
public class LocalIncrease : MenuItem
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public LocalIncrease(Packable packable, MenuBuilder builder)
    {
        Packable = packable.ThrowIfNull();
        Builder = builder.ThrowIfNull();
    }
    Packable Packable;
    MenuBuilder Builder;

    /// <summary>
    /// Print the head title of this menu item, which must end with a new line.
    /// </summary>
    public override void PrintHead()
    {
        Write("Increase and Push LOCAL package: "); WriteLine(Color.Cyan, $"{Packable}");
    }

    /// <summary>
    /// Executes the actions in this menu item.
    /// </summary>
    public override void Execute()
    {
        WriteLine();
        WriteLine(Color.Green, Menu.SeparatorLine);
        Write(Color.Green, "Increasing and Pushing local package: ");
        WriteLine(Color.Cyan, Packable.Project.File.Name);

        var oldversion = Packable.Version;
        var newversion = oldversion.Beta.Value.Length == 0
            ? oldversion.Increase(SemanticOptions.Patch, out _) with { Beta = "v001" }
            : oldversion.Increase(SemanticOptions.BetaExpand, out _);

        if (ReferenceEquals(oldversion, newversion)) return;
        Write(Color.Green, "To version: "); WriteLine(newversion);

        var done = Packable.Project.SetVersion(newversion);
        if (!done)
        {
            WriteLine(Color.Red, "Cannot write new version into packable's file.");
            return;
        }

        WriteLine();
        WriteLine(Color.Green, Menu.SeparatorLine);
        WriteLine(Color.Green, "Compiling package...");

        done = Packable.Project.Compile(CompileMode.Debug);
        if (!done)
        {
            WriteLine(Color.Red, "Cannot compile project.");
            Packable.Project.SetVersion(oldversion);
        }
        else
        {
            var temp = new Packable(Packable.Project, newversion);
            new LocalPush(temp, Builder).Execute();
        }
    }
}

// ========================================================
/// <summary>
/// Increase version and push all local packages.
/// </summary>
public class LocalIncreaseAll : MenuItem
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="builder"></param>
    public LocalIncreaseAll(MenuBuilder builder) => Builder = builder.ThrowIfNull();
    MenuBuilder Builder;

    /// <summary>
    /// Print the head title of this menu item, which must end with a new line.
    /// </summary>
    public override void PrintHead() => WriteLine("Increase and Push all LOCAL packages.");

    /// <summary>
    /// Executes the actions in this menu item.
    /// </summary>
    public override void Execute()
    {
        foreach (var packable in Builder.Packables) new LocalIncrease(packable, Builder).Execute();
    }
}

// ========================================================
/// <summary>
/// Select a local packages to increase its version and push.
/// </summary>
public class LocalIncreaseSelect : MenuItem
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="builder"></param>
    public LocalIncreaseSelect(MenuBuilder builder) => Builder = builder.ThrowIfNull();
    MenuBuilder Builder;

    /// <summary>
    /// Print the head title of this menu item, which must end with a new line.
    /// </summary>
    public override void PrintHead() => WriteLine("Select LOCAL package to Increase and Push.");

    /// <summary>
    /// Executes the actions in this menu item.
    /// </summary>
    public override void Execute()
    {
        Menu.Run(Builder.Packables.Select(x => new LocalIncrease(x, Builder)).ToArray());
    }
}
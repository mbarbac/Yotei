namespace Dev.Builder;

// ========================================================
/// <summary>
/// Pushes a local package.
/// </summary>
public class LocalPush : MenuItem
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public LocalPush(Packable packable, MenuBuilder builder)
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
        Write("Push local package: "); WriteLine(Color.Cyan, $"{Packable}");
    }

    /// <summary>
    /// Executes the actions in this menu item.
    /// </summary>
    public override void Execute()
    {
        WriteLine();
        WriteLine(Color.Green, Menu.SeparatorLine);
        Write(Color.Green, "Pushing local package: "); WriteLine($"{Packable}");
        var files = Packable.Push(PushMode.Local);

        WriteLine();
        foreach (var file in files)
        {
            Write(Color.Green, "Deleting file: "); WriteLine(file.NameAndExtension);
            file.Delete();
        }
        if (files.Length == 0) WriteLine(Color.Red, "No files were deleted...");

        var projects = Builder.Projects.Remove(Packable.Project);
        foreach (var project in projects)
        {
            var references = project.GetReferences();
            foreach (var reference in references)
            {
                if (string.Compare(reference.Name, Packable.Project.File.Name, Program.Comparison) == 0)
                {
                    WriteLine();
                    WriteLine(Color.Green, Menu.SeparatorLine);
                    Write(Color.Green, "Updating reference at project: ");
                    Write(Color.Cyan, project.File.NameAndExtension);
                    WriteLine();

                    var name = Packable.Project.File.Name;
                    var version = Packable.Version;
                    project.ReloadPackage(name, version, PushMode.Local);
                }
            }
        }
    }
}

// ========================================================
/// <summary>
/// Pushes all local packages.
/// </summary>
public class LocalPushAll : MenuItem
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="builder"></param>
    public LocalPushAll(MenuBuilder builder) => Builder = builder.ThrowIfNull();
    MenuBuilder Builder;

    /// <summary>
    /// Print the head title of this menu item, which must end with a new line.
    /// </summary>
    public override void PrintHead() => WriteLine("Pushes all Local packages.");

    /// <summary>
    /// Executes the actions in this menu item.
    /// </summary>
    public override void Execute()
    {
        foreach (var packable in Builder.Packables) new LocalPush(packable, Builder).Execute();
    }
}

// ========================================================
/// <summary>
/// Select a local packages to be pushed.
/// </summary>
public class LocalPushSelect : MenuItem
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="builder"></param>
    public LocalPushSelect(MenuBuilder builder) => Builder = builder.ThrowIfNull();
    MenuBuilder Builder;

    /// <summary>
    /// Print the head title of this menu item, which must end with a new line.
    /// </summary>
    public override void PrintHead() => WriteLine("Select Local package to push.");

    /// <summary>
    /// Executes the actions in this menu item.
    /// </summary>
    public override void Execute()
    {
        Menu.Run(Builder.Packables.Select(x => new LocalPush(x, Builder)).ToArray());
    }
}
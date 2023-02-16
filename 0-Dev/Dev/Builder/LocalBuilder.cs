namespace Dev.Builder;

// ========================================================
public class LocalBuilder : MenuItem
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public LocalBuilder(Builder builder) => Builder = builder.ThrowIfNull();
    readonly Builder Builder;

    /// <inheritdoc>
    /// </inheritdoc>
    public override void Print() => WriteLine("Manage local solution packages.");

    /// <inheritdoc>
    /// </inheritdoc>
    public override void Execute()
    {
        new Menu().Run(() =>
        {
            WriteLine();
            WriteLine(Program.Color, Program.Separator);
            WriteLine(Program.Color, "Manage local solution packages.");
            WriteLine();
        },
        new PushLocalSelect(Builder),
        new PushLocalAll(Builder));
    }
}

// ========================================================
public class PushLocal : MenuItem
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public PushLocal(Builder builder, Packable packable)
    {
        Builder = builder.ThrowIfNull();
        Packable = packable.ThrowIfNull();
    }
    readonly Builder Builder;
    Packable Packable;

    /// <inheritdoc>
    /// </inheritdoc>
    public override void Print()
    {
        Write(Color.Green, "Push local package: "); WriteLine(Packable);
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override void Execute()
    {
        WriteLine();
        WriteLine(Program.Color, Program.Separator);
        Write(Color.Green, "Push local package: "); WriteLine(Packable);
        do
        {
            if (!CompileLocalPackage()) break;
            if (!PushLocalPackage()) break;
            if (!AdjustReferences()) break;
            if (!IncreaseLocalVersion()) break;
        }
        while (false);
        Builder.ResetElements();
    }

    bool CompileLocalPackage()
    {
        WriteLine();
        WriteLine(Program.Color, Program.Separator);
        Write(Color.Green, "Compiling local package: "); WriteLine(Packable);
        WriteLine();

        var done = Packable.Project.Compile(CompileMode.Debug);
        if (!done) WriteLine(Color.Red, "Error compiling package.");
        return done;
    }

    bool PushLocalPackage()
    {
        WriteLine();
        WriteLine(Program.Color, Program.Separator);
        Write(Color.Green, "Pushing local package: "); WriteLine(Packable);
        WriteLine();

        var files = Packable.Push(PushMode.Local);
        if (files.Length == 0) WriteLine(Color.Red, "No package files found to push.");

        files = Packable.GetPackageFiles(PushMode.Local);
        if (files.Length > 0)
        {
            WriteLine();
            WriteLine(Program.Color, Program.Separator);
            WriteLine(Program.Color, "Deleting files...");
            WriteLine();

            files = Packable.GetPackageFiles(PushMode.Local);
            foreach (var file in files)
            {
                Write(Program.Color, "Deleting file: "); WriteLine(file.NameAndExtension);
                file.Delete();
            }
        }
        return true;
    }

    bool AdjustReferences()
    {
        WriteLine();
        WriteLine(Program.Color, Program.Separator);
        Write(Color.Green, "Adjusting references to: "); WriteLine(Packable);
        WriteLine();

        var projects = Builder.Projects.Remove(Packable.Project);
        foreach (var project in projects)
        {
            var references = project.GetReferences();
            if (references.Any(x => string.Compare(x.Name, Packable.Name, Program.Comparison) == 0))
            {
                WriteLine();
                WriteLine(Program.Color, Program.Separator);
                Write(Program.Color, "Updating project: "); WriteLine(project.Name);
                WriteLine(Color.Blue, "Please do not interrupt this process...");
                project.ReLoadPackage(Packable.Name, Packable.Version, PushMode.Local);
            }
        }
        return true;
    }

    bool IncreaseLocalVersion()
    {
        WriteLine();
        WriteLine(Program.Color, Program.Separator);
        Write(Color.Green, "Increasing local version of: "); Write(Packable);

        var version = Packable.Version.Increase(SemanticOptions.BetaExpand, out _);
        Packable = Packable with { Version = version };
        Write(Program.Color, "To New Version: "); WriteLine(Color.Cyan, $"{Packable}");

        var done = Packable.Project.SetVersion(Packable.Version);
        if (!done) WriteLine(Color.Red, "Cannot set the new version...");
        return true;
    }
}

// ========================================================
public class PushLocalSelect : MenuItem
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public PushLocalSelect(Builder builder) => Builder = builder.ThrowIfNull();
    readonly Builder Builder;

    /// <inheritdoc>
    /// </inheritdoc>
    public override void Print() => WriteLine("Select local package to push.");

    /// <inheritdoc>
    /// </inheritdoc>
    public override void Execute()
    {
        new Menu().Run(() =>
        {
            WriteLine();
            WriteLine(Program.Color, Program.Separator);
            WriteLine(Program.Color, "Select local package to push.");
            WriteLine();
        },
        Builder.Packables.Select(x => new PushLocal(Builder, x)).ToArray());
    }
}

// ========================================================
public class PushLocalAll : MenuItem
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public PushLocalAll(Builder builder) => Builder = builder.ThrowIfNull();
    readonly Builder Builder;

    /// <inheritdoc>
    /// </inheritdoc>
    public override void Print() => WriteLine("Push all local packages.");

    /// <inheritdoc>
    /// </inheritdoc>
    public override void Execute()
    {
        WriteLine();
        WriteLine(Program.Color, Program.Separator);
        WriteLine(Program.Color, "Push all local packages.");
        
        foreach (var packable in Builder.Packables) new PushLocal(Builder, packable).Execute();
    }
}
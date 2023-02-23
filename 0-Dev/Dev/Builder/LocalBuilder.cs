namespace Dev.Builder;

// ========================================================
public class LocalBuilder : MenuEntry
{
    public Directory Root = Directory.Empty;
    public Directory Exclude = Directory.Empty;
    public ImmutableArray<Project> Projects = ImmutableArray<Project>.Empty;
    public ImmutableArray<Packable> Packables = ImmutableArray<Packable>.Empty;

    public string Header => "Manage Local Packages.";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnPrint() => WriteLine(Header);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnExecute()
    {
        var first = true;
        var done = -1; do
        {
            WriteLine();
            WriteLine(Program.Color, Program.FatSeparator);
            WriteLine(Program.Color, Header);

            var entries = new List<MenuEntry>
            {
                new MenuEntry(() => WriteLine("Exit")),
                new LocalPushSelect(this),
                new LocalPushAll(this)
            };
            if (!first) entries.Add(new ReCaptureDirectories());

            if (first)
            {
                GetRootDirectory(first);
                GetExcludeDirectory(first);
                ResetElements();
                first = false;
            }

            WriteLine();
            done = Menu.Run(Program.Color, entries.ToArray());

            if (done == 3) first = true;
        }
        while (done > 0);
    }

    /// <summary>
    /// Gets the root directory.
    /// </summary>
    public void GetRootDirectory(bool first)
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
    public void GetExcludeDirectory(bool first)
    {
        var exclude = first ? Program.ProjectRoot().Path : Exclude.Path;

        Write(Color.Green, "Exclude Directory: "); exclude = EditLine(exclude);
        Exclude = exclude.Length > 0 ? exclude : Directory.Empty;
    }

    /// <summary>
    /// Invoked to reset the collection of projects and packages.
    /// </summary>
    public void ResetElements()
    {
        Projects = Root.FindProjects(Exclude);
        Packables = Projects.SelectPackables().OrderByReferences();
    }
}

// ========================================================
public class ReCaptureDirectories : MenuEntry
{
    public string Header => "Recapture Solution Directories.";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnPrint() => WriteLine(Header);
}

// ========================================================
public class LocalPushSelect : MenuEntry
{
    public LocalPushSelect(LocalBuilder builder) => Builder = builder.ThrowIfNull();
    LocalBuilder Builder;

    public string Header => "Select Local Package to Push.";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnPrint() => WriteLine(Header);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnExecute()
    {
        var done = false; do
        {
            WriteLine();
            WriteLine(Program.Color, Program.FatSeparator);
            WriteLine(Program.Color, Header);
            WriteLine();

            Builder.ResetElements();
            var items = new List<MenuEntry> { new MenuEntry(() => WriteLine(Program.Color, "Exit")) };
            var temps = Builder.Packables.Select(x => new LocalPush(Builder, x));
            foreach (var temp in temps) items.Add(temp);

            var entry = Menu.Run(
                Program.Color,
                items.ToArray());

            done = entry <= 0;
        }
        while (!done);
    }
}

// ========================================================
public class LocalPushAll : MenuEntry
{
    public LocalPushAll(LocalBuilder builder) => Builder = builder.ThrowIfNull();
    LocalBuilder Builder;

    public string Header => "Build & Push All Local Packages.";

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnPrint() => WriteLine(Header);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public override void OnExecute()
    {
        WriteLine();
        WriteLine(Program.Color, Program.FatSeparator);
        WriteLine(Program.Color, Header);

        Builder.ResetElements();
        foreach (var packable in Builder.Packables)
            new LocalPush(Builder, packable).Execute();
    }
}
namespace Dev.Builder;

// ========================================================
/// <summary>
/// Represents a packable project.
/// </summary>
public record Packable : Project
{
    /// <summary>
    /// An empty instance.
    /// </summary>
    public static new Packable Empty { get; } = new();
    protected Packable() { }

    /// <summary>
    /// Initializes a new instance using the given path, which can either be null, or empty, or
    /// an existing project path.
    /// </summary>
    /// <param name="path"></param>
    public Packable(string path) : base(path)
    {
        if (!this.IsPackable(out var version))
            throw new ArgumentException($"Project '{path}' is not a packable one.");

        Version = version!;
    }

    /// <summary>
    /// Initializes a new instance using its directory, name and extension.
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="name"></param>
    /// <param name="extension"></param>
    public Packable(Directory directory, string name, string extension)
        : base(directory, name, extension)
    {
        if (!this.IsPackable(out var version))
            throw new ArgumentException($"Project '{FullName}' is not a packable one.");

        Version = version!;
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => $"{Name}.{Version}";

    /// <summary>
    /// Determines if this instance is an empty one.
    /// </summary>
    public override bool IsEmpty => base.IsEmpty && Version.IsEmpty;

    /// <summary>
    /// The version of this package.
    /// </summary>
    public SemanticVersion Version
    {
        get => _Version;
        init => _Version = value.ThrowIfNull(nameof(Version));
    }
    SemanticVersion _Version = SemanticVersion.Empty;
}

// ========================================================
public static class PackableExtensions
{
    static StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    // ----------------------------------------------------

    /// <summary>
    /// Returns the collection of packable projects found among the given ones.
    /// </summary>
    /// <param name="projects"></param>
    /// <returns></returns>
    public static ImmutableList<Packable> SelectPackables(this IEnumerable<Project> projects)
    {
        projects = projects.ThrowIfNull();

        var list = new List<Packable>();
        foreach (var project in projects)
        {
            if (project.IsPackable(out var version))
            {
                var item = new Packable(project.FullName);
                list.Add(item);
            }
        }

        return list.ToImmutableList();
    }

    /// <summary>
    /// Removes from the given collection of projects the given packable ones.
    /// </summary>
    /// <param name="projects"></param>
    /// <param name="packables"></param>
    /// <returns></returns>
    public static ImmutableList<Project> RemovePackables(
        this IEnumerable<Project> projects,
        IEnumerable<Packable> packables)
    {
        projects = projects.ThrowIfNull();
        packables = packables.ThrowIfNull();

        var list = projects.ToList();
        foreach (var packable in packables)
        {
            var item = list.Find(x => string.Compare(x.FullName, packable.FullName, Comparison) == 0);
            if (item != null) list.Remove(item);
        }

        return list.ToImmutableList();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the collection of package files on disk that depend on the given packable project.
    /// </summary>
    /// <param name="packable"></param>
    /// <returns></returns>
    public static ImmutableList<File> GetPackageFiles(this Packable packable)
    {
        packable = packable.ThrowIfNull();

        var list = new List<File>();
        if (!packable.IsEmpty) Populate(list, packable.Directory.Value);
        return list.ToImmutableList();

        /// <summary> Invoked to populate the list...
        /// </summary>
        static void Populate(List<File> list, string path)
        {
            var files = _Directory.GetFiles(path, "*.nupkg");
            foreach (var file in files) list.Add(new File(file));

            var symbols = _Directory.GetFiles(path, "*.snupkg");
            foreach (var symbol in symbols) list.Add(new File(symbol));

            var dirs = _Directory.GetDirectories(path);
            foreach (var dir in dirs) Populate(list, dir);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Pushes the given package according to the given mode. Returns the collection of files
    /// that have been pushed.
    /// </summary>
    /// <param name="packable"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static ImmutableList<File> Push(this Packable packable, PushMode mode)
    {
        packable = packable.ThrowIfNull();

        var list = packable.GetPackageFiles();
        var files = list.Where(x =>
            (mode == PushMode.Local && x.IsDebug) ||
            (mode == PushMode.NuGet && x.IsRelease));

        var regular = files
            .Where(x => string.Compare(x.Extension, "nupkg", Comparison) == 0)
            .OrderBy(x => x.Name)
            .FirstOrDefault();

        if (regular != null)
        {
            PushFile(regular, mode);
            list.Add(regular);
        }

        var symbols = files
            .Where(x => string.Compare(x.Extension, "snupkg", Comparison) == 0)
            .OrderBy(x => x.Name)
            .FirstOrDefault();

        if (symbols != null)
        {
            PushFile(symbols, mode);
            list.Add(symbols);
        }

        return list.ToImmutableList();

        /// <summary> Pushes the given file...
        /// </summary>
        static void PushFile(File file, PushMode mode)
        {
            WriteLine();
            Write(Color.Green, "Pushing file: "); Write(file.FullName);
            Write(Color.Green, " For mode: "); WriteLine(mode.ToString());

            var source = mode == PushMode.Local
                ? "local"
                : "https://api.nuget.org/v3/index.json";

            var p = new Process();
            p.StartInfo.FileName = Program.NuGetExe;
            p.StartInfo.WorkingDirectory = file.Directory.Value;
            p.StartInfo.Arguments = $"push {file.NameAndExtension} -Source {source}";
            p.Start();
            p.WaitForExit();
        }
    }
}
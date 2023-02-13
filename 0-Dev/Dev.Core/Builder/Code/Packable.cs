namespace Dev.Builder;

// ========================================================
/// <summary>
/// Represents a solution project that produces a NuGet package.
/// </summary>
public record Packable
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="project"></param>
    public Packable(Project project)
    {
        if (!project.IsPackable(out var version))
            throw new ArgumentException($"Project is not a packable one: {project}");

        Project = project;
        Version = version!;
    }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="version"></param>
    public Packable(Project project, SemanticVersion version)
    {
        Project = project;
        Version = version;
    }

    /// <summary>
    /// An empty instance.
    /// </summary>
    public static Packable Empty { get; } = new();
    protected Packable() { }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => $"{Project}: {Version}";

    /// <summary>
    /// The project this instance refers to.
    /// </summary>
    public Project Project
    {
        get => _Project;
        init => _Project = value.ThrowIfNull();
    }
    Project _Project = Project.Empty;

    /// <summary>
    /// The version of this instance.
    /// </summary>
    public SemanticVersion Version
    {
        get => _Version;
        init => _Version = value.ThrowIfNull(nameof(Version));
    }
    SemanticVersion _Version = SemanticVersion.Empty;
}

// ========================================================
public static class PackageExtensions
{
    /// <summary>
    /// Orders the given collection of packable projects by their cross references.
    /// </summary>
    /// <param name="packables"></param>
    /// <returns></returns>
    public static ImmutableArray<Packable> OrderByReferences(this IEnumerable<Packable> packables)
    {
        packables = packables.ThrowIfNull();

        var list = packables.Select(x => new Weighted(x)).ToList();
        foreach (var item in list)
        {
            var references = item.Packable.Project.GetReferences();
            foreach (var reference in references)
            {
                var temp = list.Find(x => string.Compare(
                    x.Packable.Project.File.Name, reference.Name, Program.Comparison) == 0);

                if (temp != null) temp.Count++;
            }
        }

        var order = list.OrderBy(x => x.Count).ToList();
        order.Reverse();
        return order.Select(x => x.Packable).ToImmutableArray();
    }

    class Weighted
    {
        public int Count = 0;
        public Packable Packable = Packable.Empty;
        public Weighted(Packable packable) => Packable = packable.ThrowIfNull();
        public override string ToString() => $"{Packable.Project.File.Name}: {Count}";
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the collection of package files found for the given packable project and mode.
    /// </summary>
    /// <param name="packable"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static ImmutableArray<File> GetPackageFiles(this Packable packable, PushMode mode)
    {
        packable = packable.ThrowIfNull();

        var name = $"{packable.Project.File.Name}.{packable.Version}";
        var list = new List<File>(); Populate(packable.Project.File.Directory);
        return list.ToImmutableArray();

        /// <summary> Invoked to populate the list.
        /// </summary>
        void Populate(Directory directory)
        {
            if ((mode == PushMode.Local && directory.IsDebug()) ||
                (mode == PushMode.NuGet && directory.IsRelease()))
            {
                var files = directory.GetFiles($"*{name}.nupkg");
                foreach (var file in files) list.Add(file);

                var symbols = directory.GetFiles($"*{name}.snupkg");
                foreach (var symbol in symbols) list.Add(symbol);
            }
            foreach (var dir in directory.GetDirectories()) Populate(dir);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Pushes the package files found for the given package and push mode.
    /// Returns the collection of files pushed.
    /// </summary>
    /// <param name="packable"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static ImmutableArray<File> Push(this Packable packable, PushMode mode)
    {
        packable = packable.ThrowIfNull();

        var source = mode == PushMode.Local ? MenuBuilder.LocalRepoSource : MenuBuilder.NuGetRepoSource;
        var files = packable.GetPackageFiles(mode);
        var list = new List<File>();

        var regulars = files.Where(x => string.Compare(x.Extension, "nupkg", Program.Comparison) == 0);
        foreach (var file in regulars)
        {
            var done = PushFile(file);
            if (done) list.Add(file);
        }
        var symbols = files.Where(x => string.Compare(x.Extension, "snupkg", Program.Comparison) == 0);
        foreach (var file in symbols)
        {
            var done = PushFile(file);
            if (done) list.Add(file);
        }
        return list.ToImmutableArray();

        /// <summary> Invoked to push the given file.
        /// </summary>
        bool PushFile(File file)
        {
            var p = new Process();
            p.StartInfo.FileName = MenuBuilder.DotNetExe;
            p.StartInfo.WorkingDirectory = file.Directory;
            p.StartInfo.Arguments = $"nuget push {file.NameAndExtension} -s {source}";

            p.Start();
            p.WaitForExit();
            return p.ExitCode == 0;
        }
    }
}
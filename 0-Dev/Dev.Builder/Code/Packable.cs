namespace Dev.Builder;

// ========================================================
public record Packable
{
    /// <summary>
    /// An empty instance.
    /// </summary>
    public static Packable Empty { get; } = new();
    protected Packable() { }

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
public static class PackableExtensions
{
    /// <summary>
    /// Determines if the given project is a packable one, or not.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static bool IsPackable(this Project project, out SemanticVersion? version)
    {
        project = project.ThrowIfNull();

        version = project.FindVersion();
        if (version != null)
        {
            var lines = _File.ReadAllLines(project.File.Path);
            foreach (var line in lines)
            {
                var ini = line.IndexOf(PackableHead, Program.Comparison);
                if (ini < 0) continue;
                ini += PackableHead.Length;

                var end = line.IndexOf(PackableTail, ini, Program.Comparison);
                if (end < 0) continue;

                var value = line[ini..end].Trim();
                return bool.TryParse(value, out var temp) && temp;
            }
        }

        return false;
    }
    const string PackableHead = "<IsPackable>";
    const string PackableTail = "</IsPackable>";

    /// <summary>
    /// Returns the version found in the given project, or null if any.
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public static SemanticVersion? FindVersion(this Project project)
    {
        project = project.ThrowIfNull();

        var lines = _File.ReadAllLines(project.File.Path);
        foreach (var line in lines)
        {
            var ini = line.IndexOf(VersionHead, Program.Comparison);
            if (ini < 0) continue;
            ini += VersionHead.Length;

            var end = line.IndexOf(VersionTail, ini, Program.Comparison);
            if (end < 0) continue;

            var value = line[ini..end].Trim();
            if (value.Length > 0) return new SemanticVersion(value);
        }

        return null;
    }
    const string VersionHead = "<Version>";
    const string VersionTail = "</Version>";

    // ----------------------------------------------------

    /// <summary>
    /// Resets the project version to the given one, provided a valid version line is found.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="version"></param>
    public static bool ResetVersion(this Project project, SemanticVersion version)
    {
        project = project.ThrowIfNull();

        var lines = _File.ReadAllLines(project.File.Path);
        var done = false;

        for (int i = 0; i< lines.Length; i++)
        {
            var line = lines[i];

            var ini = line.IndexOf(VersionHead, Program.Comparison);
            if (ini < 0) continue;
            ini += VersionHead.Length;

            var end = line.IndexOf(VersionTail, ini, Program.Comparison);
            if (end < 0) continue;

            var value = line[ini..end].Trim();
            if (value.Length == 0) continue;

            lines[i] = line.Replace(value, version);
            done = true;
            break;
        }

        if (done) _File.WriteAllLines(project.File.Path, lines);
        return done;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Selects from the given collection of projects those that are packable ones.
    /// </summary>
    /// <param name="projects"></param>
    /// <returns></returns>
    public static ImmutableArray<Packable> SelectPackables(this IEnumerable<Project> projects)
    {
        projects = projects.ThrowIfNull();

        var list = new List<Packable>();
        foreach (var project in projects)
        {
            if (project.IsPackable(out _)) list.Add(new(project));
        }
        return list.ToImmutableArray();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the collection of files on disk the given packable has generated.
    /// </summary>
    /// <param name="packable"></param>
    /// <returns></returns>
    public static ImmutableArray<File> GetPackageFiles(this Packable packable)
    {
        packable = packable.ThrowIfNull();

        var list = new List<File>(); Populate(list, packable.Project.File.Directory);
        return list.ToImmutableArray();

        /// <summary> Invoked to populate the list...
        /// </summary>
        static void Populate(List<File> list, Directory directory)
        {
            var files = directory.GetFiles("*.nupkg");
            foreach (var file in files) list.Add(file);

            var symbols = directory.GetFiles("*.snupkg");
            foreach (var symbol in symbols) list.Add(symbol);

            var dirs = directory.GetDirectories();
            foreach (var dir in dirs) Populate(list, dir);
        }
    }

    // ----------------------------------------------------

    /// <summary>
    /// Pushes the files of the given package, according to the given mode.
    /// </summary>
    /// <param name="packable"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static bool Push(this Packable packable, PushMode mode, bool deleteOld)
    {
        packable = packable.ThrowIfNull();

        var list = packable.GetPackageFiles().Where(x =>
            (mode == PushMode.Local && x.IsDebug) ||
            (mode == PushMode.NuGet && x.IsRelease))
            .ToList();

        var regular = list
            .Where(x => string.Compare(x.Extension, "nupkg", Program.Comparison) == 0)
            .OrderByDescending(x => x.Name)
            .FirstOrDefault();

        var done = false;
        if (regular != null) done = PushFile(regular, mode);

        var symbols = list
            .Where(x => string.Compare(x.Extension, "snupkg", Program.Comparison) == 0)
            .OrderByDescending(x => x.Name)
            .FirstOrDefault();

        if (symbols != null) PushFile(symbols, mode);

        if (done && deleteOld)
        {
            if (regular != null) list.Remove(regular);
            if (symbols != null) list.Remove(symbols);
            if (list.Count > 0)
            {
                WriteLine();
                WriteLine(Color.Green, "Deleting files:");
                list.DeleteMany();
            }
        }

        return done;

        /// <summary> Invoked to push the given file...
        /// </summary>
        static bool PushFile(File file, PushMode mode)
        {
            WriteLine();
            Write(Color.Green, "Pushing file: "); Write(file.Path);
            Write(Color.Green, " For mode: "); WriteLine(mode.ToString());

            var source = mode == PushMode.Local ? "local" : "https://api.nuget.org/v3/index.json";

            var p = new Process();
            p.StartInfo.FileName = Program.NuGetExe;
            p.StartInfo.WorkingDirectory = file.Directory.Path;
            p.StartInfo.Arguments = $"push {file.NameAndExtension} -Source {source}";
            p.Start();
            p.WaitForExit();

            return p.ExitCode == 0;
        }
    }

    // ----------------------------------------------------

    class Weighted
    {
        public Weighted(Packable packable) => Packable = packable;
        public override string ToString() => $"{Packable.Project.File.Name}: {Count}";
        public int Count { get; set; }
        public Packable Packable { get; }
    }

    /// <summary>
    /// Orders the collection of packable projects by their cross dependencies.
    /// </summary>
    /// <param name="packables"></param>
    /// <returns></returns>
    public static ImmutableArray<Packable> OrderByDependencies(this IEnumerable<Packable> packables)
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
}
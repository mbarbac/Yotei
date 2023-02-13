namespace Dev.Builder;

// ========================================================
/// <summary>
/// Represents a solution project.
/// </summary>
public record Project
{
    /// <summary>
    /// An empty instance.
    /// </summary>
    public static Project Empty { get; } = new();
    protected Project() { }

    /// <summary>
    /// Initializes a new instance using the given file.
    /// </summary>
    /// <param name="file"></param>
    public Project(File file)
    {
        File = file;

        if (string.Compare(file.Extension, "csproj", Program.Comparison) != 0)
            throw new ArgumentException($"File extension is not 'csproj': {File}");
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => File.Name;

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    public static implicit operator string(Project project) => project.File.Path;

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    public static implicit operator Project(string path) => new(new File(path));

    /// <summary>
    /// The file of this project.
    /// </summary>
    public File File
    {
        get => _Value;
        init => _Value = value.ThrowIfNull();
    }
    File _Value = File.Empty;
}

// ========================================================
public static class ProjectExtensions
{
    /// <summary>
    /// Determines if the given project is a package one, or not.
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
                var ini = line.IndexOf(IsPackableHead, Program.Comparison);
                if (ini < 0) continue;
                ini += IsPackableHead.Length;

                var end = line.IndexOf(IsPackableTail, ini, Program.Comparison);
                if (end < 0) continue;

                var value = line[ini..end].Trim();
                return bool.TryParse(value, out var temp) && temp;
            }
        }

        return false;
    }
    const string IsPackableHead = "<IsPackable>";
    const string IsPackableTail = "</IsPackable>";

    // ----------------------------------------------------

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

    /// <summary>
    /// Resets the version of this project to the given one.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static bool SetVersion(this Project project, SemanticVersion version)
    {
        project = project.ThrowIfNull();
        version = version.ThrowIfNull();

        var lines = _File.ReadAllLines(project.File.Path);
        var done = false;

        for (int i = 0; i < lines.Length; i++)
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
    /// Selects from the given list of projects those that are packable ones.
    /// </summary>
    /// <param name="projects"></param>
    /// <returns></returns>
    public static ImmutableArray<Packable> SelectPackables(this IEnumerable<Project> projects)
    {
        projects = projects.ThrowIfNull();

        var list = new List<Packable>();
        foreach (var project in projects)
        {
            if (project.IsPackable(out var version)) list.Add(new Packable(project, version!));
        }

        return list.ToImmutableArray();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the collection of package references in the given project
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public static ImmutableArray<PackageReference> GetReferences(this Project project)
    {
        project = project.ThrowIfNull();

        var list = new List<PackageReference>();
        var lines = _File.ReadAllLines(project.File.Path);

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            if (!line.Contains("<PackageReference", Program.Comparison)) continue;

            var head = $"Include=\"";
            var ini = line.IndexOf(head, Program.Comparison); if (ini < 0) continue;
            ini += head.Length;
            var end = line.IndexOf("\"", ini, Program.Comparison); if (end < 0) continue;

            var name = line[ini..end].Trim();
            if (name.Length == 0) continue;

            head = $"Version=\"";
            ini = line.IndexOf(head, Program.Comparison); if (ini < 0) continue;
            ini += head.Length;
            end = line.IndexOf("\"", ini, Program.Comparison); if (end < 0) continue;

            var version = line[ini..end].Trim();
            if (version.Length == 0) continue;

            var item = new PackageReference(name, version);
            list.Add(item);
        }

        return list.ToImmutableArray();
    }

    /// <summary>
    /// Gets the collection of package references contained in the given projects.
    /// </summary>
    /// <param name="projects"></param>
    /// <returns></returns>
    public static ImmutableArray<PackageReference> GetReferences(this IEnumerable<Project> projects)
    {
        projects = projects.ThrowIfNull();

        var list = new List<PackageReference>();
        foreach (var project in projects)
        {
            var items = project.GetReferences();
            foreach (var item in items)
            {
                var temp = list.Find(x => string.Compare(x.Name, item.Name, Program.Comparison) == 0);
                if (temp == null) list.Add(item);
            }
        }

        return list.ToImmutableArray();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Reloads into this project the package whose name and version are given, by removing and
    /// then adding it again from the source specified by the given mode. This methods keeps any
    /// original attributes and xml nodes for that reference.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="name"></param>
    /// <param name="version"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static bool ReloadPackage(this Project project, string name, string version, PushMode mode)
    {
        project = project.ThrowIfNull();
        name = name.NotNullNotEmpty();
        version = version.NotNullNotEmpty();

        var lines = _File.ReadAllLines(project.File.Path);
        var done = false;

        WriteLine();
        WriteLine(Color.Green, "Removing package...");
        done = project.RemovePackage(name);
        if (!done) WriteLine(Color.Red, "Cannot remove package!");

        WriteLine();
        WriteLine(Color.Green, "Adding package...");
        done = project.AddPackage(name, version, mode);
        if (!done) WriteLine(Color.Red, "Cannot add package!");

        if (done)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];

                if (!line.Contains("<PackageReference", Program.Comparison)) continue;

                var head = $"Include=\"";
                var ini = line.IndexOf(head, Program.Comparison); if (ini < 0) continue;
                ini += head.Length;
                var end = line.IndexOf("\"", ini, Program.Comparison); if (end < 0) continue;

                var oldname = line[ini..end].Trim();
                if (oldname.Length == 0) continue;
                if (string.Compare(name, oldname, Program.Comparison) != 0) continue;

                head = $"Version=\"";
                ini = line.IndexOf(head, Program.Comparison); if (ini < 0) continue;
                ini += head.Length;
                end = line.IndexOf("\"", ini, Program.Comparison); if (end < 0) continue;

                var oldversion = line[ini..end].Trim();
                if (oldversion.Length == 0) continue;

                lines[i] = line.Replace(oldversion, version);
                break;
            }
        }

        _File.WriteAllLines(project.File.Path, lines);
        return done;
    }

    /// <summary>
    /// Removes from this project the package whose name is given.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool RemovePackage(this Project project, string name)
    {
        project = project.ThrowIfNull();
        name = name.NotNullNotEmpty();

        var p = new Process();
        p.StartInfo.FileName = MenuBuilder.DotNetExe;
        p.StartInfo.WorkingDirectory = project.File.Directory;
        p.StartInfo.Arguments = $"remove package {name}";

        p.Start();
        p.WaitForExit();
        return p.ExitCode == 0;
    }

    /// <summary>
    /// Adds to this project a reference to the package whose name and version is given, from
    /// the source specified by the given mode.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="name"></param>
    /// <param name="version"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static bool AddPackage(this Project project, string name, string version, PushMode mode)
    {
        project = project.ThrowIfNull();
        name = name.NotNullNotEmpty();
        version = version.NotNullNotEmpty();

        var source = mode == PushMode.Local ? MenuBuilder.LocalRepoPath : MenuBuilder.NuGetRepoSource;
        var p = new Process();
        p.StartInfo.FileName = MenuBuilder.DotNetExe;
        p.StartInfo.WorkingDirectory = project.File.Directory;
        p.StartInfo.Arguments = $"add package {name} -v {version} -s {source}";

        p.Start();
        p.WaitForExit();
        var done = p.ExitCode == 0;

        return done;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Compiles the given project.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static bool Compile(this Project project, CompileMode mode)
    {
        project = project.ThrowIfNull();

        var p = new Process();
        p.StartInfo.FileName = MenuBuilder.DotNetExe;
        p.StartInfo.WorkingDirectory = project.File.Directory.Path;
        p.StartInfo.Arguments = $"build -c {mode} {project.File.NameAndExtension}";

        p.Start();
        p.WaitForExit();
        return p.ExitCode == 0;
    }
}
namespace Dev.Builder;

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
            var lines = _File.ReadAllLines(project.Path);
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

        var lines = _File.ReadAllLines(project.Path);
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
    /// Sets the version of this project to the given one.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static bool SetVersion(this Project project, SemanticVersion version)
    {
        project = project.ThrowIfNull();
        version = version.ThrowIfNull();

        var lines = _File.ReadAllLines(project.Path);
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
    /// Returns from the given list of projects those that are packable ones.
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
    /// Gets the collection of references in the given project
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public static ImmutableArray<Reference> GetReferences(this Project project)
    {
        project = project.ThrowIfNull();

        var list = new List<Reference>();
        var lines = _File.ReadAllLines(project.Path);

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

            var item = new Reference(name, version);
            list.Add(item);
        }

        return list.ToImmutableArray();
    }

    /// <summary>
    /// Gets the collection of package references contained in the given projects.
    /// </summary>
    /// <param name="projects"></param>
    /// <returns></returns>
    public static ImmutableArray<Reference> GetReferences(this IEnumerable<Project> projects)
    {
        projects = projects.ThrowIfNull();

        var list = new List<Reference>();
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
    /// Compiles the given project.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    public static bool Compile(this Project project, CompileMode mode)
    {
        project = project.ThrowIfNull();

        Write(Program.Color, "Compiling: "); Write(project.Name);
        Write(Program.Color, " For mode: "); WriteLine($"{mode}");

        var code = Command.Execute(
            Program.DotNetExe,
             $"build -c {mode} {project.File.NameAndExtension}",
            project.File.Directory.Path);

        return code == 0;
    }
}
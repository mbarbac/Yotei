using System.ComponentModel.DataAnnotations;

namespace Dev.Builder;

// ========================================================
/// <summary>
/// Represents a package reference, as found in a project file.
/// </summary>
public record PackageReference
{
    /// <summary>
    /// An empty instance.
    /// </summary>
    public static PackageReference Empty { get; } = new();
    protected PackageReference() { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="version"></param>
    public PackageReference(string name, SemanticVersion version)
    {
        Name = name;
        Version = version;
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => $"{Name}.{Version}";

    /// <summary>
    /// The name of the package this instance refers to.
    /// </summary>
    public string Name
    {
        get => _Name;
        init => _Name = value.ThrowIfNull().Trim();
    }
    string _Name = string.Empty;

    /// <summary>
    /// The version of this package this instace refers to.
    /// </summary>
    public SemanticVersion Version
    {
        get => _Version;
        init => _Version = value.ThrowIfNull(nameof(Version));
    }
    SemanticVersion _Version = SemanticVersion.Empty;
}

// ========================================================
public static class PackageReferenceExtensions
{
    /// <summary>
    /// Gets the collection of package references contained in the given project.
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
    /// Gets the collection of package references contained in the given project, provided their
    /// names match with the name of any of the given packable ones.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="packables"></param>
    /// <returns></returns>
    public static ImmutableArray<PackageReference> GetReferences(
        this Project project,
        IEnumerable<Packable> packables)
    {
        project = project.ThrowIfNull();
        packables = packables.ThrowIfNull();

        var list = new List<PackageReference>();
        var items = project.GetReferences();
        foreach (var item in items)
        {
            var temp = packables.FirstOrDefault(x =>
                string.Compare(item.Name, x.Project.File.Name, Program.Comparison) == 0);

            if (temp != null) list.Add(item);
        }

        return list.ToImmutableArray();
    }

    /// <summary>
    /// Gets the collection of package references contained in the given projects, provided their
    /// names match with the name of any of the given packable ones.
    /// </summary>
    /// <param name="projects"></param>
    /// <param name="packables"></param>
    /// <returns></returns>
    public static ImmutableArray<PackageReference> GetReferences(
        this IEnumerable<Project> projects,
        IEnumerable<Packable> packables)
    {
        projects = projects.ThrowIfNull();
        packables = packables.ThrowIfNull();

        var list = new List<PackageReference>();
        foreach (var project in projects)
        {
            var items = project.GetReferences(packables);
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
    /// Updates, in the given project, the references to the given with the new version.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="reference"></param>
    /// <param name="version"></param>
    public static bool UpdateReference(
        this Project project,
        PackageReference reference, SemanticVersion version)
    {
        project = project.ThrowIfNull();
        reference = reference.ThrowIfNull();
        version = version.ThrowIfNull();

        var lines = _File.ReadAllLines(project.File.Path);
        var done = false;

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

            var value = line[ini..end].Trim();
            if (value.Length == 0) continue;

            if (string.Compare(name, reference.Name, Program.Comparison) != 0) continue;
            lines[i] = line.Replace(value, version);
            done = true;
            break;
        }

        if (done) _File.WriteAllLines(project.File.Path, lines);
        return done;
    }
}
/*
    project = project.ThrowIfNull();

        var list = new List<PackageReference>();
        var lines = _File.ReadAllLines(project.File.Path);

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            

            var version = 

            var item = new PackageReference(name, version);
            list.Add(item);
        }

        return list.ToImmutableArray();
    }
 */
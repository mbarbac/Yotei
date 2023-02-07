namespace Dev.Builder;

// ========================================================
/// <summary>
/// Represents a reference to a package found in a project file.
/// </summary>
public record PackageReference
{
    protected static StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

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
    public override string ToString() => FullName;

    /// <summary>
    /// Determines if this instance is an empty one.
    /// </summary>
    public virtual bool IsEmpty => Name.Length == 0 && Version.IsEmpty;

    /// <summary>
    /// The full name of this package, including its public name and version.
    /// </summary>
    public string FullName => $"{Name}.{Version}";

    /// <summary>
    /// The name of the package this instance refers to.
    /// </summary>
    public string Name
    {
        get => _Name;
        init => _Name = value.NotNullNotEmpty(valueName: nameof(Name));
    }
    string _Name = string.Empty;

    /// <summary>
    /// The version of the package this instance refers to.
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
    static StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    // ----------------------------------------------------

    /// <summary>
    /// Gets the collection of package references contained in the given project.
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public static ImmutableList<PackageReference> GetReferences(this Project project)
    {
        project = project.ThrowIfNull();

        var list = new List<PackageReference>();
        if (!project.IsEmpty)
        {
            var lines = _File.ReadAllLines(project.FullName);
            foreach (var line in lines)
            {
                if (!line.Contains("<PackageReference", Comparison)) continue;

                var head = $"Include=\"";
                var ini = line.IndexOf(head, Comparison); if (ini < 0) continue;
                ini += head.Length;
                var end = line.IndexOf("\"", ini, Comparison); if (end < 0) continue;

                var name = line[ini..end].Trim();
                if (name.Length == 0) continue;

                head = $"Version=\"";
                ini = line.IndexOf(head, Comparison); if (ini < 0) continue;
                ini += head.Length;
                end = line.IndexOf("\"", ini, Comparison); if (end < 0) continue;

                var version = line[ini..end].Trim();
                if (version.Length == 0) continue;

                var item = new PackageReference(name, new SemanticVersion(version));
                list.Add(item);
            }
        }

        return list.ToImmutableList();
    }

    /// <summary>
    /// Gets the collection of package references contained in the given projects.
    /// </summary>
    /// <param name="project"></param>
    /// <returns></returns>
    public static ImmutableList<PackageReference> GetReferences(this IEnumerable<Project> projects)
    {
        projects = projects.ThrowIfNull();

        var list = new List<PackageReference>();
        foreach (var project in projects)
        {
            var temps = project.GetReferences();
            foreach (var temp in temps)
            {
                var item = list.Find(x => string.Compare(x.Name, temp.Name, Comparison) == 0);
                if (item == null) list.Add(temp);
            }
        }

        return list.ToImmutableList();
    }

    // ----------------------------------------------------

    class Weighted
    {
        public Weighted() { }
        public int Count = 0;
        public Packable Packable = Packable.Empty;
        public override string ToString() => $"{Packable.Name}: {Count}";
    }

    /// <summary>
    /// Orders the given collection of packable projects by its cross dependencies.
    /// </summary>
    /// <param name="packables"></param>
    /// <returns></returns>
    public static ImmutableList<Packable> OrderByDependencies(this IEnumerable<Packable> packables)
    {
        packables = packables.ThrowIfNull();

        // Phase 1: populating the weighted list keeping original order...
        var list = new List<Weighted>();
        foreach (var packable in packables)
        {
            // Adding by finding a cross reference...
            var rs = packable.GetReferences();
            foreach (var r in rs)
            {
                // If the list does not contain the reference...
                var w1 = list.Find(x => string.Compare(x.Packable.Name, r.Name, Comparison) == 0);
                if (w1 == null)
                {
                    // And that reference is among the given ones...
                    var p = packables.FirstOrDefault(x => string.Compare(x.Name, r.Name, Comparison) == 0);
                    if (p != null)
                    {
                        w1 = new Weighted() { Packable = p };
                        list.Add(w1);
                    }
                }
            }

            // We still may need to add the package...
            var w2 = list.Find(x => string.Compare(x.Packable.Name, packable.Name, Comparison) == 0);
            if (w2 == null)
            {
                w2 = new Weighted() { Packable = packable };
                list.Add(w2);
            }
        }

        // Phase 2: increasing counts as needed...
        foreach (var w in list)
        {
            var rs = w.Packable.GetReferences();
            foreach (var r in rs)
            {
                var wp = list.Find(x => string.Compare(x.Packable.Name, r.Name, Comparison) == 0);
                if (wp != null) wp.Count++;
            }
        }

        // Phase 3: the ones with bigger counts shall appear first...
        var items = new List<Weighted>();
        foreach (var temp in list)
        {
            // If it already appears in items...
            var done = false;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (!ReferenceEquals(temp, item))
                {
                    if (temp.Count > item.Count)
                    {
                        items.Insert(i, temp);
                        done = true;
                        break;
                    }
                }
            }

            // If not, adding it...
            if (!done) items.Add(temp);
        }

        // Returning...
        return items.Select(x => x.Packable).ToImmutableList();
    }
}
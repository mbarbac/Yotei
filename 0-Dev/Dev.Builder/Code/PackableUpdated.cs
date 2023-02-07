namespace Dev.Builder;

// ========================================================
/// <summary>
/// Represents a packable project whose version has been updated.
/// </summary>
public record PackableUpdated
{
    /// <summary>
    /// An empty instance.
    /// </summary>
    public static PackableUpdated Empty { get; } = new();
    private PackableUpdated() { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="newversion"></param>
    public PackableUpdated(Packable source, SemanticVersion newversion)
    {
        Source = source;
        NewVersion = newversion;
    }

    /// <inheritdoc>
    /// </inheritdoc>
    public override string ToString() => $"{Source} ==> {NewVersion}";

    /// <summary>
    /// Determines if this instance is an empty one.
    /// </summary>
    public bool IsEmpty => Source.IsEmpty && NewVersion.IsEmpty;

    /// <summary>
    /// The original package that has been updated.
    /// </summary>
    public Packable Source
    {
        get => _Source;
        init => _Source = value.ThrowIfNull(nameof(Source));
    }
    Packable _Source = Packable.Empty;

    /// <summary>
    /// The new version in the source packable.
    /// </summary>
    public SemanticVersion NewVersion
    {
        get => _NewVersion;
        init => _NewVersion = value.ThrowIfNull(nameof(NewVersion));
    }
    SemanticVersion _NewVersion = SemanticVersion.Empty;
}

// ========================================================
public static class PackableUpdatedExtensions
{
    static StringComparison Comparison = StringComparison.OrdinalIgnoreCase;

    // ----------------------------------------------------

    /// <summary>
    /// Updates the version of the given packable project according to the given options. The
    /// <paramref name="beta"/> value can be left to null unless a beta-alike increase is
    /// requested. Returns either the updated package instance, or null if the version has not
    /// been updated.
    /// </summary>
    /// <param name="packable"></param>
    /// <param name="options"></param>
    /// <param name="beta"></param>
    /// <returns></returns>
    public static PackableUpdated? UpdateVersion(
        this Packable packable, SemanticOptions options, string? beta = null)
    {
        packable = packable.ThrowIfNull();

        if (options == SemanticOptions.None) return null;

        var lines = _File.ReadAllLines(packable.FullName);
        var done = false;
        SemanticVersion? oldVersion = null;
        SemanticVersion? newVersion = null;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];

            oldVersion = GetVersionFromLine(line);
            if (oldVersion == null) continue;

            try { newVersion = oldVersion.Increase(options, beta); }
            catch (SemanticException e)
            {
                WriteLine();
                Write(Color.Red, "Error increasing version of: "); WriteLine(packable.FullName);
                Write(Color.Red, "Because: "); WriteLine(e.Message);
                return null;
            }

            if (ReferenceEquals(oldVersion, newVersion)) continue;

            lines[i] = line.Replace(oldVersion.ToString(), newVersion.ToString());
            done = true;
            break;
        }

        if (done)
        {
            WriteLine();
            Write(Color.Cyan, "Updating version of: "); Write(packable.Name);
            Write(Color.Cyan, " From: "); Write(oldVersion!.ToString());
            Write(Color.Cyan, " To: "); WriteLine(newVersion!.ToString());

            _File.WriteAllLines(packable.FullName, lines);

            var item = new PackableUpdated(packable, newVersion);
            return item;
        }

        return null;

        /// <summary> Get the semantic version contained in the given line, or null if any...
        /// </summary>
        static SemanticVersion? GetVersionFromLine(string line)
        {
            var ini = line.IndexOf(VersionHead, Comparison);
            if (ini < 0) return null;
            ini += VersionHead.Length;

            var end = line.IndexOf(VersionTail, ini, Comparison);
            if (end < 0) return null;

            var version = line[ini..end].Trim();
            if (version.Length == 0) return null;

            var item = new SemanticVersion(version);
            return item;
        }
    }
    const string VersionHead = "<Version>";
    const string VersionTail = "</Version>";

    // ----------------------------------------------------

    /// <summary>
    /// Updates in every project of the given collection the references to the given packable
    /// project to the new given version. Returns the collection of projects updated.
    /// </summary>
    /// <param name="projects"></param>
    /// <param name="packable"></param>
    /// <param name="newVersion"></param>
    /// <returns></returns>
    public static ImmutableList<Project> UpdateReferences(
        this IEnumerable<Project> projects, Packable packable, SemanticVersion newVersion)
    {
        projects = projects.ThrowIfNull();
        packable = packable.ThrowIfNull();
        newVersion = newVersion.ThrowIfNull();

        var list = new List<Project>();
        foreach (var project in projects)
        {
            var lines = _File.ReadAllLines(project.FullName);
            var done = false;

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
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

                if (string.Compare(name, packable.Name, Comparison) != 0) continue;
                lines[i] = line.Replace(version, newVersion.ToString());
                done = true;
                break;
            }

            if (done)
            {
                _File.WriteAllLines(project.FullName, lines);
                list.Add(project);
            }
        }

        return list.ToImmutableList();
    }
}
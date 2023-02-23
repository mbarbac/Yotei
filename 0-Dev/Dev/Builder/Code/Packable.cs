namespace Dev.Builder;

// ========================================================
/// <summary>
/// Represents a packable project.
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => $"{Project} ({Version})";

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    public static implicit operator string(Packable packable) => packable.ToString();

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

    /// <summary>
    /// The name of this packable project.
    /// </summary>
    public string Name => Project.Name;

    /// <summary>
    /// The path of this packable project.
    /// </summary>
    public string Path => Project.Path;
}
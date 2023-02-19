namespace Dev.Builder;

// ========================================================
/// <summary>
/// Represents a reference to a package.
/// </summary>
public record Reference
{
    /// <summary>
    /// An empty instance.
    /// </summary>
    public static Reference Empty { get; } = new();
    protected Reference() { }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="version"></param>
    public Reference(string name, SemanticVersion version)
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
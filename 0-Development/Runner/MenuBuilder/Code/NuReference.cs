namespace Runner;

// ========================================================
/// <summary>
/// Represents a project line that contains a NuGet package reference.
/// </summary>
public class NuReference
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="line"></param>
    public NuReference(ProjectLine line) => Line = line.ThrowWhenNull();

    /// <summary>
    /// The line this instance refers to.
    /// </summary>
    public ProjectLine Line { get; }

    /// <summary>
    /// The package name.
    /// </summary>
    public string Name
    {
        get
        {
            if (!Line.IsNuReference(out var name, out _)) throw new InvalidOperationException(
                "Project line contains no valid NuGet package name.")
                .WithData(Line);

            return name;
        }
        set
        {
            value = value.NotNullNotEmpty();

            if (!Line.SetNuReferenceName(value)) throw new InvalidOperationException(
                "Cannot set the NuGet package name on the project line.")
                .WithData(Line);
        }
    }

    /// <summary>
    /// The package version.
    /// </summary>
    public SemanticVersion Version
    {
        get
        {
            if (!Line.IsNuReference(out _, out var version)) throw new InvalidOperationException(
                "Project line contains no valid NuGet package version.")
                .WithData(Line);

            return version;
        }
        set
        {
            value.ThrowWhenNull();

            if (!Line.SetNuReferenceVersion(value)) throw new InvalidOperationException(
                "Cannot set the NuGet package version on the project line.")
                .WithData(Line);
        }
    }
}
namespace Runner.Builder;

// ========================================================
/// <summary>
/// Represents a project line that holds a NuGet package reference.
/// </summary>
public class NuPackageReference
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="line"></param>
    public NuPackageReference(ProjectLine line)
    {
        Line = line.ThrowWhenNull();

        if (!Validate(line, out _, out _))
            throw new ArgumentException(
                "The given project line is not a valid NuGet package reference.")
                .WithData(line);
    }

    /// <inheritdoc/>
    public override string ToString() => Line.Value;

    /// <summary>
    /// The project line this instance refers to.
    /// </summary>
    public ProjectLine Line { get; }

    /// <summary>
    /// The name of the NuGet package.
    /// </summary>
    public string Name
    {
        get => Validate(Line, out var name, out _)
            ? name
            : throw new InvalidOperationException(
                "Line contains no valid name specification.")
                .WithData(Line);

        set
        {
            value.ThrowWhenNull();

            if (!Line.SetWrappedValue(INCLUDE, TAIL, value)) throw new ArgumentException(
                "Invalid NuGet name specification.")
                .WithData(value);
        }
    }

    /// <summary>
    /// The version of the NuGet package.
    /// </summary>
    public SemanticVersion Version
    {
        get => Validate(Line, out _, out var version)
            ? version
            : throw new InvalidOperationException(
                "Line contains no valid semantic version specification.")
                .WithData(Line);

        set
        {
            value.ThrowWhenNull();

            if (!Line.SetWrappedValue(VERSION, TAIL, value)) throw new ArgumentException(
                "Invalid NuGet semantic version specification.")
                .WithData(value);
        }
    }

    // ----------------------------------------------------

    const string PACKAGEREFERENCE = "<PackageReference";
    const string INCLUDE = "Include=\"";
    const string VERSION = "Version=\"";
    const string TAIL = "\"";

    /// <summary>
    /// Validates that the given project line contains a valid NuGet package reference. If so,
    /// returns its name and its semantic version in the respective out arguments.
    /// </summary>
    /// <param name="line"></param>
    /// <param name="name"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static bool Validate(
        ProjectLine line,
        [NotNullWhen(true)] out string? name,
        [NotNullWhen(true)] out SemanticVersion? version)
    {
        line.ThrowWhenNull();

        while (true)
        {
            if (!line.Contains(PACKAGEREFERENCE)) break;

            if (!line.GetWrappedValue(INCLUDE, TAIL, out name)) break;
            if ((name = name.NullWhenEmpty()) == null) break;

            if (!line.GetWrappedValue(VERSION, TAIL, out var temp)) break;
            if ((temp = temp.NullWhenEmpty()) == null) break;
            version = new(temp);

            return true;
        }

        name = null;
        version = null;
        return false;
    }
}
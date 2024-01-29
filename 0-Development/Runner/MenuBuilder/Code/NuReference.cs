namespace Runner.Builder;

// ========================================================
/// <summary>
/// Represents a wrapper over a project line that carries a NuGet package reference.
/// </summary>
public class NuReference
{
    /// <summary>
    /// Initializes a new instance that refers to the given line.
    /// </summary>
    /// <param name="line"></param>
    public NuReference(ProjectLine line)
    {
        Line = line.ThrowWhenNull();

        if (!IsValid(line, out _, out _))
            throw new ArgumentException("The given project line is not a valid one.")
            .WithData(line);
    }

    /// <inheritdoc/>
    public override string ToString() => Line.Value;

    /// <summary>
    /// The project line this instance refers to.
    /// </summary>
    public ProjectLine Line { get; }

    /// <summary>
    /// The name of the package, or null if the line is not a valid one.
    /// </summary>
    public string? Name
    {
        get => IsValid(Line, out var name, out _) ? name : null;
        set
        {
            value = value.ThrowWhenNull();

            var done = Line.SetValue(INCLUDE, TAIL, value!);

            if (!done) throw new InvalidOperationException(
                "Cannot set the package name on this line.")
                .WithData(Line)
                .WithData(value);
        }
    }

    /// <summary>
    /// The version of the package, or null if the line is not a valid one.
    /// </summary>
    public SemanticVersion? Version
    {
        get => IsValid(Line, out _, out var version) ? version : null;
        set
        {
            value = value.ThrowWhenNull();

            var done = Line.SetValue(VERSION, TAIL, value!);

            if (!done) throw new InvalidOperationException(
                "Cannot set the package version on this line.")
                .WithData(Line)
                .WithData(value);
        }
    }

    // ----------------------------------------------------

    const string PACKAGEREFERENCE = "<PackageReference";
    const string INCLUDE = "Include=\"";
    const string VERSION = "Version=\"";
    const string TAIL = "\"";

    /// <summary>
    /// Determines if a given line carries a NuGet package reference and, if so, obtains its
    /// package name and semantic version.
    /// </summary>
    /// <param name="line"></param>
    /// <param name="name"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static bool IsValid(
        ProjectLine line,
        [NotNullWhen(true)] out string? name,
        [NotNullWhen(true)] out SemanticVersion? version)
    {
        line = line.ThrowWhenNull();

        while (true)
        {
            if (!line.Contains(PACKAGEREFERENCE)) break;

            if (!line.GetValue(INCLUDE, TAIL, out name)) break;
            if ((name = name.NullWhenEmpty()) == null) break;

            if (!line.GetValue(VERSION, TAIL, out var temp)) break;
            if ((temp = temp.NullWhenEmpty()) == null) break;
            version = temp;

            return true;
        }

        name = null;
        version = null;
        return false;
    }
}
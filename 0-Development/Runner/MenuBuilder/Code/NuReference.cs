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

    /// <inheritdoc/>
    public override string ToString() => Line.ToString();

    /// <summary>
    /// The line this instance refers to.
    /// </summary>
    public ProjectLine Line { get; }

    /// <summary>
    /// The name of the NuGet Package this instance refers to.
    /// </summary>
    public string Name
    {
        get
        {
            if (!IsReference(Line, out var name, out _)) throw new InvalidOperationException(
                "Project line contains no valid NuGet Package name.")
                .WithData(Line);

            return name;
        }
        set
        {
            if (!SetReferenceName(Line, value)) throw new InvalidOperationException(
                "Project line contains no valid NuGet Package name.")
                .WithData(Line);
        }
    }

    /// <summary>
    /// The version of the NuGet Package this instance refers to.
    /// </summary>
    public SemanticVersion Version
    {
        get
        {
            if (!IsReference(Line, out _, out var version)) throw new InvalidOperationException(
                "Project line contains no valid NuGet Package version.")
                .WithData(Line);

            return version;
        }
        set
        {
            if (!SetReferenceVersion(Line, value)) throw new InvalidOperationException(
                "Project line contains no valid NuGet Package version.")
                .WithData(Line);
        }
    }

    // ----------------------------------------------------

    const string PACKAGEREFERENCE = "<PackageReference";
    const string INCLUDE = "Include=\"";
    const string VERSION = "Version=\"";
    const string TAIL = "\"";

    /// <summary>
    /// Determines if the given line represents a NuGet Package reference, or not.
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    public static bool IsReference(ProjectLine line) => IsReference(line, out _, out _);

    /// <summary>
    /// Determines if the given line represents a NuGet Package reference, or not. If so, then
    /// this method returns the package name and version in the out arguments.
    /// </summary>
    /// <param name="line"></param>
    /// <param name="name"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static bool IsReference(
        ProjectLine line,
        [NotNullWhen(true)] out string? name,
        [NotNullWhen(true)] out SemanticVersion? version)
    {
        line.ThrowWhenNull();
        name = null;
        version = null;

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

        return false;
    }

    /// <summary>
    /// Sets the package name, provided that the given line represents a valid NuGet Package
    /// reference.
    /// </summary>
    /// <param name="line"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static bool SetReferenceName(ProjectLine line, string name)
    {
        line.ThrowWhenNull();
        name = name.NotNullNotEmpty();

        if (!line.Contains(PACKAGEREFERENCE)) return false;
        return line.SetWrappedValue(INCLUDE, TAIL, name);
    }

    /// <summary>
    /// Sets the package version, provided that the given line represents a valid NuGet Package
    /// reference.
    /// </summary>
    /// <param name="line"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    public static bool SetReferenceVersion(ProjectLine line, SemanticVersion version)
    {
        line.ThrowWhenNull();
        version.ThrowWhenNull();
        if (version.IsEmpty) throw new ArgumentException("Version cannot be an empty one.");

        if (!line.Contains(PACKAGEREFERENCE)) return false;
        return line.SetWrappedValue(VERSION, TAIL, version);
    }
}
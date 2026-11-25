namespace Runner;

// ========================================================
/// <summary>
/// Represents a line in a project file.
/// </summary>
public class ProjectFileLine
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public ProjectFileLine(string value) => Value = value;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Value.Trim();

    // ----------------------------------------------------

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="item"></param>
    public static implicit operator string(ProjectFileLine item) => item.ThrowWhenNull().Value;

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator ProjectFileLine(string value) => new(value);

    /// <summary>
    /// The actual contents of this line.
    /// </summary>
    public string Value
    {
        get;
        set => field = value.ThrowWhenNull();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract the value wrapped by the given head and tail sequences.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    /// <param name="value"></param>
    /// <param name="ignoreCase"></param>
    /// <returns></returns>
    public bool Extract(
        string head, string tail,
        [NotNullWhen(true)] out string? value,
        StringComparison ignoreCase = StringComparison.Ordinal)
    {
        head = head.NotNullNotEmpty(trim: true);
        tail = tail.NotNullNotEmpty(trim: true);
        value = null;

        var ini = Value.IndexOf(head, ignoreCase);
        if (ini < 0) return false;
        ini += head.Length;

        var end = Value.IndexOf(tail, ini, ignoreCase);
        if (end < 0) return false;

        value = Value[ini..end];
        return true;
    }

    /// <summary>
    /// Tries to update the value wrapped by the given head and tail sequences.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool Update(
        string head, string tail,
        string value,
        StringComparison comparison = StringComparison.Ordinal)
    {
        head = head.NotNullNotEmpty(trim: true);
        tail = tail.NotNullNotEmpty(trim: true);
        ArgumentNullException.ThrowIfNull(value);

        var ini = Value.IndexOf(head, comparison);
        if (ini < 0) return false;
        ini += head.Length;

        var end = Value.IndexOf(tail, ini, comparison);
        if (end < 0) return false;

        var old = Value[..ini];
        old += value;
        old += Value[end..];

        Value = old;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract the value wrapped by the given XML delimiter, which must be provided
    /// without any angle bracket.
    /// </summary>
    /// <param name="delimiter"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool ExtractXML(
        string delimiter,
        [NotNullWhen(true)] out string? value,
        StringComparison comparison = StringComparison.Ordinal)
    {
        delimiter = delimiter.NotNullNotEmpty(trim: true);

        var head = $"<{delimiter}>";
        var tail = $"</{delimiter}>";
        return Extract(head, tail, out value, comparison);
    }

    /// <summary>
    /// Tries to update the value wrapped by the given XML delimiter, which must be provided
    /// without any angle bracket.
    /// </summary>
    /// <param name="delimiter"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool UpdateXML(
        string delimiter,
        string value,
        StringComparison comparison = StringComparison.Ordinal)
    {
        delimiter = delimiter.NotNullNotEmpty(trim: true);
        ArgumentNullException.ThrowIfNull(value);

        var head = $"<{delimiter}>";
        var tail = $"</{delimiter}>";
        return Update(head, tail, value, comparison);
    }

    // ----------------------------------------------------

    const string PACKAGEREFERENCE = "<PackageReference";
    const string NUINCLUDE = "Include=\"";
    const string NUVERSION = "Version=\"";
    const string NUTAIL = "\"";

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this line represents a NuGet package reference.
    /// </summary>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool IsNuReference(
        StringComparison comparison = StringComparison.Ordinal)
        => IsNuReference(out _, out _, comparison);

    /// <summary>
    /// Determines if this line represents a NuGet package reference and, if so, returns the
    /// package name and version in the out arguments.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="version"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool IsNuReference(
        [NotNullWhen(true)] out string? name,
        [NotNullWhen(true)] out SemanticVersion? version,
        StringComparison comparison = StringComparison.Ordinal)
    {
        name = null;
        version = null;

        if (!Value.Contains(PACKAGEREFERENCE, comparison)) return false;
        if (!GetNuPackageName(out name)) return false;
        if (!GetNuPackageVersion(out version)) return false;
        return true;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to obtain a NuGet package name from this line.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool GetNuPackageName([NotNullWhen(true)] out string? name)
    {
        if (!Extract(NUINCLUDE, NUTAIL, out name)) return false;

        if ((name = name.NullWhenEmpty(trim: true)) is null) return false;
        return true;
    }

    /// <summary>
    /// Tries to update the NuGet package name in this line.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool UpdateNuPackageName(string name)
    {
        name = name.NotNullNotEmpty(trim: true);
        return Update(NUINCLUDE, NUTAIL, name);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to obtain a NuGet package version from this line.
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public bool GetNuPackageVersion([NotNullWhen(true)] out SemanticVersion? version)
    {
        version = null;

        var done = Extract(NUVERSION, NUTAIL, out var temp);
        if (!done) return false;

        version = temp.NotNullNotEmpty(trim: true);
        return true;
    }

    /// <summary>
    /// Tries to update the NuGet package version in this line.
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public bool UpdateNuPackageVersion(SemanticVersion version)
    {
        ArgumentNullException.ThrowIfNull(version);
        if (version.IsEmpty) throw new EmptyException("Version cannot be empty.");

        return Update(NUVERSION, NUTAIL, version);
    }
}
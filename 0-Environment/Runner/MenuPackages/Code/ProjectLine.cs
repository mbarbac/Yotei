namespace Runner;

// ========================================================
/// <summary>
/// Represents a line in a project file, where its contents can be mutated as needed.
/// </summary>
public class ProjectLine
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public ProjectLine(string value) => Value = value;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => Value.Trim();

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="item"></param>
    public static implicit operator string(ProjectLine item) => item.ThrowWhenNull().Value;

    /// <summary>
    /// Conversion operator.
    /// </summary>
    /// <param name="item"></param>
    public static implicit operator ProjectLine(string item) => new(item);

    /// <summary>
    /// The actual contents of this line, which can be empty but not null.
    /// </summary>
    public string Value
    {
        get;
        set => field = value.ThrowWhenNull();
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to extract from this line a value wrapped by the given head and tail sequences,
    /// provided they already exist.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool Extract(
        string head, string tail,
        [NotNullWhen(true)] out string? value,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        ArgumentNullException.ThrowIfNull(head);
        ArgumentNullException.ThrowIfNull(tail);
        
        value = null;

        var ini = Value.IndexOf(head, comparison);
        if (ini < 0) return false;

        ini += head.Length;
        var end = Value.IndexOf(tail, ini, comparison);
        if (end < 0) return false;

        value = Value[ini..end];
        return true;
    }

    /// <summary>
    /// Tries to update the value carried by this line between the given head and tail sequences,
    /// provided they already exist.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool Update(
        string head, string tail,
        string value,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        ArgumentNullException.ThrowIfNull(head);
        ArgumentNullException.ThrowIfNull(tail);

        value = value.NotNullNotEmpty(trim: true);

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
    /// Tries to extract from this line a value wrapped by a XML element whose delimiter is given
    /// (without angle brackets), provided such already exist.
    /// </summary>
    /// <param name="delimiter"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool ExtractXML(
        string delimiter,
        [NotNullWhen(true)] out string? value,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        delimiter = delimiter.NotNullNotEmpty(trim: true);

        var head = $"<{delimiter}>";
        var tail = $"</{delimiter}>";
        return Extract(head, tail, out value, comparison);
    }

    /// <summary>
    /// Tries to update the value carried by line in a XML whose delimiter is given (without angle
    /// brackets), provided such already exist.
    /// </summary>
    /// <param name="delimiter"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool UpdateXML(
        string delimiter,
        string value,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        delimiter = delimiter.NotNullNotEmpty(trim: true);

        var head = $"<{delimiter}>";
        var tail = $"</{delimiter}>";
        return Update(head, tail, value, comparison);
    }

    // ----------------------------------------------------

    const string PACKAGEREFERENCE = "<PackageReference";
    const string INCLUDE = "Include=\"";
    const string VERSION = "Version=\"";
    const string TAIL = "\"";

    /// <summary>
    /// Determines if this line represents a NuGet package reference and, if so, gets the package
    /// name and version.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="version"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool IsNuReference(
        [NotNullWhen(true)] out string? name,
        [NotNullWhen(true)] out SemanticVersion? version,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        name = null;
        version = null;

        if (!Value.Contains(PACKAGEREFERENCE, comparison)) return false;
        if (!Extract(INCLUDE, TAIL, out name)) return false;
        if ((name = name.NullWhenEmpty(true)) is null) return false;
        
        if (!Extract(VERSION, TAIL, out var temp)) return false;
        if ((temp = temp.NullWhenEmpty(true)) is null) return false;
        version = temp;
        
        return true;
    }

    /// <summary>
    /// Determines if this line represents a NuGet package reference.
    /// </summary>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool IsNuReference(
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        => IsNuReference(out _, out _, comparison);

    /// <summary>
    /// Tries to obtain the NuGet package name carried by this line, if any.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool GetNuPackageName(
        [NotNullWhen(true)] out string? name) => Extract(INCLUDE, TAIL, out name);

    /// <summary>
    /// Updates the NuGet package name carried by this, provided there was any.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool UpdateNuPackageName(string name)
    {
        name = name.NotNullNotEmpty(trim: true);
        return Update(INCLUDE, TAIL, name);
    }

    /// <summary>
    /// Tries to obtain the NuGet package version carried by this line, if any.
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public bool GetNuPackageVersion([NotNullWhen(true)] out SemanticVersion? version)
    {
        version = null;

        var done = Extract(VERSION, TAIL, out var temp);
        if (!done) return false;

        if ((temp = temp.NotNullNotEmpty(true)) is null) return false;
        version = temp;
        return true;
    }

    /// <summary>
    /// Updates the NuGet package varsion carried by this, provided there was any.
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public bool UpdateNuPackageVersion(SemanticVersion version)
    {
        version.ThrowWhenNull();
        if (version.IsEmpty) throw new ArgumentException("Version cannot be empty.");
        return Update(VERSION, TAIL, version);
    }
}
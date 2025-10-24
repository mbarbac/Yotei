namespace Runner;

// ========================================================
/// <summary>
/// Represents a text line in a project file, which can be mutated as needed.
/// </summary>
public class ProjectLine
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public ProjectLine(string value) => Value = value;

    /// <summary>
    /// Conversion operator.
    /// </summary>
    public static implicit operator string(ProjectLine item) => item.ThrowWhenNull().Value;

    /// <summary>
    /// Conversion operator.
    /// </summary>
    public static implicit operator ProjectLine(string value) => new(value);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override string ToString() => Value.Trim();

    /// <summary>
    /// The actual contents of this line, which may be empty or spaces-only, but not null.
    /// </summary>
    public string Value
    {
        get => _Value;
        set => _Value = value.ThrowWhenNull();
    }
    string _Value = string.Empty;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance contains the given not-null and not-empty sequence.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="trim"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool Contains(
        string value,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        value = value.NotNullNotEmpty(true);
        return Value.Contains(value, comparison);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value wrapped by the given head and tail sequences, if any.
    /// <br/> Returns true if the value has been obtained, or false otherwise.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool GetWrappedValue(
        string head, string tail,
        [NotNullWhen(true)] out string? value,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        head = head.NotNullNotEmpty(true);
        tail = tail.NotNullNotEmpty(true);

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
    /// Tries to set the value wrapped by the given head and tail sequences, if any.
    /// <br/> Returns true if the value has been set, or false otherwise.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool SetWrappedValue(
        string head, string tail,
        string value,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        head = head.NotNullNotEmpty(true);
        tail = tail.NotNullNotEmpty(true);
        value = value.NotNullNotEmpty(true);

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
    /// Tries to get the value wrapped by the given XML delimiter, if any.
    /// <br/> The delimiter must be specified without any angle brackets.
    /// <br/> Returns true if the value has been obtained, or false otherwise.
    /// </summary>
    /// <param name="delimiter"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool GetXMLWrappedValue(
        string delimiter,
        [NotNullWhen(true)] out string? value,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        delimiter = delimiter.NotNullNotEmpty(true);

        var head = $"<{delimiter}>";
        var tail = $"</{delimiter}>";
        return GetWrappedValue(head, tail, out value, comparison);
    }

    /// <summary>
    /// Tries to set the value wrapped by the given XML delimiter, if any.
    /// <br/> The delimiter must be specified without any angle brackets.
    /// <br/> Returns true if the value has been set, or false otherwise.
    /// </summary>
    /// <param name="delimiter"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool SetXMLWrappedValue(
        string delimiter,
        string value,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        delimiter = delimiter.NotNullNotEmpty(true);

        var head = $"<{delimiter}>";
        var tail = $"</{delimiter}>";
        return SetWrappedValue(head, tail, value, comparison);
    }

    // ----------------------------------------------------

    const string PACKAGEREFERENCE = "<PackageReference";
    const string INCLUDE = "Include=\"";
    const string VERSION = "Version=\"";
    const string TAIL = "\"";

    /// <summary>
    /// Determines if this line represents a NuGet package reference or not.
    /// </summary>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool IsNuReference(
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        => IsNuReference(out _, out _, comparison);

    /// <summary>
    /// Determines if this line represents a NuGet package reference and, if so, gets the package
    /// name and version.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="version"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool IsNuReference(
        [NotNullWhen(true)] out string? name, [NotNullWhen(true)] out SemanticVersion? version,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        name = null;
        version = null;

        if (!Contains(PACKAGEREFERENCE, comparison)) return false;
        if (!GetWrappedValue(INCLUDE, TAIL, out name)) return false;
        if ((name = name.NullWhenEmpty(true)) is null) return false;
        if (!GetWrappedValue(VERSION, TAIL, out var temp)) return false;
        if ((temp = temp.NullWhenEmpty(true)) is null) return false;
        version = temp;
        return true;
    }

    /// <summary>
    /// Tries to get a NuGet package name from this line.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool GetNuName(
        [NotNullWhen(true)] out string? name) => GetWrappedValue(INCLUDE, TAIL, out name);

    /// <summary>
    /// Sets the package name, provided this line represents a NuGet package.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool SetNuName(string name)
    {
        name = name.NotNullNotEmpty(true);
        return SetWrappedValue(INCLUDE, TAIL, name);
    }

    /// <summary>
    /// Tries to get a NuGet package version from this line.
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public bool GetNuVersion([NotNullWhen(true)] out SemanticVersion? version)
    {
        version = null;

        var done = GetWrappedValue(VERSION, TAIL, out var temp);
        if (!done) return false;

        if ((temp = temp.NotNullNotEmpty(true)) is null) return false;
        version = temp;
        return true;
    }

    /// <summary>
    /// Sets the package version, provided this line represents a NuGet package.
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public bool SetNuVersion(SemanticVersion version)
    {
        version.ThrowWhenNull();
        if (version.IsEmpty) throw new ArgumentException("Version cannot be empty.");
        return SetWrappedValue(VERSION, TAIL, version);
    }
}
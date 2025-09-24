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

    /// <inheritdoc/>
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
        bool trim = true,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        value = value.NotNullNotEmpty(trim);
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
}
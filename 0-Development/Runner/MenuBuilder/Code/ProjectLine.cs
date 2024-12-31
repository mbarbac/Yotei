namespace Runner;

// ========================================================
/// <summary>
/// Represents a text line in a project file.
/// <br/> Values in instances of this class are mutable ones.
/// </summary>
public class ProjectLine
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value"></param>
    public ProjectLine(string value) => Value = value;

    /// <inheritdoc/>
    public override string ToString() => Value.Trim();

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    public static implicit operator string(ProjectLine item)
    {
        item = item.ThrowWhenNull();
        return item.Value;
    }

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    public static implicit operator ProjectLine(string value) => new(value);

    /// <summary>
    /// The actual contents of this line.
    /// </summary>
    public string Value
    {
        get => _Value;
        set => _Value = value.ThrowWhenNull();
    }
    string _Value = string.Empty;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance contains the given not-null sequence.
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
    /// Tries to get the value wrapped by the given head ant tail sequences, if any. If so, the
    /// value is returned in the out argument, which is set to null otherwise.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool GetWrappedValue(
        string head,
        string tail,
        [NotNullWhen(true)] out string? value,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        head.NotNullNotEmpty();
        tail.NotNullNotEmpty();
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
    /// Tries to set the value wrapped by the given head and tail sequences, if any. Returns
    /// <c>true</c> if the value has been set, or <c>false</c> otherwise.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool SetWrappedValue(
        string head,
        string tail,
        string value,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        head.NotNullNotEmpty();
        tail.NotNullNotEmpty();
        value.ThrowWhenNull();

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
    /// Tries to get the value wrapped by the given XML delimiter, if any. The delimiter must have
    /// no termination characters. If found, the value is returned in the out argument, which is
    /// set to null otherwise.
    /// </summary>
    /// <param name="delimiter"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool GetXMLValue(
        string delimiter,
        [NotNullWhen(true)] out string? value,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        delimiter.NotNullNotEmpty();

        var head = $"<{delimiter}>";
        var tail = $"</{delimiter}>";

        return GetWrappedValue(head, tail, out value, comparison);
    }

    /// <summary>
    /// Tries to set the value wrapped by the given delimiter, if any. The delimiter must have
    /// no termination characters. Returns <c>true</c> if the value has been set, or <c>false</c>
    /// otherwise.
    /// </summary>
    /// <param name="delimiter"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool SetXMLValue(
        string delimiter,
        string value,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        delimiter.NotNullNotEmpty();

        var head = $"<{delimiter}>";
        var tail = $"</{delimiter}>";

        return SetWrappedValue(head, tail, value, comparison);
    }
}
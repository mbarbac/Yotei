namespace Runner.Builder;

// ========================================================
/// <summary>
/// Represents a text line in a project file.
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
    /// Implicit conversion operator.
    /// </summary>
    public static implicit operator string(ProjectLine item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return item.Value;
    }

    /// <summary>
    /// Implicit conversion operator.
    /// </summary>
    public static implicit operator ProjectLine(string value) => new(value);

    /// <summary>
    /// The actual contents of this instance.
    /// </summary>
    public string Value
    {
        get => _Value;
        set => _Value = value.ThrowWhenNull();
    }
    string _Value = string.Empty;

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this line contains the given value, or not.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool Contains(
        string value, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        value = value.NotNullNotEmpty();
        return Value.Contains(value, comparison);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value wrapped by the given head and tail sequences, if any.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool GetValue(
        string head, string tail, [NotNullWhen(true)] out string? value,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        head = head.NotNullNotEmpty();
        tail = tail.NotNullNotEmpty();
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
    /// Tries to get the value wrapped by the given delimiter, which shall appear in the line
    /// using the <c>{delimiter}...{/delimiter}</c> XML syntax. 
    /// </summary>
    /// <param name="delimiter"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool GetXMLValue(
        string delimiter, [NotNullWhen(true)] out string? value,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        delimiter = delimiter.NotNullNotEmpty();

        var head = $"<{delimiter}>";
        var tail = $"</{delimiter}>";
        return GetValue(head, tail, out value, comparison);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to set the value wrapped by the given head and tail sequences, if any.
    /// </summary>
    /// <param name="head"></param>
    /// <param name="tail"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool SetValue(
        string head, string tail, string value,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        head = head.NotNullNotEmpty();
        tail = tail.NotNullNotEmpty();
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

    /// <summary>
    /// Tries to set the value wrapped by the given delimiter, which shall appear in the line
    /// using the <c>{delimiter}...{/delimiter}</c> XML syntax. 
    /// </summary>
    /// <param name="delimiter"></param>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <returns></returns>
    public bool SetXMLValue(
        string delimiter, string value,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
        delimiter = delimiter.NotNullNotEmpty();

        var head = $"<{delimiter}>";
        var tail = $"</{delimiter}>";
        return SetValue(head, tail, value, comparison);
    }
}
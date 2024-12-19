using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Text.Json;

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
    /// Determines if this line contains the given not-null string sequence, or not. The sequence
    /// is trimmed by default before performing the search.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="comparison"></param>
    /// <param name="trim"></param>
    /// <returns></returns>
    public bool Contains(
        string value,
        StringComparison comparison = StringComparison.OrdinalIgnoreCase, bool trim = true)
    {
        value = value.NotNullNotEmpty();
        return Value.Contains(value, comparison);
    }

    // ----------------------------------------------------

    /// <summary>
    /// Tries to get the value wrapped by the given not-null and not-empty head ant tail sequences,
    /// if any. If so, the value is returned in the out argument, which is set to null otherwise.
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
    /// Tries to set the value of the not-null sequence wrapped by the given not-null and not-empty
    /// head and tail ones, if any.
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
    /// Tries to get the value wrapped by the given not-null and not-empty XML delimiter, if any.
    /// The delimiter must be a literal one without any termination characters. If found, the value
    /// is returned in the out argument, which is set to null otherwise.
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
    /// Tries to set the value of the not-null sequence wrapped by the given not-null and not-empty
    /// XML delimiter, if any. The delimiter must be a literal one without any termination characters.
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
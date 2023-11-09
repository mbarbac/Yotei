namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// An immutable object that represents a tag and value metadata pair.
/// </summary>
/// <param name="tag"></param>
/// <param name="value"></param>
public readonly struct MetadataPair(string tag, object? value)
{
    /// <summary>
    /// Deconstructs this instance.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    public void Deconstruct(out string tag, out object? value)
    {
        tag = Tag;
        value = Value;
    }

    /// <summary>
    /// Returns the string representation of this instance.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"{Tag}='{Value.Sketch()}'";

    /// <summary>
    /// The tag name by which this pair is known.
    /// </summary>
    public string Tag { get; } = tag.NotNullNotEmpty();

    /// <summary>
    /// The value carried by this metadata pair.
    /// </summary>
    public object? Value { get; } = value;
}
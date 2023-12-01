namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// An immutable object that represents a tag and value metadata pair.
/// </summary>
public interface IMetadataPair
{
    /// <summary>
    /// The tag name by which this pair is known.
    /// </summary>
    string Tag { get; }

    /// <summary>
    /// The value carried by this metadata pair.
    /// </summary>
    object? Value { get; }
}
namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a metadata entry.
/// <br/> Instances of this type are essentially key/value pairs, where the key itself is a
/// collection of names.
/// </summary>
public partial interface IMetadataEntry
{
    /// <summary>
    /// The collection of tag names by which this instance is known.
    /// </summary>
    [WithGenerator] IMetadataTag Tag { get; }

    /// <summary>
    /// The value carried by this instance.
    /// </summary>
    [WithGenerator] object? Value { get; }
}
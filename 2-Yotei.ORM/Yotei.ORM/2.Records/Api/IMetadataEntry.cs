namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a metadata entry.
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
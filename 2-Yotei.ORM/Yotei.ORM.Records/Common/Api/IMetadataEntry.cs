namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a metadata pair.
/// </summary>
public partial interface IMetadataEntry : IEquatable<IMetadataEntry>
{
    /// <summary>
    /// The collection of tag names by which this instance is known, acting as the key of
    /// the pair.
    /// </summary>
    [WithGenerator] IMetadataTag Tag { get; }

    /// <summary>
    /// The value carried by this instance.
    /// </summary>
    [WithGenerator] object? Value { get; }
}
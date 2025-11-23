namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a metadata entry.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public partial interface IMetadataItem : IEquatable<IMetadataItem>
{
    /// <summary>
    /// Determines if this instance is equal to the other given one, using the given comparison
    /// mode to compare their metadata names.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="caseSensitiveTags"></param>
    /// <returns></returns>
    bool Equals(IMetadataItem? other, bool caseSensitiveTags);

    /// <summary>
    /// The name by which this metadata item is known.
    /// </summary>
    [With] string Name { get; }

    /// <summary>
    /// The value carried by this metadata item.
    /// </summary>
    [With] object? Value { get; }
}
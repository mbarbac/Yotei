namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a metadata entry using its collection of tag names and its value.
/// <para>Instances of this class are intended to be immutable ones.</para>
/// </summary>
public partial interface IMetadataEntry : IEquatable<IMetadataEntry>
{
    /// <summary>
    /// The collection of tag names by which this entry is known.
    /// </summary>
    [With] IMetadataTag Tag { get; }

    /// <summary>
    /// The value carried by this entry.
    /// </summary>
    [With] object? Value { get; }
}
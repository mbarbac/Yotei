namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a metadata entry.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface IMetadataEntry : IEquatable<IMetadataEntry>
{
    /// <summary>
    /// Determines if this instance shall be considered equal to the other given one, using
    /// the given <paramref name="ignoreNameCase"/> value to compare the names.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="ignoreNameCase"></param>
    /// <returns></returns>
    bool Equals(IMetadataEntry? other, bool ignoreNameCase);

    /// <summary>
    /// The name by which this entry is known.
    /// </summary>
    [With] string Name { get; }

    /// <summary>
    /// The value maintained by this entry.
    /// </summary>
    [With] object? Value { get; }
}
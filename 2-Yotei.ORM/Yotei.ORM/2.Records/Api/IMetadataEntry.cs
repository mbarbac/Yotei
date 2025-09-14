namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a metadata entry, which carries a metadata name and a value.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public partial interface IMetadataEntry : IEquatable<IMetadataEntry>
{
    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    IMetadataEntry WithName(string value);

    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    IMetadataEntry WithValue(object? value);

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance is equal to the other given one, using the given comparison
    /// mode to compare their metadata names.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="caseSensitiveNames"></param>
    /// <returns></returns>
    bool Equals(IMetadataEntry? other, bool caseSensitiveNames);

    /// <summary>
    /// The name by which this metadata entry is known.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The value carried by this entry.
    /// </summary>
    object? Value { get; }
}
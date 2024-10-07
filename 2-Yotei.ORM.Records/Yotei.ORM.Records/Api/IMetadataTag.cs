namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the not-empty collection of names by which a metadata entry is known.
/// </summary>
public interface IMetadataTag : IEquatable<IMetadataTag>
{
    /// <summary>
    /// Determines if the tag names in this instance are case sensitive, or not.
    /// </summary>
    bool CaseSensitive { get; }
}
namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the collection of tags that are well-known for an underlying engine.
/// </summary>
[Cloneable]
public partial interface IKnownTags : IEnumerable<IMetadataTag>, IEquatable<IKnownTags>
{
    /// <summary>
    /// Determines if the names in this instance are considered case sensitive or not.
    /// </summary>
    bool CaseSensitiveTags { get; }

    /// <summary>
    /// The ordered collection of tags that describes the maximal structure of the identifiers in
    /// the underlying database, or an empty collection if this information is not available.
    /// </summary>
    [WithGenerator] IIdentifierTags IdentifierTags { get; }

    /// <summary>
    /// The tag used to determine if a given entry is a primary key one, or part of a primary
    /// key group, or null if this information is not available.
    /// </summary>
    [WithGenerator] IMetadataTag? PrimaryKeyTag { get; }

    /// <summary>
    /// The tag used to determine if a given entry is a unique valued one, or part of a unique
    /// valued group, or null if this information is not available.
    /// </summary>
    [WithGenerator] IMetadataTag? UniqueValuedTag { get; }

    /// <summary>
    /// The tag used to determine if a given entry is a read only one, or null if this information
    /// is not available.
    /// </summary>
    [WithGenerator] IMetadataTag? ReadOnlyTag { get; }

    /// <summary>
    /// The collection of names carried by all tags in this instance.
    /// </summary>
    IEnumerable<string> Names { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance contains a tag that carries the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this instance contains a tag that carries any name from the given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    bool ContainsAny(IEnumerable<string> range);

    /// <summary>
    /// Returns a new instance where all the original tags have been removed.
    /// </summary>
    /// <returns></returns>
    IKnownTags Clear();
}
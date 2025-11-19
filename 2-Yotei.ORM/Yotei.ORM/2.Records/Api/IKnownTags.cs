namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the collection of metadata tags that are well-known to a given engine. The tag
/// names carried by this instance are unique among all the ones in this collection.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface IKnownTags : IEnumerable<IMetadataTag>, IEquatable<IKnownTags>
{
    /// <summary>
    /// Whether the tag names in this instance are case sensitive, or not.
    /// </summary>
    bool CaseSensitiveTags { get; }

    /// <summary>
    /// The ordered collection of tags that describes the maximal structure of the allowed
    /// identifiers in an underlying database, or an empty collection if this information
    /// is not available.
    /// </summary>
    [With] IIdentifierTags IdentifierTags { get; }

    /// <summary>
    /// The tag used to determine if a given element is a primary key one, or part of a primary
    /// key group, or null if this information is not available.
    /// </summary>
    [With] IMetadataTag? PrimaryKeyTag { get; }

    /// <summary>
    /// The tag used to determine if a given element is a unique valued one, or part of a unique
    /// valued group, or null if this information is not available.
    /// </summary>
    [With] IMetadataTag? UniqueValuedTag { get; }

    /// <summary>
    /// The tag used to determine if a given element is a read only one, or null if this
    /// information is not available.
    /// </summary>
    [With] IMetadataTag? ReadOnlyTag { get; }

    /// <summary>
    /// The collection of names carried by all tags in this instance.
    /// </summary>
    IEnumerable<string> Names { get; }

    /// <summary>
    /// The number of well-known metadata tags.
    /// <br/> Note that this number is not the same as the number of names, because each tag may
    /// carry more than one name.
    /// </summary>
    int Count { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this collection contains a tag that carries the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this collection contains any of the tag names in the given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    bool ContainsAny(IEnumerable<string> range);

    /// <summary>
    /// Returns the tag that contains the given name, or null if any.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IMetadataTag? Find(string name);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where all the original tags have been removed.
    /// </summary>
    /// <returns></returns>
    IKnownTags Clear();
}
namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an engine's collection of well-known metadata tags.
/// <br/> The names carried by the tags in this instance are not duplicated.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface IKnownTags : IEnumerable<IMetadataTag>, IEquatable<IKnownTags>
{
    /// <summary>
    /// Determines if the names in this collection are case sensitive or not.
    /// </summary>
    bool IgnoreCase { get; }

    /// <summary>
    /// The ordered collection of tags that describe the maximal structure of identifiers in the
    /// underlying database, or null if if this information is not available.
    /// <br/> If not null, then the collection is guaranteed not to be an empty one.
    /// </summary>
    [With] ImmutableArray<IMetadataTag>? IdentifierTags { get; }

    /// <summary>
    /// The tag of the metadata entry that determines if a given element is a primary key, or part
    /// of a primary key group, or null if if this information is not available.
    /// </summary>
    [With] IMetadataTag? PrimaryKeyTag { get; }

    /// <summary>
    /// The tag of the metadata entry that determines if a given element is a unique valued one,
    /// or part of a unique valued group, or null if if this information is not available.
    /// </summary>
    [With] IMetadataTag? UniqueValuedTag { get; }

    /// <summary>
    /// The tag of the metadata entry that determines if a given element is a read-only one, or null
    /// if this information is not available.
    /// </summary>
    [With] IMetadataTag? ReadOnlyTag { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance contains the given name in any of its tags.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this instance contains any of the given names in any of its tags.
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    bool Contains(IEnumerable<string> names);

    /// <summary>
    /// Returns the unique tag that contains the given name, or null if any.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IMetadataTag? Find(string name);

    /// <summary>
    /// Returns the collection of tags that contains any of the given names.
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    List<IMetadataTag> Find(IEnumerable<string> names);

    /// <summary>
    /// Enumerates the actual collection of names in this instance.
    /// </summary>
    IEnumerable<string> Names { get; }
}
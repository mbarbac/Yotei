namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// An immutable object that represents the collection of well-known metadata tags of a given
/// engine, for the purposes of the framework.
/// <br/> Tags cannot be null or empty. Identifier tags cannot contain dots.
/// <br/> Duplicate tags are not allowed.
/// </summary>
public partial interface IKnownTags : IEnumerable<string>, IEquatable<IKnownTags>
{
    /// <summary>
    /// Determine if the tags in this instance are case sensitive or not.
    /// </summary>
    [WithGenerator] bool CaseSensitiveTags { get; }

    /// <summary>
    /// The ordered collection of metadata tags that describes the maximal known structure of
    /// identifiers in the underlying database.
    /// </summary>
    [WithGenerator] IIdentifierTags IdentifierTags { get; }

    /// <summary>
    /// If not null, the metadata tag used to identify if a given entry in a record is a primary
    /// key, or part of a primary key group, or not. If null, the underlying database engine does
    /// not support this capability.
    /// </summary>
    [WithGenerator] string? PrimaryKeyTag { get; }

    /// <summary>
    /// If not null, the metadata tag used to identify if a given entry in a record is a unique
    /// valued one, or part of a unique valued group, or not. If null, the underlying database
    /// engine does not support this capability.
    /// </summary>
    [WithGenerator] string? UniqueValuedTag { get; }

    /// <summary>
    /// If not null, the metadata tag used to identify if a given entry in a record is a readonly
    /// one, or not. If null, the underlying database engine does not support this capability.
    /// </summary>
    [WithGenerator] string? ReadOnlyTag { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance carries the given metadata tag, or not.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    bool Contains(string tag);

    /// <summary>
    /// Obtains a new instance where all the original contents have been removed.
    /// </summary>
    /// <returns></returns>
    IKnownTags Clear();
}
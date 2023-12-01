namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// An immutable object that contains the well-known metadata tags of an underlying database
/// engine, for the purposes of the framework.
/// </summary>
public partial interface IKnownTags : IEnumerable<string>
{
    /// <summary>
    /// Whether the tags in this instance are case sensitive, or not.
    /// </summary>
    [WithGenerator] bool CaseSensitiveTags { get; }

    /// <summary>
    /// The ordered collection of metadata tags describes the maximal known structure of the
    /// identifiers in the underlying database, or an empty collection if they are not known.
    /// </summary>
    [WithGenerator] IIdentifierTags IdentifierTags { get; }

    /// <summary>
    /// The metadata tags that identifies if a given entry is a primary key one, or null if
    /// it is not known.
    /// </summary>
    [WithGenerator] string? PrimaryKeyTag { get; }

    /// <summary>
    /// The metadata tags that identifies if a given entry is a unique valued one, or null if
    /// it is not known.
    /// </summary>
    [WithGenerator] string? UniqueValuedTag { get; }

    /// <summary>
    /// The metadata tags that identifies if a given entry is a read only one, or null if it is
    /// not known.
    /// </summary>
    [WithGenerator] string? ReadOnlyTag { get; }

    /// <summary>
    /// Determines if this instance carries the given tag, or not.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    bool Contains(string tag);

    /// <summary>
    /// Returns a new instance where all the original tags have been removed.
    /// </summary>
    /// <returns></returns>
    IKnownTags Clear();
}
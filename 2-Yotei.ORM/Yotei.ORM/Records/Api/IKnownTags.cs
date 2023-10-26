namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the collection of not-duplicated metadata tags that are well-known to a given
/// underlying engine.
/// </summary>
public partial interface IKnownTags
{
    /// <summary>
    /// Determines if the metadata tags in this collection are case sensitive, or not.
    /// </summary>
    [WithGenerator] bool CaseSensitiveTags { get; }

    /// <summary>
    /// The ordered collection of not-duplicated metadata tags that describe the maximal
    /// structure of the database identifiers associated with the underlying engine.
    /// </summary>
    [WithGenerator] IIdentifierTags IdentifierTags { get; }

    /// <summary>
    /// If not null, the metadata tag used to identify if a given entry in a record is a
    /// primary key one, or a part of it, or not. If null, then the underlying engine does not
    /// support this capability.
    /// </summary>
    [WithGenerator] string? PrimaryKeyTag { get; }

    /// <summary>
    /// If not null, the metadata tag used to identify if a given entry in a record is a
    /// uniqued value one, or a part of it, or not. If null, then the underlying engine does
    /// not support this capability.
    /// </summary>
    [WithGenerator] string? UniqueValuedTag { get; }

    /// <summary>
    /// If not null, the metadata tag used to identify if a given entry in a record is a
    /// read only one, or not. If null, then the underlying engine does not support this
    /// capability.
    /// </summary>
    [WithGenerator] string? ReadOnlyTag { get; }

    /// <summary>
    /// Determines if this intance carries the given metadata tag, or not.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    bool Contains(string tag);
}
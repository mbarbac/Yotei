namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// The collection of metadata tags well-known by a given engine.
/// <br/> Duplicate elements are not allowed.
/// </summary>
public partial interface IKnownTags
{
    /// <summary>
    /// Determines if the metadata tags are case sensitive or not.
    /// </summary>
    [WithGenerator] bool CaseSensitiveTags { get; }

    /// <summary>
    /// The ordered collection of metadata tags that describes the maximal structure of the
    /// database identifiers of the underlying engine.
    /// </summary>
    [WithGenerator] IIdentifierTags IdentifierTags { get; }

    /// <summary>
    /// If not null, the metadata tag used to identify the metadata pair that determines if a
    /// given entry in a record is a primary key, or a part of it, or not. If null, then the
    /// underlying database does not support this capability.
    /// </summary>
    [WithGenerator] string? PrimaryKeyTag { get; }

    /// <summary>
    /// If not null, the metadata tag used to identify the metadata pair that determines if a
    /// given entry in a record is a unique valued one, or a part of it, or not. If null, then
    /// the underlying database does not support this capability.
    /// </summary>
    [WithGenerator] string? UniqueValuedTag { get; }

    /// <summary>
    /// If not null, the metadata tag used to identify the metadata pair that determines if a
    /// given entry in a record is read only one, or not. If null, then the underlying database
    /// does not support this capability.
    /// </summary>
    [WithGenerator] string? ReadOnlyTag { get; }

    /// <summary>
    /// Determines if this instance carries the given metadata tag, or not.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    bool Contains(string tag);
}
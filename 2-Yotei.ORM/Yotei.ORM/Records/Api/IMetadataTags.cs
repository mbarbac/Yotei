namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the collection of an engine's known metadata tags names.
/// </summary>
public partial interface IMetadataTags
{
    /// <summary>
    /// Determines if the engine ignores the case when comparing metadata tag names, or not.
    /// </summary>
    [With] bool IgnoreCase { get; }

    /// <summary>
    /// The ordered collection of tag names' collections that identify the identifiers in the
    /// underlying database. For instance, in relational databases its standard first-level
    /// entries are for the schema, table and column tags.
    /// <br/> If an empty collection, then this capability is not supported.
    /// <br/> Second-level entries are guaranteed to be not-emty ones.
    /// </summary>
    [With] ImmutableArray<ImmutableArray<string>> IdentifierTags { get; }

    /// <summary>
    /// The collection of metadata tag names that determine if a database column is a primary
    /// key one, or part of the primary key group, or not.
    /// <br/> If an empty collection, then this capability is not supported.
    /// </summary>
    [With] ImmutableArray<string> PrimaryKeyTags { get; }

    /// <summary>
    /// The collection of metadata tag names that determine if a database column is an unique
    /// valued one, or part of a unique valued group, or not. Only one group is supported.
    /// <br/> If an empty collection, then this capability is not supported.
    /// </summary>
    [With] ImmutableArray<string> UniqueValuedTags { get; }

    /// <summary>
    /// The collection of metadata tag names that determine if a database column is a read only
    /// one or not.
    /// <br/> If an empty collection, then this capability is not supported.
    /// </summary>
    [With] ImmutableArray<string> ReadOnlyTags { get; }
}
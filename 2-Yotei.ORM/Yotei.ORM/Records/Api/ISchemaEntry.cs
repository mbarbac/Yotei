namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the metadata associated with a given schema element (the equivalent of a column
/// in a relational world).
/// <br/> Instances of this type provide a set of well-known properties whose values might not
/// apperar in the metadata collection, either because the underlying database engine does not
/// support them, or because they have not been explicitly set yet.
/// <br/> Instance of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface ISchemaEntry : IEnumerable<IMetadataEntry>, IEquatable<ISchemaEntry>
{
    /// <summary>
    /// The identifier by which the associated schema element (column) is knowm, or null if this
    /// property has not been either set yet explicitly or through its associated well-known
    /// metadata tags, if any.
    /// </summary>
    [With] IIdentifier? Identifier { get; init; }

    /// <summary>
    /// Whether the associated schema element (column) is a primary key one, or part of a primary
    /// key group, or null if this property has not been either set yet explicitly or through its
    /// associated well-known metadata tags, if any. Only one primary key group is supported per
    /// schema.
    /// </summary>
    [With] bool? IsPrimaryKey { get; init; }

    /// <summary>
    /// Whether the associated schema element (column) is a unique valued one, or part of a unique
    /// valued group, or null if this property has not been either set yet explicitly or through
    /// its associated well-known metadata tags, if any. Only one unique valued group is supported
    /// per schema.
    /// </summary>
    [With] bool? IsUniqueValued { get; init; }

    /// <summary>
    /// Whether the associated schema element (column) is a read only one, or null if this property
    /// has not been either set yet explicitly or through its associated well-known metadata tags,
    /// if any.
    /// </summary>
    [With] bool? IsReadOnly { get; init; }

    // ----------------------------------------------------

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// Gets the actual number of metadata entries kept by this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the value kept by the metadata entry associated with the given name, or throws an
    /// exception if that entry is not found.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    object? this[string name] { get; }

    /// <summary>
    /// Determines if this instance contains a metadata entry associated with the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this instance contains a metadata entry associated with any of the given
    /// names.
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    bool Contains(IEnumerable<string> names);

    /// <summary>
    /// Returns the unique metadata entry associated with the given name, or null if not found.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IMetadataEntry? Find(string name);

    /// <summary>
    /// Returns the collection of metadata entries associated with the given names.
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    List<IMetadataEntry> Find(IEnumerable<string> names);

    /// <summary>
    /// Returns the collection of metadata entries that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<IMetadataEntry> Find(Predicate<IMetadataEntry> predicate);
    
    // ------------------------------------------------

    /// <summary>
    /// Returns a builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder ToBuilder();

    /// <summary>
    /// Returns a copy of this instance with the given metadata entry added to it.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    ISchemaEntry Add(IMetadataEntry item);

    /// <summary>
    /// Returns a copy of this instance with the entries of the given range added to it.
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    ISchemaEntry AddRange(IEnumerable<IMetadataEntry> range);

    /// <summary>
    /// Returns a copy of this instance where the entry associated with the given name removed,
    /// if any.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    ISchemaEntry Remove(string name);

    /// <summary>
    /// Returns a copy of this instance with all its entries removed and its standard properties
    /// reset to their default values.
    /// </summary>
    /// <returns></returns>
    ISchemaEntry Clear();
}
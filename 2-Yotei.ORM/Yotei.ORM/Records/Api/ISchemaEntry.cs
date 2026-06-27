namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the metadata associated with a given schema element (the equivalent of a column
/// definition in the relational world).
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface ISchemaEntry : IEnumerable<IMetadataItem>, IEquatable<ISchemaEntry>
{
    /// <summary>
    /// The identifier by which the associated schema element is known, or null if it this
    /// information is not available. Entries with no identifier, or with an empty one, are
    /// not considered valid schema elements.
    /// </summary>
    [With] IIdentifier? Identifier { get; init; }

    /// <summary>
    /// Whether the associated schema element is a primary key one, or part of a primary key
    /// group, or null if this information is not available. Only one group is supported per
    /// schema.
    /// </summary>
    [With] bool? IsPrimaryKey { get; init; }

    /// <summary>
    /// Whether the associated schema element is a unique valued one, or part of an unique
    /// valued group, or null if this information is not available. Only one group is supported
    /// per schema.
    /// </summary>
    [With] bool? IsUniqueValued { get; init; }

    /// <summary>
    /// Whether the associated schema element is a read-only one, or null if this information
    /// is not available.
    /// </summary>
    [With] bool? IsReadOnly { get; init; }

    // ------------------------------------------------

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// Gets the number of metadata entries in this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the metadata value carried by the entry whose metadata name is given, or throws an
    /// exception if such entry does not exist yet.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    object? this[string name] { get; }

    /// <summary>
    /// Determines if this instance carries a metadata entry with the given metadata name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this instance carries any metadata entry with any of the given metadata
    /// names.
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    bool Contains(IEnumerable<string> names);

    /// <summary>
    /// Returns the unique metadata entry in this collection with the given metadata name,
    /// or null if such cannot be found.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IMetadataItem? Find(string name);

    /// <summary>
    /// Returns the collection of metadata entries whose metadata names are given. This method
    /// guarantees that in the returned collection there will be no duplicated elements.
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    List<IMetadataItem> Find(IEnumerable<string> names);

    /// <summary>
    /// Returns an array with the entries in this instance.
    /// </summary>
    /// <returns></returns>
    IMetadataItem[] ToArray();

    /// <summary>
    /// Returns a list with the entries in this instance.
    /// </summary>
    /// <returns></returns>
    List<IMetadataItem> ToList();

    // ------------------------------------------------

    /// <summary>
    /// Returns a new buider based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder ToBuilder();

    /// <summary>
    /// Returns a copy of this instance where the given metadata entry has been added to it.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    ISchemaEntry Add(IMetadataItem item);

    /// <summary>
    /// Returns a copy of this instance where a metadata entry built from the given name and
    /// value has been added to it.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    ISchemaEntry Add(string name, object? value);

    /// <summary>
    /// Returns a copy of this instance where the given range of metadata entries has been added
    /// to it.
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    ISchemaEntry AddRange(IEnumerable<IMetadataItem> range);

    /// <summary>
    /// Returns a copy of this instance where the given metadata entry has been either updated,
    /// or added to it.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    ISchemaEntry Update(IMetadataItem item);

    /// <summary>
    /// Returns a copy of this instance where the given metadata entries have been either updated
    /// or added to it.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ISchemaEntry UpdateRange(IEnumerable<IMetadataItem> range);

    /// <summary>
    /// Returns a copy of this instance where the metadata entry associated with the given name
    /// has been removed, if possible.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    ISchemaEntry Remove(string name);

    /// <summary>
    /// Returns a copy of this instance where all its metadata and well-known properties have
    /// been cleared.
    /// </summary>
    /// <returns></returns>
    ISchemaEntry Clear();
}
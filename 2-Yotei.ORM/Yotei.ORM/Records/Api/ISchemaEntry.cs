namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the metadata associated with a given schema element (the equivalent of a column
/// in a relational world).
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public partial interface ISchemaEntry : IEnumerable<IMetadataEntry>, IEquatable<ISchemaEntry>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The identifier by which the associated schema element (column) is known. The default value
    /// of this property is an empty identifier.
    /// </summary>
    [With] IIdentifier Identifier { get; }

    /// <summary>
    /// Whether the associated schema element (column) is a primary key one, or part of a primary
    /// key group. Only one group per schema is supported. The default value of this property is
    /// false.
    /// </summary>
    [With] bool IsPrimaryKey { get; }

    /// <summary>
    /// Whether the associated schema element (column) is a unique valued one, or part of a unique
    /// valued group. Only one group per schema is supported. The default value of this property is
    /// false.
    /// </summary>
    [With] bool IsUniqueValued { get; }

    /// <summary>
    /// Whether the associated schema element (column) is a read only one. The default value of this
    /// property is false.
    /// </summary>
    [With] bool IsReadOnly { get; }

    // ------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the metadata value of the entry associated with the given name. Throws an exception if
    /// if that entry does not exist.
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
    /// Adds to this instance the given metadata entry.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    ISchemaEntry Add(IMetadataEntry item);

    /// <summary>
    /// Adds to this instance the entries of the given range.
    /// </summary>
    /// <param name="entry"></param>
    /// <returns></returns>
    ISchemaEntry AddRange(IEnumerable<IMetadataEntry> range);

    /// <summary>
    /// Removes from this instance the metadata entry associated with the given name, if any.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    ISchemaEntry Remove(string name);

    /// <summary>
    /// Clears this instance.
    /// </summary>
    /// <returns></returns>
    ISchemaEntry Clear();
}
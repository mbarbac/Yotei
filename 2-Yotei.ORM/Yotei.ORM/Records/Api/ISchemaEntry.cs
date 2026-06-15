namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the metadata associated with a given column in a record.
/// </summary>
[Cloneable]
public partial interface ISchemaEntry : IEnumerable<IMetadataEntry>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The identifier by which this instance (column) is known.
    /// <br/> The value of this property might not be kept in the associated metadata if this
    /// capability is not supported by the underlying engine. Its default value, is not set,
    /// is an empty identifier.
    /// </summary>
    [With] IIdentifier Identifier { get; }

    /// <summary>
    /// Whether this instance (column) refers to a primary key column, or it is part of a primary
    /// key column group.
    /// <br/> The value of this property might not be kept in the associated metadata if this
    /// capability is not supported by the underlying engine. Its default value, if not set, is
    /// false.
    /// </summary>
    [With] bool IsPrimaryKey { get; }

    /// <summary>
    /// Whether this instance (column) refers to a unique valued column, or it is part of a unique
    /// valued column group.
    /// <br/> The value of this property might not be kept in the associated metadata if this
    /// capability is not supported by the underlying engine. Its default value, if not set, is
    /// false.
    /// </summary>
    [With] bool IsUniqueValued { get; }

    /// <summary>
    /// Whether this instance (column) refers to a read only column.
    /// <br/> The value of this property might not be kept in the associated metadata if this
    /// capability is not supported by the underlying engine. Its default value, if not set, is
    /// false.
    /// </summary>
    [With] bool IsReadOnly { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the metadata value associated with the given tag name. If such entry did not exist,
    /// an exception is thrown.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    object? this[string name] { get; }

    /// <summary>
    /// The actual number of metadata entries maintained by this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Determines if this instance contains a metadata entry with the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this instance contains a metadata entry with any of the given names.
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    bool ContainsAny(IEnumerable<string> names);

    /// <summary>
    /// Tries to find the metadata entry associated with the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    bool Find(string name, [MaybeNullWhen(false)] out IMetadataEntry entry);

    /// <summary>
    /// Tries to find the first entry whose metadata name is any of the given ones.
    /// </summary>
    /// <param name="names"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    bool FindAny(IEnumerable<string> names, [MaybeNullWhen(false)] out IMetadataEntry entry);

    /// <summary>
    /// Returns an array with the elements of this collection.
    /// </summary>
    /// <returns></returns>
    IMetadataEntry[] ToArray();

    /// <summary>
    /// Returns a list with the elements of this collection.
    /// </summary>
    /// <returns></returns>
    List<IMetadataEntry> ToList();
}
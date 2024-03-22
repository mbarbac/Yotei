using T = Yotei.ORM.Records.IMetadataEntry;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Maintains the metadata pairs that describe an associated column in a record, which is
/// identified by its unique identifier.
/// </summary>
public partial interface ISchemaEntry : IEnumerable<T>, IEquatable<ISchemaEntry>
{
    /// <summary>
    /// The identifier by which this instance is known. It cannot be null, and its last part
    /// cannot be null either.
    /// </summary>
    [WithGenerator] IIdentifier Identifier {  get; }

    /// <summary>
    /// Determines if this instance describes a primary key column, or one that it is part of the
    /// primary key group, if any, or not. Only one primary key group is supported per schema.
    /// </summary>
    [WithGenerator] bool IsPrimaryKey { get; }

    /// <summary>
    /// Determines if this instance describes an unique valued column, or one that it is part of
    /// the unique valued group, if any, or not. Only one unique valued group is supported per
    /// schema.
    /// </summary>
    [WithGenerator] bool IsUniqueValued { get; }

    /// <summary>
    /// Determines if this instance describes a read only column, or not.
    /// </summary>
    [WithGenerator] bool IsReadOnly { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of metadata pairs in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the metadata pair whose tag contains the given tag name. If no pair is found, then
    /// an exception will be thrown.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    T this[string name] { get; }

    /// <summary>
    /// Tries to obtain the metadata pair whose tag contains the given tag name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    bool TryGet(string name, [NotNullWhen(true)] out T? item);

    /// <summary>
    /// Tries to obtains the metadata pair whose tag contains any of the tag names from the given
    /// range.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    bool TryGet(IEnumerable<string> range,  [NotNullWhen(true)] out T? item);

    /// <summary>
    /// Determines if this instance contains a metadata pair whose tag contains the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this instance contains a metadata pair whose tag contains any of the names
    /// from the given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    bool Contains(IEnumerable<string> range);

    /// <summary>
    /// Gets an array with the metadata pairs in this collection.
    /// </summary>
    /// <returns></returns>
    T[] ToArray();

    /// <summary>
    /// Gets a list with the metadata pairs in this collection.
    /// </summary>
    /// <returns></returns>
    List<T> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the metadata pair that contains the given tag name has been
    /// replaced by the new given one.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    ISchemaEntry Replace(string name, T item);

    /// <summary>
    /// Returns a new instance where the metadata pair that contains any of the names from the
    /// given range has been replaced by the new given one.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    ISchemaEntry Replace(IEnumerable<string> range, T item);

    /// <summary>
    /// Returns a new instance where the value of the metadata pair that contains the given tag
    /// name has been replaced by the new given one.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    ISchemaEntry ReplaceValue(string name, object? value);

    /// <summary>
    /// Returns a new instance where the value of the metadata pair that contains any of the given
    /// tag names has been replaced by the new given one.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    ISchemaEntry ReplaceValue(IEnumerable<string> range, object? value);

    /// <summary>
    /// Returns a new instance where the given metadata pair has been added to it.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    ISchemaEntry Add(T item);

    /// <summary>
    /// Returns a new instance where the metadata pairs from the given range have been added
    /// to it.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ISchemaEntry AddRange(IEnumerable<T> range);

    /// <summary>
    /// Returns a new instance where the metadata pair that contains the given name has been
    /// removed. If no pair can be found, returns the original instance.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    ISchemaEntry Remove(string name);

    /// <summary>
    /// Returns a new instance where the metadata pair that contains any of the names from the
    /// given range has been removed. If no pair can be found, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ISchemaEntry Remove(IEnumerable<string> range);

    /// <summary>
    /// Returns a new instance where all the original metadata pairs have been removed.
    /// </summary>
    /// <returns></returns>
    ISchemaEntry Clear();
}
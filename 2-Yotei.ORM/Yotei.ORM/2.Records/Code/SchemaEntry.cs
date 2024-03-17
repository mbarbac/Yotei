using T = Yotei.ORM.IMetadataEntry;

namespace Yotei.ORM.Code;

// ========================================================
/// <inheritdoc cref="ISchemaEntry"/>
[WithGenerator]
public sealed partial class SchemaEntry : ISchemaEntry
{
    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    SchemaEntry(SchemaEntry source) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// The identifier by which this instance is known. It cannot be null, and its last part
    /// cannot be null either.
    /// </summary>
    public IIdentifier Identifier
    {
        get => throw null;
        init => throw null;
    }

    /// <summary>
    /// Determines if this instance describes a primary key column, or one that it is part of the
    /// primary key group, if any, or not. Only one primary key group is supported per schema.
    /// </summary>
    public bool IsPrimaryKey
    {
        get => throw null;
        init => throw null;
    }

    /// <summary>
    /// Determines if this instance describes an unique valued column, or one that it is part of
    /// the unique valued group, if any, or not. Only one unique valued group is supported per
    /// schema.
    /// </summary>
    public bool IsUniqueValued
    {
        get => throw null;
        init => throw null;
    }

    /// <summary>
    /// Determines if this instance describes a read only column, or not.
    /// </summary>
    public bool IsReadOnly
    {
        get => throw null;
        init => throw null;
    }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of metadata pairs in this collection.
    /// </summary>
    public int Count { get => throw null; }

    /// <summary>
    /// Gets the metadata pair whose tag contains the given tag name. If no pair is found, then
    /// an exception will be thrown.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public T this[string name] { get => throw null; }

    /// <summary>
    /// Tries to obtain the metadata pair whose tag contains the given tag name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGet(string name, out T? value) => throw null;

    /// <summary>
    /// Tries to obtains the metadata pair whose tag contains any of the tag names from the given
    /// range.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGet(IEnumerable<string> range, out T? value) => throw null;

    /// <summary>
    /// Determines if this instance contains a metadata pair whose tag contains the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool Contains(string name) => throw null;

    /// <summary>
    /// Determines if this instance contains a metadata pair whose tag contains any of the names
    /// from the given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool Contains(IEnumerable<string> range) => throw null;

    /// <summary>
    /// Gets an array with the metadata pairs in this collection.
    /// </summary>
    /// <returns></returns>
    public T[] ToArray() => throw null;

    /// <summary>
    /// Gets a list with the metadata pairs in this collection.
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the metadata pair that contains the given tag name has been
    /// replaced by the new given one.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public ISchemaEntry Replace(string name, T item) => throw null;

    /// <summary>
    /// Returns a new instance where the metadata pair that contains any of the names from the
    /// given range has been replaced by the new given one.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public ISchemaEntry Replace(IEnumerable<string> range, T item) => throw null;

    /// <summary>
    /// Returns a new instance where the given metadata pair has been added to it.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public ISchemaEntry Add(T item) => throw null;

    /// <summary>
    /// Returns a new instance where the metadata pairs from the given range have been added
    /// to it.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public ISchemaEntry AddRange(IEnumerable<T> range) => throw null;

    /// <summary>
    /// Returns a new instance where the metadata pair that contains the given name has been
    /// removed. If no pair can be found, returns the original instance.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public ISchemaEntry Remove(string name) => throw null;

    /// <summary>
    /// Returns a new instance where the metadata pair that contains any of the names from the
    /// given range has been removed. If no pair can be found, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    public ISchemaEntry Remove(IEnumerable<string> range) => throw null;

    /// <summary>
    /// Returns a new instance where all the original metadata pairs have been removed.
    /// </summary>
    /// <returns></returns>
    public ISchemaEntry Clear() => throw null;
}
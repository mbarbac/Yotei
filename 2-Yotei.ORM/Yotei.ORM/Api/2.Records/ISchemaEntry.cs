using T = Yotei.ORM.IMetadataEntry;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Maintains the metadata pairs that describe an associated entry in a record, identified by
/// its unique identifier.
/// </summary>
[Cloneable]
public partial interface ISchemaEntry : IEnumerable<T>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The identifier by which this instance is known.
    /// </summary>
    [WithGenerator] IIdentifier Identifier { get; }

    /// <summary>
    /// Determines if this instance describes a primary key entry, or that it is part of the
    /// primary key group, if any, or not.
    /// </summary>
    [WithGenerator] bool IsPrimaryKey { get; }

    /// <summary>
    /// Determines if this instance describes an unique valued entry, or that it is part of the
    /// unique valued group, if any, or not.
    /// </summary>
    [WithGenerator] bool IsUniqueValued { get; }

    /// <summary>
    /// Determines if this instance describes a read only entry, or not.
    /// </summary>
    [WithGenerator] bool IsReadOnly { get; }

    /// <summary>
    /// The collection of tag names carried by all entries in this instance.
    /// </summary>
    IEnumerable<string> Names { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the value of the metadata pair that contains the given tag name.
    /// <br/> If such metadata pair cannot be found using the given tag name, and exception will
    /// be thrown.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    object? this[string name] { get; }

    /// <summary>
    /// Obtains the value of the metadata pair that contains the given tag name, provided that
    /// there is such metadata pair.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGetValue(string name, out object? value);

    /// <summary>
    /// Obtains the value of the metadata pair that contains the given tag name, provided that
    /// there is such metadata pair.
    /// </summary>
    /// <typeparam name="V"></typeparam>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGetValue<V>(string name, out V value);

    /// <summary>
    /// Determines if this collection contains a metadata pair that contains the given tag name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this collection contains a metadata pair that contains any of the names from
    /// the given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    bool ContainsAny(IEnumerable<string> range);

    /// <summary>
    /// Returns the metadata pair that contains the given tag name, or null if such cannot be
    /// found.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    T? Find(string name);

    /// <summary>
    /// Returns the metadata pair that contains any of the names from the given range, or null if
    /// such cannot be found.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    T? Find(IEnumerable<string> range);

    /// <summary>
    /// Returns the first metadata pair that matches the given predicate, or null if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    T? Find(Predicate<T> predicate);

    /// <summary>
    /// Returns the last metadata pair that matches the given predicate, or null if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    T? FindLast(Predicate<T> predicate);

    /// <summary>
    /// Returns all the metadata pairs that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<T> FindAll(Predicate<T> predicate);

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
    /// Returns a new instance where the value of the metadata pair that contains the given tag
    /// name has been replaced by the new one.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    ISchemaEntry Replace(string name, object? value);

    /// <summary>
    /// Returns a new instance with the given metadata pair added to it.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    ISchemaEntry Add(T item);

    /// <summary>
    /// Returns a new instance with the metadata pairs from the given range added to it. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ISchemaEntry AddRange(IEnumerable<T> range);

    /// <summary>
    /// Returns a new instance with the metadata pair that contains the given tag name removed.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    ISchemaEntry Remove(string name);

    /// <summary>
    /// Returns a new instance with the first metadata pair that matches the given predicate
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    ISchemaEntry Remove(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance with the last metadata pair that matches the given predicate
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    ISchemaEntry RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance with all the metadata pairs that match the given predicate
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    ISchemaEntry RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Returns a new instance with all the original metadata pairs removed.
    /// </summary>
    /// <returns></returns>
    ISchemaEntry Clear();
}
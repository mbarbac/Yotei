using TPair = System.Collections.Generic.KeyValuePair<string, object?>;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// An immutable object that represents the metadata associated with a given record entry.
/// <br/> Elements with duplicated tag names are not allowed.
/// </summary>
public partial interface ISchemaEntry : IEnumerable<TPair>
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
    /// Determines if this instance describes an entry that is a primary key, or is part of the
    /// primary key group, if any, or not.
    /// </summary>
    [WithGenerator] bool IsPrimaryKey { get; }

    /// <summary>
    /// Determines if this instance describes an entry that is a unique valued one, or is part of
    /// the unique valued group, if any, or not.
    /// </summary>
    [WithGenerator] bool IsUniqueValued { get; }

    /// <summary>
    /// Determines if this instance describes a read only entry in a record, or not.
    /// </summary>
    [WithGenerator] bool IsReadOnly { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of metadata entries in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the metadata tag names in this collection.
    /// </summary>
    IEnumerable<string> Names { get; }

    /// <summary>
    /// Returns the value of the metadata entry whose tag name is given. If such tag does not
    /// exist, an exception is thrown.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    object? this[string name] { get; }

    /// <summary>
    /// Determines if this instance contains a metadata entry with the given tag name, or not.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Tries to obtain the value of the metadata entry with the given tag name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGetValue(string name, out object? value);

    /// <summary>
    /// Tries to obtain the value of the metadata entry with the given tag name, provided that
    /// value is of the requested type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGetValue<T>(string name, out T value);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance where the value of the metadata entry with the given tag name has
    /// been replaced with the given one. If such entry did not exist, a new metadata entry is
    /// added to the returned instance.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    ISchemaEntry Replace(string name, object? value);

    /// <summary>
    /// Returns a new instance where a new metadata pair with the given tag name and value has been
    /// added. If a pair with the given tag already exists, and exception is thrown.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    ISchemaEntry Add(string name, object? value);

    /// <summary>
    /// Returns a new instance where the given metadata pair has been added. If a pair with the
    /// given tag already exists, and exception is thrown.
    /// </summary>
    /// <param name="pair"></param>
    /// <returns></returns>
    ISchemaEntry Add(TPair pair);

    /// <summary>
    /// Returns a new instance where the metadata pairs from the given range have been added to the
    /// original instance. If any has a tag name that already exist, an exception is thrown.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    ISchemaEntry AddRange(IEnumerable<TPair> range);

    /// <summary>
    /// Returns a new instance where the metadata entry with the given tag has been removed.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    ISchemaEntry Remove(string name);

    /// <summary>
    /// Returns a new instance where all the original metadata entries have been removed.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <returns></returns>
    ISchemaEntry Clear();
}
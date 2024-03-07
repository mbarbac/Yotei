namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents the contents retrieved from, or to be persisted into, an underlying database.
/// </summary>
[Cloneable]
public partial interface IRecord : IEnumerable<object?>
{
    /// <summary>
    /// The schema that describes the structure and contents of this record.
    /// </summary>
    ISchema Schema { get; }

    /// <summary>
    /// Gets the number of entries in this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the value stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    object? this[int index] { get; }

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance that contains the requested number of values, and their associated
    /// schema entries, starting from the given index. If no changes are detected, returns the
    /// original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IRecord GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance with the value at the given index replaced by the new given one.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Replace(int index, object? value);

    /// <summary>
    /// Returns a new instance with the value and entry at the given index replaced by the new
    /// given ones. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    IRecord Replace(int index, object? value, ISchemaEntry entry);

    /// <summary>
    /// Returns a new instance with the given value and entry pair added to the original one.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    IRecord Add(object? value, ISchemaEntry entry);

    /// <summary>
    /// Returns a new instance with the value and entry pairs from the given range added to the
    /// original one. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord AddRange(IEnumerable<(object?, ISchemaEntry)> range);

    /// <summary>
    /// Returns a new instance with the value and entry pairs from the given ranges added to the
    /// original one. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    IRecord AddRange(IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries);

    /// <summary>
    /// Returns a new instance with the given value and entry pair inserted into the original
    /// one at the given index.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    IRecord Insert(object? value, ISchemaEntry entry);

    /// <summary>
    /// Returns a new instance with the value and entry pairs from the given range inserted into
    /// the original one, starting at the given index. If no changes are detected, returns the
    /// original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord InsertRange(int index, IEnumerable<(object?, ISchemaEntry)> range);

    /// <summary>
    /// Returns a new instance with the value and entry pairs from the given ranges inserted into
    /// the original one, starting at the given index. If no changes are detected, returns the
    /// original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    IRecord InsertRange(int index, IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries);

    /// <summary>
    /// Returns a new instance with the value and entry at the given index removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IRecord RemoveAt(int index);

    /// <summary>
    /// Returns a new instance with the requested number or values and entries removed, starting
    /// from the given index. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IRecord RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance with all the original values and entries removed. If no changes
    /// are detected, detected, returns the original instance.
    /// </summary>
    /// <returns></returns>
    IRecord Clear();
}
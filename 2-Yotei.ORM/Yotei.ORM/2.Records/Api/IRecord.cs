namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the ordered collection of related values to be retrieved or to be persisted into
/// an underlying database.
/// </summary>
[Cloneable]
public partial interface IRecord : IEnumerable<object?>, IEquatable<IRecord>
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder GetBuilder();

    /// <summary>
    /// The schema that describes the structure and contents of this instance, or <c>null</c>
    /// if it is a schema-less one.
    /// </summary>
    [With] ISchema? Schema { get; }

    /// <summary>
    /// The number of values carried by this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the value at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    object? this[int index] { get; }

    /// <summary>
    /// Tries to get the value associated to the first entry with the given identifier.
    /// <br/> This method throws an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="value"></param>
    /// <param name="entry">
    /// <returns></returns>
    bool TryGet(string identifier, out object? value, [NotNullWhen(true)] out ISchemaEntry? entry);

    /// <summary>
    /// Gets an array with the values in this instance.
    /// </summary>
    /// <returns></returns>
    object?[] ToArray();

    /// <summary>
    /// Gets a list with the values in this instance.
    /// </summary>
    /// <returns></returns>
    List<object?> ToList();

    /// <summary>
    /// Gets a list with the given number of values starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<object?> ToList(int index, int count);

    // ------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of original elements, starting at the given
    /// index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IRecord GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the value at the given index has been replaced by the new
    /// given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Replace(int index, object? value);

    /// <summary>
    /// Returns a new instance where the value and schema entry at the given index have been
    /// replaced by the new given ones.
    /// <br/> This method throw an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    IRecord Replace(int index, object? value, ISchemaEntry entry);

    /// <summary>
    /// Returns a new instance where the given value has been added to the original instance.
    /// <br/> This method throw an exception if this instance is a schema-full one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Add(object? value);

    /// <summary>
    /// Returns a new instance where the given value and schema entry have been added to the
    /// original instance.
    /// <br/> This method throw an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    IRecord Add(object? value, ISchemaEntry entry);

    /// <summary>
    /// Returns a new instance where the values from the given range have been added to the
    /// original instance.
    /// <br/> This method throw an exception if this instance is a schema-full one.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord AddRange(IEnumerable<object?> range);

    /// <summary>
    /// Returns a new instance where the values and schema entries from the given ranges have
    /// been added to the original instance.
    /// <br/> This method throw an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    IRecord AddRange(IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries);

    /// <summary>
    /// Returns a new instance where the given value has been inserter into the original instance,
    /// at the given index.
    /// <br/> This method throw an exception if this instance is a schema-full one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Insert(int index, object? value);

    /// <summary>
    /// Returns a new instance where the given value and schema entry have been inserted into the
    /// original instance, at the given index.
    /// <br/> This method throw an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    IRecord Insert(int index, object? value, ISchemaEntry entry);

    /// <summary>
    /// Inserts the values from the given range into this instance, starting at the given index.
    /// <br/> This method throw an exception if this instance is a schema-full one.
    /// <br/> Returns whether changes have been made, or not.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord InsertRange(int index, IEnumerable<object?> range);

    /// <summary>
    /// Returns a new instance where the values from the given range have been inserted into the
    /// original instance, starting at the given index.
    /// <br/> This method throw an exception if this instance is a schema-full one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    IRecord InsertRange(int index, IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries);

    /// <summary>
    /// Returns a new instance where the values and schema entries from the given ranges have
    /// been inserted into the original instance.
    /// <br/> This method throw an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IRecord RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of values and schema entries, if any, have
    /// been removed from the original collection, stating from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IRecord RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where all the original contents have been cleared.
    /// </summary>
    /// <returns></returns>
    IRecord Clear();
}
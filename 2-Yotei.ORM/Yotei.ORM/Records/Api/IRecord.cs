namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an ordered collection of values to be retrieved from or persisted into an underlying
/// database as a single unit. Instances of this type may carry an optional schema that describes
/// their structure and metadata.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface IRecord : IEnumerable<object?>, IEquatable<IRecord>
{
    /// <summary>
    /// Returns a new builder based upon de contents of this record.
    /// </summary>
    /// <returns></returns>
    IBuilder ToBuilder();

    // ------------------------------------------------

    /// <summary>
    /// Gets the schema associated with this instance. If the value of this property is null, then
    /// this instance is a schema-less one that only contains values and no metadata.
    /// </summary>
    [With] ISchema? Schema { get; }

    /// <summary>
    /// Gets the number of values in this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the value carried by this instance at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    object? this[int index] { get; }

    /// <summary>
    /// Gets the value and metadata carried by this instance at the given index. This method
    /// throws an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    object? Get(int index, out ISchemaEntry entry);

    /// <summary>
    /// Gets the value of the element whose identifier is given. This property throws an exception
    /// if such element cannot be found, or if this instance is schema-less one.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    object? this[string identifier] { get; }

    /// <summary>
    /// Tries to get the value of the element whose identifier is given. This method throws an
    /// exception if this instance is schema-less one.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    bool TryGet(
        string identifier,
        out object? value, [NotNullWhen(true)] out ISchemaEntry? entry);

    /// <summary>
    /// Gets an array with the values of this instance.
    /// </summary>
    /// <returns></returns>
    object?[] ToArray();

    /// <summary>
    /// Gets a list with the values of this instance.
    /// </summary>
    /// <returns></returns>
    List<object?> ToList();

    /// <summary>
    /// Gets a list with the given number of values from this instance, starting at the given
    /// index.
    /// </summary>
    /// <returns></returns>
    List<object?> ToList(int index, int count);

    // ------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance where the value at the given index has been replaced by
    /// new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Replace(int index, object? value);

    /// <summary>
    /// Returns a copy of this instance where the value and metadata at the given index have been
    /// replaced by the given ones.This method throws an exception if this instance is a schema-less
    /// one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    IRecord Replace(int index, object? value, ISchemaEntry entry);

    /// <summary>
    /// Returns a copy of this instance where the given values has been added to it. This method
    /// throws an exception if this instance is a schema-ready one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Add(object? value);

    /// <summary>
    /// Returns a copy of this instance where the given value and metadata have been added to it.
    /// This method throws an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    IRecord Add(object? value, ISchemaEntry entry);

    /// <summary>
    /// Returns a copy of this instance where the values from the given range have been added to
    /// it. This method throws an exception if this instance is a schema-ready one.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    IRecord AddRange(IEnumerable<object?> values);

    /// <summary>
    /// Returns a copy of this instance where the values and metadata from the given ranges have
    /// been added to it. This method throws an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    IRecord AddRange(IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries);

    /// <summary>
    /// Returns a copy of this instance where the given value has been inserted into it, at the
    /// given index. This method throws an exception if this instance is a schema-ready one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Insert(int index, object? value);

    /// <summary>
    /// Returns a copy of this instance where the given value and metadata have been inserted into
    /// it, at the given index. This method throws an exception if this instance is a schema-less
    /// one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    IRecord Insert(int index, object? value, ISchemaEntry entry);

    /// <summary>
    /// Returns a copy of this instance where the values from the given range have been inserted
    /// into it, starting at the given index. This method throws an exception if this instance is
    /// a schema-ready one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    IRecord InsertRange(int index, IEnumerable<object?> values);

    /// <summary>
    /// Returns a copy of this instance where the values and metadata from the given ranges have
    /// been inserted into it, starting at the given index. This method throws an exception if this
    /// instance is a schema-less one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="values"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    IRecord InsertRange(int index, IEnumerable<object?> values, IEnumerable<ISchemaEntry> entries);

    /// <summary>
    /// Returns a copy of this instance where the value and metadata at the given index, and
    /// redundant ones, if any, have been removed from it.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IRecord RemoveAt(int index);

    /// <summary>
    /// Returns a copy of this instance where where the value and metadata of the entry whose
    /// identifier is given, and redundant ones, if any, have been removed from it. This method
    /// throws an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    IRecord Remove(string identifier);

    /// <summary>
    /// Returns a copy of this instance where all the values and metadata, if any, have been
    /// cleared.
    /// </summary>
    /// <returns></returns>
    IRecord Clear();
}
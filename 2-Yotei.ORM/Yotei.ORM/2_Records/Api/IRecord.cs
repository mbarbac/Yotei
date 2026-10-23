namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents a record, an ordered collection of values to be retrieved from or persisted into
/// an underlying database as a single unit. Instances of this type may have a schema descriptor
/// that describes its structure and carry associated metadata.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface IRecord
    : IEnumerable<object?>, IEquatable<IRecord>, IDynamicMetaObjectProvider
{
    /// <summary>
    /// Creates a new builder initialized with the current object's values.
    /// </summary>
    /// <returns>A builder instance with values copied from the current object.</returns>
    IBuilder ToBuilder();

    // ----------------------------------------------------

    /// <summary>
    /// The schema that describes the structure and metadata of this record, or null if this
    /// instance is a schema-less one.
    /// </summary>
    [With] ISchemaDescriptor? Schema { get; }

    /// <summary>
    /// Gets the size of this instance (the number of values it contains).
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the value carried by this instance at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    object? this[int index] { get; }

    /// <summary>
    /// Gets the value carried by this instance at the index associated with the schema element
    /// whose identifier is given.
    /// <br/> An exception is thrown if several schema elements are found with that identifier.
    /// <br/> An exception is thrown if this instance is a schema-less one.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    object? this[string identifier] { get; }

    /// <summary>
    /// Attempts to retrieve the value and schema element associated with the specified identifier.
    /// <br/> An exception is thrown if several schema elements are found with that identifier.
    /// <br/> An exception is thrown if this instance is a schema-less one.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="value"></param>
    /// <param name="element"></param>
    /// <returns></returns>
    bool TryGet(string identifier, out object? value, [NotNullWhen(true)] out ISchemaElement element);

    /// <summary>
    /// Gets an array with the values carried by this instance.
    /// </summary>
    /// <returns></returns>
    object?[] ToArray();

    /// <summary>
    /// Gets a list with the values carried by this instance.
    /// </summary>
    /// <returns></returns>
    List<object?> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance containing the given number of elements, starting at the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IRecord GetRange(int index, int count);

    /// <summary>
    /// Returns a copy of this instance where the existing value has been replaced by the new
    /// given one.
    /// <br/> An exception is thrown if this instance is a schema-full one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns>A new instance, or the original one if no changes have been made.</returns>
    IRecord Replace(int index, object? value);

    /// <summary>
    /// Returns a copy of this instance where the existing value, and schema element, have been
    /// replaced by the new given ones.
    /// <br/> An exception is thrown if this instance is a schema-less one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="element"></param>
    /// <returns>A new instance, or the original one if no changes have been made.</returns>
    IRecord Replace(int index, object? value, ISchemaElement element);

    /// <summary>
    /// Returns a copy of this instance where the given value has been added to it.
    /// <br/> An exception is thrown if this instance is a not-empty schema-full one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>A new instance, or the original one if no changes have been made.</returns>
    IRecord Add(object? value);

    /// <summary>
    /// Returns a copy of this instance where the given value and schema element have been added
    /// to it.
    /// <br/> An exception is thrown if this instance is a not-empty schema-less one.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="element"></param>
    /// <returns>A new instance, or the original one if no changes have been made.</returns>
    IRecord Add(object? value, ISchemaElement element);

    /// <summary>
    /// Returns a copy of this instance where the given values have been added to it.
    /// <br/> An exception is thrown if this instance is a not-empty schema-full one.
    /// </summary>
    /// <param name="values"></param>
    /// <returns>A new instance, or the original one if no changes have been made.</returns>
    IRecord AddRange(IEnumerable<object?> values);

    /// <summary>
    /// Returns a copy of this instance where the given values and associated schema elements
    /// have been added to it.
    /// <br/> An exception is thrown if this instance is a not-empty schema-less one.
    /// </summary>
    /// <param name="values"></param>
    /// <param name="elements"></param>
    /// <returns>A new instance, or the original one if no changes have been made.</returns>
    IRecord AddRange(IEnumerable<object?> values, IEnumerable<ISchemaElement> elements);

    /// <summary>
    /// Returns a copy of this instance where the given value has been inserted into it, at the
    /// given index.
    /// <br/> An exception is thrown if this instance is a not-empty schema-full one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns>A new instance, or the original one if no changes have been made.</returns>
    IRecord Insert(int index, object? value);

    /// <summary>
    /// Returns a copy of this instance where the given value and schema element have been inserted
    /// into it, at the given index.
    /// <br/> An exception is thrown if this instance is a not-empty schema-less one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="element"></param>
    /// <returns>A new instance, or the original one if no changes have been made.</returns>
    IRecord Insert(int index, object? value, ISchemaElement element);

    /// <summary>
    /// Returns a copy of this instance where the given values and associated schema elements have
    /// been inserted into it, starting at the given index.
    /// <br/> An exception is thrown if this instance is a not-empty schema-full one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="values"></param>
    /// <returns>A new instance, or the original one if no changes have been made.</returns>
    IRecord InsertRange(int index, IEnumerable<object?> values);

    /// <summary>
    /// Returns a copy of this instance where the given values and associated schema elements have
    /// been inserted into it, starting at the given index.
    /// <br/> An exception is thrown if this instance is a not-empty schema-less one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="values"></param>
    /// <param name="elements"></param>
    /// <returns>A new instance, or the original one if no changes have been made.</returns>
    IRecord InsertRange(int index, IEnumerable<object?> values, IEnumerable<ISchemaElement> elements);

    /// <summary>
    /// Removes the value at the specified index, along with its associated schema element, if
    /// any.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>A new instance, or the original one if no changes have been made.</returns>
    IRecord RemoveAt(int index);

    /// <summary>
    /// Removes the first value associated with a schema element whose identifier is given.
    /// <br/> An exception is thrown if this instance is a schema-less one.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns>A new instance, or the original one if no changes have been made.</returns>
    IRecord Remove(string identifier);

    /// <summary>
    /// Removes the last value associated with a schema element whose identifier is given.
    /// <br/> An exception is thrown if this instance is a schema-less one.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns>A new instance, or the original one if no changes have been made.</returns>
    IRecord RemoveLast(string identifier);

    /// <summary>
    /// Removes all the values associated with schema elements whose identifier is given.
    /// <br/> An exception is thrown if this instance is a schema-less one.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns>A new instance, or the original one if no changes have been made.</returns>
    IRecord RemoveAll(string identifier);

    /// <summary>
    /// Clears the values of this instance, and removes the schema if such is requested.
    /// </summary>
    /// <returns></returns>
    IRecord Clear(bool removeSchema);
}
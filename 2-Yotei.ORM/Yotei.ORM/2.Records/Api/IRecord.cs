namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the ordered collection of related values to be retrieved from, or persisted into,
/// an underlying database as a single unit. Instances of this type may carry a schema that, if
/// not null, acts as a descriptor for the record structure and contents.
/// </summary>
[Cloneable]
public partial interface IRecord : IEnumerable<object?>, IEquatable<IRecord>
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    // ----------------------------------------------------

    /// <summary>
    /// If not null, the schema that describes the structure and contents of this record.
    /// If null, then this instance is a schema-less one.
    /// </summary>
    [With] ISchema? Schema { get; }

    /// <summary>
    /// The number of elements carried by this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the value at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    object? this[int index] { get; }

    /// <summary>
    /// Gets the value associated with the entry whose unique identifier is given. This property
    /// throws an exception if that identifier is not found, or if this instance is a schema-less
    /// one.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    object? this[string identifier] { get; }

    /// <summary>
    /// Tries to get the value associated with the entry whose unique identifier is given. This
    /// method throws an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGet(string identifier, out object? value);

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
    /// Gets a list with the given number of values, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<object?> ToList(int index, int count);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of original elements starting at the given
    /// index.
    /// <br/> If this instance carried a schema, then the new instance carries an adapted one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IRecord GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the element at the given index has been replaced by the given
    /// one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Replace(int index, object? value);

    /// <summary>
    /// Returns a new instance where the element at the given index has been replaced by the given
    /// one.
    /// <br/> The original schema is replaced by the new given one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="schema"></param>
    /// <returns></returns>
    IRecord Replace(int index, object? value, ISchema schema);

    /// <summary>
    /// Returns a new instance where the given element has been added to the collection.
    /// <br/> This method fails if this instance is a schema-full one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Add(object? value);

    /// <summary>
    /// Returns a new instance where the given element has been added to the collection.
    /// <br/> The original schema is replaced by the new given one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="schema"></param>
    /// <returns></returns>
    IRecord Add(object? value, ISchema schema);

    /// <summary>
    /// Returns a new instance where the elements of the given range have been added to the
    /// collection.
    /// <br/> This method fails if this instance is a schema-full one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord AddRange(IEnumerable<object?> range);

    /// <summary>
    /// Returns a new instance where the elements of the given range have been added to the
    /// collection.
    /// <br/> The original schema is replaced by the new given one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="schema"></param>
    /// <returns></returns>
    IRecord AddRange(IEnumerable<object?> range, ISchema schema);

    /// <summary>
    /// Returns a new instance where the given element has been inserted into the collection at
    /// the given index.
    /// <br/> This method fails if this instance is a schema-full one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Insert(int index, object? value);

    /// <summary>
    /// Returns a new instance where the given element has been inserted into the collection at
    /// the given index.
    /// <br/> The original schema is replaced by the new given one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="schema"></param>
    /// <returns></returns>
    IRecord Insert(int index, object? value, ISchema schema);

    /// <summary>
    /// Returns a new instance where the elements of the given range have been inserted into the
    /// collection, starting from the given index.
    /// <br/> This method fails if this instance is a schema-full one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord InsertRange(int index, IEnumerable<object?> range);

    /// <summary>
    /// Returns a new instance where the elements of the given range have been inserted into the
    /// collection, starting from the given index.
    /// <br/> The original schema is replaced by the new given one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <param name="schema"></param>
    /// <returns></returns>
    IRecord InsertRange(int index, IEnumerable<object?> range, ISchema schema);

    /// <summary>
    /// Returns a new instance where the element at the given index has been removed.
    /// <br/> If this instance carried a schema, then the new instance carries an adapted one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IRecord RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of elements, starting at the given index,
    /// have been removed.
    /// <br/> If this instance carried a schema, then the new instance carries an adapted one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IRecord RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the element associated with the entry whose unique identifier
    /// id given has been removed.
    /// <br/> This method throws an exception if this instance is a schema-less one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    IRecord Remove(string identifier);

    /// <summary>
    /// Returns a new instance where all the original elements have been removed.
    /// <br/> If this instance carried a schema, then the new instance carries an adapted one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <returns></returns>
    IRecord Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new record with the changes detected comparing this instance against the given
    /// one, using a default comparer to compare their respective values, or null if no changes
    /// are detected.
    /// <br/> Both this record and the target one must be schema-full instances.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="orphanSources"></param>
    /// <param name="orphanTargets"></param>
    /// <returns></returns>
    IRecord? GetChanges(
        IRecord target,
        bool orphanSources = false, bool orphanTargets = false);

    /// <summary>
    /// Returns a new record with the changes detected comparing this instance against the given
    /// one, using a default comparer to compare their respective values, or null if no changes
    /// are detected.
    /// <br/> Both this record and the target one must be schema-full instances.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="comparer"></param>
    /// <param name="orphanSources"></param>
    /// <param name="orphanTargets"></param>
    /// <returns></returns>
    IRecord? GetChanges(
        IRecord target,
        IEqualityComparer comparer,
        bool orphanSources = false, bool orphanTargets = false);
}
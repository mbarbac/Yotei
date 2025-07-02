namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents an ordered collection of related values to be retrieved from or to be persisted
/// into an undeerlying database. Instances of this type may carry a schema object that, if not
/// null, acts as a descriptor of its structure and contents.
/// <para>Instances of this type are intended to be immutable ones.</para>
/// </summary>
[Cloneable]
public partial interface IRecord : IEnumerable<object?>, IEquatable<IRecord>
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    /// <summary>
    /// The schema that describes the structure and contents of this instance, or <c>null</c>
    /// if it this instance is a schema-less one.
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
    bool TryGet(string identifier, out object value);

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

    // ----------------------------------------------------

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
    /// <br/> This method throws an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    IRecord Replace(int index, object? value, ISchemaEntry entry);

    /// <summary>
    /// Returns a new instance where the given value has been added to the original instance.
    /// <br/> This method throws an exception if this instance is a schema-full one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Add(object? value);

    /// <summary>
    /// Returns a new instance where the given value and schema entry have been added to the
    /// original instance.
    /// <br/> This method throws an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    IRecord Add(object? value, ISchemaEntry entry);

    /// <summary>
    /// Returns a new instance where the values from the given range have been added to the
    /// original instance.
    /// <br/> This method throws an exception if this instance is a schema-full one.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord AddRange(IEnumerable<object?> range);

    /// <summary>
    /// Returns a new instance where the values and schema entries from the given ranges have
    /// been added to the original instance.
    /// <br/> This method throws an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    IRecord AddRange(IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries);

    /// <summary>
    /// Returns a new instance where the given value has been inserted into the original instance,
    /// at the given index.
    /// <br/> This method throws an exception if this instance is a schema-full one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Insert(int index, object? value);

    /// <summary>
    /// Returns a new instance where the given value and schema entry have been inserted into the
    /// original instance, at the given index.
    /// <br/> This method throws an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    IRecord Insert(int index, object? value, ISchemaEntry entry);

    /// <summary>
    /// Returns a new instance where the values from the given range have been inserted into
    /// the original instamce, starting at the given index.
    /// <br/> This method throws an exception if this instance is a schema-full one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord InsertRange(int index, IEnumerable<object?> range);

    /// <summary>
    /// Returns a new instance where the values and schema entries from the given ranges have
    /// been inserted into the original instance, starting at the given index.
    /// <br/> This method throws an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    IRecord InsertRange(int index, IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries);

    /// <summary>
    /// Returns a new instance where the value and schema entry, if any, at the given index
    /// have been removed from the original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IRecord RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of values and schema entries, if any, have
    /// been removed from the original collection, starting from the given index.
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

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new record with the changes detected at the target one when it is compared
    /// against this instance, using a default comparer to compare their respective values, or
    /// null if no changes are detected.
    /// <br/> Both records must be schema-full instances.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="orphanSources"></param>
    /// <param name="orphanTargets"></param>
    /// <returns></returns>
    IRecord? GetChanges(
        IRecord target,
        bool orphanSources = false, bool orphanTargets = false);

    /// <summary>
    /// Returns a new record with the changes detected at the target one when it is compared
    /// against this instance, using the given comparer to compare their respective values, or
    /// null if no changes are detected.
    /// <br/> Both records must be schema-full instances.
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
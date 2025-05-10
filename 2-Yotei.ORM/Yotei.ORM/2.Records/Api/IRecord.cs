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
    /// The schema that describes the structure and contents of this instance, or <c>null</c> if
    /// it is a schema-less one.
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
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGet(string identifier, out object? value);

    /// <summary>
    /// Gets the value at the given index, casted to the given type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"></param>
    /// <returns></returns>
    [return: MaybeNull]
    T This<T>(int index);

    /// <summary>
    /// Tries to get the value associated to the first entry with the given identifier, casted
    /// to the given type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="identifier"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool TryGet<T>(string identifier, out T? value);

    /// <summary>
    /// Gets an array with the values in this record.
    /// </summary>
    /// <returns></returns>
    object?[] ToArray();

    /// <summary>
    /// Gets a list with the values in this record.
    /// </summary>
    /// <returns></returns>
    List<object?> ToList();

    /// <summary>
    /// Gets a list with the requested number of values starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<object?> ToList(int index, int count);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of elements, starting from the given index.
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
    /// replacedby the new given ones.
    /// <br/> This method throw an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    IRecord Replace(int index, object? value, ISchemaEntry entry);

    /// <summary>
    /// Returns a new instance where the given value has been added to the original record.
    /// <br/> This method throw an exception if this instance is a schema-full one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Add(object? value);

    /// <summary>
    /// Returns a new instance where the given value and schema entry have been added to the
    /// original record.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    IRecord Add(object? value, ISchemaEntry entry);

    /// <summary>
    /// Returns a new instance where the values from the given range have been added to the
    /// original record.
    /// <br/> This method throw an exception if this instance is a schema-full one.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord AddRange(IEnumerable<object?> range);

    /// <summary>
    /// Returns a new instance where the values and schema entries from the given ranges have
    /// been added to the original record.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    IRecord AddRange(IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries);

    /// <summary>
    /// Returns a new instance where the given value has been inserted into the original record
    /// at the given index.
    /// <br/> This method throw an exception if this instance is a schema-full one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Insert(int index, object? value);

    /// <summary>
    /// Returns a new instance where the given value and schema entry have been inserted into
    /// the original record at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    IRecord Insert(int index, object? value, ISchemaEntry entry);

    /// <summary>
    /// Returns a new instance where the values and schema entries from the given ranges have been
    /// inserted into the original record, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord InsertRange(int index, IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries);

    /// <summary>
    /// Returns a new instance where the original value at the given index has been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IRecord RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of values, starting at the given index,
    /// have been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IRecord RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the first value that matches the given predicate has been
    /// removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IRecord Remove(Predicate<object?> predicate);

    /// <summary>
    /// Returns a new instance where the last value that matches the given predicate has been
    /// removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IRecord RemoveLast(Predicate<object?> predicate);

    /// <summary>
    /// Returns a new instance where all the values that match the given predicate have been
    /// removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IRecord RemoveAll(Predicate<object?> predicate);

    /// <summary>
    /// Returns a new instance where all the original values have been removed.
    /// </summary>
    /// <returns></returns>
    IRecord Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Gets a new record with the changes detected at the given target one when it is compared
    /// with this instance, using a default object comparer to compare their respective values,
    /// or null if any changes are detected.
    /// <br/> If schema usage is requested, then both instances must either be schema-less or
    /// schema-full ones. If not, or no schema is available in neither one, then comparison is
    /// performed comparing in order their respective values. Otherwise, the values of their
    /// identifiers are used to select the values to compare.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="orphanSources"></param>
    /// <param name="orphanTargets"></param>
    /// <returns></returns>
    IRecord? GetChanges(
        IRecord target,
        bool useSchema = true,
        bool orphanSources = false, bool orphanTargets = false);

    /// <summary>
    /// Gets a new record with the changes detected at the given target one when it is compared
    /// with this instance, using the given comparer to compare their respective values, or null
    /// if any changes are detected.
    /// <br/> If schema usage is requested, then both instances must either be schema-less or
    /// schema-full ones. If not, or no schema is available in neither one, then comparison is
    /// performed comparing in order their respective values. Otherwise, the values of their
    /// identifiers are used to select the values to compare.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="comparer"></param>
    /// <param name="orphanSources"></param>
    /// <param name="orphanTargets"></param>
    /// <returns></returns>
    IRecord? GetChanges(
        IRecord target,
        IEqualityComparer comparer,
        bool useSchema = true,
        bool orphanSources = false, bool orphanTargets = false);
}
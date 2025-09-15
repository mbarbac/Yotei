namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the ordered collection of related values to be retrieved from, or persisted into,
/// an underlying database as a single unit. Instances of this type may carry a schema that, if
/// not null, acts as a descriptor for the record structure and contents.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public partial interface IRecord : IEnumerable<object?>, IEquatable<IRecord>
{
    /// <inheritdoc cref="ICloneable.Clone"/>
    IRecord Clone();

    /// <summary>
    /// Emulates the 'with' keyword with instances of this type.
    /// </summary>
    IRecord WithSchema(ISchema? value);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    // ----------------------------------------------------

    /// <summary>
    /// The schema that describes the structure and contents of this instance, or <c>null</c> if
    /// it is a schema-less one.
    /// </summary>
    ISchema? Schema { get; }

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
    /// <param name="entry"></param>
    /// <returns></returns>
    bool TryGet(
        string identifier,
        out object? value, [NotNullWhen(true)] out ISchemaEntry? entry);

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
    /// Returns a new instance with the given number of values and associated entries (if any)
    /// starting from the given index.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IRecord GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the value at the given index has been replaced by the new
    /// given one, regardless if this instance is a schema-less or schema-full one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Replace(int index, object? value);

    /// <summary>
    /// Returns a new instance where where the value at the given index and its associated entry
    /// have been replaced by the new given one.
    /// <br/> This method throws an exception if it is a schema-less one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    IRecord Replace(int index, object? value, ISchemaEntry entry);

    /// <summary>
    /// Returns a new instance where the given value has been added to this instance.
    /// <br/> This method throws an exception if it is a schema-full one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Add(object? value);

    /// <summary>
    /// Returns a new instance where the given value has been added to this instance.
    /// <br/> This method throws an exception if it is a schema-less one (unless the given pair
    /// is the first one to add to this instance).
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    IRecord Add(object? value, ISchemaEntry entry);

    /// <summary>
    /// Returns a new instance where the values from the given collection have been added to this
    /// instance.
    /// <br/> This method throws an exception if it is a schema-full one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord AddRange(IEnumerable<object?> range);

    /// <summary>
    /// Returns a new instance where the values and the associates entries from the given
    /// collections have been added to this instance.
    /// <br/> This method throws an exception if it is a schema-less one (unless the given pairs
    /// are the first ones to add to this instance).
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    IRecord AddRange(IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries);

    /// <summary>
    /// Returns a new instance where the given value has been inserted into this instance at the
    /// given index.
    /// <br/> This method throws an exception if it is a schema-full one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Insert(int index, object? value);

    /// <summary>
    /// Returns a new instance where the given value and associated entry have been inserted into
    /// this instance at the given index.
    /// <br/> This method throws an exception if it is a schema-less one (unless the given pairs
    /// are the first ones to add to this instance).
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="entry"></param>
    /// <returns></returns>
    IRecord Insert(int index, object? value, ISchemaEntry entry);

    /// <summary>
    /// Returns a new instance where the given values have been inserted into this instance at
    /// the given index.
    /// <br/> This method throws an exception if it is a schema-full one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord InsertRange(int index, IEnumerable<object?> range);

    /// <summary>
    /// Returns a new instance where the given values and their associated entries have been
    /// inserted into this instance at the given index.
    /// <br/> This method throws an exception if it is a schema-less one (unless the given pairs
    /// are the first ones to add to this instance).
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <param name="entries"></param>
    /// <returns></returns>
    IRecord InsertRange(int index, IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries);

    /// <summary>
    /// Returns a new instance where the value and associated entry (if any) have been removed
    /// from the original one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IRecord RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of values and associated entries (if any)
    /// have been removed from the original one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IRecord RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the value associated with the entry whose unique identifier
    /// is given, along with that entry, have been removed.
    /// <br/> This method throws an exception if it is a schema-less one.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    IRecord Remove(string identifier);

    /// <summary>
    /// Returns a new instance where all the original elements have been removed.
    /// <br/> Returns a new modified instance, or the original one if no changes were made.
    /// </summary>
    /// <returns></returns>
    IRecord Clear();
    
    // ----------------------------------------------------

    /// <summary>
    /// Returns a new record with the changes detected comparing this instance against the given
    /// one, using a default comparer to compare their respective values, or null if no changes
    /// are detected.
    /// <br/> Both this record and the target one must be schema-full instances, and their
    /// engines must be compatible ones.
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
    /// <br/> Both this record and the target one must be schema-full instances, and their
    /// engines must be compatible ones.
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
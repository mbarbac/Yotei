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
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder ToBuilder();

    // ----------------------------------------------------

    /// <summary>
    /// Enumerates the elements in this instance. This property throws an exception if this
    /// instance is a schema-less one.
    /// </summary>
    IEnumerable<IElement> Elements { get; }

    /// <summary>
    /// The schema this instance is associated with, or <see langword="null"/> if any.
    /// </summary>
    [With] ISchema? Schema { get; }

    /// <summary>
    /// The number of elements in this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the value of the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    object? this[int index] { get; }

    /// <summary>
    /// Gets the value of the element whose unique identifier is given. This property throws an
    /// exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    object? this[string identifier] { get; }

    /// <summary>
    /// Tries to get the value and metadata of the element whose unique identifier is given. This
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
    /// Determines if this instance contains a value that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool ContainsValue(Predicate<object?> predicate);

    /// <summary>
    /// Determines if this instance contains an element that matches the given predicate. This
    /// method throws an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<IElement> predicate);

    /// <summary>
    /// Returns the index of the first value that matches the given predicate, or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOfValue(Predicate<object?> predicate);

    /// <summary>
    /// Returns the index of the first element that matches the given predicate, or -1 if any. This
    /// method throws an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<IElement> predicate);

    /// <summary>
    /// Returns the index of the last value that matches the given predicate, or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOfValue(Predicate<object?> predicate);

    /// <summary>
    /// Returns the index of the last element that matches the given predicate, or -1 if any. This
    /// method throws an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<IElement> predicate);

    /// <summary>
    /// Returns the indexes of the values that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOfValue(Predicate<object?> predicate);

    /// <summary>
    /// Returns the indexes of the elements that match the given predicate. This method throws an
    /// exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<IElement> predicate);

    /// <summary>
    /// Returns an array with the values in this collection.
    /// </summary>
    /// <returns></returns>
    object?[] ToArrayOfValues();

    /// <summary>
    /// Returns an array with the elements in this collection. This method throws an exception if
    /// this instance is a schema-less one.
    /// </summary>
    /// <returns></returns>
    IElement[] ToArray();

    /// <summary>
    /// Returns a list with the values in this collection.
    /// </summary>
    /// <returns></returns>
    List<object?> ToListOfValues();

    /// <summary>
    /// Returns a list with the elements in this collection. This method throws an exception if
    /// this instance is a schema-less one.
    /// </summary>
    /// <returns></returns>
    List<IElement> ToList();

    /// <summary>
    /// Returns a list with the requested number of values, starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<object?> ToListOfValues(int index, int count);

    /// <summary>
    /// Returns a list with the requested number of elements, starting from the given index. This
    /// method throws an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<IElement> ToList(int index, int count);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance with the requested number of elements, starting from the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IRecord GetRange(int index, int count);

    /// <summary>
    /// Returns a copy of this instance where the value at the given index has been replaced by
    /// the newly given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Replace(int index, object? value);

    /// <summary>
    /// Returns a copy of this instance where the element at the given index has been replaced by
    /// the newly given one. This method throws an exception if this instance is a schema-less one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="element"></param>
    /// <returns></returns>
    IRecord Replace(int index, IElement element);

    /// <summary>
    /// Returns a copy of this instance where the given value has been added to it. This method
    /// throws an exception if this instance is a not-empty schema-full one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Add(object? value);

    /// <summary>
    /// Returns a copy of this instance where the given element has been added to it. This method
    /// throws an exception if this instance is a not-empty schema-less one.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    IRecord Add(IElement element);

    /// <summary>
    /// Returns a copy of this instance where the values from the given range have been added
    /// to it. This method throws an exception if this instance is a not-empty schema-full one.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord AddRange(IEnumerable<object?> range);

    /// <summary>
    /// Returns a copy of this instance where the elements from the given range have been added
    /// to it. This method throws an exception if this instance is a not-empty schema-less one.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord AddRange(IEnumerable<IElement> range);

    /// <summary>
    /// Returns a copy of this instance where the given value has been inserted into it at the
    /// given index. This method throws an exception if this instance is a not-empty schema-full
    /// one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Insert(int index, object? value);

    /// <summary>
    /// Returns a copy of this instance where the given element has been inserted into it at the
    /// given index. This method throws an exception if this instance is a not-empty schema-less
    /// one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="element"></param>
    /// <returns></returns>
    IRecord Insert(int index, IElement element);

    /// <summary>
    /// Returns a copy of this instance where the values from the given range have been inserted
    /// into it starting at the given index. This method throws an exception if this instance is
    /// a not-empty schema-full one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord InsertRange(int index, IEnumerable<object?> range);

    /// <summary>
    /// Returns a copy of this instance where the values from the given range have been inserted
    /// into it starting at the given index. This method throws an exception if this instance is
    /// a not-empty schema-less one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord InsertRange(int index, IEnumerable<IElement> range);

    /// <summary>
    /// Returns a copy of this instance where the value at the given index has been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IRecord RemoveAt(int index);

    /// <summary>
    /// Returns a copy of this instance where the given number of elements, starting from the
    /// given index, have been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IRecord RemoveRange(int index, int count);

    /// <summary>
    /// Returns a copy of this instance where the first ocurrence of a value that matches the given
    /// predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IRecord RemoveValue(Predicate<object?> predicate);

    /// <summary>
    /// Returns a copy of this instance where the first ocurrence of an element that matches the
    /// given predicate has been removed. This method throws an exception if this instance is a
    /// schema-less one.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IRecord Remove(Predicate<IElement> predicate);

    /// <summary>
    /// Returns a copy of this instance where the last ocurrence of a value that matches the given
    /// predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IRecord RemoveLastValue(Predicate<object?> predicate);

    /// <summary>
    /// Returns a copy of this instance where the last ocurrence of an element that matches the
    /// given predicate has been removed. This method throws an exception if this instance is a
    /// schema-less one.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IRecord RemoveLast(Predicate<IElement> predicate);

    /// <summary>
    /// Returns a copy of this instance where all the ocurrences of values that matches the given
    /// predicate have been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IRecord RemoveValues(Predicate<object?> predicate);

    /// <summary>
    /// Returns a copy of this instance where the ocurrences of elements that match the given
    /// predicate have been removed. This method throws an exception if this instance is a
    /// schema-less one.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IRecord RemoveAll(Predicate<IElement> predicate);

    /// <summary>
    /// Returns a copy of this instance that has been cleared.
    /// </summary>
    /// <returns></returns>
    IRecord Clear();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new record with the changes detected comparing the values of this instance against
    /// the ones at the given target record, using a defaulr comparer, or null if no changes were
    /// detected. If requested, the orphan source and target elements are included in the returned
    /// record.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="orphanSources"></param>
    /// <param name="orphanTargets"></param>
    /// <returns></returns>
    IRecord? GetChanges(
        IRecord target,
        bool orphanSources = false, bool orphanTargets = false);

    /// <summary>
    /// Returns a new record with the changes detected comparing the values of this instance against
    /// the ones at the given target record, using the given comparer, or null if no changes were
    /// detected. If requested, the orphan source and target elements are included in the returned
    /// record.
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
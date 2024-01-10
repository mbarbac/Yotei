namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// An immutable array-like object that represents the contents retrieved from, or to be persisted
/// into, an underlying database.
/// </summary>
public interface IRecord : IEnumerable<object?>
{
    /// <summary>
    /// Gets the number of values in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the value stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    object? this[int index] { get; }

    /// <summary>
    /// Determines if this collection contains a value that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<object?> predicate);

    /// <summary>
    /// Returns the index of the first ocurrence of a value that matches the given predicate,
    /// or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<object?> predicate);

    /// <summary>
    /// Returns the index of the last ocurrence of a value that matches the given predicate,
    /// or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<object?> predicate);

    /// <summary>
    /// Returns the indexes of the values in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<object?> predicate);

    /// <summary>
    /// Returns an array with the values in this collection.
    /// </summary>
    /// <returns></returns>
    object?[] ToArray();

    /// <summary>
    /// Returns a list with the values in this collection.
    /// </summary>
    /// <returns></returns>
    List<object?> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of values starting from the given index.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IRecord GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the value at the given index has been replaced by the new
    /// given one, if not equal to the existing one. If no changes are detected, returns the
    /// original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IRecord Replace(int index, object? item);

    /// <summary>
    /// Returns a new instance where the given value has been added to the collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IRecord Add(object? item);

    /// <summary>
    /// Returns a new instance where the values from the given range have been added to the
    /// collection. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord AddRange(IEnumerable<object?> range);

    /// <summary>
    /// Returns a new instance where the given value has been inserted into the collection at
    /// the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IRecord Insert(int index, object? item);

    /// <summary>
    /// Returns a new instance the values from the given range have been inserted into the
    /// collection. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord InsertRange(int index, IEnumerable<object?> range);

    /// <summary>
    /// Returns a new instance where the value at the given index has been removed from the
    /// original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IRecord RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of values, starting from the given index,
    /// have been removed from the collection.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IRecord RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the first value that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IRecord Remove(Predicate<object?> predicate);

    /// <summary>
    /// Returns a new instance where the last value that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IRecord RemoveLast(Predicate<object?> predicate);

    /// <summary>
    /// Returns a new instance where all values that match the given predicate has been removed.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IRecord RemoveAll(Predicate<object?> predicate);

    /// <summary>
    /// Returns a new instance where all the values have been removed. If no changes are
    /// detected, returns the original instance.
    /// </summary>
    /// <returns></returns>
    IRecord Clear();
}
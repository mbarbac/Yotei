namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents an ordered collection of values retrieved from or persisted into an underlying
/// database as a single unit.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface IRecord : IEnumerable<object?>, IEquatable<IRecord>
{
    /// <summary>
    /// Returns a mutable builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder ToBuilder();

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the value at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    object? this[int index] { get; }

    /// <summary>
    /// Determines if this instance carries any element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<object?> predicate);

    /// <summary>
    /// Returns the index of the first element that matches the given predicate, or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<object?> predicate);

    /// <summary>
    /// Returns the index of the last element that matches the given predicate, or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<object?> predicate);

    /// <summary>
    /// Returns the indexes of all the elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<object?> predicate);

    /// <summary>
    /// Gets a list with the values in this instance.
    /// </summary>
    /// <returns></returns>
    List<object?> ToList();

    /// <summary>
    /// Gets a list with the given number of values from this instance, starting at the given
    /// index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<object?> ToList(int index, int count);

    /// <summary>
    /// Gets an array with the values in this instance.
    /// </summary>
    /// <returns></returns>
    object?[] ToArray();

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
    /// the new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Replace(int index, object? value);

    /// <summary>
    /// Returns a copy of this instance where the given value has been added to it.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Add(object? value);

    /// <summary>
    /// Returns a copy of this instance where the values from the given range have been added
    /// to it.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord AddRange(IEnumerable<object?> range);

    /// <summary>
    /// Returns a copy of this instance where the given value has been inserted into it at the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IRecord Insert(int index, object? value);

    /// <summary>
    /// Returns a copy of this instance where the values from the given range have been inserted
    /// into it starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IRecord InsertRange(int index, IEnumerable<object?> range);

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
    /// Returns a copy of this instance where the first ocurrence of an element that matches the
    /// given predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IRecord Remove(Predicate<object?> predicate);

    /// <summary>
    /// Returns a copy of this instance where the last ocurrence of an element that matches the
    /// given predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IRecord RemoveLast(Predicate<object?> predicate);

    /// <summary>
    /// Returns a copy of this instance where all the ocurrences of elements that matches the
    /// given predicate have been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IRecord RemoveAll(Predicate<object?> predicate);

    /// <summary>
    /// Returns a copy of this instance that has been cleared.
    /// </summary>
    /// <returns></returns>
    IRecord Clear();
}
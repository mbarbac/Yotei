namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents an immutable list-alike collection of elements.
/// </summary>
/// <typeparam name="T"></typeparam>
[Cloneable]
public partial interface IInvariantList<T> : IReadOnlyList<T>, IReadOnlyCollection<T>
{
    /// <summary>
    /// Returns a mutable builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    ICoreList<T> ToBuilder();

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Gets or sets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new T this[int index] { get; set; }

    /// <summary>
    /// Determines if this collection contains the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Contains(T value);

    /// <summary>
    /// Returns the index of the first ocurrence of the given value, or -1 if any.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int IndexOf(T value);

    /// <summary>
    /// Returns the index of the last ocurrence of the given value, or -1 if any.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int LastIndexOf(T value);

    /// <summary>
    /// Returns the indexes of all the ocurrences of the given value, if any.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    List<int> IndexesOf(T value);

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of the first ocurrence of an element that matches the given predicate,
    /// or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of the last ocurrence of an element that matches the given predicate,
    /// or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns the indexes of all the ocurrences of elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<T> predicate);

    /// <summary>
    /// Tries to find the first ocurrence of an element that matches the given predicate. If so,
    /// returns true and sets the out argument to the found one. Otherwise returns false and the
    /// out argument is undetermined.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Find(Predicate<T> predicate, out T value);

    /// <summary>
    /// Tries to find the last ocurrence of an element that matches the given predicate. If so,
    /// returns true and sets the out argument to the found one. Otherwise returns false and the
    /// out argument is undetermined.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    bool FindLast(Predicate<T> predicate, out T value);

    /// <summary>
    /// Tries to find all the ocurrences of elements that match the given predicate. If so, returns
    /// true and the returned list contains the found values. Otherwise, returns false and the list
    /// is undetermined.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    bool FindAll(Predicate<T> predicate, out List<T> range);

    /// <summary>
    /// Returns an array with the elements of this collection.
    /// </summary>
    /// <returns></returns>
    T[] ToArray();

    /// <summary>
    /// Returns a list with the elements of this collection.
    /// </summary>
    /// <returns></returns>
    List<T> ToList();

    /// <summary>
    /// Returns a list with the requested number of elements, starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<T> ToList(int index, int count);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance with the requested number of elements, starting from the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IInvariantList<T> GetRange(int index, int count);

    /// <summary>
    /// Returns a copy of this instance where the value at the given index has been replaced by
    /// the newly given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IInvariantList<T> Replace(int index, T value);

    /// <summary>
    /// Returns a copy of this instance where the given value has been added to it.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IInvariantList<T> Add(T value);

    /// <summary>
    /// Returns a copy of this instance where the values from the given range have been added
    /// to it.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IInvariantList<T> AddRange(IEnumerable<T> range);

    /// <summary>
    /// Returns a copy of this instance where the given value has been inserted into it at the
    /// given index.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IInvariantList<T> Insert(T value);

    /// <summary>
    /// Returns a copy of this instance where the values from the given range have been inserted
    /// into it starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IInvariantList<T> InsertRange(int index, IEnumerable<T> range);

    /// <summary>
    /// Returns a copy of this instance where the value at the given index has been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveAt(int index);

    /// <summary>
    /// Returns a copy of this instance where the given number of elements, starting from the
    /// given index, have been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveRange(int index, int count);

    /// <summary>
    /// Returns a copy of this instance where the first ocurrence of the given value, if any,
    /// has been removed.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IInvariantList<T> Remove(T value);

    /// <summary>
    /// Returns a copy of this instance where the last ocurrence of the given value, if any,
    /// has been removed.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveLast(T value);

    /// <summary>
    /// Returns a copy of this instance where all the ocurrences of the given value, if any,
    /// have been removed.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveAll(T value);

    /// <summary>
    /// Returns a copy of this instance where the first ocurrence of an element that matches the
    /// given predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<T> Remove(Predicate<T> predicate);

    /// <summary>
    /// Returns a copy of this instance where the last ocurrence of an element that matches the
    /// given predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Returns a copy of this instance where all the ocurrences of elements that matches the
    /// given predicate have been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<T> RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Returns a copy of this instance that has been cleared.
    /// </summary>
    /// <returns></returns>
    IInvariantList<T> Clear();
}
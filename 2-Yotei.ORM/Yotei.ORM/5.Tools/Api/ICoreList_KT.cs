namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements, identified by their respective keys, with
/// customizable behavior.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItem"></typeparam>
[Cloneable]
public partial interface ICoreList<TKey, TItem> : IList<TItem>, IList, ICollection<TItem>, ICollection
{
    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Gets or sets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new TItem this[int index] { get; set; }

    /// <summary>
    /// Determines if this collection contains an element with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Contains(TKey key);

    /// <summary>
    /// Returns the index of the first ocurrence of an element with the given key, or -1 if not
    /// found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int IndexOf(TKey key);

    /// <summary>
    /// Returns the index of the last ocurrence of an element with the given key, or -1 if not
    /// found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int LastIndexOf(TKey key);

    /// <summary>
    /// Returns the indexes of the ocurrences of elements with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    List<int> IndexesOf(TKey key);

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the first ocurrence of an element that matches the given predicate,
    /// or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the last ocurrence of an element that matches the given predicate,
    /// or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the indexes of the elements in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    TItem[] ToArray();

    /// <summary>
    /// Returns a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    List<TItem> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Replaces the element at the given index with the new given one. Returns the number of
    /// changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    int Replace(int index, TItem item);

    /// <summary>
    /// Adds to this collection the given element. Returns the number of changes made.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Add(TItem item);

    /// <summary>
    /// Adds to this collection the elements from the given range. Returns the number of changes
    /// made.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    int AddRange(IEnumerable<TItem> range);

    /// <summary>
    /// Inserts into this collection the given element at the given index. Returns the number of
    /// changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Insert(int index, TItem item);

    /// <summary>
    /// Inserts into this collection the elements from the given range, starting at the given
    /// index. Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    int InsertRange(int index, IEnumerable<TItem> range);

    /// <summary>
    /// Removes from this collection the element at the given index. Returns the number of
    /// changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new int RemoveAt(int index);

    /// <summary>
    /// Removes from this collection the given number of elements, starting from the given index.
    /// Returns the number of changes made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    int RemoveRange(int index, int count);

    /// <summary>
    /// Removes from this collection the first ocurrence of an element with the given key.
    /// Returns the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int Remove(TKey key);

    /// <summary>
    /// Removes from this collection the last ocurrence of an element with the given key.
    /// Returns the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int RemoveLast(TKey key);

    /// <summary>
    /// Removes from this collection all the ocurrences of elements with the given key. Returns
    /// the number of changes made.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int RemoveAll(TKey key);

    /// <summary>
    /// Removes from this collection the first ocurrence of an element that matches the given
    /// predicate. Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int Remove(Predicate<TItem> predicate);

    /// <summary>
    /// Removes from this collection the last ocurrence of an element that matches the given
    /// predicate. Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// Removes from this collection all the ocurrences of elements that match the given
    /// predicate. Returns the number of changes made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// Clears all the elements in this collection. Returns the number of changes made.
    /// </summary>
    /// <returns></returns>
    new int Clear();
}
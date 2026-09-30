namespace Experimental.Collections;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements identified by their respective keys.
/// </summary>
/// <typeparam name="TKey">The type of the keys associated with the elements of this collection.</typeparam>
/// <typeparam name="TItem">The type of the elements of this collection.</typeparam>
public interface ICoreList<TKey, TItem>
    : IList<TItem>, IList, ICollection<TItem>, ICollection
    , ICloneable
{
    /// <summary>
    /// The number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Gets or sets the element at the given index.
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
    /// Returns the index of the first element in this collection with the given key,
    /// or -1 if any.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int IndexOf(TKey key);

    /// <summary>
    /// Returns the index of the last element in this collection with the given key,
    /// or -1 if any.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int LastIndexOf(TKey key);

    /// <summary>
    /// Returns the indexes of all the elements in this collection with the given key.
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
    /// Returns the index of the first element in this collection that matches the given predicate,
    /// or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the last element in this collection that matches the given predicate,
    /// or -1 if any.
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

    /// <summary>
    /// Returns a list with the requested number of elements from this collection, starting at
    /// the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<TItem> ToList(int index, int count);

    /// <summary>
    /// Trims the internal structures of this collection.
    /// </summary>
    void Trim();

    // ----------------------------------------------------

    /// <summary>
    /// Replaces the element at the given index with the new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns>The number of changes made.</returns>
    int Replace(int index, TItem value);

    /// <summary>
    /// Adds to this collection the given element.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>The number of changes made.</returns>
    new int Add(TItem value);

    /// <summary>
    /// Adds to this collection the elements from the given range.
    /// </summary>
    /// <param name="values"></param>
    /// <returns>The number of changes made.</returns>
    int AddRange(IEnumerable<TItem> values);

    /// <summary>
    /// Inserts into this collection the given element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns>The number of changes made.</returns>
    new int Insert(int index, TItem value);

    /// <summary>
    /// Inserts into this collection the elements from the given range, starting at the given
    /// index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="values"></param>
    /// <returns>The number of changes made.</returns>
    int InsertRange(int index, IEnumerable<TItem> values);

    /// <summary>
    /// Removes from this collection the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>The number of changes made.</returns>
    new int RemoveAt(int index);

    /// <summary>
    /// Removes from this collection the requested number of elements, starting at the given
    /// index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns>The number of changes made.</returns>
    int RemoveRange(int index, int count);

    /// <summary>
    /// Removes from this collection the first element with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>The number of changes made.</returns>
    int Remove(TKey key);

    /// <summary>
    /// Removes from this collection the last element with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>The number of changes made.</returns>
    int RemoveLast(TKey key);

    /// <summary>
    /// Removes from this collection all the elements with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>The number of changes made.</returns>
    int RemoveAll(TKey key);

    /// <summary>
    /// Removes from this collection the first element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>The number of changes made.</returns>
    int Remove(Predicate<TItem> predicate);

    /// <summary>
    /// Removes from this collection the last element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>The number of changes made.</returns>
    int RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// Removes from this collection all the elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>The number of changes made.</returns>
    int RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// Clears this collection.
    /// </summary>
    /// <returns>The number of changes made.</returns>
    new int Clear();
}
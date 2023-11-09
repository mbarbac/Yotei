namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents an ordered list-alike collection of elements identified by their respective keys.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItem"></typeparam>
public interface ICoreList<TKey, TItem>
    : IList<TItem>, IList, ICollection<TItem>, ICollection, IEnumerable<TItem>, ICloneable
{
    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns></returns>
    new ICoreList<TKey, TItem> Clone();

    /// <summary>
    /// Gets the number of elements in this instance.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Minimizes the memory consumption of this instance.
    /// </summary>
    void Trim();

    /// <summary>
    /// Gets or sets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new TItem this[int index] { get; set; }

    /// <summary>
    /// Determines if this collection contains any elements with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Contains(TKey key);

    /// <summary>
    /// Returns the index of the first element in this collection with the given key, or -1 if
    /// any can be found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int IndexOf(TKey key);

    /// <summary>
    /// Returns the index of the last element in this collection with the given key, or -1 if
    /// any can be found.
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
    /// Determines if this collection contains any elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the first element in this collection that match the given predicate,
    /// or -1 if any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the last element in this collection that match the given predicate,
    /// or -1 if any can be found.
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
    /// Returns a list with the given number of elements starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<TItem> GetRange(int index, int count);

    // ----------------------------------------------------

    /// <summary>
    /// Sets the element at the given index with the new given one.
    /// <br/> If the given element is strictly the same as the existing one at the given index,
    /// then no replacement is done.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns>The number of elements inserted.</returns>
    int Replace(int index, TItem item);

    /// <summary>
    /// Adds to this collection the given element.
    /// </summary>
    /// <param name="item"></param>
    /// <returns>The number of elements added.</returns>
    new int Add(TItem item);

    /// <summary>
    /// Adds to this collection the elements from the given range.
    /// </summary>
    /// <param name="range"></param>
    /// <returns>The number of elements added.</returns>
    int AddRange(IEnumerable<TItem> range);

    /// <summary>
    /// Inserts into this collection the given element.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns>The number of elements inserted.</returns>
    new int Insert(int index, TItem item);

    /// <summary>
    /// Inserts into this collection the elements from the given range, starting at the given
    /// index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns>The number of elements inserted.</returns>
    int InsertRange(int index, IEnumerable<TItem> range);

    /// <summary>
    /// Removes from this collection the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>The number of elements removed.</returns>
    new int RemoveAt(int index);

    /// <summary>
    /// Removes from this collection the given number of elements, starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns>The number of elements removed.</returns>
    int RemoveRange(int index, int count);

    /// <summary>
    /// Removes from this collection the first element with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>The number of elements removed.</returns>
    int Remove(TKey key);

    /// <summary>
    /// Removes from this collection the last element with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>The number of elements removed.</returns>
    int RemoveLast(TKey key);

    /// <summary>
    /// Removes from this collection all the elements with the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns>The number of elements removed.</returns>
    int RemoveAll(TKey key);

    /// <summary>
    /// Removes from this collection the first ocurrence of an element that matches the given
    /// predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>The number of elements removed.</returns>
    int Remove(Predicate<TItem> predicate);

    /// <summary>
    /// Removes from this collection the last ocurrence of an element that matches the given
    /// predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>The number of elements removed.</returns>
    int RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// Removes from this collection all the ocurrences of elements that match the given
    /// predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>The number of elements removed.</returns>
    int RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// Clears this collection.
    /// </summary>
    /// <returns>The number of elements removed.</returns>
    new int Clear();
}
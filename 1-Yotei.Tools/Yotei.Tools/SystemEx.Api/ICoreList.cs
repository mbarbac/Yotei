namespace Yotei.Tools;

// ========================================================
/// <summary>
/// Represents a list-alike collection of arbitrary elements whose behavior can be customized.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ICoreList<T>
    : IList<T>, IList, ICollection<T>, ICollection, IEnumerable<T>, IEnumerable, ICloneable
{
    /// <summary>
    /// <inheritdoc cref="ICloneable.Clone"/>
    /// </summary>
    /// <returns></returns>
    new ICoreList<T> Clone();

    /// <summary>
    /// Invoked to copy the configuration settings from the given source to this instance. This
    /// is clone contexts by overriding it to copy any arbitrary configuration state that may be
    /// necessary.
    /// </summary>
    /// <param name="source"></param>
    void CopySettings(ICoreList<T> source);

    /// <summary>
    /// The (item, add) delegate invoked to determine if the given item is valid for this instance
    /// or not. Its second argument determines if the item is to be added or inserted, or not. The
    /// default value of this setting just returns the given item.
    /// </summary>
    Func<T, bool, T> Validator { get; set; }

    /// <summary>
    /// The (x, y) delegate invoked to determine if the two given items shall be considered equal
    /// or not. The default value of this setting just invokes the default comparer of the type of
    /// the elements in this collection.
    /// </summary>
    Func<T, T, bool> Comparer { get; set; }

    /// <summary>
    /// Determines the behavior of this collection when adding or inserting duplicate elements.
    /// </summary>
    CoreListBehavior Behavior { get; set; }

    /// <summary>
    /// Determines if this collection shall be a flat one or not. Flat collections intercepts any
    /// attempts of adding or inserting elements that are themselves an enumeration of the type of
    /// its elements, adding or inserting their own elements instead. Its default value is false.
    /// </summary>
    bool Flatten { get; set; }

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Gets or sets the number of elements that the internal data structures can hold without
    /// resizing.
    /// </summary>
    int Capacity { get; set; }

    /// <summary>
    /// Minimizes the footprint of this collection.
    /// </summary>
    void Trim();

    /// <summary>
    /// Gets or sets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new T this[int index] { get; set; }

    /// <summary>
    /// Determines if this collection contains the given element, or an equivalent one, or not.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new bool Contains(T item);

    /// <summary>
    /// Returns the index of the first ocurrence in this collection of the given element, or an
    /// equivalent one, or -1 if any is found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int IndexOf(T item);

    /// <summary>
    /// Returns the index of the last ocurrence in this collection of the given element, or an
    /// equivalent one, or -1 if any is found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int LastIndexOf(T item);

    /// <summary>
    /// Returns the indexes of the ocurrences in this collection of the given element, or any
    /// equivalent ones.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    List<int> IndexesOf(T item);

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of the first element that matches the given predicate, or or -1 if any
    /// is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns the index of the last element that matches the given predicate, or or -1 if any
    /// is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<T> predicate);

    /// <summary>
    /// Returns the indexes of all the elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<T> predicate);

    /// <summary>
    /// Returns a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    List<T> ToList();

    /// <summary>
    /// Returns an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    T[] ToArray();

    // ----------------------------------------------------

    /// <summary>
    /// Gets a list with the given number of elements from this collection, starting at the given
    /// index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<T> GetRange(int index, int count);

    /// <summary>
    /// Sets the element stored in this collection at the given index. Returns the number of
    /// replaced elements.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    int ReplaceItem(int index, T item);

    /// <summary>
    /// Adds the given element to this collection. Returns the number of added elements.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Add(T item);

    /// <summary>
    /// Adds the elements from the given range to this collection. Returns the number of added
    /// elements.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    int AddRange(IEnumerable<T> range);

    /// <summary>
    /// Inserts the given element into this collection, at the given index. Returns the number of
    /// inserted elements.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Insert(int index, T item);

    /// <summary>
    /// Inserts the elements from the given range into this collection, at the given index.
    /// Returns the number of inserted elements.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    int InsertRange(int index, IEnumerable<T> range);

    /// <summary>
    /// Removes from this collection the element at the given index. Returns the number of removed
    /// elements.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new int RemoveAt(int index);

    /// <summary>
    /// Removes from this collection the first ocurrence of the given element, or an equivalent
    /// one. Returns the number of removed elements.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Remove(T item);

    /// <summary>
    /// Removes from this collection the last ocurrence of the given element, or an equivalent
    /// one. Returns the number of removed elements.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int RemoveLast(T item);

    /// <summary>
    /// Removes from this collection all the ocurrences of the given element, or equivalent ones.
    /// Returns the number of removed elements.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int RemoveAll(T item);

    /// <summary>
    /// Removes from this collection the given number of elements, starting at the given index.
    /// Returns the number of removed elements.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    int RemoveRange(int index, int count);

    /// <summary>
    /// Removes from this collection the first element in this collection that matches the given
    /// predicate. Returns the number of removed elements.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int Remove(Predicate<T> predicate);

    /// <summary>
    /// Removes from this collection the last element in this collection that matches the given
    /// predicate. Returns the number of removed elements.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int RemoveLast(Predicate<T> predicate);

    /// <summary>
    /// Removes from this collection the elements in this collection that match the given
    /// predicate. Returns the number of removed elements.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int RemoveAll(Predicate<T> predicate);

    /// <summary>
    /// Removes from this collection all the elements it contains. Returns the number of removed
    /// elements.
    /// </summary>
    /// <returns></returns>
    new int Clear();
}
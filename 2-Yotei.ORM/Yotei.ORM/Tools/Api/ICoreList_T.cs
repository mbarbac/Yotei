namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements that provides customizable item validation and
/// duplicates acceptance or denial capabilities.
/// </summary>
/// <typeparam name="TItem"></typeparam>
public interface ICoreList<TItem>
    : IList<TItem>, IList, ICollection<TItem>, ICollection, IEnumerable<TItem>
{
    /// <summary>
    /// Determines if the given element is valid for this collection, or throws an appropriate
    /// exception otherwise.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    TItem Validate(TItem item);

    /// <summary>
    /// Determines if the given target element, being a duplicate of the given source one, can be
    /// added or inserted. Returns '<c>true</c>' if it can be added or inserted, '<c>false</c>' if
    /// it shall just be ignored, or throws an appropriate exception if this instance does not
    /// accept that duplicate.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    bool CanDuplicate(TItem source, TItem target);

    /// <summary>
    /// Determines if the given target element can be considered equal to the given source one,
    /// or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    bool Compare(TItem source, TItem target);

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    new int Count { get; }

    /// <summary>
    /// Minimizes the memory consumption of this instance.
    /// </summary>
    void Trim();

    /// <summary>
    /// Gets or sets the element at the given index. The setter may throw an exception if the
    /// new item is a duplicate of an existing one.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new TItem this[int index] { get; set; }

    /// <summary>
    /// Determines if this instance contains an element that matches the given one.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new bool Contains(TItem item);

    /// <summary>
    /// Returns the index of the first element in this instance that matches the given one.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int IndexOf(TItem item);

    /// <summary>
    /// Returns the index of the last element in this instance that matches the given one.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int LastIndexOf(TItem item);

    /// <summary>
    /// Returns the index of all the elements in this instance that match the given one, or -1
    /// if any is found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    List<int> IndexesOf(TItem item);

    /// <summary>
    /// Determines if this instance contains an element that matches the the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the first element in this instance that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the last element in this instance that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of all the element in this instance that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns an array with the elements in this instace.
    /// </summary>
    /// <returns></returns>
    TItem[] ToArray();

    /// <summary>
    /// Returns a list with the elements in this instace.
    /// </summary>
    /// <returns></returns>
    List<TItem> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a list with the given number of elements, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    List<TItem> GetRange(int index, int count);

    /// <summary>
    /// Replaces the element at the given index with the new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    int Replace(int index, TItem item);

    /// <summary>
    /// Adds the given element to this instance.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Add(TItem item);

    /// <summary>
    /// Adds the elements of the given range to this instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    int AddRange(IEnumerable<TItem> range);

    /// <summary>
    /// Inserts the given element into this instance at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Insert(int index, TItem item);

    /// <summary>
    /// Inserts the elements of the given range into this instance, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    int InsertRange(int index, IEnumerable<TItem> range);

    /// <summary>
    /// Removes from this instance the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    new int RemoveAt(int index);

    /// <summary>
    /// Removes from this instance the given number of elements, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    int RemoveRange(int index, int count);

    /// <summary>
    /// Removes from this instance the first element that matches the given one.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    new int Remove(TItem item);

    /// <summary>
    /// Removes from this instance the last element that matches the given one.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int RemoveLast(TItem item);

    /// <summary>
    /// Removes from this instance all the elements that match the given one.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int RemoveAll(TItem item);

    /// <summary>
    /// Removes from this instance the first element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int Remove(Predicate<TItem> predicate);

    /// <summary>
    /// Removes from this instance the last element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// Removes from this instance all the elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// Clears this instance.
    /// </summary>
    /// <returns></returns>
    new int Clear();
}
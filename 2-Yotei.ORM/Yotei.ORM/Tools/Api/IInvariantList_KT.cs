namespace Yotei.ORM.Tools;

// ========================================================
/// <summary>
/// Represents an immutable list-alike collection of elements identified by their respective keys
/// that provides customizable item validation and duplicates acceptance or denial capabilities.
/// </summary>
/// <typeparam name="TKey"></typeparam>
/// <typeparam name="TItem"></typeparam>
[Cloneable]
public partial interface IInvariantList<TKey, TItem> : IEnumerable<TItem>
{
    /// <summary>
    /// Determines if the given element is valid for this collection, or throws an appropriate
    /// exception otherwise.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    TItem ValidateItem(TItem item);

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
    /// Returns the key associated with the given element, or throws an exception if that key
    /// cannot be obtained.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    TKey GetKey(TItem key);

    /// <summary>
    /// Determines if the given key is valid for this collection, or throws an appropriate
    /// exception otherwise.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    TKey ValidateKey(TKey key);

    /// <summary>
    /// Determines if the given target key can be considered equal to the given source one,
    /// or not.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    bool Compare(TKey source, TKey target);

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Minimizes the memory consumption of this instance.
    /// </summary>
    void Trim();

    /// <summary>
    /// Gets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    TItem this[int index] { get; }

    /// <summary>
    /// Determines if this instance contains an element whose key matches the given one.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Contains(TKey key);

    /// <summary>
    /// Returns the index of the first element in this instance whose key matches the given one,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int IndexOf(TKey key);

    /// <summary>
    /// Returns the index of the last element in this instance whose key matches the given one,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int LastIndexOf(TKey key);

    /// <summary>
    /// Returns the indexes of all elements in this instance whose keys matches the given one.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    List<int> IndexesOf(TKey key);

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
    /// Returns a new instance with the given number of elements, starting at the given index.
    /// If no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IInvariantList<TKey, TItem>  GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the element at the given index has been replaced by the new
    /// given one. If no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<TKey, TItem>  Replace(int index, TItem item);

    /// <summary>
    /// Returns a new instance where the given element has been added to the original one. If no
    /// changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<TKey, TItem>  Add(TItem item);

    /// <summary>
    /// Returns a new instance where the elements of the given range have been added to the
    /// original one. If no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IInvariantList<TKey, TItem>  AddRange(IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance where the given element has been inserted at the given index. If
    /// no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IInvariantList<TKey, TItem>  Insert(int index, TItem item);

    /// <summary>
    /// Returns a new instance where the given element has been inserted at the given index. If
    /// no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IInvariantList<TKey, TItem>  InsertRange(int index, IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance where the element at the given index has been removed. If no
    /// changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IInvariantList<TKey, TItem>  RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of elements, starting at the given index,
    /// have been removed. If no changes are detected, then the original instance is returned
    /// instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IInvariantList<TKey, TItem>  RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the first element whose key that matches the given one is
    /// removed. If no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IInvariantList<TKey, TItem>  Remove(TKey key);

    /// <summary>
    /// Returns a new instance where the last element whose key that matches the given one is
    /// removed. If no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IInvariantList<TKey, TItem>  RemoveLast(TKey key);

    /// <summary>
    /// Returns a new instance where all the elements whose key that matches the given one are
    /// removed. If no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IInvariantList<TKey, TItem>  RemoveAll(TKey key);

    /// <summary>
    /// Returns a new instance where the first element that matches the given predicate has been
    /// removed. If no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<TKey, TItem>  Remove(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where the last element that matches the given predicate has been
    /// removed. If no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<TKey, TItem>  RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all the elements that match the given predicate have been
    /// removed. If no changes are detected, then the original instance is returned instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IInvariantList<TKey, TItem>  RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all the elements have been removed. If no changes are
    /// detected, then the original instance is returned instead.
    /// </summary>
    /// <returns></returns>
    IInvariantList<TKey, TItem>  Clear();
}
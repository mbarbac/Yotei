using THost = Experimental.Yotei.ITemplateList;
using TItem = Experimental.Yotei.ITemplateElement;
using TKey = Experimental.Yotei.ITemplateKey;

namespace Experimental.Yotei;

// ========================================================
/// <summary>
/// An immutable object that represents ...
/// </summary>
[Cloneable]
public partial interface ITemplateList : IEnumerable<TItem>, IEquatable<THost>
{
    /// <summary>
    /// Determines if the keys of the elements in this instance are case sensitive, or not.
    /// </summary>
    bool CaseSensitive { get; }

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Minimizes the memory consumption of this instance.
    /// </summary>
    void Trim();

    /// <summary>
    /// Gets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    TItem this[int index] { get; }

    /// <summary>
    /// Determines if this collection contains any elements that match the given key.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Contains(TKey key);

    /// <summary>
    /// Returns the index of the first element in this collection with a key that matches the
    /// given one, or -1 if any can be found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int IndexOf(TKey key);

    /// <summary>
    /// Returns the index of the last element in this collection with a key that matches the
    /// given one, or -1 if any can be found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int LastIndexOf(TKey key);

    /// <summary>
    /// Returns the indexes of all the elements in this collection keys that match the given one.
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

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of elements starting from the given index.
    /// <br/> If the index is zero and the requested number of elements is the same as the size
    /// of the original collection, then it is returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the element at the given index has been replaced by the new
    /// given one.
    /// <br/> If the given element can be considered as equal to the existing one at that index,
    /// then the original collection is returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    THost Replace(int index, TItem item);

    /// <summary>
    /// Returns a new instance where the given element has been added to the original collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    THost Add(TItem item);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been added to the
    /// original collection.
    /// <br/> If the range was an empty one, then the original collection is returned instead.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    THost AddRange(IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance where the given element has been inserted into the original
    /// collection, at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    THost Insert(int index, TItem item);

    /// <summary>
    /// Returns a new instance where the given element has been inserted into the original
    /// collection, at the given index.
    /// <br/> If the range was an empty one, then the original collection is returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    THost InsertRange(int index, IEnumerable<TItem> range);

    /// <summary>
    /// Returns a new instance where the element at the given index has been removed from the
    /// original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    THost RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of elements, starting from the given index,
    /// have been removed from the original collection.
    /// <br/> If the number of elements to remove is zero, then the original collection is
    /// returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the first element that matches the given key has been removed
    /// from the original collection.
    /// <br/> If no matching element was found, the original collection is returned instead.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    THost Remove(TKey key);

    /// <summary>
    /// Returns a new instance where the last element that matches the given key has been removed
    /// from the original collection.
    /// <br/> If no matching element was found, the original collection is returned instead.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    THost RemoveLast(TKey key);

    /// <summary>
    /// Returns a new instance where all the elements that match the given key has been removed
    /// from the original collection.
    /// <br/> If no matching element was found, the original collection is returned instead.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    THost RemoveAll(TKey key);

    /// <summary>
    /// Returns a new instance where the first ocurrence of an element that matches the given
    /// predicate has been removed from the original collection.
    /// <br/> If no matching element was found, the original collection is returned instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost Remove(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where the last ocurrence of an element that matches the given
    /// predicate has been removed from the original collection.
    /// <br/> If no matching element was found, the original collection is returned instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all the ocurrences of elements that match the given predicate
    /// have been removed from the original collection.
    /// <br/> If no matching element was found, the original collection is returned instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all the original elements have been removed.
    /// <br/> If the original collection was an empty one, it is returned instead.
    /// </summary>
    /// <returns></returns>
    THost Clear();
}
using THost = Experimental.Yotei.ITemplateList;
using TItem = Experimental.Yotei.ITemplateElement;
using TKey = Experimental.Yotei.ITemplateKey;

namespace Experimental.Yotei;

// ========================================================
/// <summary>
/// An immutable object that ...
/// </summary>
[Cloneable]
public partial interface ITemplateList : IEnumerable<TItem>
{
    /// <summary>
    /// Gets the number of elements in this instance.
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

    // ----------------------------------------------------

    /// <summary>
    /// Obtains an instance that contains the given number of elements starting from the given
    /// index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost GetRange(int index, int count);

    /// <summary>
    /// Obtains an instance where the element at the given index has been replaced with the new
    /// given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    THost Replace(int index, TItem item);

    /// <summary>
    /// Obtains an instance where the given element has been added to the original one.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    THost Add(TItem item);

    /// <summary>
    /// Obtains an instance where the elements from the given range have been added to the
    /// original one.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    THost AddRange(IEnumerable<TItem> range);

    /// <summary>
    /// Obtains an instance where the given element has been inserted into the original one, at
    /// the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    THost Insert(int index, TItem item);

    /// <summary>
    /// Obtains an instance where the elements from the given range have been inserted into the
    /// original one, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    THost InsertRange(int index, IEnumerable<TItem> range);

    /// <summary>
    /// Obtains an instance where the element at the given index has been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    THost RemoveAt(int index);

    /// <summary>
    /// Obtains an instance where the given number of elements have been removed from the
    /// original one, starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost RemoveRange(int index, int count);

    /// <summary>
    /// Obtains an instance where the first element with the given key has been removed.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    THost Remove(TKey key);

    /// <summary>
    /// Obtains an instance where the last element with the given key has been removed.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    THost RemoveLast(TKey key);

    /// <summary>
    /// Obtains an instance where all the elements with the given key have been removed.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    THost RemoveAll(TKey key);

    /// <summary>
    /// Obtains an instance where the first ocurrence of an element that matches the given
    /// predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost Remove(Predicate<TItem> predicate);

    /// <summary>
    /// Obtains an instance where the last ocurrence of an element that matches the given
    /// predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// Obtains an instance where all the ocurrences of elements that match the given predicate
    /// have been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// Obtains an instance where all the original elements have been removed.
    /// </summary>
    /// <returns></returns>
    THost Clear();
}

// ========================================================
public interface ITemplateKey { }
public interface ITemplateElement { TKey Key { get; } }
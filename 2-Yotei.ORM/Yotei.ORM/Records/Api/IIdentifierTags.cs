using THost = Yotei.ORM.IIdentifierTags;
using TItem = string;

namespace Yotei.ORM;

// ========================================================
/// <summary>
/// An immutable object that represents the ordered collection of metadata tags that describes
/// the maximal known structure of the identifiers in an underlying database.
/// <br/> Tags cannot be null or empty, and cannot contain dots.
/// <br/> Duplicate tags are not allowed.
/// </summary>
[Cloneable]
public partial interface IIdentifierTags : IEnumerable<TItem>, IEquatable<THost>
{
    /// <summary>
    /// Determines if the tags in this instance are case sensitive, or not.
    /// </summary>
    [WithGenerator] bool CaseSensitiveTags { get; }

    /// <summary>
    /// Minimizes the memory consumption of this instance.
    /// </summary>
    void Trim();

    /// <summary>
    /// Gets the number of elements in this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the tag at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    TItem this[int index] { get; }

    /// <summary>
    /// Determines if this instance has a tag that matches the given one.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    bool Contains(TItem tag);

    /// <summary>
    /// Returns the index of the tag in this instance that matches the given one, or -1 if it is
    /// not found.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    int IndexOf(TItem tag);

    /// <summary>
    /// Determines if this instance has at least one tag that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the first tag in this instance that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of the last tag in this instance that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns the index of all the elements in this instance that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<TItem> predicate);

    /// <summary>
    /// Returns an array with the elements in this instance.
    /// </summary>
    /// <returns></returns>
    TItem[] ToArray();

    /// <summary>
    /// Returns a list with the elements in this instance.
    /// </summary>
    /// <returns></returns>
    List<TItem> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of tags, starting at the given index. If no
    /// changes are detected, returns the original instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the tag at the given index has been replaced by the new ones
    /// obtained from the dotted value. If no changes are detected, returns the original instance
    /// instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="dotted"></param>
    /// <returns></returns>
    THost Replace(int index, TItem dotted);

    /// <summary>
    /// Returns a new instance where the elements obtained from the given dotted value have been
    /// added to the original collection.
    /// If no changes are detected, returns the original instance instead.
    /// </summary>
    /// <param name="dotted"></param>
    /// <returns></returns>
    THost Add(TItem dotted);

    /// <summary>
    /// Returns a new instance where the elements obtained from the given range of dotted values
    /// have been added to the original collection.
    /// If no changes are detected, returns the original instance instead.
    /// </summary>
    /// <param name="dottedrange"></param>
    /// <returns></returns>
    THost AddRange(IEnumerable<TItem> dottedrange);

    /// <summary>
    /// Returns a new instance where the elements obtained from the given dotted value have been
    /// inserted at the given index into the original collection. If no changes are detected,
    /// returns the original instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="dotted"></param>
    /// <returns></returns>
    THost Insert(int index, TItem dotted);

    /// <summary>
    /// Returns a new instance where the elements obtained from the given range of dotted valued
    /// have been inserted into this collection, starting at the given index. If no changes are
    /// detected, returns the original instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="dottedrange"></param>
    /// <returns></returns>
    THost InsertRange(int index, IEnumerable<TItem> dottedrange);

    /// <summary>
    /// Returns a new instance where the element at the given index has been removed. If no
    /// changes are detected, returns the original instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    THost RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of elements, starting at the given index,
    /// have been removed. If no changes are detected, returns the original instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the tag that matches the given one has been removed. If no
    /// changes are detected, returns the original instance instead.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    THost Remove(TItem tag);

    /// <summary>
    /// Returns a new instance where the first tag that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost Remove(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where the last tag that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveLast(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all the tags that match the given predicate have been removed.
    /// If no changes are detected, returns the original instance instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveAll(Predicate<TItem> predicate);

    /// <summary>
    /// Returns a new instance where all the tags have been removed. If no changes are detected,
    /// returns the original instance instead.
    /// </summary>
    /// <returns></returns>
    THost Clear();
}
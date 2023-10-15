using IHost = Yotei.ORM.Records.IIdentifierTags;
using IItem = string;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// The ordered collection of metadata tags that describes the maximal structure of the database
/// identifiers of an underlying engine.
/// <br/> Duplicate elements are not allowed.
/// </summary>
public partial interface IIdentifierTags : IEnumerable<IItem>
{
    /// <summary>
    /// Determines if the metadata tags are case sensitive or not.
    /// </summary>
    [WithGenerator] bool CaseSensitiveTags { get; }

    /// <summary>
    /// Gets the number of elements in this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Minimizes the memory footprint of this instance.
    /// </summary>
    void Trim();

    /// <summary>
    /// Gets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IItem this[int index] { get; }

    /// <summary>
    /// Determines if this instance contains the given element, or not.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool Contains(IItem item);

    /// <summary>
    /// Returns the index of the given element in this collection, or -1 if it cannot be found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int IndexOf(IItem item);

    /// <summary>
    /// Determines if this instance contains an element that matches the given predicate, or not.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<IItem> predicate);

    /// <summary>
    /// Returns the index of the first ocurrence of an element in this collection that matches
    /// the given predicate, or -1 if any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<IItem> predicate);

    /// <summary>
    /// Returns the index of the last ocurrence of an element in this collection that matches
    /// the given predicate, or -1 if any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<IItem> predicate);

    /// <summary>
    /// Returns a list containing the indexes of all the elements in this collection that match
    /// the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<IItem> predicate);

    /// <summary>
    /// Returns an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    IItem[] ToArray();

    /// <summary>
    /// Returns a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    List<IItem> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance that contains the given number of elements from the original one,
    /// starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IHost GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the original element at the given index has been replaced by
    /// the given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost Replace(int index, IItem item);

    /// <summary>
    /// Returns a new instance where the given element has been added to the original collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost Add(IItem item);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been added into the
    /// original collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost AddRange(IEnumerable<IItem> range);

    /// <summary>
    /// Returns a new instance where the given element has been insert at the given index into
    /// the original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost Insert(int index, IItem item);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been insert, starting
    /// at the given index, into the original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost InsertRange(int index, IEnumerable<IItem> range);

    /// <summary>
    /// Returns a new instance where the element at the given index has been removed from the
    /// original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IHost RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given element has been removed from the original
    /// collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost Remove(IItem item);

    /// <summary>
    /// Returns a new instance where the given number of elements, starting from the given index,
    /// have been removed from the original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IHost RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the first ocurrence of an element that matches the given
    /// predicate has been removed from the original collection.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IHost Remove(Predicate<IItem> predicate);

    /// <summary>
    /// Returns a new instance where the last ocurrence of an element that matches the given
    /// predicate has been removed from the original collection.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IHost RemoveLast(Predicate<IItem> predicate);

    /// <summary>
    /// Returns a new instance where all the ocurrences of elements that match the given predicate
    /// have been removed from the original collection.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IHost RemoveAll(Predicate<IItem> predicate);

    /// <summary>
    /// Returns a new instance where all the elements have been removed.
    /// </summary>
    /// <returns></returns>
    IHost Clear();
}
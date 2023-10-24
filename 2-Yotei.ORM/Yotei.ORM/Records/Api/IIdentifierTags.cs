using IHost = Yotei.ORM.Records.IIdentifierTags;
using IItem = string;
using IKey = string;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the immutable ordered collection of not-duplicated metadata tags that describes
/// the maximal structure of the identifiers associated with an underlying engine.
/// </summary>
[Cloneable]
public partial interface IIdentifierTags : IEnumerable<IItem>
{
    /// <summary>
    /// Determines if the metadata tags in this collection are case sensitive, or not.
    /// </summary>
    [WithGenerator] bool CaseSensitiveTags { get; }

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
    IItem this[int index] { get; }

    /// <summary>
    /// Determines if this collection contains the given element, or not.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Contains(IKey key);

    /// <summary>
    /// Returns the index of the the given element, or -1 if any can be found.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    int IndexOf(IKey key);

    /// <summary>
    /// Determines if this collection contains any elements that match the given predicate,
    /// or not.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<IItem> predicate);

    /// <summary>
    /// Returns the index of the first element in this collection that match the given predicate,
    /// or -1 if any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<IItem> predicate);

    /// <summary>
    /// Returns the index of the last element in this collection that match the given predicate,
    /// or -1 if any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<IItem> predicate);

    /// <summary>
    /// Returns the indexes of the elements in this collection that match the given predicate.
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
    /// Obtains a new instance that contains the given number of elements starting from the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IHost GetRange(int index, int count);

    /// <summary>
    /// Obtains a new instance where the element at the given index has been replaced with the
    /// new given one.
    /// <br/> This method accepts a multi-part input element, which is splitted into its dot
    /// separated parts.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost Replace(int index, IItem item);

    /// <summary>
    /// Obtains a new instance where the given element has been added to the original one.
    /// <br/> This method accepts a multi-part input element, which is splitted into its dot
    /// separated parts.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost Add(IItem item);

    /// <summary>
    /// Obtains a new instance where the elements from the given range have been added to the
    /// original one.
    /// <br/> This method accepts multi-part input elements, which are splitted into their dot
    /// separated parts.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost AddRange(IEnumerable<IItem> range);

    /// <summary>
    /// Obtains a new instance where the given element has been inserted into the original one,
    /// at the given index.
    /// <br/> This method accepts a multi-part input element, which is splitted into its dot
    /// separated parts.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IHost Insert(int index, IItem item);

    /// <summary>
    /// Obtains a new instance where the elements from the given range have been inserted into
    /// the original one, starting at the given index.
    /// <br/> This method accepts multi-part input elements, which are splitted into their dot
    /// separated parts.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IHost InsertRange(int index, IEnumerable<IItem> range);

    /// <summary>
    /// Obtains a new instance where the element at the given index has been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IHost RemoveAt(int index);

    /// <summary>
    /// Obtains a new instance where the given number of elements have been inserted into the
    /// original one, starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IHost RemoveRange(int index, int count);

    /// <summary>
    /// Obtains a new instance where the given element has been removed.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IHost Remove(IKey key);

    /// <summary>
    /// Obtains a new instance where the first ocurrence of an element that matches the given
    /// predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IHost Remove(Predicate<IItem> predicate);

    /// <summary>
    /// Obtains a new instance where the last ocurrence of an element that matches the given
    /// predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IHost RemoveLast(Predicate<IItem> predicate);

    /// <summary>
    /// Obtains a new instance where all the ocurrences of elements that match the given
    /// predicate have been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IHost RemoveAll(Predicate<IItem> predicate);

    /// <summary>
    /// Obtains a new instance where all the original elements have been removed.
    /// </summary>
    /// <returns></returns>
    IHost Clear();
}
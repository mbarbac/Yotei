using THost = Yotei.ORM.Records.IIdentifierTags;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// An immutable object that contains the ordered collection of metadata tags that describes
/// the maximal structure of the identifiers allowed by an underlying database.
/// </summary>
[Cloneable]
public partial interface IIdentifierTags : IEnumerable<string>, IEquatable<THost>
{
    /// <summary>
    /// Determines if the keys of the elements in this instance are case sensitive, or not.
    /// </summary>
    [WithGenerator] bool CaseSensitiveTags { get; }

    /// <summary>
    /// Gets the number of tags in this collection.
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
    string this[int index] { get; }

    /// <summary>
    /// Determines if this collection contains the given tag.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    bool Contains(string value);

    /// <summary>
    /// Returns the index of the given tag, or -1 if any can be found.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    int IndexOf(string value);

    /// <summary>
    /// Determines if this collection contains any tags that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<string> predicate);

    /// <summary>
    /// Returns the index of the first tag in this collection that match the given predicate,
    /// or -1 if any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<string> predicate);

    /// <summary>
    /// Returns the index of the last tag in this collection that match the given predicate,
    /// or -1 if any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<string> predicate);

    /// <summary>
    /// Returns the indexes of the tags in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<string> predicate);

    /// <summary>
    /// Returns an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    string[] ToArray();

    /// <summary>
    /// Returns a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    List<string> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of tags starting from the given index.
    /// <br/> If the index is zero and the requested number of elements is the same as the size
    /// of the original collection, then it is returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the tag at the given index has been replaced by the ones
    /// obtained from the given value.
    /// <br/> If the given value can be considered as equal to the existing tag at that index,
    /// then the original collection is returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    THost Replace(int index, string value);

    /// <summary>
    /// Returns a new instance where the tags obtained from the given value have been added to
    /// the original collection.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    THost Add(string value);

    /// <summary>
    /// Returns a new instance where the tags obtained from the given range of values have been
    /// added to the original collection.
    /// <br/> If the range was an empty one, then the original collection is returned instead.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    THost AddRange(IEnumerable<string> range);

    /// <summary>
    /// Returns a new instance where the tags obtained from the given value have been inserted
    /// into the original collection, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    THost Insert(int index, string value);

    /// <summary>
    /// Returns a new instance where the tags obtained from the given value have been inserted
    /// into the original collection, at the given index.
    /// <br/> If the range was an empty one, then the original collection is returned instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    THost InsertRange(int index, IEnumerable<string> range);

    /// <summary>
    /// Returns a new instance where the tag at the given index has been removed from the
    /// original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    THost RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of tags, starting from the given index,
    /// have been removed from the original collection.
    /// <br/> If the number of tags to remove is zero, then the original collection is returned
    /// instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the given tag has been removed from the original collection.
    /// <br/> If no matching tag was found, the original collection is returned instead.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    THost Remove(string tag);

    /// <summary>
    /// Returns a new instance where the first ocurrence of a tag that matches the given
    /// predicate has been removed from the original collection.
    /// <br/> If no matching tag was found, the original collection is returned instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost Remove(Predicate<string> predicate);

    /// <summary>
    /// Returns a new instance where the last ocurrence of a tag that matches the given
    /// predicate has been removed from the original collection.
    /// <br/> If no matching tag was found, the original collection is returned instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveLast(Predicate<string> predicate);

    /// <summary>
    /// Returns a new instance where all the ocurrences of tags that match the given predicate
    /// have been removed from the original collection.
    /// <br/> If no matching tag was found, the original collection is returned instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveAll(Predicate<string> predicate);

    /// <summary>
    /// Returns a new instance where all the original tags have been removed.
    /// <br/> If the original collection was an empty one, it is returned instead.
    /// </summary>
    /// <returns></returns>
    THost Clear();
}
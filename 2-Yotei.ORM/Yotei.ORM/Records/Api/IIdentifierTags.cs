using THost = Yotei.ORM.Records.IIdentifierTags;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// An immutable object that contains the ordered collection of metadata tags that describes
/// the maximal structure of the identifiers allowed by an underlying database.
/// </summary>
[Cloneable]
public partial interface IIdentifierTags : IEnumerable<string>
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
    /// Gets the tag stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    string this[int index] { get; }

    /// <summary>
    /// Determines if this collection contains the given tag.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    bool Contains(string tag);

    /// <summary>
    /// Returns the index of the the given tag in this collection, or -1 if it cannot be found.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    int IndexOf(string tag);

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
    /// Returns the indexes of the elements in this collection that match the given predicate.
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
    /// Obtains an instance that contains the given number of tags starting from the given
    /// index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost GetRange(int index, int count);

    /// <summary>
    /// Obtains an instance where the tag at the given index has been replaced with the new
    /// ones obtained from the given value.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    THost Replace(int index, string value);

    /// <summary>
    /// Obtains an instance where the tags obtained from the given value have been added to the
    /// original one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    THost Add(string value);

    /// <summary>
    /// Obtains an instance where the tags obtained from the given range of values have been
    /// added to the original one.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    THost AddRange(IEnumerable<string> range);

    /// <summary>
    /// Obtains an instance where the tags obtained from the given value have been inserted into
    /// the original one, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    THost Insert(int index, string value);

    /// <summary>
    /// Obtains an instance where the tags obtained from the given range of values have been
    /// inserted into the original one, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    THost InsertRange(int index, IEnumerable<string> range);

    /// <summary>
    /// Obtains an instance where the tag at the given index has been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    THost RemoveAt(int index);

    /// <summary>
    /// Obtains an instance where the given number of tags have been removed from the original
    /// one, starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost RemoveRange(int index, int count);

    /// <summary>
    /// Obtains an instance where the given tag has been removed.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    THost Remove(string tag);

    /// <summary>
    /// Obtains an instance where the first ocurrence of a tag that matches the given predicate
    /// has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost Remove(Predicate<string> predicate);

    /// <summary>
    /// Obtains an instance where the last ocurrence of a tag that matches the given predicate
    /// has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveLast(Predicate<string> predicate);

    /// <summary>
    /// Obtains an instance where all the ocurrences of tags that match the given predicate
    /// have been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveAll(Predicate<string> predicate);

    /// <summary>
    /// Obtains an instance where all the original tags have been removed.
    /// </summary>
    /// <returns></returns>
    THost Clear();
}
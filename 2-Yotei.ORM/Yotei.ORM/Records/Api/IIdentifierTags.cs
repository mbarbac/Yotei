using THost = Yotei.ORM.Records.IIdentifierTags;

namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// An immutable object that contains the ordered collection of metadata tags that describes the
/// maximal known structure of the identifiers in the underlying database.
/// <br/> Duplicate tags are not allowed.
/// <br/> Identifier tags cannot contain embedded dots.
/// </summary>
public partial interface IIdentifierTags : IEnumerable<string>
{
    /// <summary>
    /// Whether the tags in this instance are case sensitive, or not.
    /// </summary>
    [WithGenerator] bool CaseSensitiveTags { get; }

    /// <summary>
    /// Gets the number of tags in this collection.
    /// </summary>
    int Count { get; }

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
    /// Returns the index of the given tag in this collection, or -1 if it cannot be found.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    int IndexOf(string tag);

    /// <summary>
    /// Determines if this collection contains a tag that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<string> predicate);

    /// <summary>
    /// Returns the index of the first tag that matches the given predicate, or -1 if no one
    /// can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<string> predicate);

    /// <summary>
    /// Returns the index of the last tag that matches the given predicate, or -1 if no one
    /// can be found.
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
    /// An array with the tags in this instance.
    /// </summary>
    /// <returns></returns>
    string[] ToArray();

    /// <summary>
    /// A list with the tags in this instance.
    /// </summary>
    /// <returns></returns>
    List<string> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of tags from the original one, starting
    /// at the given index. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the original tag at the given index has been replaced by 
    /// the new ones obtained from the given dotted value. If no changes were detected, returns
    /// the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="dotted"></param>
    /// <returns></returns>
    THost Replace(int index, string dotted);

    /// <summary>
    /// Returns a new instance where the tags obtained from the given dotted value have been added
    /// to the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="dotted"></param>
    /// <returns></returns>
    THost Add(string dotted);

    /// <summary>
    /// Returns a new instance where the tags obtained from the given range of dotted values have
    /// been added to the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    THost AddRange(IEnumerable<string> range);

    /// <summary>
    /// Returns a new instance where tags obtained from the given range of dotted values have been
    /// inserted, starting at the given index, into the original one. If no changes were detected,
    /// returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="dotted"></param>
    /// <returns></returns>
    THost Insert(int index, string dotted);

    /// <summary>
    /// Returns a new instance where the tags obtained from the given range of dotted values have
    /// been inserted, starting at the given index, into the original one. If no changes were
    /// detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    THost InsertRange(int index, IEnumerable<string> range);

    /// <summary>
    /// Returns a new instance where the original tag at the given index has been removed from
    /// the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    THost RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of tags, starting at the given index,
    /// have been removed from the original one. If no changes were detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    THost RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the given tag has been removed from the original one. If no
    /// changes were detected, returns the original instance.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    THost Remove(string tag);

    /// <summary>
    /// Returns a new instance where the first tag that matches the given predicate has been
    /// removed from the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost Remove(Predicate<string> predicate);

    /// <summary>
    /// Returns a new instance where the last tag that matches the given predicate has been
    /// removed from the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveLast(Predicate<string> predicate);

    /// <summary>
    /// Returns a new instance where all the tags that match the given predicate have been
    /// removed from the original one. If no changes were detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    THost RemoveAll(Predicate<string> predicate);

    /// <summary>
    /// Returns a new instance where all the original tags have been removed. If no changes
    /// were detected, returns the original instance.
    /// </summary>
    /// <returns></returns>
    THost Clear();
}
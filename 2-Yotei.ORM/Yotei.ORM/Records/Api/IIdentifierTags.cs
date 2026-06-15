namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// Represents the ordered collection of metadata tags that describe the maximal structure of
/// identifiers in the underlying database engine.
/// </summary>
[Cloneable]
public partial interface IIdentifierTags : IEnumerable<IMetadataTag>
{
    /// <summary>
    /// Determines if the names in this collection are case sensitive or not.
    /// </summary>
    bool IgnoreCase { get; }

    /// <summary>
    /// Gets the metadata tag at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IMetadataTag this[int index] { get; }

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Determines if this collection contains a tag that carries the given name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    bool Contains(string name);

    /// <summary>
    /// Determines if this collection contains a tag that carries any of the given names.
    /// </summary>
    /// <param name="names"></param>
    /// <returns></returns>
    bool ContainsAny(IEnumerable<string> names);

    /// <summary>
    /// Gets the index of the tag that carries the given name, or -1 if any.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    int IndexOf(string name);

    /// <summary>
    /// Gets the index of the first tag that carries any the given names, or -1 if any.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    int IndexOfAny(IEnumerable<string> names);

    /// <summary>
    /// Returns the index of the first element in this collection that matches the given predicate,
    /// or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<IMetadataTag> predicate);

    /// <summary>
    /// Returns the index of the last element in this collection that matches the given predicate,
    /// or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<IMetadataTag> predicate);

    /// <summary>
    /// Returns the indexes of the elements in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<IMetadataTag> predicate);

    /// <summary>
    /// Returns an array with the elements of this collection.
    /// </summary>
    /// <returns></returns>
    IMetadataTag[] ToArray();

    /// <summary>
    /// Returns a list with the elements of this collection.
    /// </summary>
    /// <returns></returns>
    List<IMetadataTag> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder ToBuilder();

    /// <summary>
    /// Returns a copy of this instance where the given tag has been added to it.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    IIdentifierTags Add(IMetadataTag tag);

    /// <summary>
    /// Returns a copy of this instance where the tag or tags obtained from the given dot-separated
    /// value have been added to it.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifierTags Add(string value);

    /// <summary>
    /// Returns a copy of this instance where the given tags have been added to it.
    /// </summary>
    /// <param name="tag"></param>
    /// <returns></returns>
    IIdentifierTags AddRange(IEnumerable<IMetadataTag> tag);

    /// <summary>
    /// Returns a copy of this instance where the tags or tags obtained from the given range of
    /// values have been added to it.
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    IIdentifierTags AddRange(IEnumerable<string> values);

    /// <summary>
    /// Returns a copy of this instance where the given tag has been inserted into it at the given
    /// index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    IIdentifierTags Insert(int index, IMetadataTag tag);

    /// <summary>
    /// Returns a copy of this instance where tag or tags obtained from the given dot-separated
    /// value, have been inserted into it, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifierTags Insert(int index, string value);

    /// <summary>
    /// Returns a copy of this instance where the given tags have been inserted into it starting
    /// at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="tag"></param>
    /// <returns></returns>
    IIdentifierTags InsertRange(int index, IEnumerable<IMetadataTag> tag);

    /// <summary>
    /// Returns a copy of this instance where the tag or tags obtained from the given range of
    /// dot-separated values have been inserted into it, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    IIdentifierTags InsertRange(int index, IEnumerable<string> values);

    /// <summary>
    /// Returns a copy of this instance where the tag at the given index has been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IIdentifierTags RemoveAt(int index);

    /// <summary>
    /// Returns a copy of this instance where the given number of tags, starting at the given
    /// index, have been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IIdentifierTags RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new copy of this instance with all its tags removed.
    /// </summary>
    /// <returns></returns>
    IIdentifierTags Clear();
}
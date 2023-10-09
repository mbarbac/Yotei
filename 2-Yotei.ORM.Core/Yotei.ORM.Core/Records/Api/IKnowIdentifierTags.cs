namespace Yotei.ORM.Records;

// ========================================================
/// <summary>
/// The immutable object that contains the collection of ordered metadata tags that describes
/// the maximal structure of the database identifiers of an underlying engine.
/// </summary>
public partial interface IKnownIdentifierTags : IEnumerable<string>
{
    /// <summary>
    /// Determines if the metadata tags in this instance are case sensitive or not.
    /// </summary>
    [WithGenerator] bool CaseSensitiveTags { get; }

    /// <summary>
    /// The number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Minimizes the memory footprint of this instance.
    /// </summary>
    void Trim();

    /// <summary>
    /// Gets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    string this[int index] { get; }

    /// <summary>
    /// Determines if this instance contains the given element, or not.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool Contains(string item);

    /// <summary>
    /// Returns the index of the the given element, or -1 if it cannot be found.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    int IndexOf(string item);

    /// <summary>
    /// Returns a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    List<string> ToList();

    /// <summary>
    /// Returns an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    string[] ToArray();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance that contains the given number of elements from the original
    /// collection, starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IKnownIdentifierTags GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the original element at the given index has been replaced by
    /// the new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IKnownIdentifierTags ReplaceItem(int index, string item);

    /// <summary>
    /// Returns a new instance where the given element has been added to the original collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IKnownIdentifierTags Add(string item);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been added to the
    /// original collection.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IKnownIdentifierTags AddRange(IEnumerable<string> range);

    /// <summary>
    /// Returns a new instance where the given element has been inserted into the original
    /// collection at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IKnownIdentifierTags Insert(int index, string item);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been inserted into
    /// the original collection, starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IKnownIdentifierTags InsertRange(int index, IEnumerable<string> range);

    /// <summary>
    /// Returns a new instance where the original element at the given index has been removed
    /// from the original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IKnownIdentifierTags RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the first ocurrence of the given element has been removed
    /// from the original collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IKnownIdentifierTags Remove(string item);

    /// <summary>
    /// Returns a new instance where the given number of elements, starting from the given index,
    /// have been removed from the original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IKnownIdentifierTags RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where all the original elements have been removed.
    /// </summary>
    /// <returns></returns>
    IKnownIdentifierTags Clear();
}
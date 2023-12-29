namespace Yotei.ORM;

// ========================================================
/// <summary>
/// An immutable object that represents an arbitrary multipart database identifier.
/// <br/> Parts with duplicated values are allowed.
/// </summary>
public interface IIdentifier : IEnumerable<IIdentifierPart>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The value carried by this identifier, or null if it represents an empty or missed one.
    /// If not, its parts are wrapped with the appropriate engine terminators, if used.
    /// </summary>
    string? Value { get; }

    /// <summary>
    /// Determines if this instance matches the given specifications. Matching is performed by
    /// comparing parts from left to right, where any null or empty specification is excluded
    /// from the comparison and considered an implicit match.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    bool Match(string? specs);

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IIdentifierPart this[int index] { get; }

    /// <summary>
    /// Determines if this collection contains an element with the given part value.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    bool Contains(string? part);

    /// <summary>
    /// Returns the index of the first element with the given part value, or -1 if not found.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    int IndexOf(string? part);

    /// <summary>
    /// Returns the index of the last element with the given part value, or -1 if not found.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    int LastIndexOf(string? part);

    /// <summary>
    /// Returns the indexes of the elements with the given part value.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    List<int> IndexesOf(string? part);

    /// <summary>
    /// Determines if this collection contains an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<IIdentifierPart> predicate);

    /// <summary>
    /// Returns the index of the first ocurrence of an element that matches the given predicate,
    /// or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<IIdentifierPart> predicate);

    /// <summary>
    /// Returns the index of the last ocurrence of an element that matches the given predicate,
    /// or -1 if such cannot be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<IIdentifierPart> predicate);

    /// <summary>
    /// Returns the indexes of the elements in this collection that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<IIdentifierPart> predicate);

    /// <summary>
    /// Returns an array with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    IIdentifierPart[] ToArray();

    /// <summary>
    /// Returns a list with the elements in this collection.
    /// </summary>
    /// <returns></returns>
    List<IIdentifierPart> ToList();

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new instance with the given number of elements starting from the given index.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IIdentifier GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the element at the given index has been replaced by the new
    /// given one, if not equal to the existing one. If no changes are detected, returns the
    /// original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IIdentifier Replace(int index, IIdentifierPart item);

    /// <summary>
    /// Returns a new instance where the element at the given index has been replaced by the ones
    /// obtained from the given value, if not equal to the existing one.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifier Replace(int index, string? value);

    /// <summary>
    /// Returns a new instance where the given element has been added to the collection.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IIdentifier Add(IIdentifierPart item);

    /// <summary>
    /// Returns a new instance where the elements obtained from the given value have been added
    /// to the collection. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifier Add(string? value);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been added to the
    /// collection. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifier AddRange(IEnumerable<IIdentifierPart> range);

    /// <summary>
    /// Returns a new instance where the elements obtained from the given range of values have
    /// been added to the collection. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifier AddRange(IEnumerable<string?> range);

    /// <summary>
    /// Returns a new instance where the given element has been inserted into the collection at
    /// the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IIdentifier Insert(int index, IIdentifierPart item);

    /// <summary>
    /// Returns a new instance where the elements obtained from the given value have been inserted
    /// into the collection at the given index. If no changes are detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IIdentifier Insert(int index, string? value);

    /// <summary>
    /// Returns a new instance the elements from the given range have been inserted into the
    /// collection, starting at the given index. If no changes are detected, returns the original
    /// instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifier InsertRange(int index, IEnumerable<IIdentifierPart> range);

    /// <summary>
    /// Returns a new instance the elements obtained from the given range of values have been
    /// inserted into the collection, starting at the given index. If no changes are detected,
    /// returns the original instance.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifier InsertRange(int index, IEnumerable<string?> range);

    /// <summary>
    /// Returns a new instance where the element at the given index has been removed from the
    /// original collection.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IIdentifier RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number of elements, starting from the given index,
    /// have been removed from the collection.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IIdentifier RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the first element with the given part has been removed. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    IIdentifier Remove(string? part);

    /// <summary>
    /// Returns a new instance where the last element with the given part has been removed. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    IIdentifier RemoveLast(string? part);

    /// <summary>
    /// Returns a new instance where all the elements with the given part have been removed. If no
    /// changes are detected, returns the original instance.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    IIdentifier RemoveAll(string? part);

    /// <summary>
    /// Returns a new instance where the first element that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IIdentifier Remove(Predicate<IIdentifierPart> predicate);

    /// <summary>
    /// Returns a new instance where the last element that matches the given predicate has been
    /// removed. If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IIdentifier RemoveLast(Predicate<IIdentifierPart> predicate);

    /// <summary>
    /// Returns a new instance where all elements that match the given predicate has been removed.
    /// If no changes are detected, returns the original instance.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IIdentifier RemoveAll(Predicate<IIdentifierPart> predicate);

    /// <summary>
    /// Returns a new instance where all the elements have been removed. If no changes are
    /// detected, returns the original instance.
    /// </summary>
    /// <returns></returns>
    IIdentifier Clear();
}
namespace Yotei.ORM;

// ========================================================
/// <summary>
/// An immutable object that represents a database identifier.
/// <br/> Duplicate values are allowed.
/// </summary>
public interface IIdentifier : IEnumerable<IIdentifierPart>, IEquatable<IIdentifier>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The value carried by this instance, or null if it represents an empty or missed one. If
    /// not, that value of each dotted-separated part is wrapped with the engine terminators, if
    /// they are used.
    /// </summary>
    string? Value { get; }

    /// <summary>
    /// Determines if the value of this instance matches the given one, which can be a wrapped,
    /// unwrapped or null value. Matching is determined by comparing the unwrapped values in this
    /// instance with the target ones, from right to left, where any missed or empty target part
    /// is excluded from the comparison and considered an implicit match.
    /// </summary>
    /// <param name="specifications"></param>
    /// <returns></returns>
    bool Match(string? specifications);

    /// <summary>
    /// Gets the number of elements in this collection.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the element at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IIdentifierPart this[int index] { get; }

    /// <summary>
    /// Determines if this instance has an element with the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool Contains(string? value);

    /// <summary>
    /// Returns the index of the first element in this instance with the given value, or -1 if
    /// any is found.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int IndexOf(string? value);

    /// <summary>
    /// Returns the index of the last element in this instance with the given value, or -1 if
    /// any is found.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    int LastIndexOf(string? value);

    /// <summary>
    /// Returns the indexes of the elements in this instance with the given value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    List<int> IndexesOf(string? value);

    /// <summary>
    /// Determines if this instance has an element that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<IIdentifierPart> predicate);

    /// <summary>
    /// Returns the index of the first element in this instance that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<IIdentifierPart> predicate);

    /// <summary>
    /// Returns the index of the last element in this instance that matches the given predicate,
    /// or -1 if any is found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<IIdentifierPart> predicate);

    /// <summary>
    /// Returns the indexes of the elements in this instance that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<IIdentifierPart> predicate);

    /// <summary>
    /// Returns an array with the elements in this instance.
    /// </summary>
    /// <returns></returns>
    IIdentifierPart[] ToArray();

    /// <summary>
    /// Returns a list with the elements in this instance.
    /// </summary>
    /// <returns></returns>
    List<IIdentifierPart> ToList();

    /// <summary>
    /// Returns a new instance with the given number of elements from the original instance,
    /// starting from the given index. If no changes were needed, returns the original instance
    /// instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IIdentifier GetRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the original element at the given index has been replaced
    /// by the new given one. If no changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IIdentifier Replace(int index, IIdentifierPart item);

    /// <summary>
    /// Returns a new instance where the given element has been added to the original one. If no
    /// changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    IIdentifier Add(IIdentifierPart item);

    /// <summary>
    /// Returns a new instance where the elements from the given dotted separated value have been
    /// added to the original one. If no changes were needed, returns the original instance
    /// instead.
    /// </summary>
    /// <param name="dotted"></param>
    /// <returns></returns>
    IIdentifier Add(string? dotted);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been added to the
    /// original one. If no changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifier AddRange(IEnumerable<IIdentifierPart> range);

    /// <summary>
    /// Returns a new instance where the elements from the given range of dotted separated values
    /// have been added to the original one. If no changes were needed, returns the original
    /// instance instead.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifier AddRange(IEnumerable<string?> range);

    /// <summary>
    /// Returns a new instance where the given element has been inserted into the original one,
    /// at the given index. If no changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    IIdentifier Insert(int index, IIdentifierPart item);

    /// <summary>
    /// Returns a new instance where the elements from the given dotted separarted value have been
    /// inserted into the original one, at the given index. If no changes were needed, returns the
    /// original instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="dotted"></param>
    /// <returns></returns>
    IIdentifier Insert(int index, string? dotted);

    /// <summary>
    /// Returns a new instance where the elements from the given range have been inserted into
    /// the original one. If no changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifier InsertRange(int index, IEnumerable<IIdentifierPart> range);

    /// <summary>
    /// Returns a new instance where the elements from the given range of dotted separated values
    /// have been inserted into the original one. If no changes were needed, returns the original
    /// instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifier InsertRange(int index, IEnumerable<string?> range);

    /// <summary>
    /// Returns a new instance the original element at the given index has been removed from the
    /// original one. If no changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IIdentifier RemoveAt(int index);

    /// <summary>
    /// Returns a new instance where the given number elements, starting from the given index,
    /// have been removed from the original one. If no changes were needed, returns the original
    /// instance instead.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IIdentifier RemoveRange(int index, int count);

    /// <summary>
    /// Returns a new instance where the first element with the given value has been removed from
    /// the original one. If no changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifier Remove(string? value);

    /// <summary>
    /// Returns a new instance where the last element with the given value has been removed from
    /// the original one. If no changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifier RemoveLast(string? value);

    /// <summary>
    /// Returns a new instance where all the elements with the given value have been removed from
    /// the original one. If no changes were needed, returns the original instance instead.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifier RemoveAll(string? value);

    /// <summary>
    /// Returns a new instance where the first element that matches the given predicate has been
    /// removed from the original one. If no changes were needed, returns the original instance
    /// instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IIdentifier Remove(Predicate<IIdentifierPart> predicate);

    /// <summary>
    /// Returns a new instance where the last element that matches the given predicate has been
    /// removed from the original one. If no changes were needed, returns the original instance
    /// instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IIdentifier RemoveLast(Predicate<IIdentifierPart> predicate);

    /// <summary>
    /// Returns a new instance where all last elements that match the given predicate have been
    /// removed from the original one. If no changes were needed, returns the original instance
    /// instead.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IIdentifier RemoveAll(Predicate<IIdentifierPart> predicate);

    /// <summary>
    /// Returns a new instance where all the original elements have been removed. If no changes
    /// were needed, returns the original instance instead.
    /// </summary>
    /// <returns></returns>
    IIdentifier Clear();
}
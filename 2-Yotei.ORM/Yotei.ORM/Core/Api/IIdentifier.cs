namespace Yotei.ORM;

// ========================================================
/// <summary>
/// An immutable object that represents a database identifier.
/// </summary>
[Cloneable]
public partial interface IIdentifier : IEnumerable<IIdentifierPart>, IEquatable<IIdentifier>
{
    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The value carried by this instance, or null if it represents an empty or missed one. If
    /// not, each part is wrapped with the engine terminators, if they are used.
    /// </summary>
    string? Value { get; }

    /// <summary>
    /// Returns a literal with the unwrapped values carried by this instance separated by dots,
    /// or null if this instance represents an empty or missed identifier.
    /// </summary>
    /// <returns></returns>
    string? ToUnwrappedValue();

    /// <summary>
    /// Returns an array with the unwrapped values carried by this instance separated by dots,
    /// or an empty one if this instance represents an empty or missed identifier.
    /// </summary>
    /// <returns></returns>
    string?[] ToUnwrappedValues();

    /// <summary>
    /// Determines if the value of this identifier matches the given one, or not. Matching is
    /// determined by comparing the unwrapped parts of this instance against the target ones,
    /// from right to left. Any null, empty, or missed target part is excluded from the
    /// comparison and considered an implicit match.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    bool Match(IIdentifier target);

    // ----------------------------------------------------

    /// <summary>
    /// Gets the number of elements in this instance.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the element stored at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IIdentifierPart this[int index] { get; }

    /// <summary>
    /// Determines if this collection contains any elements with the given single part value.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    bool Contains(string? part);

    /// <summary>
    /// Returns the index of the first element in this collection with the given single part
    /// value, or -1 if any can be found.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    int IndexOf(string? part);

    /// <summary>
    /// Returns the index of the last element in this collection with the given single part
    /// value, or -1 if any can be found.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    int LastIndexOf(string? part);

    /// <summary>
    /// Returns the indexes of all the elements in this collection with the given single part
    /// value.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    List<int> IndexesOf(string? part);

    /// <summary>
    /// Determines if this collection contains any elements that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<IIdentifierPart> predicate);

    /// <summary>
    /// Returns the index of the first element in this collection that match the given predicate,
    /// or -1 if any can be found.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<IIdentifierPart> predicate);

    /// <summary>
    /// Returns the index of the last element in this collection that match the given predicate,
    /// or -1 if any can be found.
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
    /// Obtains an instance that contains the given number of elements starting from the given
    /// index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IIdentifier GetRange(int index, int count);

    /// <summary>
    /// Obtains an instance where the element at the given index has been replaced with the ones
    /// obtained from the given value.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifier Replace(int index, string? value);

    /// <summary>
    /// Obtains an instance where the elements obtained from the given value have been added to
    /// the original one.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifier Add(string? value);

    /// <summary>
    /// Obtains an instance where the elements obtained from the given range of values have been
    /// added to the original one.
    /// </summary>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifier AddRange(IEnumerable<string?> range);

    /// <summary>
    /// Obtains an instance where the elements obtained from the given value have been inserted
    /// into the original one, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    IIdentifier Insert(int index, string? value);

    /// <summary>
    /// Obtains an instance where the elements obtained from the given range of values have been
    /// inserted into the original one, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    IIdentifier InsertRange(int index, IEnumerable<string?> range);

    /// <summary>
    /// Obtains an instance where the element at the given index has been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    IIdentifier RemoveAt(int index);

    /// <summary>
    /// Obtains an instance where the given number of elements have been removed from the
    /// original one, starting from the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    IIdentifier RemoveRange(int index, int count);

    /// <summary>
    /// Obtains an instance where the first element with the given single part value has been
    /// removed.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    IIdentifier Remove(string? part);

    /// <summary>
    /// Obtains an instance where the last element with the given single part value has been
    /// removed.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    IIdentifier RemoveLast(string? part);

    /// <summary>
    /// Obtains an instance where all the elements with the given single part value have been
    /// removed.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    IIdentifier RemoveAll(string? part);

    /// <summary>
    /// Obtains an instance where the first ocurrence of an element that matches the given
    /// predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IIdentifier Remove(Predicate<IIdentifierPart> predicate);

    /// <summary>
    /// Obtains an instance where the last ocurrence of an element that matches the given
    /// predicate has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IIdentifier RemoveLast(Predicate<IIdentifierPart> predicate);

    /// <summary>
    /// Obtains an instance where all the ocurrences of elements that match the given predicate
    /// have been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    IIdentifier RemoveAll(Predicate<IIdentifierPart> predicate);

    /// <summary>
    /// Obtains an instance where all the original elements have been removed.
    /// </summary>
    /// <returns></returns>
    IIdentifier Clear();
}
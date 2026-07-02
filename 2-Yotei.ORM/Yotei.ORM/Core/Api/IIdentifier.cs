namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a database identifier.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[Cloneable]
public partial interface IIdentifier : IEquatable<IIdentifier>
{
    /// <summary>
    /// Returns a string representation of this instance using or not its null head parts, and
    /// wrapping or not the remaining parts with the engine terminators, as requested.
    /// </summary>
    /// <param name="reduce"></param>
    /// <param name="useTerminators"></param>
    /// <returns></returns>
    string ToStringEx(bool reduce = true, bool useTerminators = true);

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The actual value carried by this instance, or <see langword="null"/> if it represents an
    /// empty or missed identifier. The null or empty head parts, if any, are removed, and then
    /// each part is wrapped with the appropriate terminators.
    /// <para>
    /// The <see cref="ToStringEx(bool, bool)"/> method can be used to obtain a custom string
    /// representation, removing or not the head parts, and wrapping or not the remaining ones,
    /// as needed.
    /// </para>
    /// </summary>
    string? Value { get; }

    /// <summary>
    /// The number of parts in this identifier, which can be zero if it represents an empty or
    /// missed identifier.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the part at the given index, without any engine terminators.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    string? this[int index] { get; }

    /// <summary>
    /// Gets the part at the given index, wrapped with the appropriate terminators if the engine
    /// uses them and if such is requested.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="useTerminators"></param>
    /// <returns></returns>
    string? this[int index, bool useTerminators] { get; }

    /// <summary>
    /// Obtains the collection of parts in this identifier, or an empty one if it represents an
    /// empty or missed one. If the associated engine uses identifier terminators, and they are
    /// requested, then each part (provided it is not a null one) is wrapped with the appropriate
    /// ones.
    /// </summary>
    /// <param name="useTerminators"></param>
    /// <returns></returns>
    IEnumerable<string?> Enumerate(bool useTerminators = false);

    // ----------------------------------------------------

    /// <summary>
    /// Determines if this instance carries a part with the given value.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    bool Contains(string? part);

    /// <summary>
    /// Returns the index of the first part that matches the given value, or -1 if any.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    int IndexOf(string? part);

    /// <summary>
    /// Returns the index of the last part that matches the given value, or -1 if any.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    int LastIndexOf(string? part);

    /// <summary>
    /// Returns the indexes of all the parts that match the given value.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    List<int> IndexesOf(string? part);

    /// <summary>
    /// Determines if this instance carries a part that matches the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<string?> predicate);

    /// <summary>
    /// Returns the index of the first part that matches the given predicate, or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<string?> predicate);

    /// <summary>
    /// Returns the index of the last part that matches the given predicate, or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<string?> predicate);

    /// <summary>
    /// Returns the indexes of all the parts that match the given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<string?> predicate);

    /// <summary>
    /// Determines if this instance matches the given specifications or not. Specifications take
    /// the form of a string with dot-separated parts, where any empty or null one is considered an
    /// implicit match. Comparison is performed for each part, from right to left.
    /// </summary>
    /// <param name="specs"></param>
    /// <returns></returns>
    bool Match(string? specs);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder ToBuilder();

    /// <summary>
    /// Returns a copy of this instance reduced to a simpler from by removing its null heading
    /// parts, if any.
    /// <br/> Return this instance if no changes were made.
    /// </summary>
    /// <returns></returns>
    IIdentifier Reduce();

    /// <summary>
    /// Returns a copy of this instance where the part at the given index was replaced by the parts
    /// obtained from the given value.
    /// <br/> Return this instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    IIdentifier Replace(int index, string? value, bool reduce = true);

    /// <summary>
    /// Returns a copy of this instance where the parts obtained from the given value were added
    /// to it.
    /// <br/> Return this instance if no changes were made.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    IIdentifier Add(string? value, bool reduce = true);

    /// <summary>
    /// Returns a copy of this instance where the parts obtained from the given range of values
    /// were added to it.
    /// <br/> Return this instance if no changes were made.
    /// </summary>
    /// <param name="range"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    IIdentifier AddRange(IEnumerable<string?> range, bool reduce = true);

    /// <summary>
    /// Returns a copy of this instance where the parts obtained from the given value were inserted
    /// into it, starting at the given index.
    /// <br/> Return this instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="value"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    IIdentifier Insert(int index, string? value, bool reduce = true);

    /// <summary>
    /// Returns a copy of this instance where the parts obtained from the given range of values
    /// were inserted into it, starting at the given index.
    /// <br/> Return this instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    IIdentifier InsertRange(int index, IEnumerable<string?> range, bool reduce = true);

    /// <summary>
    /// Returns a copy of this instance where the part at the given index was removed.
    /// <br/> Return this instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    IIdentifier RemoveAt(int index, bool reduce = true);

    /// <summary>
    /// Returns a copy of this instance where the given number of parts, starting at the given
    /// index, were removed.
    /// <br/> Return this instance if no changes were made.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    IIdentifier RemoveRange(int index, int count, bool reduce = true);

    /// <summary>
    /// Returns a copy of this instance where the first ocurrence of given part was removed.
    /// <br/> Return this instance if no changes were made.
    /// </summary>
    /// <param name="part"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    IIdentifier Remove(string? part, bool reduce = true);

    /// <summary>
    /// Returns a copy of this instance where the last ocurrence of given part was removed.
    /// <br/> Return this instance if no changes were made.
    /// </summary>
    /// <param name="part"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    IIdentifier RemoveLast(string? part, bool reduce = true);

    /// <summary>
    /// Returns a copy of this instance where all the ocurrences of given part were removed.
    /// <br/> Return this instance if no changes were made.
    /// </summary>
    /// <param name="part"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    IIdentifier RemoveAll(string? part, bool reduce = true);

    /// <summary>
    /// Returns a copy of this instance where the first part that matches the given predicate
    /// was removed.
    /// <br/> Return this instance if no changes were made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    IIdentifier Remove(Predicate<string?> predicate, bool reduce = true);

    /// <summary>
    /// Returns a copy of this instance where the last part that matches the given predicate
    /// was removed.
    /// <br/> Return this instance if no changes were made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    IIdentifier RemoveLast(Predicate<string?> predicate, bool reduce = true);

    /// <summary>
    /// Returns a copy of this instance where all the parts that match the given predicate were
    /// removed.
    /// <br/> Return this instance if no changes were made.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="reduce"></param>
    /// <returns></returns>
    IIdentifier RemoveAll(Predicate<string?> predicate, bool reduce = true);

    /// <summary>
    /// Returns a copy of this instance where all its parts were removed.
    /// <br/> Return this instance if no changes were made.
    /// </summary>
    /// <returns></returns>
    IIdentifier Clear();
}
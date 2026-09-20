namespace Yotei.ORM;

// ========================================================
/// <summary>
/// Represents a database identifier, whose value can be null if it represents a null, empty or
/// missed one.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
public partial interface IIdentifier : IEquatable<IIdentifier>
{
    /// <summary>
    /// Returns an alternate string representation of this instance where the null or empty head
    /// parts may have been removed (reduced), and the remaining ones are wrapped or not with the
    /// engine's terminators if it uses them and if such was explicitly requested.
    /// </summary>
    /// <param name="reduce"></param>
    /// <param name="useTerminators"></param>
    /// <returns></returns>
    string ToString(bool reduce, bool useTerminators);
    
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder ToBuilder();

    // ----------------------------------------------------

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }

    /// <summary>
    /// The actual value of this identifier, or <see langword="null"/> if it represents a null,
    /// empty or missed one.
    /// </summary>
    string? Value { get; }

    /// <summary>
    /// Enumerates the dot-separated parts contained by this instance. Each will be returned
    /// wrapped or not with the engine's terminators if it uses them and if such was explicitly
    /// requested.
    /// </summary>
    /// <param name="useTerminators"></param>
    /// <returns></returns>
    public IEnumerable<string?> Enumerate(bool useTerminators);

    /// <summary>
    /// The number of dot-separared parts in this identifier.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// The value of the dot-separated part of this identifier at the index-th position, wrapped
    /// or not with the engine's terminators as defined by the engine itself.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public string this[int index] { get; }

    /// <summary>
    /// The value of the dot-separated part of this identifier at the index-th position, wrapped
    /// with the engine's terminators if it uses them and if such was explicitly requested.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="useTerminators"></param>
    /// <returns></returns>
    public string this[int index, bool useTerminators] { get; }

    /// <summary>
    /// Determines if this instance contains the given part, or not.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    bool Contains(string? part);

    /// <summary>
    /// Determines the index of the first ocurrence of the given part in this instance, or -1
    /// if any.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    int IndexOf(string? part);

    /// <summary>
    /// Determines the index of the last ocurrence of the given part in this instance, or -1
    /// if any.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    int LastIndexOf(string? part);

    /// <summary>
    /// Determines the indexes of all the ocurrences of the given part in this instance.
    /// </summary>
    /// <param name="part"></param>
    /// <returns></returns>
    List<int> IndexesOf(string? part);

    /// <summary>
    /// Determines if this instance contains a part that matches the given predicate, or not.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    bool Contains(Predicate<string?> predicate);

    /// <summary>
    /// Determines the index of the first ocurrence of a part in this instance that matches the
    /// given predicate, or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int IndexOf(Predicate<string?> predicate);

    /// <summary>
    /// Determines the index of the last ocurrence of a part in this instance that matches the
    /// given predicate, or -1 if any.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    int LastIndexOf(Predicate<string?> predicate);

    /// <summary>
    /// Determines the indexes of all the ocurrences of parts in this instance that match the
    /// given predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    List<int> IndexesOf(Predicate<string?> predicate);

    // ----------------------------------------------------

    /// <summary>
    /// Returns a copy of this instance reduced to a simpler form by removing its null or empty
    /// heading parts.
    /// </summary>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IIdentifier Reduce();

    /// <summary>
    /// Returns a copy of this instance with the requested number of parts, starting from the
    /// given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IIdentifier GetRange(int index, int count);

    /// <summary>
    /// Returns a copy of this instance where the part at the given index has been replaced by
    /// the new given one.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="part"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IIdentifier Replace(int index, string? part);

    /// <summary>
    /// Returns a copy of this instance where the given part has been added to it.
    /// </summary>
    /// <param name="part"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IIdentifier Add(string? part);

    /// <summary>
    /// Returns a copy of this instance where the parts from the given range have been added
    /// to it.
    /// </summary>
    /// <param name="range"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IIdentifier AddRange(IEnumerable<string?> range);

    /// <summary>
    /// Returns a copy of this instance where the given part has been added inserted into it at
    /// the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="part"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IIdentifier Insert(int index, string? part);

    /// <summary>
    /// Returns a copy of this instance where the parts from the given range have been inserted
    /// into it, starting at the given index.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="range"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IIdentifier InsertRange(int index, IEnumerable<string?> range);

    /// <summary>
    /// Returns a copy of this instance where the element at the given index has been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IIdentifier RemoveAt(int index);

    /// <summary>
    /// Returns a copy of this instance where the requested number of parts, starting at the
    /// given index, have been removed.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="count"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IIdentifier RemoveRange(int index, int count);

    /// <summary>
    /// Returns a copy of this instance where the first ocurrence of the given part, if any, has
    /// been removed.
    /// </summary>
    /// <param name="part"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IIdentifier Remove(string? part);

    /// <summary>
    /// Returns a copy of this instance where the last ocurrence of the given part, if any, has
    /// been removed.
    /// </summary>
    /// <param name="part"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IIdentifier RemoveLast(string? part);

    /// <summary>
    /// Returns a copy of this instance where the all the ocurrences of the given part, if any,
    /// have been removed.
    /// </summary>
    /// <param name="part"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IIdentifier RemoveAll(string? part);

    /// <summary>
    /// Returns a copy of this instance where the first ocurrence of a part that matches the
    /// given predicate, if any, has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IIdentifier Remove(Predicate<string?> predicate);

    /// <summary>
    /// Returns a copy of this instance where the last ocurrence of a part that matches the
    /// given predicate, if any, has been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IIdentifier RemoveLast(Predicate<string?> predicate);

    /// <summary>
    /// Returns a copy of this instance where all the ocurrences of parts that match the given
    /// predicate, if any, have been removed.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IIdentifier RemoveAll(Predicate<string?> predicate);

    /// <summary>
    /// Returns a copy of this instance that has been cleared.
    /// </summary>
    /// <returns>A new copy, or this instance if no changes have been made.</returns>
    IIdentifier Clear();
}
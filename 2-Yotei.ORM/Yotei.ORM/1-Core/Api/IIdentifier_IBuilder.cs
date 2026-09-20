namespace Yotei.ORM;

partial interface IIdentifier
{
    // ====================================================
    /// <summary>
    /// Represents a builder of <see cref="IIdentifier"/> instances.
    /// </summary>
    public partial interface IBuilder
    {
        /// <summary>
        /// <inheritdoc cref="IIdentifier.ToString(bool, bool)"/>
        /// </summary>
        /// <param name="reduce"></param>
        /// <param name="useTerminators"></param>
        /// <returns></returns>
        string ToString(bool reduce, bool useTerminators);

        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IBuilder ToInstance();

        // ------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="IIdentifier.Engine"/>
        /// </summary>
        IEngine Engine { get; }

        /// <summary>
        /// <inheritdoc cref="IIdentifier.Value"/>
        /// </summary>
        string? Value { get; }

        /// <summary>
        /// <inheritdoc cref="IIdentifier.Enumerate(bool)"/>
        /// </summary>
        /// <param name="useTerminators"></param>
        /// <returns></returns>
        public IEnumerable<string?> Enumerate(bool useTerminators);

        /// <summary>
        /// <inheritdoc cref="IIdentifier.Count"/>
        /// </summary>
        int Count { get; }

        /// <summary>
        /// <inheritdoc cref="IIdentifier.this[int]"/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string this[int index] { get; }

        /// <summary>
        /// <inheritdoc cref="IIdentifier.this[int, bool]"/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="useTerminators"></param>
        /// <returns></returns>
        public string this[int index, bool useTerminators] { get; }

        /// <summary>
        /// <inheritdoc cref="IIdentifier.Contains(string?)"/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        bool Contains(string? part);

        /// <summary>
        /// <inheritdoc cref="IIdentifier.IndexOf(string?)"/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        int IndexOf(string? part);

        /// <summary>
        /// <inheritdoc cref="IIdentifier.LastIndexOf(string?)"/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        int LastIndexOf(string? part);

        /// <summary>
        /// <inheritdoc cref="IIdentifier.IndexesOf(string?)"/>
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        List<int> IndexesOf(string? part);

        /// <summary>
        /// <inheritdoc cref="IIdentifier.Contains(Predicate{string?})"/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Contains(Predicate<string?> predicate);

        /// <summary>
        /// <inheritdoc cref="IIdentifier.IndexOf(Predicate{string?})"/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int IndexOf(Predicate<string?> predicate);

        /// <summary>
        /// <inheritdoc cref="IIdentifier.LastIndexOf(Predicate{string?})"/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int LastIndexOf(Predicate<string?> predicate);

        /// <summary>
        /// <inheritdoc cref="IIdentifier.IndexesOf(Predicate{string?})"/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        List<int> IndexesOf(Predicate<string?> predicate);

        // ----------------------------------------------------

        /// <summary>
        /// Reduces this instance to a simpler form by removing its null or empty heading parts.
        /// </summary>
        /// <returns>The number of changes made.</returns>
        int Reduce();

        /// <summary>
        /// Returns a copy of this instance with the requested number of parts, starting from the
        /// given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns>A new instance, or this one if no changes were made.</returns>
        IBuilder GetRange(int index, int count);

        /// <summary>
        /// Replaces the part at the given index by the new given one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="part"></param>
        /// <returns>The number of changes made.</returns>
        int Replace(int index, string? part);

        /// <summary>
        /// Adds to this instance the given part.
        /// </summary>
        /// <param name="part"></param>
        /// <returns>The number of changes made.</returns>
        int Add(string? part);

        /// <summary>
        /// Adds to this instance the parts from the given range.
        /// </summary>
        /// <param name="range"></param>
        /// <returns>The number of changes made.</returns>
        int AddRange(IEnumerable<string?> range);

        /// <summary>
        /// Inserts into this instance the given part at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="part"></param>
        /// <returns>The number of changes made.</returns>
        int Insert(int index, string? part);

        /// <summary>
        /// Inserts into this instance the parts from the given range starting at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <returns>The number of changes made.</returns>
        int InsertRange(int index, IEnumerable<string?> range);

        /// <summary>
        /// Removes from this instance the part at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The number of changes made.</returns>
        int RemoveAt(int index);

        /// <summary>
        /// Removes from this instance the requested number of parts, starting at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns>The number of changes made.</returns>
        int RemoveRange(int index, int count);

        /// <summary>
        /// Removes from this instance the first ocurrence of the given part, if any.
        /// </summary>
        /// <param name="part"></param>
        /// <returns>The number of changes made.</returns>
        int Remove(string? part);

        /// <summary>
        /// Removes from this instance the last ocurrence of the given part, if any.
        /// </summary>
        /// <param name="part"></param>
        /// <returns>The number of changes made.</returns>
        int RemoveLast(string? part);

        /// <summary>
        /// Removes from this instance all the ocurrences of the given part, if any.
        /// </summary>
        /// <param name="part"></param>
        /// <returns>The number of changes made.</returns>
        int RemoveAll(string? part);

        /// <summary>
        /// Removes from this instance the first ocurrence of a part that matches the given
        /// predicate, if any.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>The number of changes made.</returns>
        int Remove(Predicate<string?> predicate);

        /// <summary>
        /// Removes from this instance the last ocurrence of a part that matches the given
        /// predicate, if any.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>The number of changes made.</returns>
        int RemoveLast(Predicate<string?> predicate);

        /// <summary>
        /// Removes from this instance all the ocurrences of parts that match the given predicate,
        /// if any.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>The number of changes made.</returns>
        int RemoveAll(Predicate<string?> predicate);

        /// <summary>
        /// Clears this instance by removing all its parts.
        /// </summary>
        /// <returns>The number of changes made.</returns>
        int Clear();
    }
}

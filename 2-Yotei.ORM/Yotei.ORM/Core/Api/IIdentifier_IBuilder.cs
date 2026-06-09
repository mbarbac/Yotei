namespace Yotei.ORM;
partial interface IIdentifier
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IIdentifier"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder
    {
        /// <summary>
        /// <inheritdoc cref="IIdentifier.Engine"/>
        /// </summary>
        IEngine Engine { get; }

        /// <summary>
        /// <inheritdoc cref="IIdentifier.Value"/>
        /// </summary>
        string? Value { get; set; }

        /// <summary>
        /// <inheritdoc cref="IIdentifier.Count"/>
        /// </summary>
        int Count { get; }

        /// <summary>
        /// <inheritdoc cref="IIdentifier.this[int]"/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        string? this[int index] { get; }

        /// <summary>
        /// <inheritdoc cref="IIdentifier.this[int, bool]"/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="useTerminators"></param>
        /// <returns></returns>
        string? this[int index, bool useTerminators] { get; }

        /// <summary>
        /// <inheritdoc cref="GetParts(bool)"/>
        /// </summary>
        /// <param name="useTerminators"></param>
        /// <returns></returns>
        IEnumerable<string?> Enumerate(bool useTerminators);

        /// <summary>
        /// Reduces this instance by removing its null heading parts.
        /// </summary>
        void Reduce();

        // ----------------------------------------------------

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
        /// <inheritdoc cref="IndexesOf(string?)"/>
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
        /// Returns a new identifier based upon the contents of this instance.
        /// </summary>
        /// <returns></returns>
        IIdentifier ToInstance();

        /// <summary>
        /// Replaces the part at the given index with the parts obtained from the given value.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        int Replace(int index, string? value, bool reduce = true);

        /// <summary>
        /// Adds to this instance the parts obtained from the given value.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        int Add(string? value, bool reduce = true);

        /// <summary>
        /// Adds to this instance the parts obtained from the given range of values.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        int AddRange(IEnumerable<string?> range, bool reduce = true);

        /// <summary>
        /// Inserts into this instance the parts obtained from the given value, starting at the
        /// given index.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        int Insert(int index, string? value, bool reduce = true);

        /// <summary>
        /// Adds to this instance the parts obtained from the given range of values, starting at
        /// the given index.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <param name="reduce"></param>
        /// <returns></returns>
        int InsertRange(int index, IEnumerable<string?> range, bool reduce = true);

        /// <summary>
        /// Removes from this instance the part at the given index.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int RemoveAt(int index);

        /// <summary>
        /// Removes from this instance the given number of parts starting from the given index.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        int RemoveRange(int index, int count);

        /// <summary>
        /// Removes from this collection the first ocurrence of the given part.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        int Remove(string? part);

        /// <summary>
        /// Removes from this collection the last ocurrence of the given part.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        int RemoveLast(string? part);

        /// <summary>
        /// Removes from this collection all the ocurrences of the given part.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        int RemoveAll(string? part);

        /// <summary>
        /// Removes from this collection the first ocurrence of a part that matches the given
        /// predicate.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Remove(Predicate<string?> predicate);

        /// <summary>
        /// Removes from this collection the last ocurrence of a part that matches the given
        /// predicate.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int RemoveLast(Predicate<string?> predicate);

        /// <summary>
        /// Removes from this collection all the ocurrences of parts that match the given predicate.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int RemoveAll(Predicate<string?> predicate);

        /// <summary>
        /// Clears this collection.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <returns></returns>
        int Clear();
    }
}
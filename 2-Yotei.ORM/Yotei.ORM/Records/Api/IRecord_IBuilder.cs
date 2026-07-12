namespace Yotei.ORM.Records;

partial interface IRecord
{
    // ====================================================
    /// <summary>
    /// Represents a builder of <see cref="IRecord"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : IEnumerable<object?>
    {
        /// <summary>
        /// Returns a new builder based upon the contents of this instance, including or not the
        /// captured schema as requested.
        /// </summary>
        /// <returns></returns>
        IRecord ToRecord(bool withSchema);

        // ----------------------------------------------------

        /// <summary>
        /// The engine this instance is associated with, if it is a schema-ready one, or null if
        /// it is a schema-less one.
        /// </summary>
        IEngine? Engine { get; }

        /// <summary>
        /// <inheritdoc cref="IRecord.Elements"/>
        /// </summary>
        IEnumerable<IElement> Elements { get; }

        /// <summary>
        /// Gets a new schema based upon the contents captured by this instance, or null if it is
        /// a schema-less one, or sets the given one (including null).
        /// </summary>
        ISchema? Schema { get; set; }

        /// <summary>
        /// <inheritdoc cref="IRecord.Count"/>
        /// </summary>
        int Count { get; }

        /// <summary>
        /// <inheritdoc cref="IRecord.this[int]"/>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        object? this[int index] { get; }

        /// <summary>
        /// <inheritdoc cref="IRecord.this[string]"/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        object? this[string identifier] { get; }

        /// <summary>
        /// Tries to get the value and metadata of the element whose unique identifier is given.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="value"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool TryGet(
            string identifier,
            out object? value, [NotNullWhen(true)] out ISchemaEntry? entry);

        /// <summary>
        /// Determines if this instance contains an element that matches the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Contains(Predicate<IElement> predicate);

        /// <summary>
        /// Returns the index of the first element that matches the given predicate, or -1 if any.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int IndexOf(Predicate<IElement> predicate);

        /// <summary>
        /// Returns the index of the last element that matches the given predicate, or -1 if any.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int LastIndexOf(Predicate<IElement> predicate);

        /// <summary>
        /// Returns the indexes of the elements that match the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        List<int> IndexesOf(Predicate<IElement> predicate);

        /// <summary>
        /// <inheritdoc cref="IRecord.ToArrayOfValues"/>
        /// </summary>
        /// <returns></returns>
        object?[] ToArrayOfValues();

        /// <summary>
        /// <inheritdoc cref="IRecord.ToArray"/>
        /// </summary>
        /// <returns></returns>
        IElement[] ToArray();

        /// <summary>
        /// <inheritdoc cref="IRecord.ToListOfValues()"/>
        /// </summary>
        /// <returns></returns>
        List<object?> ToListOfValues();

        /// <summary>
        /// <inheritdoc cref="IRecord.ToList()"/>
        /// </summary>
        /// <returns></returns>
        List<IElement> ToList();

        /// <summary>
        /// <inheritdoc cref="IRecord.ToListOfValues(int, int)"/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        List<object?> ToListOfValues(int index, int count);

        /// <summary>
        /// <inheritdoc cref="ToList(int, int)"/>
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        List<IElement> ToList(int index, int count);

        // ----------------------------------------------------

        /// <summary>
        /// Replaces the value of the element at the given index by the new given one.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Replace(int index, object? value);

        /// <summary>
        /// Replaces the element at the given index by the new given one.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Replace(int index, IElement item);

        /// <summary>
        /// Adds the given value to this instance.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Add(object? value);

        /// <summary>
        /// Adds the given element to this instance.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Add(IElement item);

        /// <summary>
        /// Adds the values from the given range to this instance.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<object?> range);

        /// <summary>
        /// Adds the elements from the given range to this instance.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<IElement> range);

        /// <summary>
        /// Inserts into this instance the given value at the given index.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Insert(int index, object? value);

        /// <summary>
        /// Inserts into this instance the given element at the given index.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Insert(int index, IElement item);

        /// <summary>
        /// Inserts into this instance the given values starting at the given index.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        bool InsertRange(int index, IEnumerable<object?> range);

        /// <summary>
        /// Inserts into this instance the given elements starting at the given index.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        bool InsertRange(int index, IEnumerable<IElement> range);

        /// <summary>
        /// Removes from this instance the element at the given index.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool RemoveAt(int index);

        /// <summary>
        /// Removes from this instance the requested number of elements, starting at the given
        /// index.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        bool RemoveRange(int index, int count);

        /// <summary>
        /// Removes from this instance the first ocurrence of a value that matches the given
        /// predicate.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool RemoveValue(Predicate<object?> predicate);

        /// <summary>
        /// Removes from this instance first ocurrence of an element that matches the given
        /// predicate.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Remove(Predicate<IElement> predicate);

        /// <summary>
        /// Removes from this instance the last ocurrence of a value that matches the given
        /// predicate.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool RemoveLastValue(Predicate<object?> predicate);

        /// <summary>
        /// Removes from this instance the last ocurrence of an element that matches the given
        /// predicate.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool RemoveLast(Predicate<IElement> predicate);

        /// <summary>
        /// Removes from this instance all the ocurrences of values that matches the given
        /// predicate.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool RemoveValues(Predicate<object?> predicate);

        /// <summary>
        /// Removes from this instance the ocurrences of elements that match the given predicate.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool RemoveAll(Predicate<IElement> predicate);

        /// <summary>
        /// Clears this instance.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}
namespace Yotei.ORM.Records;

partial interface IRecord
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IRecord"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : IEnumerable<object?>
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IRecord CreateInstance();

        // ----------------------------------------------------

        /// <inheritdoc cref="IRecord.Schema"/>
        ISchema? Schema { get; set; }

        /// <inheritdoc cref="IRecord.Count"/>
        int Count { get; }

        /// <inheritdoc cref="IRecord.this[int]"/>
        object? this[int index] { get; set; }

        /// <inheritdoc cref="IRecord.this[string]"/>
        object? this[string identifier] { get; set; }

        /// <inheritdoc cref="IRecord.TryGet(string, out object?)"/>
        bool TryGet(string identifier, out object? value);

        /// <inheritdoc cref="IRecord.ToArray"/>
        object?[] ToArray();

        /// <inheritdoc cref="IRecord.ToList()"/>
        List<object?> ToList();

        /// <inheritdoc cref="IRecord.ToList(int, int)"/>
        List<object?> ToList(int index, int count);

        // ----------------------------------------------------

        /// <summary>
        /// Replaces the the element at the given index by the given one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Replace(int index, object? value);

        /// <summary>
        /// Replaces the the element at the given index by the given one.
        /// <br/> The original schema is replaced by the new given one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        bool Replace(int index, object? value, ISchema schema);

        /// <summary>
        /// Adds the given value to this collection.
        /// <br/> This method fails if this instance is a schema-full one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Add(object? value);

        /// <summary>
        /// Adds the given value to this collection.
        /// <br/> The original schema is replaced by the new given one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        bool Add(object? value, ISchema schema);

        /// <summary>
        /// Adds the values from the given range to this collection.
        /// <br/> This method fails if this instance is a schema-full one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<object?> range);

        /// <summary>
        /// Adds the values from the given range to this collection.
        /// <br/> The original schema is replaced by the new given one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<object?> range, ISchema schema);

        /// <summary>
        /// Inserts the given value into this collection at the given index.
        /// <br/> This method fails if this instance is a schema-full one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Insert(int index, object? value);

        /// <summary>
        /// Inserts the given value into this collection at the given index.
        /// <br/> The original schema is replaced by the new given one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        bool Insert(int index, object? value, ISchema schema);

        /// <summary>
        /// Inserts the values from the given range into this collection, starting at the given
        /// index.
        /// <br/> This method fails if this instance is a schema-full one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        bool InsertRange(int index, IEnumerable<object?> range);

        /// <summary>
        /// Inserts the values from the given range into this collection, starting at the given
        /// index.
        /// <br/> The original schema is replaced by the new given one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        bool InsertRange(int index, IEnumerable<object?> range, ISchema schema);

        /// <summary>
        /// Removes from this collection the element at the given index.
        /// <br/> If this instance carried a schema, then the new instance carries an adapted one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool RemoveAt(int index);

        /// <summary>
        /// Removes from this collection the given number of elements, starting at the given index.
        /// <br/> If this instance carried a schema, then the new instance carries an adapted one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        bool RemoveRange(int index, int count);

        /// <summary>
        /// Removes from this collection the element associated with the entry whose unique
        /// identifier is given, if any.
        /// <br/> This method throws an exception if this instance is a schema-less one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        bool Remove(string identifier);

        /// <summary>
        /// Clears this instance.
        /// <br/> If this instance carried a schema, then the new instance carries an adapted one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}
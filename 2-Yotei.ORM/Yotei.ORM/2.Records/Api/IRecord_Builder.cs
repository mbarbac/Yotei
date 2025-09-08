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

        /// <inheritdoc cref="IRecord.TryGet(string, out object?, out ISchemaEntry?)/>
        bool TryGet(
            string identifier,
            out object? value, [NotNullWhen(true)] out ISchemaEntry? entry);

        /// <inheritdoc cref="IRecord.ToArray"/>
        object?[] ToArray();

        /// <inheritdoc cref="IRecord.ToList()"/>
        List<object?> ToList();

        /// <inheritdoc cref="IRecord.ToList(int, int)"/>
        List<object?> ToList(int index, int count);

        // ----------------------------------------------------

        /// <summary>
        /// Replaces the value at the given index by the given one, regardless if this instance is a
        /// schema-less or schema-full one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Replace(int index, object? value);

        /// <summary>
        /// Replaces both the value at the given index and its associated schema entry with the new
        /// given ones.
        /// <br/> This method throws an exception if it is a schema-less one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool Replace(int index, object? value, ISchemaEntry entry);

        /// <summary>
        /// Adds the given value to this instance.
        /// <br/> This method throws an exception if it is a schema-full one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Add(object? value);

        /// <summary>
        /// Adds the given value to this instance.
        /// <br/> This method throws an exception if it is a schema-less one (unless the given pair
        /// is the first one to add to this instance).
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool Add(object? value, ISchemaEntry entry);

        /// <summary>
        /// Adds the values from the given range to this instance.
        /// <br/> This method throws an exception if it is a schema-full one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<object?> range);

        /// <summary>
        /// Adds the values and associated schema entries from the given ranges to this instance.
        /// <br/> This method throws an exception if it is a schema-less one (unless the given pairs
        /// are the first ones to add to this instance).
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries);

        /// <summary>
        /// Inserts the given value into this instance, at the given index.
        /// <br/> This method throws an exception if it is a schema-full one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Insert(int index, object? value);

        /// <summary>
        /// Inserts the given value and associated entry into this instance, at the given index.
        /// <br/> This method throws an exception if it is a schema-less one (unless the given pairs
        /// are the first ones to add to this instance).
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool Insert(int index, object? value, ISchemaEntry entry);

        /// <summary>
        /// Inserts the values from the given range into this instance, starting at the given index.
        /// <br/> This method throws an exception if it is a schema-full one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        bool InsertRange(int index, IEnumerable<object?> range);

        /// <summary>
        /// Inserts the values and associated metadata entries from the given ranges into this instance
        /// starting at the given index.
        /// <br/> This method throws an exception if it is a schema-less one (unless the given pairs
        /// are the first ones to add to this instance).
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        bool InsertRange(int index, IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries);

        /// <summary>
        /// Removes from this instance the value at the given index, and also its associated metadata
        /// entry, if any.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool RemoveAt(int index);

        /// <summary>
        /// Removes from this instance the given number of values starting at the given index, and also
        /// their associated metadata entries, if any.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        bool RemoveRange(int index, int count);

        /// <summary>
        /// Removes from this instance the value associated with the entry whose unique identifier is
        /// given, along with that entry.
        /// <br/> This method throws an exception if it is a schema-less one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        bool Remove(string identifier);

        /// <summary>
        /// Clears this instance.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}
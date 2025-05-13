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
        /// Returns a new host instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IRecord ToInstance();

        /// <summary>
        /// The schema that describes the structure and contents of this instance, or <c>null</c>
        /// if it is a schema-less one.
        /// </summary>
        ISchema? Schema { get; set; }

        /// <summary>
        /// The number of values carried by this instance.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets or sets the value at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        object? this[int index] { get; set; }

        /// <summary>
        /// Tries to get the value associated to the entry whose identifier is given.
        /// <br/> This method throws an exception if this instance is a schema-less one.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGet(string identifier, out object? value);

        /// <summary>
        /// Gets an array with the values in this instance.
        /// </summary>
        /// <returns></returns>
        object?[] ToArray();

        /// <summary>
        /// Gets a list with the values in this instance.
        /// </summary>
        /// <returns></returns>
        List<object?> ToList();

        /// <summary>
        /// Gets a list with the given number of values starting from the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        List<object?> ToList(int index, int count);

        // ----------------------------------------------------

        /// <summary>
        /// Keeps the given number of original elements, starting at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        bool GetRange(int index, int count);

        /// <summary>
        /// Replaces the value at the given index by the new given one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Replace(int index, object? value);

        /// <summary>
        /// Replaces the the value and schema entry at the given index by the new given ones.
        /// <br/> This method throws an exception if this instance is a schema-less one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool Replace(int index, object? value, ISchemaEntry entry);

        /// <summary>
        /// Adds the given value to this instance.
        /// <br/> This method throws an exception if this instance is a schema-full one.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Add(object? value);

        /// <summary>
        /// Adds the given value and schema entry to this collection.
        /// <br/> This method throws an exception if this instance is a schema-less one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool Add(object? value, ISchemaEntry entry);

        /// <summary>
        /// Adds the values from the given range to this instance.
        /// <br/> This method throws an exception if this instance is a schema-full one.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<object?> range);

        /// <summary>
        /// Adds the values and schema entries from the given ranges to this collection.
        /// <br/> This method throws an exception if this instance is a schema-less one.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries);

        /// <summary>
        /// Inserts the given value into this instance, at the given index.
        /// <br/> This method throws an exception if this instance is a schema-full one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Insert(int index, object? value);

        /// <summary>
        /// Inserts the given value and schema entry into this collection, at the given index.
        /// <br/> This method throws an exception if this instance is a schema-less one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool Insert(int index, object? value, ISchemaEntry entry);

        /// <summary>
        /// Inserts the values from the given range into this instance, starting at the given index.
        /// <br/> This method throws an exception if this instance is a schema-full one.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        bool InsertRange(int index, IEnumerable<object?> range);

        /// <summary>
        /// Inserts the values and schema entries from the given ranges into this collection,
        /// starting at the given index.
        /// <br/> This method throws an exception if this instance is a schema-less one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        bool InsertRange(int index, IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries);

        /// <summary>
        /// Removes the value and schema entry, if any, at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool RemoveAt(int index);

        /// <summary>
        /// Removes the given number of values and schema entries, if any, from this collection,
        /// starting from the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        bool RemoveRange(int index, int count);

        /// <summary>
        /// Clears all the contents in this instance.
        /// </summary>
        /// <returns></returns>
        bool Clear();

        // ----------------------------------------------------
    }
}
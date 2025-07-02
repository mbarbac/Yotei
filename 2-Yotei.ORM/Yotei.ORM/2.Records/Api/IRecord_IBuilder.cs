namespace Yotei.ORM.Records;

partial interface IRecord
{
    // ====================================================
    [Cloneable]
    public partial interface IBuilder : IEnumerable<object?>
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IRecord CreateInstance();

        /// <summary>
        /// The schema that describes the structure and contents of this instance, or <c>null</c>
        /// if it this instance is a schema-less one. The setter only validates that the given
        /// instance is of the correct size.
        /// </summary>
        ISchema? Schema { get; set; }

        /// <summary>
        /// The number of values carried by this instance.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the value at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        object? this[int index] { get; }

        /// <summary>
        /// Gets the value associated with the entry whose unique identifier is given. This property
        /// throws an exception if that identifier is not found, or if this instance is a schema-less
        /// one.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        object? this[string identifier] { get; }

        /// <summary>
        /// Tries to get the value associated with the entry whose unique identifier is given. This
        /// method throws an exception if this instance is a schema-less one.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool TryGet(string identifier, out object value);

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

        // ----------------------------------------------------

        /// <summary>
        /// Removes all values and schema entries from this instance, except the given number
        /// of them starting at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        bool GetRange(int index, int count);

        /// <summary>
        /// Replaces the value at the given index with the new given one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Replace(int index, object? value);

        /// <summary>
        /// Replaces the value and schema entry at the given index with the new given one.
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
        /// Adds the given value and entry to this instance.
        /// <br/> This method throws an exception if this instance is a schema-less one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool Add(object? value, ISchemaEntry entry);

        /// <summary>
        /// Adds the given range of values to this instance.
        /// <br/> This method throws an exception if this instance is a schema-full one.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<object?> range);

        /// <summary>
        /// Adds the given ranges of values and entries to this instance.
        /// <br/> This method throws an exception if those ranges are not of the same size.
        /// <br/> This method throws an exception if this instance is a schema-less one.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries);

        /// <summary>
        /// Inserts the given value into this instance at the given index.
        /// <br/> This method throws an exception if this instance is a schema-full one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Insert(int index, object? value);

        /// <summary>
        /// Inserts the given value and schema entry into this instance at the given index.
        /// <br/> This method throws an exception if this instance is a schema-less one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool Insert(int index, object? value, ISchemaEntry entry);

        /// <summary>
        /// Inserts the given range of valuesinto this instance starting at the given index.
        /// <br/> This method throws an exception if this instance is a schema-full one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        bool InsertRange(int index, IEnumerable<object?> range);

        /// <summary>
        /// Inserts the given ranges of values and entries into this instance, starting at the
        /// given index.
        /// <br/> This method throws an exception if those ranges are not of the same size.
        /// <br/> This method throws an exception if this instance is a schema-less one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        bool InsertRange(int index, IEnumerable<object?> range, IEnumerable<ISchemaEntry> entries);

        /// <summary>
        /// Removes from this instance the value, and schema entry if any, at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool RemoveAt(int index);

        /// <summary>
        /// Removes from this collection the given number of values, and schema entries, if any,
        /// starting at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        bool RemoveRange(int index, int count);

        /// <summary>
        /// Returns a new instance where all the original contents have been cleared.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}
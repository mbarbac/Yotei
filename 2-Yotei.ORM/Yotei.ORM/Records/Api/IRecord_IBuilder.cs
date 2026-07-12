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
        /// Returns a new record based upon de contents of this builder.
        /// </summary>
        /// <returns></returns>
        IRecord ToInstance();

        // ------------------------------------------------

        /// <summary>
        /// Gets or sets the schema associated with this instance. If the value of this property
        /// is <see langword="null"/>, then this instance is a schema-less one that only contain
        /// values and no metadata.
        /// </summary>
        ISchema? Schema { get; set; }

        /// <summary>
        /// Gets the number of values in this instance.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets or sets the value carried by this instance at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        object? this[int index] { get; set; }

        /// <summary>
        /// Gets the value and metadata carried by this instance at the given index. This method
        /// throws an exception if this instance is a schema-less one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        object? Get(int index, out ISchemaEntry entry);

        /// <summary>
        /// Gets or sets the value of the element whose identifier is given, including redundant
        /// ones if any. This property throws an exception if such element cannot be found, or if
        /// this instance is schema-less one.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        object? this[string identifier] { get; set; }

        /// <summary>
        /// Tries to get the value of the element whose identifier is given. This method throws an
        /// exception if this instance is schema-less one.
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="value"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool TryGet(
            string identifier,
            out object? value, [NotNullWhen(true)] out ISchemaEntry? entry);

        /// <summary>
        /// Gets an array with the values of this instance.
        /// </summary>
        /// <returns></returns>
        object?[] ToArray();

        /// <summary>
        /// Gets a list with the values of this instance.
        /// </summary>
        /// <returns></returns>
        List<object?> ToList();

        /// <summary>
        /// Gets a list with the given number of values from this instance, starting at the given
        /// index.
        /// </summary>
        /// <returns></returns>
        List<object?> ToList(int index, int count);

        // ------------------------------------------------

        /// <summary>
        /// Replaces the value at the given index with the new given one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Replace(int index, object? value);

        /// <summary>
        /// Replaces the value and metadata at the given index with the new given one. This method
        /// throws an exception if this instance is a schema-less one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool Replace(int index, object? value, ISchemaEntry entry);

        /// <summary>
        /// Adds to this instance the given value. This method throws an exception if this instance
        /// is a schema-ready one.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Add(object? value);

        /// <summary>
        /// Adds to this instance the given value and metadata. This method throws an exception if
        /// this instance is a schema-less one.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool Add(object? value, ISchemaEntry entry);

        /// <summary>
        /// Adds to this instance the values from the given range. This method throws an exception
        /// if this instance is a schema-ready one.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<object> values);

        /// <summary>
        /// Adds to this instance the values and metadata from the given ranges. This method throws
        /// an exception if this instance is a schema-less one.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<object> values, IEnumerable<ISchemaEntry> entries);

        /// <summary>
        /// Inserts into this instance the given value at the given index. This method throws an
        /// exception if this instance is a schema-ready one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Insert(int index, object? value);

        /// <summary>
        /// Inserts into this instance the given value and metadata at the given index. This method
        /// throws an exception if this instance is a schema-less one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool Insert(int index, object? value, ISchemaEntry entry);

        /// <summary>
        /// Inserts into this instance the values from the given range, starting at the given index.
        /// This method throws an exception if this instance is a schema-ready one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        bool InsertRange(int index, IEnumerable<object> values);

        /// <summary>
        /// Inserts into this instance the values and metadata from the given ranges, starting at
        /// the given index. This method throws an exception if this instance is a schema-less one.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="values"></param>
        /// <param name="entries"></param>
        /// <returns></returns>
        bool InsertRange(int index, IEnumerable<object> values, IEnumerable<ISchemaEntry> entries);

        /// <summary>
        /// Removes from this instance the values and metadata at the given index, or indexes if
        /// there are redundant ones.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool RemoveAt(int index);

        /// <summary>
        /// Removes from this instance all the values and entries with the given identifier. This
        /// This method throws an exception if this instance is a schema-less one.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        bool Remove(string identifier);

        /// <summary>
        /// Clear this instance. By default, an empty schema is kept unless otherwise requested.
        /// </summary>
        /// <param name="keepEmptySchema"></param>
        /// <returns></returns>
        bool Clear(bool keepEmptySchema = true);
    }
}
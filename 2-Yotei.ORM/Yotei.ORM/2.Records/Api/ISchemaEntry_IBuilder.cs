namespace Yotei.ORM.Records;

partial interface ISchemaEntry
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="ISchemaEntry"/> instances.
    /// </summary>
    public partial interface IBuilder : IEnumerable<IMetadataEntry>
    {
        /// <inheritdoc cref="ICloneable.Clone"/>
        IBuilder Clone();

        // ----------------------------------------------------

        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        ISchemaEntry CreateInstance();

        // ----------------------------------------------------

        /// <inheritdoc cref="ISchemaEntry.Engine"/>
        IEngine Engine { get; }

        /// <inheritdoc cref="ISchemaEntry.Identifier"/>
        IIdentifier Identifier { get; set; }

        /// <inheritdoc cref="ISchemaEntry.IsPrimaryKey"/>
        bool IsPrimaryKey { get; set; }

        /// <inheritdoc cref="ISchemaEntry.IsUniqueValued"/>
        bool IsUniqueValued { get; set; }

        /// <inheritdoc cref="ISchemaEntry.IsReadOnly"/>
        bool IsReadOnly { get; set; }

        // ----------------------------------------------------

        /// <inheritdoc cref="ISchemaEntry.Count"/>
        int Count { get; }

        /// <inheritdoc cref="ISchemaEntry.Find(string)"/>
        IMetadataEntry? Find(string name);

        /// <inheritdoc cref="ISchemaEntry.Find(IEnumerable{string})"/>
        IMetadataEntry? Find(IEnumerable<string> range);

        /// <inheritdoc cref="ISchemaEntry.Contains(string)"/>
        bool Contains(string name);

        /// <inheritdoc cref="ISchemaEntry.Contains(IEnumerable{string})"/>
        bool Contains(IEnumerable<string> range);

        /// <inheritdoc cref="ISchemaEntry.ToArray"/>
        IMetadataEntry[] ToArray();

        /// <inheritdoc cref="ISchemaEntry.ToList"/>
        List<IMetadataEntry> ToList();

        /// <inheritdoc cref="ISchemaEntry.Trim"/>
        void Trim();

        // ----------------------------------------------------

        /// <summary>
        /// Replaces the the value of the original entry whose name is given (or can be obtained
        /// from any of the well-known tags in the associated engine) by the new given one.
        /// <br/> If name refers to a standard property (by itself or via a well-known metadata
        /// tag) then the entry and the property value are synchronized.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Replace(string name, object? value);

        /// <summary>
        /// Adds the given element to the original collection.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Add(IMetadataEntry item);

        /// <summary>
        /// Adds the elements from the given range to the original collection.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<IMetadataEntry> range);

        /// <summary>
        /// Removes the original entry whose name is given, or can be obtained from any of the
        /// well-known tags in the associated engine.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Remove(string name);

        /// <summary>
        /// Removes the first element that matches the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Remove(Predicate<IMetadataEntry> predicate);

        /// <summary>
        /// Removes the last element that matches the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool RemoveLast(Predicate<IMetadataEntry> predicate);

        /// <summary>
        /// Removes all the elements that match the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool RemoveAll(Predicate<IMetadataEntry> predicate);

        /// <summary>
        /// Clears this instance, including both its standard properties and the list it maintains.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}
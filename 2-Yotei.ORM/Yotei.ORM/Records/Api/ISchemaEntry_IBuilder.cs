namespace Yotei.ORM.Records;

partial interface ISchemaEntry
{
    // ====================================================
    /// <summary>
    /// Represents a builder of <see cref="ISchemaEntry"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : IEnumerable<IMetadataEntry>
    {
        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Identifier"/>
        /// </summary>
        IIdentifier? Identifier { get; set; }

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.IsPrimaryKey"/>
        /// </summary>
        bool? IsPrimaryKey { get; set; }

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.IsUniqueValued"/>
        /// </summary>
        bool? IsUniqueValued { get; set; }

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.IsReadOnly"/>
        /// </summary>
        bool? IsReadOnly { get; set; }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Engine"/>
        /// </summary>
        IEngine Engine { get; }

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Count"/>
        /// </summary>
        int Count { get; }

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.this[string, bool]"/> The setter creates an ad-hoc
        /// entry if such is needed.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        object? this[string name, bool strict = false] { get; set; }

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Contains(string, bool)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        bool Contains(string name, bool strict = false);

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Contains(IEnumerable{string}, bool)"/>
        /// </summary>
        /// <param name="names"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        bool Contains(IEnumerable<string> names, bool strict = false);

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Find(string, bool)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        IMetadataEntry? Find(string name, bool strict = false);

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Find(IEnumerable{string}, bool)"/>
        /// </summary>
        /// <param name="names"></param>
        /// <param name="strict"></param>
        /// <returns></returns>
        List<IMetadataEntry> Find(IEnumerable<string> names, bool strict = false);

        // ------------------------------------------------

        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        ISchemaEntry ToInstance();

        /// <summary>
        /// Adds to this instance the given metadata entry.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Add(IMetadataEntry item);

        /// <summary>
        /// Adds to this instance the entries of the given range.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<IMetadataEntry> range);

        /// <summary>
        /// Either adds or updates the existing metadata entry whose name is associated with
        /// the name of the given one.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Update(IMetadataEntry item);

        /// <summary>
        /// Updates in this instance the entries of the given range without throwing exceptions
        /// if any is a duplicated one. Last one updated wins.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool UpdateRange(IEnumerable<IMetadataEntry> range);

        /// <summary>
        /// Removes from this instance the entry associated with the given name, if any.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Remove(string name);

        /// <summary>
        /// Clears this instance.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}
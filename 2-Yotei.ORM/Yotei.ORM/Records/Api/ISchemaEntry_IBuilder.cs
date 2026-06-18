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
        /// <inheritdoc cref="ISchemaEntry.this[string]"/> The setter creates an ad-hoc entry
        /// if such is needed.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object? this[string name] { get; set; }

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Contains(string)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Contains(string name);

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Contains(IEnumerable{string})"/>
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        bool Contains(IEnumerable<string> names);

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Find(string)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IMetadataEntry? Find(string name);

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Find(IEnumerable{string})"/>
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        List<IMetadataEntry> Find(IEnumerable<string> names);

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Find(Predicate{IMetadataEntry})"/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        List<IMetadataEntry> Find(Predicate<IMetadataEntry> predicate);

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
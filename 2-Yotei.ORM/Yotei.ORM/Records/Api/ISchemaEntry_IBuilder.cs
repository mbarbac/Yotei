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
        /// The identifier by which the associated schema element is known, or null if it this
        /// information is not available. Entries with no identifier, or with an empty one, are
        /// not considered valid schema elements.
        /// </summary>
        IIdentifier? Identifier { get; set; }

        /// <summary>
        /// Whether the associated schema element is a primary key one, or part of a primary key
        /// group, or null if this information is not available. Only one group is supported per
        /// schema.
        /// </summary>
        bool? IsPrimaryKey { get; set; }

        /// <summary>
        /// Whether the associated schema element is a unique valued one, or part of an unique
        /// valued group, or null if this information is not available. Only one group is supported
        /// per schema.
        /// </summary>
        bool? IsUniqueValued { get; set; }

        /// <summary>
        /// Whether the associated schema element is a read-only one, or null if this information
        /// is not available.
        /// </summary>
        bool? IsReadOnly { get; set; }

        // ------------------------------------------------

        /// <summary>
        /// The engine this instance is associated with.
        /// </summary>
        IEngine Engine { get; }

        /// <summary>
        /// Gets the number of metadata entries in this instance,.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets or sets the metadata value carried by the entry whose metadata name is given.
        /// <br/> The getter throws an exception if such entry does not exist yet.
        /// <br/> The setter either updates an existing entry, or creates an ad-hoc one.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object? this[string name] { get; set; }

        /// <summary>
        /// Determines if this instance carries a metadata entry with the given metadata name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Contains(string name);

        /// <summary>
        /// Determines if this instance carries any metadata entry with any of the given metadata
        /// names.
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        bool Contains(IEnumerable<string> names);

        /// <summary>
        /// Returns the unique metadata entry in this collection with the given metadata name,
        /// or null if such cannot be found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IMetadataEntry? Find(string name);

        /// <summary>
        /// Returns the collection of metadata entries whose metadata names are given. This method
        /// guarantees that in the returned collection there will be no duplicated elements.
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        List<IMetadataEntry> Find(IEnumerable<string> names);

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
        /// Either adds or updates the existing metadata entry that correspond with the given one.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Update(IMetadataEntry item);

        /// <summary>
        /// Either adds or updates the existing metadata entries that correspond to the given ones.
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
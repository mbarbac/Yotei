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
        /// <inheritdoc cref="ISchemaEntry.Engine"/>
        /// </summary>
        IEngine Engine { get; }

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Identifier"/>
        /// </summary>
        IIdentifier Identifier { get; set; }

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.IsPrimaryKey"/>
        /// </summary>
        bool IsPrimaryKey { get; set; }

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.IsUniqueValued"/>
        /// </summary>
        bool IsUniqueValued { get; set; }

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.IsReadOnly"/>
        /// </summary>
        bool IsReadOnly { get; set; }

        // ----------------------------------------------------

        /// <summary>
        /// Gets or sets the metadata value associated with the given tag name. The getter throws
        /// an exception if the entry did not exist yet. The setter creates an entry if such happens.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object? this[string name] { get; set; }

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Count"/>
        /// </summary>
        int Count { get; }

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Contains(string)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Contains(string name);

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.ContainsAny(IEnumerable{string})"/>
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        bool ContainsAny(IEnumerable<string> names);

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Find(string, out IMetadataEntry)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool Find(string name, [MaybeNullWhen(false)] out IMetadataEntry entry);

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.FindAny(IEnumerable{string}, out IMetadataEntry)"/>
        /// </summary>
        /// <param name="names"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool FindAny(IEnumerable<string> names, [MaybeNullWhen(false)] out IMetadataEntry entry);

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.ToArray"/>
        /// </summary>
        /// <returns></returns>
        IMetadataEntry[] ToArray();

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.ToList"/>
        /// </summary>
        /// <returns></returns>
        List<IMetadataEntry> ToList();

        // ----------------------------------------------------

        /// <summary>
        /// Adds to this instance the given entry.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        bool Add(IMetadataEntry entry);

        /// <summary>
        /// Adds to this instance the entries of the given range.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<IMetadataEntry> range);

        /// <summary>
        /// Removes from this instance the metadata entry whose name is given.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Remove(string name);

        /// <summary>
        /// Removes from this instance the first metadata entry that matches the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Remove(Predicate<IMetadataEntry> predicate);

        /// <summary>
        /// Removes from this instance all the metadata entries that match the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool RemoveAll(Predicate<IMetadataEntry> predicate);

        /// <summary>
        /// Clears this instance removing all its metadata entries and reverting its well-known
        /// properties to their default values.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}
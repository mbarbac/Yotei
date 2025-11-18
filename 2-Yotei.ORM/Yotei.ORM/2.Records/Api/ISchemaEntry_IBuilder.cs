namespace Yotei.ORM.Records;

partial interface ISchemaEntry
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="ISchemaEntry"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : IEnumerable<IMetadataEntry>
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        ISchemaEntry CreateInstance();

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Engine"/>
        /// </summary>
        IEngine Engine { get; }

        // ------------------------------------------------

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

        // ------------------------------------------------

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
        /// <inheritdoc cref="ISchemaEntry.Contains(IEnumerable{string})"/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Contains(IEnumerable<string> range);

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Find(string)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IMetadataEntry? Find(string name);

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Find(IEnumerable{string})"/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        IMetadataEntry? Find(IEnumerable<string> range);

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

        /// <summary>
        /// <inheritdoc cref="ISchemaEntry.Trim"/>
        /// </summary>
        void Trim();

        // ----------------------------------------------------

        /// <summary>
        /// Adds to this collection the given metadata pair.
        /// <br/> Returns if changes have been made or not.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Add(IMetadataEntry item);

        /// <summary>
        /// Adds to this collection the metadata pairs of the given range.
        /// <br/> Returns if changes have been made or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Add(IEnumerable<IMetadataEntry> range);

        /// <summary>
        /// Removes from this collection the metadata pair whose name is given, or with a name
        /// that can be associated to it via the well-known multi-name metadata tags.
        /// <br/> Returns if changes have been made or not.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Remove(string name);

        /// <summary>
        /// Removes from this collection the first metadata pair that matches the given predicate.
        /// <br/> Returns if changes have been made or not.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Remove(Predicate<IMetadataEntry> predicate);

        /// <summary>
        /// Removes from this collection the last metadata pair that matches the given predicate.
        /// <br/> Returns if changes have been made or not.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool RemoveLast(Predicate<IMetadataEntry> predicate);

        /// <summary>
        /// Removes from this collection all the metadata pairs that match the given predicate.
        /// <br/> Returns if changes have been made or not.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool RemoveAll(Predicate<IMetadataEntry> predicate);

        /// <summary>
        /// Clears this collection.
        /// <br/> Returns if changes have been made or not.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}
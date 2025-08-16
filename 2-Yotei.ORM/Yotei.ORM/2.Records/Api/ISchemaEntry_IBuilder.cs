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
        /// Replaces the value of the entry whose name is given by the new given one.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool Replace(string name, object? value);

        /// <summary>
        /// Adds the given element to this collection.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Add(IMetadataEntry item);

        /// <summary>
        /// Adds to this collection the elements of the given range.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<IMetadataEntry> range);

        /// <summary>
        /// Removes from this collection the entry whose name is given.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Remove(string name);

        /// <summary>
        /// Removes from this collection the first element that matches the given predicate.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Remove(Predicate<IMetadataEntry> predicate);

        /// <summary>
        /// Removes from this collection the last element that matches the given predicate.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool RemoveLast(Predicate<IMetadataEntry> predicate);

        /// <summary>
        /// Removes from this collection all the elements that match the given predicate.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool RemoveAll(Predicate<IMetadataEntry> predicate);

        /// <summary>
        /// Clears this collection.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}
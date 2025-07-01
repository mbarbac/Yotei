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

        /// <summary>
        /// The engine this instance is associated with.
        /// </summary>
        IEngine Engine { get; }

        /// <summary>
        /// The identifier by which this instance is known.
        /// </summary>
        IIdentifier Identifier { get; set; }

        /// <summary>
        /// Determines if this instance is a primary key, or is part of a primary key group, or not.
        /// </summary>
        bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Determines if this instance is a unique valued one, or is part of a unique valued group,
        /// or not.
        /// </summary>
        bool IsUniqueValued { get; set; }

        /// <summary>
        /// Determines if this instance is a read only one or not.
        /// </summary>
        bool IsReadOnly { get; set; }

        // ----------------------------------------------------

        /// <summary>
        /// Gets the number of metadata pairs in this collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the metadata pair whose tag contains the given name. If no pair is found, then an
        /// exception is thrown.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IMetadataEntry this[string name] { get; set; }

        /// <summary>
        /// Returns the metadata entry whose tag contains the given name, or <c>null</c> if not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IMetadataEntry? Find(string name);

        /// <summary>
        /// Returns the metadata entry whose tag contains any name from the given range, or <c>null</c>
        /// if not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IMetadataEntry? Find(IEnumerable<string> range);

        /// <summary>
        /// Determines if this instance contains an entry whose tag contains the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Contains(string name);

        /// <summary>
        /// Determines if this instance contains an entry whose tag contains tany name from the given
        /// range.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Contains(IEnumerable<string> range);

        /// <summary>
        /// Gets an array with the metadata pairs in this collection.
        /// </summary>
        /// <returns></returns>
        IMetadataEntry[] ToArray();

        /// <summary>
        /// Gets a list with the metadata pairs in this collection.
        /// </summary>
        /// <returns></returns>
        List<IMetadataEntry> ToList();

        /// <summary>
        /// Trims the internal structures of this collection.
        /// </summary>
        void Trim();

        // ----------------------------------------------------

        /// <summary>
        /// Replaces the original source entry associated with the given tag name by the given
        /// target one.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        bool Replace(string name, IMetadataEntry target);

        /// <summary>
        /// Replaces the source entry in this collection by the the target given one.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        bool Replace(IMetadataEntry source, IMetadataEntry target);

        /// <summary>
        /// Adds to this collection the given element.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Add(IMetadataEntry item);

        /// <summary>
        /// Adds to this collection the elements from the given range.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<IMetadataEntry> range);

        /// <summary>
        /// Removes from this collection the given element.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Remove(IMetadataEntry item);

        /// <summary>
        /// Removes from this collection the first element that matches the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool Remove(Predicate<IMetadataEntry> predicate);

        /// <summary>
        /// Removes from this collection the last element that matches the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool RemoveLast(Predicate<IMetadataEntry> predicate);

        /// <summary>
        /// Removes from this collection all the elements that match the given predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool RemoveAll(Predicate<IMetadataEntry> predicate);

        /// <summary>
        /// Removes from this collection all its elements.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}
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
        /// Returns a new host instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        ISchemaEntry ToInstance();

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

        // ------------------------------------------------

        /// <summary>
        /// Gets the number of metadata pairs in this collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Tries to obtain he metadata pair whose tag contains the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        bool TryGet(string name, [NotNullWhen(true)] out IMetadataEntry? item);

        /// <summary>
        /// Tries to obtain he metadata pair whose tag contains the any of the names from the given
        /// range.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        bool TryGet(IEnumerable<string> range, [NotNullWhen(true)] out IMetadataEntry? item);

        /// <summary>
        /// Determines if this instance contains a metadata pair whose tag contains the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Contains(string name);

        /// <summary>
        /// Determines if this instance contains a metadata pair whose tag contains any of the names
        /// from the given range.
        /// </summary>
        /// <param name="range"></param>
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

        // ------------------------------------------------

        /// <summary>
        /// Replaces the metadata pair whose tag contains the given name with the new given one.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Replace(string name, IMetadataEntry item);

        /// <summary>
        /// Replaces the metadata pair whose tag contains any of the names from the given range
        /// with the new given one.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Replace(IEnumerable<string> range, IMetadataEntry item);

        /// <summary>
        /// Replaces the value of the metadata pair whose tag contains the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool ReplaceValue(string name, object? value);

        /// <summary>
        /// Replaces the value of the metadata pair whose tag contains any name from the given
        /// range.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool ReplaceValue(IEnumerable<string> range, object? value);

        /// <summary>
        /// Adds the given element has been added to this collection.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool Add(IMetadataEntry item);

        /// <summary>
        /// Adds the elements from the given range to this collection.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<IMetadataEntry> range);

        /// <summary>
        /// Removes the metadata pair whose tag contains the given name from this collection.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Remove(string name);

        /// <summary>
        /// Removes the metadata pair whose tag contains any of the names from the given range
        /// from this collection.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Remove(IEnumerable<string> range);

        /// <summary>
        /// Clears all the contents of this instance.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}
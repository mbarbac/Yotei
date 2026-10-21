namespace Yotei.ORM.Records;

partial interface ISchemaElement
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="ISchemaElement"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : IEnumerable<IMetadataItem>
    {
        /// <summary>
        /// <inheritdoc cref="ISchemaElement.Identifier"/>
        /// </summary>
        IIdentifier? Identifier { get; set; }

        /// <summary>
        /// <inheritdoc cref="ISchemaElement.IsPrimaryKey"/>
        /// </summary>
        bool? IsPrimaryKey { get; set; }

        /// <summary>
        /// <inheritdoc cref="ISchemaElement.IsUniqueValued"/>
        /// </summary>
        bool? IsUniqueValued { get; set; }

        /// <summary>
        /// <inheritdoc cref="ISchemaElement.IsReadOnly"/>
        /// </summary>
        bool? IsReadOnly { get; set; }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="ISchemaElement.Engine"/>
        /// </summary>
        IEngine Engine { get; }

        /// <summary>
        /// <inheritdoc cref="ISchemaElement.Count"/>
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The getter returns the value of the metadata item whose metadata name is given, or
        /// throws an exception if such item does not exist. The setter either creates such item,
        /// or updates the value of the existing one.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object? this[string name] { get; }

        /// <summary>
        /// <inheritdoc cref="ISchemaElement.Contains(string)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Contains(string name);

        /// <summary>
        /// 
        /// <inheritdoc cref="ISchemaElement.Contains(IEnumerable{string})"/>
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        bool Contains(IEnumerable<string> names);

        /// <summary>
        /// 
        /// <inheritdoc cref="ISchemaElement.Find(string)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IMetadataItem? Find(string name);

        /// <summary>
        /// <inheritdoc cref="ISchemaElement.Find(IEnumerable{string})"/>
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        List<IMetadataItem> Find(IEnumerable<string> names);

        /// <summary>
        /// <inheritdoc cref="ISchemaElement.ToArray"/>
        /// </summary>
        /// <returns></returns>
        IMetadataItem[] ToArray();

        /// <summary>
        /// <inheritdoc cref="ISchemaElement.ToList"/>
        /// </summary>
        /// <returns></returns>
        List<IMetadataItem> ToList();

        // ----------------------------------------------------

        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        ISchemaElement ToInstance();

        /// <summary>
        /// Adds to this instance the given metadata item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Whether changes have been made, or not.</returns>
        bool Add(IMetadataItem item);

        /// <summary>
        /// Adds to this instance a new metadata item based upon the given metadata name and
        /// value.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Whether changes have been made, or not.</returns>
        bool Add(string name, object? value);

        /// <summary>
        /// Adds to this instance the metadata items from the given range.
        /// </summary>
        /// <param name="range"></param>
        /// <returns>Whether changes have been made, or not.</returns>
        bool AddRange(IEnumerable<IMetadataItem> range);

        /// <summary>
        /// Either adds to this instance the given metadata item, or updates the value of the
        /// existing one.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Whether changes have been made, or not.</returns>
        bool Update(IMetadataItem item);

        /// <summary>
        /// Either adds to this instance a new metadata item based upon the given metadata name
        /// and value, or updates the value of the existing one.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Whether changes have been made, or not.</returns>
        bool Update(string name, object? value);

        /// <summary>
        /// Either adds to this instance the metadata items from the given range, or updates the
        /// values of the existing ones.
        /// </summary>
        /// <param name="range"></param>
        /// <returns>Whether changes have been made, or not.</returns>
        bool UpdateRange(IEnumerable<IMetadataItem> range);

        /// <summary>
        /// Removes from this instance the metadata item associated with the given metadata name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Whether changes have been made, or not.</returns>
        bool Remove(string name);

        /// <summary>
        /// Clears from this instance all its metadata items and well-known properties.
        /// </summary>
        /// <returns>Whether changes have been made, or not.</returns>
        bool Clear();
    }
}
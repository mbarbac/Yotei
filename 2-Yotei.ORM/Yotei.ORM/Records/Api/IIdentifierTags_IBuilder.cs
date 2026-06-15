namespace Yotei.ORM.Records;
partial interface IIdentifierTags
{
    // ====================================================
    /// <summary>
    /// Represents a builder of <see cref="IIdentifier"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : IEnumerable<IMetadataTag>
    {
        /// <summary>
        /// <inheritdoc cref="IIdentifierTags.IgnoreCase"/>
        /// </summary>
        bool IgnoreCase { get; }

        /// <summary>
        /// Gets or sets the metadata tag at the given index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IMetadataTag this[int index] { get; }

        /// <summary>
        /// <inheritdoc cref="IIdentifierTags.Count"/>
        /// </summary>
        int Count { get; }

        /// <summary>
        /// <inheritdoc cref="IIdentifierTags.Contains(string)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Contains(string name);

        /// <summary>
        /// <inheritdoc cref="IIdentifierTags.ContainsAny(IEnumerable{string})"/>
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        bool ContainsAny(IEnumerable<string> names);

        /// <summary>
        /// <inheritdoc cref="IIdentifierTags.IndexOf(string)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int IndexOf(string name);

        /// <summary>
        /// <inheritdoc cref="IIdentifierTags.IndexOfAny(IEnumerable{string})"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int IndexOfAny(IEnumerable<string> names);

        /// <summary>
        /// <inheritdoc cref="IIdentifierTags.IndexOf(Predicate{IMetadataTag})"/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int IndexOf(Predicate<IMetadataTag> predicate);

        /// <summary>
        /// <inheritdoc cref="IIdentifierTags.LastIndexOf(Predicate{IMetadataTag})"/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int LastIndexOf(Predicate<IMetadataTag> predicate);

        /// <summary>
        /// <inheritdoc cref="IIdentifierTags.IndexesOf(Predicate{IMetadataTag})"/>
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        List<int> IndexesOf(Predicate<IMetadataTag> predicate);

        /// <summary>
        /// <inheritdoc cref="IIdentifierTags.ToArray"/>
        /// </summary>
        /// <returns></returns>
        IMetadataTag[] ToArray();

        /// <summary>
        /// <inheritdoc cref="IIdentifierTags.ToList"/>
        /// </summary>
        /// <returns></returns>
        List<IMetadataTag> ToList();

        // ----------------------------------------------------

        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IIdentifierTags ToInstance();

        /// <summary>
        /// Adds the given tag to this collection.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        int Add(IMetadataTag tag);

        /// <summary>
        /// Adds to this collection the tag or tags obtained from the given dot-separated value.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        int Add(string value);

        /// <summary>
        /// Adds to this collection the tags of the given range.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        int AddRange(IEnumerable<IMetadataTag> tag);

        /// <summary>
        /// Adds to this collection the tags or tags obtained from of the given range of dot-separated
        /// values.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        int AddRange(IEnumerable<string> values);

        /// <summary>
        /// Inserts into this collection the given tag, at the given index.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        int Insert(int index, IMetadataTag tag);

        /// <summary>
        /// Inserts into this collection the tag or tags obtained from the given dot-separated
        /// value, starting at the given index.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        int Insert(int index, string value);

        /// <summary>
        /// Inserts into this collection the tags of the given range, starting at the given index.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        int InsertRange(int index, IEnumerable<IMetadataTag> tag);

        /// <summary>
        /// Inserts into this collection the tag or tags obtained from the given range of dot
        /// separated values, starting at the given index.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        int InsertRange(int index, IEnumerable<string> values);

        /// <summary>
        /// Removes from this collection the tag at the given index.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int RemoveAt(int index);

        /// <summary>
        /// Removes from this collection the given number of tags, starting at the given index.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        int RemoveRange(int index, int count);

        /// <summary>
        /// Clears this collection.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <returns></returns>
        int Clear();
    }
}
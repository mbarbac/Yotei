namespace Yotei.ORM.Records;
partial interface IMetadataTag
{
    // ====================================================
    /// <summary>
    /// Represents a builder of <see cref="IMetadataTag"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : IEnumerable<string>
    {
        /// <summary>
        /// <inheritdoc cref="IMetadataTag.IgnoreCase"/>
        /// </summary>
        bool IgnoreCase { get; }

        /// <summary>
        /// An arbitrary default name of this collection of tags. The setter sets the given value as
        /// the new default one, provided that it belongs to this collection. If not, an exception is
        /// thrown.
        /// </summary>
        string Default { get; set; }

        /// <summary>
        /// <inheritdoc cref="IMetadataTag.Count"/>
        /// </summary>
        int Count { get; }

        /// <summary>
        /// <inheritdoc cref="IMetadataTag.Contains(string)"/>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Contains(string name);

        /// <summary>
        /// <inheritdoc cref="IMetadataTag.Contains(IEnumerable{string})"/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool ContainsAny(IEnumerable<string> range);

        /// <summary>
        /// <inheritdoc cref="IMetadataTag.ToArray"/>
        /// </summary>
        /// <returns></returns>
        string[] ToArray();

        /// <summary>
        /// <inheritdoc cref="IMetadataTag.ToList"/>
        /// </summary>
        /// <returns></returns>
        List<string> ToList();

        // ------------------------------------------------

        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IMetadataTag ToInstance();

        /// <summary>
        /// Replaces the original name with the new given one.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="oldname"></param>
        /// <param name="newname"></param>
        /// <returns></returns>
        bool Replace(string oldname, string newname);

        /// <summary>
        /// Adds the given name to this instance.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Add(string name);

        /// <summary>
        /// Adds to this instance the names from the given range.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<string> range);

        /// <summary>
        /// Removes from this instance the given name, provided it is not the only remaining one.
        /// If so, an exception is thrown.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Remove(string name);

        /// <summary>
        /// Clears the contents of this collection except the default one.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}
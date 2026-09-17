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
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IMetadataTag ToInstance();

        /// <summary>
        /// <inheritdoc cref="IMetadataTag.IgnoreCase"/>
        /// </summary>
        bool IgnoreCase { get; }

        /// <summary>
        /// <inheritdoc cref="IMetadataTag.Default"/>
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
        /// <inheritdoc cref="IMetadataTag.ContainsAny(IEnumerable{string})"/>
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

        /// <summary>
        /// Trims the internal data structures of this instance.
        /// </summary>
        void Trim();

        // ------------------------------------------------

        /// <summary>
        /// Replaces the original tag name (alias) by the new given one.
        /// </summary>
        /// <param name="oldname"></param>
        /// <param name="newname"></param>
        /// <returns>True if changes have been made. False otherwise.</returns>
        bool Replace(string oldname, string newname);

        /// <summary>
        /// Adds to this collection the given tag name (alias).
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if changes have been made. False otherwise.</returns>
        bool Add(string name);

        /// <summary>
        /// Adds to this collection the tag names (aliases) from the given range.
        /// </summary>
        /// <param name="range"></param>
        /// <returns>True if changes have been made. False otherwise.</returns>
        bool AddRange(IEnumerable<string> range);

        /// <summary>
        /// Removes from this collection the given tag name (alias).
        /// <br/> An exception is thrown if it is the only remaining one.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>True if changes have been made. False otherwise.</returns>
        bool Remove(string name);

        /// <summary>
        /// Clears this collection, except the default tag name (alias).
        /// </summary>
        /// <returns>True if changes have been made. False otherwise.</returns>
        bool Clear();
    }
}
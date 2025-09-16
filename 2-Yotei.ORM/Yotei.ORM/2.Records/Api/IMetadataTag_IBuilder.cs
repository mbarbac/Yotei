namespace Yotei.ORM.Records;

partial interface IMetadataTag
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IMetadataTag"/> instances.
    /// </summary>
    public interface IBuilder : IEnumerable<string>
    {
        /// <inheritdoc cref="ICloneable.Clone"/>
        IBuilder Clone();

        // ----------------------------------------------------

        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IMetadataTag CreateInstance();

        // ----------------------------------------------------

        /// <inheritdoc cref="IMetadataTag.CaseSensitiveTags"/>
        bool CaseSensitiveTags { get; }

        /// <inheritdoc cref="IMetadataTag.Default"/>
        string Default { get; set; }

        /// <inheritdoc cref="IMetadataTag.Count"/>
        int Count { get; }

        /// <inheritdoc cref="IMetadataTag.Contains(string)"/>
        bool Contains(string name);

        /// <inheritdoc cref="IMetadataTag.Contains(IEnumerable{string})"/>
        bool Contains(IEnumerable<string> range);

        /// <inheritdoc cref="IMetadataTag.ToArray"/>
        string[] ToArray();

        /// <inheritdoc cref="IMetadataTag.ToList"/>
        List<string> ToList();

        /// <inheritdoc cref="IMetadataTag.Trim"/>
        void Trim();

        // ----------------------------------------------------

        /// <summary>
        /// Replaces the given old named by the new given one.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="oldname"></param>
        /// <param name="newname"></param>
        /// <returns></returns>
        bool Replace(string oldname, string newname);

        /// <summary>
        /// Adds the given name to this collection.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Add(string name);

        /// <summary>
        /// Adds the names of the given range to this collection.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<string> range);

        /// <summary>
        /// Removes the given name from this collection.
        /// <br/> If the name was the only one in the collection, an exception is thrown.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Remove(string name);

        /// <summary>
        /// Removes all contents from this collection, except the default one
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}
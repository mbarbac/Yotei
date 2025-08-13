namespace Yotei.ORM.Records;

partial interface IMetadataName
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IMetadataName"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : IEnumerable<string>
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IMetadataName CreateInstance();

        // ----------------------------------------------------

        /// <inheritdoc cref="IMetadataName.CaseSensitiveNames"/>
        bool CaseSensitiveNames { get; }

        /// <inheritdoc cref="IMetadataName.Default"/>
        string Default { get; set; }

        /// <inheritdoc cref="IMetadataName.Count"/>
        int Count { get; }

        /// <inheritdoc cref="IMetadataName.Contains(string)"/>
        bool Contains(string name);

        /// <inheritdoc cref="IMetadataName.Contains(IEnumerable{string})"/>
        bool Contains(IEnumerable<string> range);

        /// <inheritdoc cref="IMetadataName.ToArray"/>
        string[] ToArray();

        /// <inheritdoc cref="IMetadataName.ToList"/>
        List<string> ToList();

        /// <inheritdoc cref="IMetadataName.Trim"/>
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
namespace Yotei.ORM.Records;

partial interface IMetadataTag
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IMetadataTag"/> instances.
    /// <br/> Duplicated tag names are not allowed.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : IEnumerable<string>
    {
        /// <summary>
        /// Returns a new instance based upon the contents in this builder.
        /// </summary>
        /// <returns></returns>
        public IMetadataTag ToInstance();

        // ------------------------------------------------

        /// <summary>
        /// Determines if the tag names in this instance are case sensitive, or not.
        /// </summary>
        bool CaseSensitiveTags { get; }

        /// <summary>
        /// Gets or sets the default tag name of this instance. The setter throws an exception
        /// if that tag name is not part of this collection.
        /// </summary>
        string Default { get; set; }

        /// <summary>
        /// The number of tag names carried by this instance.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Determines if this instance contains the given tag name, or not.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Contains(string name);

        /// <summary>
        /// Determines if this instance contains any of the names in the given range, or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Contains(IEnumerable<string> range);

        /// <summary>
        /// Returns an array with the tag names in this instance.
        /// </summary>
        /// <returns></returns>
        string[] ToArray();

        /// <summary>
        /// Returns a list with the tag names in this instance.
        /// </summary>
        /// <returns></returns>
        List<string> ToList();

        // ------------------------------------------------

        /// <summary>
        /// Replaces the given existing tag name with the new given one. If the original tag
        /// name was not found, returns <c>false</c>.
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
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Remove(string name);

        /// <summary>
        /// Clears all the names in this collection, except the default one.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}
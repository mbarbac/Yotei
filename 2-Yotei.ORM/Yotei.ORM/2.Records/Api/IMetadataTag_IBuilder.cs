namespace Yotei.ORM.Records;

partial interface IMetadataTag
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IMetadataTag"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : IEnumerable<string>
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IMetadataTag CreateInstance();

        /// <summary>
        /// Determines if the tag names in this instance are case sensitive or not.
        /// </summary>
        bool CaseSensitiveTags { get; }

        /// <summary>
        /// The default tag name among the ones carried by this instance.
        /// </summary>
        string Default { get; }

        /// <summary>
        /// Gets the number of tag names in this collection.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Determines if this instance contains the given tag name, or not.
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        bool Contains(string tagname);

        /// <summary>
        /// Determines if this instance contains any tag name in the given range, or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Contains(IEnumerable<string> range);

        /// <summary>
        /// Obtains an array with the tag names in this instance.
        /// </summary>
        /// <returns></returns>
        string[] ToArray();

        /// <summary>
        /// Obtains a list with the tag names in this instance.
        /// </summary>
        /// <returns></returns>
        List<string> ToList();

        /// <summary>
        /// Trims the excess of capacity in this collection.
        /// </summary>
        void Trim();

        // ----------------------------------------------------

        /// <summary>
        /// Replaces the specified original tag name with the new given one.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="oldtagname"></param>
        /// <param name="newtagname"></param>
        /// <returns></returns>
        bool Replace(string oldtagname, string newtagname);

        /// <summary>
        /// Adds the given name to this instance.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        bool Add(string tagname);

        /// <summary>
        /// Adds the names from the given range to this instance.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool AddRange(IEnumerable<string> range);

        /// <summary>
        /// Removes from this instance the given name, if possible.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        bool Remove(string tagname);

        /// <summary>
        /// Clears this instance, keeping a default name.
        /// <br/> Returns whether changes have been made or not.
        /// </summary>
        /// <returns></returns>
        bool Clear();
    }
}
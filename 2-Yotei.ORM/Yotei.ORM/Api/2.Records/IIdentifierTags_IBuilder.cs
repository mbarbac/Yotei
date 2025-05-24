using IHost = Yotei.ORM.Records.IIdentifierTags;
using IItem = Yotei.ORM.Records.IMetadataTag;

namespace Yotei.ORM;

partial interface IIdentifierTags
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : ICoreList<IItem>
    {
        /// <summary>
        /// Returns a new host instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IHost ToInstance();

        /// <summary>
        /// Determines if the tag names in this instance are case sensitive, or not.
        /// </summary>
        bool CaseSensitiveTags { get; }

        /// <summary>
        /// The collection of all tag names carried by this instance.
        /// </summary>
        IEnumerable<string> Names { get; }

        // ------------------------------------------------

        /// <summary>
        /// Determines if this collection contains any element that carries the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Contains(string name);

        /// <summary>
        /// Determines if this collection contains any element that carries any of the names from
        /// the given range.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Contains(IEnumerable<string> range);

        /// <summary>
        /// Gets the index of the element in this collection that carries the given name, or -1
        /// if not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int IndexOf(string name);

        /// <summary>
        /// Gets the index of the first element in this collection that carries any of the names
        /// from the given range.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        int IndexOf(IEnumerable<string> range);

        /// <summary>
        /// Gets the index of the last element in this collection that carries any of the names
        /// from the given range.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        int LastIndexOf(IEnumerable<string> range);

        /// <summary>
        /// Gets the indexes of the elements in this collection that carries any of the names from
        /// the given range.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        List<int> IndexesOf(IEnumerable<string> range);

        /// <summary>
        /// Removes from this collection the element that carries the given name, if any.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>The number of changes made.</returns>
        int Remove(string name);

        /// <summary>
        /// Removes from this collection all the elements that contains any of the names from
        /// the given range.
        /// </summary>
        /// <param name="range"></param>
        /// <returns>The number of changes made.</returns>
        int Remove(IEnumerable<string> range);
    }
}
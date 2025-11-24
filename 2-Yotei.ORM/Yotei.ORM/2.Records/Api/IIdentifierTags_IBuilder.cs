using IHost = Yotei.ORM.Records.IIdentifierTags;
using IItem = Yotei.ORM.Records.IMetadataTag;

namespace Yotei.ORM.Records;

partial interface IIdentifierTags
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    [Cloneable<IBuilder>]
    public partial interface IBuilder : ICoreList<IItem>
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IHost CreateInstance();

        /// <summary>
        /// Whether the tag names in this instance are case sensitive, or not.
        /// </summary>
        bool CaseSensitiveTags { get; }

        /// <summary>
        /// The flattened collection of tag names carried by this instance.
        /// </summary>
        IEnumerable<string> TagNames { get; }

        // ----------------------------------------------------

        /// <summary>
        /// Determines if this collection contains the given tag name, or not.
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        bool Contains(string tagname);

        /// <summary>
        /// Determines if this collection contains any tag name in the given range, or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        bool Contains(IEnumerable<string> range);

        /// <summary>
        /// Gets the index of the element in this collection that carries the given tag name, or
        /// -1 if any.
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        int IndexOf(string tagname);

        /// <summary>
        /// Gets the index of the first element in this collection that carries any of the tag
        /// names in given range, or -1 if any.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        int IndexOf(IEnumerable<string> range);

        /// <summary>
        /// Returns the collection of tag names the given one belongs to, or null if any.
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        IItem? Find(string tagname);

        /// <summary>
        /// Returns the collection of tag names the first match on the given onea belongs to, or
        /// null if any.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        IItem? Find(IEnumerable<string> range);

        // ----------------------------------------------------

        /// <summary>
        /// Removes from this instance the element that carried the given tag name, if any,
        /// <br/> Returns whether changes has been made, or not.
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        bool Remove(string tagname);
    }
}
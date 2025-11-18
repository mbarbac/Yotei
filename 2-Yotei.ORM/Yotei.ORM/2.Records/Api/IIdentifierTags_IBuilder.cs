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
        /// <param name="name"></param>
        /// <returns></returns>
        bool Contains(string name);

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
        /// <param name="name"></param>
        /// <returns></returns>
        int IndexOf(string name);

        /// <summary>
        /// Gets the index of the first element in this collection that carries any of the tag
        /// names in given range, or -1 if any.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        int IndexOf(IEnumerable<string> range);

        // ----------------------------------------------------

        /// <summary>
        /// Removes from this instance the element that carried the given tag name, if any,
        /// <br/> Returns whether changes has been made, or not.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Remove(string name);
    }
}
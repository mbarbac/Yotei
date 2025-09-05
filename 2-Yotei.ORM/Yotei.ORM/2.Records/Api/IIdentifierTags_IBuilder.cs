using IHost = Yotei.ORM.Records.IIdentifierTags;
using IItem = Yotei.ORM.Records.IMetadataTag;

namespace Yotei.ORM.Records;

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
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IHost CreateInstance();

        /// <inheritdoc cref="IHost.CaseSensitiveTags"/>
        bool CaseSensitiveTags { get; }

        /// <inheritdoc cref="IHost.TagNames"/>
        IEnumerable<string> TagNames { get; }

        // ----------------------------------------------------

        /// <inheritdoc cref="IHost.Contains(string)"/>
        bool Contains(string name);

        /// <inheritdoc cref="IHost.Contains(IEnumerable{string})"/>
        bool Contains(IEnumerable<string> range);

        /// <inheritdoc cref="IHost.IndexOf(string)"/>
        int IndexOf(string name);

        /// <inheritdoc cref="IHost.IndexOf(IEnumerable{string})"/>
        int IndexOf(IEnumerable<string> range);

        /// <inheritdoc cref="IHost.IndexesOf(IEnumerable{string})"/>
        List<int> IndexesOf(IEnumerable<string> range);

        // ------------------------------------------------

        /// <summary>
        /// Removes the element in this collection that carries the given name, if any.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        int Remove(string name);

        /// <summary>
        /// Removes all the elements in this collection that carry any of the names from the given
        /// range.
        /// <br/> Returns whether changes have been made, or not.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        int Remove(IEnumerable<string> range);
    }
}
using TKey = Yotei.ORM.IIdentifier;
using IItem = Yotei.ORM.Records.ISchemaEntry;
using IHost = Yotei.ORM.Records.ISchema;

namespace Yotei.ORM.Records;

partial interface ISchema
{
    // ====================================================
    /// <summary>
    /// Represents a builder of <see cref="IHost"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : ICoreList<TKey, IItem>
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IHost ToInstance();

        /// <summary>
        /// <inheritdoc cref="IHost.Engine"/>
        /// </summary>
        IEngine Engine { get; }

        // ----------------------------------------------------

        /// <summary>
        /// <inheritdoc cref="IHost.Contains(string)"/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        bool Contains(string identifier);

        /// <summary>
        /// <inheritdoc cref="IHost.IndexOf(string)"/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        int IndexOf(string identifier);

        /// <summary>
        /// <inheritdoc cref="IHost.LastIndexOf(string)"/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        int LastIndexOf(string identifier);

        /// <summary>
        /// <inheritdoc cref="IHost.IndexesOf(string)"/>
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        List<int> IndexesOf(string identifier);

        /// <summary>
        /// <inheritdoc cref="IHost.Match(string?)"/>
        /// </summary>
        /// <param name="specs"></param>
        /// <returns></returns>
        List<int> Match(string? specs);

        /// <summary>
        /// <inheritdoc cref="IHost.Match(string?, out IItem?)"/>
        /// </summary>
        /// <param name="specs"></param>
        /// <param name="unique"></param>
        /// <returns></returns>
        List<int> Match(string? specs, out IItem? unique);

        /// <summary>
        /// Removes from this instance the first entry with the given identifier, if any.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        bool Remove(string identifier);

        /// <summary>
        /// Removes from this instance the last entry with the given identifier, if any.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        bool RemoveLast(string identifier);

        /// <summary>
        /// Removes from this instance all the entries with the given identifier.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        bool RemoveAll(string identifier);
    }
}
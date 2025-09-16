using IHost = Yotei.ORM.Records.ISchema;
using IItem = Yotei.ORM.Records.ISchemaEntry;
using TKey = Yotei.ORM.IIdentifier;

namespace Yotei.ORM.Records;

partial interface ISchema
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    public interface IBuilder : ICoreList<TKey, IItem>
    {
        /// <inheritdoc cref="ICloneable.Clone"/>
        new IBuilder Clone();

        // ----------------------------------------------------

        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IHost CreateInstance();

        /// <inheritdoc cref="IHost.Engine"/>
        IEngine Engine { get; }

        // ----------------------------------------------------

        /// <inheritdoc cref="IHost.Contains(string)"/>
        bool Contains(string identifier);

        /// <inheritdoc cref="IHost.IndexOf(string)"/>
        int IndexOf(string identifier);

        /// <inheritdoc cref="IHost.Match(string?)"/>
        List<int> Match(string? specs);

        /// <inheritdoc cref="IHost.Match(string?, out IItem?)"/>
        List<int> Match(string? specs, out IItem? unique);

        // ----------------------------------------------------

        /// <summary>
        /// Removes from this collection the entry with the given identifier, if any.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        bool Remove(string identifier);
    }
}
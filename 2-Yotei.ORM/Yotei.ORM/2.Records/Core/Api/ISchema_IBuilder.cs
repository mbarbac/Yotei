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
    [Cloneable]
    public partial interface IBuilder : ICoreList<TKey, IItem>
    {/// <summary>
     /// Returns a new instance based upon the contents of this builder.
     /// </summary>
     /// <returns></returns>
        IHost CreateInstance();

        /// <summary>
        /// The engine this instance is associated with.
        /// </summary>
        IEngine Engine { get; }

        // ------------------------------------------------

        /// <summary>
        /// Determines if this collection contains an element with the given identifier.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        bool Contains(string identifier);

        /// <summary>
        /// Gets the index of the element in this collection with the given identifier, or -1
        /// if not found.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        int IndexOf(string identifier);

        /// <summary>
        /// Returns the indexes of the elements (entries) in this collection that match the given
        /// specifications.
        /// <br/> Comparison is performed comparing the respective parts, from right to left, where
        /// an empty or null specification is taken as an implicit match.
        /// </summary>
        /// <param name="specs"></param>
        /// <returns></returns>
        List<int> Match(string? specs);

        /// <summary>
        /// Returns the indexes of the elements (entries) in this collection that match the given
        /// specifications. If a unique match is detected, then it is placed in the out argument.
        /// <br/> Comparison is performed comparing the respective parts, from right to left, where
        /// an empty or null specification is taken as an implicit match.
        /// </summary>
        /// <param name="specs"></param>
        /// <param name="unique"></param>
        /// <returns></returns>
        List<int> Match(string? specs, out IItem? unique);

        // ------------------------------------------------

        /// <summary>
        /// Removes from this collection the element with the given identifier.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        bool Remove(string identifier);
    }
}
using IHost = Yotei.ORM.IIdentifierChain;
using IItem = Yotei.ORM.IIdentifierPart;
using TKey = string;

namespace Yotei.ORM;

partial interface IIdentifierChain
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : ICoreList<TKey?, IItem>
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IHost CreateInstance();

        /// <inheritdoc cref="IIdentifier.Engine"/>
        IEngine Engine { get; }

        /// <inheritdoc cref="IIdentifier.Value"/>
        string? Value { get; }

        // ------------------------------------------------

        /// <summary>
        /// Replaces the element at the given index with the ones obtained from the given value.
        /// Returns the number of changes made.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        int Replace(int index, string? value);

        /// <summary>
        /// Adds to this instance the elements obtained from the given value.
        /// Returns the number of changes made.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        int Add(string? value);

        /// <summary>
        /// Adds to this instance the elements obtained from the given range of values.
        /// Returns the number of changes made.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        int AddRange(IEnumerable<string?> range);

        /// <summary>
        /// Inserts into this instance the elements obtained from the given value, starting at the
        /// given index. Returns the number of changes made.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        int Insert(int index, string? value);

        /// <summary>
        /// Inserts into this instance the elements obtained from the given range of values, starting
        /// at the given index. Returns the number of changes made.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        int InsertRange(int index, IEnumerable<string?> range);
    }
}
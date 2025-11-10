using IHost = Yotei.ORM.IIdentifierChain;
using IItem = Yotei.ORM.IIdentifierUnit;
using TKey = string;

namespace Yotei.ORM;

partial interface IIdentifierChain
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    [Cloneable<IBuilder>]
    public partial interface IBuilder : ICoreList<TKey?, IItem>
    {
        /// <summary>
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IHost CreateInstance();

        /// <summary>
        /// The engine this instance is associated with.
        /// </summary>
        IEngine Engine { get; }

        /// <summary>
        /// <inheritdoc cref="IIdentifier.Value"/>
        /// </summary>
        string? Value { get; }

        // ------------------------------------------------

        /// <summary>
        /// Replaces the element at the given index with the ones obtained from the given value.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        int Replace(int index, string? value);

        /// <summary>
        /// Adds the elements obtained from the given value to this collection.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        int Add(string? value);

        /// <summary>
        /// Adds the elements obtained from the given range of values to this collection.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        int AddRange(IEnumerable<string?> range);

        /// <summary>
        /// Inserts the elements obtained from the given value into this collection, starting at
        /// the given index.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        int Insert(int index, string? value);

        /// <summary>
        /// Inserts the elements obtained from the given range of values into this collection,
        /// starting at the given index.
        /// <br/> Returns the number of changes made.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        int InsertRange(int index, IEnumerable<string?> range);
    }
}
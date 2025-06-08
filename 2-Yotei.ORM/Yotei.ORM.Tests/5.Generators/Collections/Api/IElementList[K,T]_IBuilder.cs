using IHost = Yotei.ORM.Tests.Generators.IElementList_KT;
using IItem = Yotei.ORM.Tests.Generators.IElement;
using TKey = string;

namespace Yotei.ORM.Tests.Generators;

partial interface IElementList_KT
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> intances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : ICoreList<TKey, IItem>
    {
        /// <summary>
        /// Gets a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IHost ToInstance();

        /// <summary>
        /// The comparison mode to use to compare the names of the elements in this collection.
        /// </summary>
        StringComparison Comparison { get; }
    }
}
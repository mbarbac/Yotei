using IHost = Yotei.ORM.Tests.Generators.IElementList_T;
using IItem = Yotei.ORM.Tests.Generators.IElement;

namespace Yotei.ORM.Tests.Generators;

partial interface IElementList_T
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> intances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : ICoreList<IItem>
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
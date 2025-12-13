using IHost = Yotei.ORM.Generators.Invariant.Tests.IElementList_T;
using IItem = Yotei.ORM.Generators.Invariant.Tests.IElement;

namespace Yotei.ORM.Generators.Invariant.Tests;

partial interface IElementList_T
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : ICoreList<IItem>
    {
        /// <summary>
        /// Creates a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IHost ToInstance();

        /// <summary>
        /// <inheritdoc cref="IHost.Engine"/>
        /// </summary>
        IEngine Engine { get; }
    }
}

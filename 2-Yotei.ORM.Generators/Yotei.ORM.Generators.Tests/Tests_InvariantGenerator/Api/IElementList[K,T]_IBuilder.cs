using IHost = Yotei.ORM.Generators.Invariant.Tests.IElementList_KT;
using IItem = Yotei.ORM.Generators.Invariant.Tests.IElement;
using TKey = string;

namespace Yotei.ORM.Generators.Invariant.Tests;

partial interface IElementList_KT
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    [Cloneable<IBuilder>]
    public partial interface IBuilder : ICoreList<TKey, IItem>
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
    }
}
using IHost = Yotei.ORM.Generators.Tests.IElementList_T;
using IItem = Yotei.ORM.Generators.Tests.IElement;

namespace Yotei.ORM.Generators.Tests;

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
        /// Returns a new instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IHost ToInstance();

        /// <summary>
        /// <inheritdoc cref="IHost.IgnoreCase"/>
        /// </summary>
        bool IgnoreCase { get; }

        /// <summary>
        /// <inheritdoc cref="IHost.AcceptDuplicates"/>
        /// </summary>
        bool AcceptDuplicates{ get; }
    }
}
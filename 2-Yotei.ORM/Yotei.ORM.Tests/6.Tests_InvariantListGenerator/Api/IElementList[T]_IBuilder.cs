using IHost = Yotei.ORM.Tests.InvariantListGenerator.IElementListT;
using IItem = Yotei.ORM.Tests.InvariantListGenerator.IElement;

namespace Yotei.ORM.Tests.InvariantListGenerator;

partial interface IElementListT
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
        IHost CreateInstance();

        /// <inheritdoc cref="IHost.CaseSensitive"/>
        bool CaseSensitive { get; }
    }
}
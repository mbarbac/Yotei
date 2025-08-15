using IHost = Yotei.ORM.Tests.Tools.Generators.Collections.IElementList_T;
using IItem = Yotei.ORM.Tests.Tools.Generators.Collections.IElement;

namespace Yotei.ORM.Tests.Tools.Generators.Collections;

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
        IHost CreateInstance();

        /// <summary>
        /// Whether the names of the elements in this collection are case sensitive or not.
        /// </summary>
        bool CaseSensitive { get; }
    }
}
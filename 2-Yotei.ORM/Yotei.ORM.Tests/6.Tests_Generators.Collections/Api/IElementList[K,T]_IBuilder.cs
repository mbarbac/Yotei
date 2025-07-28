using IHost = Yotei.ORM.Tests.Tools.Generators.Collections.IElementList_KT;
using IItem = Yotei.ORM.Tests.Tools.Generators.Collections.IElement;
using TKey = string;

namespace Yotei.ORM.Tests.Tools.Generators.Collections;

partial interface IElementList_KT
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : ICoreList<TKey, IItem>
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

using IHost = Yotei.ORM.Tests.Generators.IElementList_KT;
using IItem = Yotei.ORM.Tests.Generators.IElement;
using TKey = string;

namespace Yotei.ORM.Tests.Generators;

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
        /// Whether the names of the elements in this collection are case sensitive or not. If any
        /// element to compare is not a named one, then this property is ignored.
        /// </summary>
        bool CaseSensitive { get; }

        /// <summary>
        /// The name of this element.
        /// </summary>
        string Name { get; }
    }
}
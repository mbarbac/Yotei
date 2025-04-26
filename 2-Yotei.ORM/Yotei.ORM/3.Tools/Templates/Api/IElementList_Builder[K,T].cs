using IHost = Yotei.ORM.Tools.Code.Templates.IElementList_KT;
using IItem = Yotei.ORM.Tools.Code.Templates.IElement;
using TKey = string;

namespace Yotei.ORM.Tools.Code.Templates;

public partial interface IElementList_KT
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : ICoreList<TKey, IItem>
    {
        /// <summary>
        /// Returns a new host instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IHost ToInstance();
    }
}
using IHost = Yotei.ORM.Tools.Code.Templates.IElementList_T;
using IItem = Yotei.ORM.Tools.Code.Templates.IElement;

namespace Yotei.ORM.Tools.Code.Templates;

public partial interface IElementList_T
{
    // ====================================================
    /// <summary>
    /// Represents a builder for <see cref="IHost"/> instances.
    /// </summary>
    [Cloneable]
    public partial interface IBuilder : ICoreList<IItem>
    {
        /// <summary>
        /// Returns a new host instance based upon the contents of this builder.
        /// </summary>
        /// <returns></returns>
        IHost ToInstance();
    }
}
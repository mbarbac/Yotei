using IHost = Yotei.ORM.Tools.Code.Templates.IElementList_KT;
using IItem = Yotei.ORM.Tools.Code.Templates.IElement;
using TKey = string;

namespace Yotei.ORM.Tools.Code.Templates;

// ========================================================
/// <summary>
/// Represents a collection of elements.
/// </summary>
[IInvariantList<TKey, IItem>]
public partial interface IElementList_KT : IEquatable<IHost>
{
    /// <inheritdoc cref="IInvariantList{K, T}.GetBuilder"/>
    new IBuilder GetBuilder();
}
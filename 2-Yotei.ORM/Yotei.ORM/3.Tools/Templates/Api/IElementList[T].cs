using IHost = Yotei.ORM.Tools.Code.Templates.IElementList_T;
using IItem = Yotei.ORM.Tools.Code.Templates.IElement;

namespace Yotei.ORM.Tools.Code.Templates;

// ========================================================
/// <summary>
/// Represents a collection of elements.
/// </summary>
[IInvariantList<IItem>]
public partial interface IElementList_T : IEquatable<IHost>
{
    /// <inheritdoc cref="IInvariantList{T}.GetBuilder"/>
    new IBuilder GetBuilder();
}
using IHost = Yotei.ORM.Generators.Invariant.Tests.IElementList_T;
using IItem = Yotei.ORM.Generators.Invariant.Tests.IElement;

namespace Yotei.ORM.Generators.Invariant.Tests;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements.
/// </summary>
[IInvariantList<IItem>(ReturnType = typeof(IHost))]
public partial interface IElementList_T : IItem
{
    /// <summary>
    /// <inheritdoc cref="IInvariantList{T}.ToBuilder"/>
    /// </summary>
    /// <returns></returns>
    new IBuilder ToBuilder();

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }
}
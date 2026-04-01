using IItem = Yotei.ORM.Generators.InvariantGenerator.Tests.IElement;
using IHost = Yotei.ORM.Generators.InvariantGenerator.Tests.IElementList_T;

namespace Yotei.ORM.Generators.InvariantGenerator.Tests;

// ========================================================
/// <summary>
/// <inheritdoc cref="IInvariantList{T}"/>
/// </summary>
[IInvariantList<IItem>(ReturnType = typeof(IHost))]
public partial interface IElementList_T : IItem
{
    /// <summary>
    /// <inheritdoc cref="InvariantList{T}.ToBuilder"/>
    /// </summary>
    /// <returns></returns>
    new IBuilder ToBuilder();

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }
}
using IHost = Yotei.ORM.Generators.Invariant.Tests.IElementList_T;
using IItem = Yotei.ORM.Generators.Invariant.Tests.IElement;

namespace Yotei.ORM.Generators.Invariant.Tests;

// ========================================================
/// <summary>
/// <inheritdoc cref="IInvariantList{T}"/>
/// </summary>
[IInvariantList<IItem>(ReturnType = typeof(IHost))]
public partial interface IElementList_T : IItem
{
    /// <summary>
    /// Returns a new builder base upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }
}
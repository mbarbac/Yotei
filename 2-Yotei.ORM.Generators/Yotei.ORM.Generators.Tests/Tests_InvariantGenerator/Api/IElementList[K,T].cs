using IHost = Yotei.ORM.Generators.Invariant.Tests.IElementList_KT;
using IItem = Yotei.ORM.Generators.Invariant.Tests.IElement;
using TKey = string;

namespace Yotei.ORM.Generators.Invariant.Tests;

// ========================================================
/// <summary>
/// <inheritdoc cref="IInvariantList{K, T}"/>
/// </summary>
[IInvariantList<TKey, IItem>(ReturnType = typeof(IHost))]
public partial interface IElementList_KT : IItem
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
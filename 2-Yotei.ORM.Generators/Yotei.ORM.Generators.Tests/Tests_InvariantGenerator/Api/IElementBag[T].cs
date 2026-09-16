using IItem = Yotei.ORM.InvariantGenerator.Tests.IElement;
using IHost = Yotei.ORM.InvariantGenerator.Tests.IElementBag_T;

namespace Yotei.ORM.InvariantGenerator.Tests;

// ========================================================
/// <summary>
/// Represents a bag of elements.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[IInvariantBag<IItem>(ReturnType = typeof(IHost))]
public partial interface IElementBag_T : IItem
{
    /// <summary>
    /// <inheritdoc cref="IInvariantBag{T}.ToBuilder"/>
    /// </summary>
    /// <returns></returns>
    new IBuilder ToBuilder();

    /// <summary>
    /// Determines how to compare elements' names.
    /// </summary>
    bool IgnoreCase { get; init; }

    /// <summary>
    /// For DEBUG purposes only.
    /// </summary>
    bool AcceptDuplicates { get; init; }

    /// <summary>
    /// For DEBUG purposes only.
    /// </summary>
    bool FlattenElements { get; init; }
}
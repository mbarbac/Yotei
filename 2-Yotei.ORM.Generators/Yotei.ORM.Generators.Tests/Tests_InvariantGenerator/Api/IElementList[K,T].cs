using TKey = string;
using IItem = Yotei.ORM.InvariantGenerator.Tests.IElement;
using IHost = Yotei.ORM.InvariantGenerator.Tests.IElementList_KT;

namespace Yotei.ORM.InvariantGenerator.Tests;

// ========================================================
/// <summary>
/// Represents a list of elements identified by their names.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[IInvariantList<TKey, IItem>(ReturnType = typeof(IHost))]
public partial interface IElementList_KT : IItem
{
    /// <summary>
    /// <inheritdoc cref="IInvariantList{T}.ToBuilder"/>
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
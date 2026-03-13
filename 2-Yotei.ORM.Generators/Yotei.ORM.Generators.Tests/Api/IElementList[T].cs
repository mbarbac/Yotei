using IHost = Yotei.ORM.Generators.Tests.IElementList_T;
using IItem = Yotei.ORM.Generators.Tests.IElement;

namespace Yotei.ORM.Generators.Tests;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[IInvariantList<IItem>(ReturnType = typeof(IHost))]
public partial interface IElementList_T : IItem, IInvariantList<IItem>
{
    /// <summary>
    /// <inheritdoc cref="IIvar"/>
    /// </summary>
    /// <returns></returns>
    new IBuilder ToBuilder();

    /// <summary>
    /// Determines how to compare <see cref="IItem"/> instances when they are named ones.
    /// </summary>
    bool IgnoreCase { get; }

    /// <summary>
    /// Determines if this collection accepts duplicates, or not.
    /// </summary>
    bool AcceptDuplicates { get; }
}
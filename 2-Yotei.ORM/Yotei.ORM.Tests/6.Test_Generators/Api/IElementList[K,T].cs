using IHost = Yotei.ORM.Tests.Generators.IElementList_KT;
using IItem = Yotei.ORM.Tests.Generators.IElement;
using TKey = string;

namespace Yotei.ORM.Tests.Generators;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements identified by their respective keys.
/// <para>Instances of this type are intended to be immutable ones.</para>
/// </summary>
[IInvariantList<TKey, IItem>]
public partial interface IElementList_KT : IItem
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    /// <summary>
    /// Whether the names of the elements in this collection are case sensitive or not. If any
    /// element to compare is not a named one, then this property is ignored.
    /// </summary>
    bool CaseSensitive { get; }
}
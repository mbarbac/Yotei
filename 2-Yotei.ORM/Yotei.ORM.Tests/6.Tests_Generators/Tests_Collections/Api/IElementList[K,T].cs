using IHost = Yotei.ORM.Tests.Tools.Generators.Collections.IElementList_KT;
using IItem = Yotei.ORM.Tests.Tools.Generators.Collections.IElement;
using TKey = string;

namespace Yotei.ORM.Tests.Tools.Generators.Collections;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements identified by their respective keys.
/// <br/> Instances of this type are intended to be immutable ones.
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
    /// Whether the names of the elements in this collection are case sensitive or not.
    /// </summary>
    bool CaseSensitive { get; }
}
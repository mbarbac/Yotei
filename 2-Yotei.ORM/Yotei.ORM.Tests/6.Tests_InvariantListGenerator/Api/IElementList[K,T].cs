using IHost = Yotei.ORM.Tests.InvariantListGenerator.IElementListKT;
using IItem = Yotei.ORM.Tests.InvariantListGenerator.IElement;
using TKey = string;

namespace Yotei.ORM.Tests.InvariantListGenerator;

// ========================================================
/// <summary>
/// Represents an immutable list-alike collection of elements identified by their keys.
/// <br/> Instances of this type are intended to be immutable ones.
/// </summary>
[IInvariantList<TKey, IItem>]
public partial interface IElementListKT : IItem
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    IEngine Engine { get; }
}
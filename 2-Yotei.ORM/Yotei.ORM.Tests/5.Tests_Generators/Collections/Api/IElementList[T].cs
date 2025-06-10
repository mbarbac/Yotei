using IHost = Yotei.ORM.Tests.Generators.IElementList_T;
using IItem = Yotei.ORM.Tests.Generators.IElement;

namespace Yotei.ORM.Tests.Generators;

// ========================================================
/// <summary>
/// Represents a list-alike collection of elements..
/// <para>Instances of this class are intended to be immutable ones.</para>
/// </summary>
[IInvariantList<IItem>]
public partial interface IElementList_T : IEquatable<IHost>
{
    /// <summary>
    /// Returns a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder CreateBuilder();

    /// <summary>
    /// Whether the names of the elements in this instance are case sensitive or not.
    /// </summary>
    bool CaseSensitive { get; }
}
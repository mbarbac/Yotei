using IHost = Yotei.ORM.Tests.Generators.IElementList_KT;
using IItem = Yotei.ORM.Tests.Generators.IElement;
using TKey = string;

namespace Yotei.ORM.Tests.Generators;

// ========================================================
/// <summary>
/// Represents an immutable and customizable list-alike collection of elements identified by
/// their respective keys.
/// <para>Instances of this class are intended to be immutable ones.</para>
/// </summary>
[IInvariantList<TKey, IItem>]
public partial interface IElementList_KT : IEquatable<IHost>
{
    /// <summary>
    /// Gets a new builder based upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    IBuilder GetBuilder();

    /// <summary>
    /// The comparison mode to use to compare the names of the elements in this collection.
    /// </summary>
    StringComparison Comparison { get; }
}
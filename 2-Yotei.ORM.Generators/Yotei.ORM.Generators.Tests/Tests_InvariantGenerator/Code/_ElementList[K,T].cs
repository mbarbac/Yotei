/*using TKey = string;
using IItem = Yotei.ORM.InvariantGenerator.Tests.IElement;
using IHost = Yotei.ORM.InvariantGenerator.Tests.IElementList_KT;
using THost = Yotei.ORM.InvariantGenerator.Tests.ElementList_KT;

namespace Yotei.ORM.InvariantGenerator.Tests;

// ========================================================
/// <summary>
/// <inheritdoc cref="IHost"/>
/// </summary>
[InvariantList<TKey, IItem>(ReturnType = typeof(IHost))]
[DebuggerDisplay("{ToDebugString(3)}")]
public partial class ElementList_KT : IHost
{
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="ignoreCase"></param>
    public ElementList_KT(bool ignoreCase) => throw null;

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="ignoreCase"></param>
    /// <param name="range"></param>
    public ElementList_KT(
        bool ignoreCase, IEnumerable<IItem> range) => throw null;

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected ElementList_KT(THost other) => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    protected override Builder CreateItems() => new Builder(IgnoreCase);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override IHost.IBuilder ToBuilder() => throw null;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IgnoreCase
    {
        get => throw null;
    }
}*/
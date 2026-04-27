#pragma warning disable CS0436, IDE0290

using IItem = Yotei.ORM.Generators.InvariantGenerator.Tests.IElement;
using IHost = Yotei.ORM.Generators.InvariantGenerator.Tests.IElementList_T;
using THost = Yotei.ORM.Generators.InvariantGenerator.Tests.ElementList_T;

namespace Yotei.ORM.Generators.InvariantGenerator.Tests;

// ========================================================
/// <summary>
/// <inheritdoc cref="IHost"/>
/// </summary>
[DebuggerDisplay("{ToDebugString(3)}")]
[InvariantList<IItem>(ReturnType = typeof(IHost))]
public partial class ElementList_T : IHost
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override IHost.IBuilder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public ElementList_T(IEngine engine) => Items = new Builder(engine);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public ElementList_T(
        IEngine engine, IEnumerable<IItem> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected ElementList_T(THost source)
        : this(source.ThrowWhenNull().Engine) => Items.AddRange(source);

    // ------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override IHost.IBuilder ToBuilder() => (IHost.IBuilder)base.ToBuilder();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine => Items.Engine;

    /// <summary>
    /// Determines if this instance allow duplicated elements, or not.
    /// </summary>
    [With(ReturnType = typeof(IHost))]
    public bool AllowDuplicates
    {
        get => ((Builder)Items).AllowDuplicates;
        init => ((Builder)Items).AllowDuplicates = value;
    }
}
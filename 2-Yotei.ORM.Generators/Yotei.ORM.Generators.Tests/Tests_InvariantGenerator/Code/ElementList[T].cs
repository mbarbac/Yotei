#pragma warning disable CS0436, IDE0290

using IItem = Yotei.ORM.Generators.InvariantGenerator.Tests.IElement;
using IHost = Yotei.ORM.Generators.InvariantGenerator.Tests.IElementList_T;
using THost = Yotei.ORM.Generators.InvariantGenerator.Tests.ElementList_T;
using Xunit.v3;

namespace Yotei.ORM.Generators.InvariantGenerator.Tests;

// ========================================================
/// <summary>
/// <inheritdoc cref="IHost"/>
/// </summary>
[DebuggerDisplay("{ToDebugString(3)}")]
[InheritsWith(ReturnType = typeof(IHost))]
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
    protected ElementList_T(THost source) : this(source.ThrowWhenNull().Engine)
    {
        AllowDuplicates = source.AllowDuplicates;
        Items.AddRange(source);
    }

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
    /// <inheritdoc/>
    /// </summary>
    public bool? AllowDuplicates
    {
        get => ((Builder)Items).AllowDuplicates;
        init => ((Builder)Items).AllowDuplicates = value;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IItem? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not IHost valid) return false;

        if (!Engine.EqualsEx(valid.Engine)) return false;
        if (AllowDuplicates != valid.AllowDuplicates) return false;

        if (Count != valid.Count) return false;
        for (int i = 0; i < Count; i++)
        {
            var source = Items[i];
            var target = valid[i];
            var same = source is NamedElement snamed && target is NamedElement tnamed
                ? snamed.Equals(tnamed, Engine.IgnoreCase)
                : source.EqualsEx(target);

            if (!same) return false;
        }

        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj) => Equals(obj as IItem);

    /// <summary>
    /// Equality semantics.
    /// </summary>
    public static bool operator ==(THost? host, IItem? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    /// <summary>
    /// Inequality semantics.
    /// </summary>
    public static bool operator !=(THost? host, IItem? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = Engine.GetHashCode();
        code = HashCode.Combine(code, AllowDuplicates);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }
}
using IItem = Yotei.ORM.Generators.InvariantGenerator.Tests.IElement;
using IHost = Yotei.ORM.Generators.InvariantGenerator.Tests.IElementList_T;
using THost = Yotei.ORM.Generators.InvariantGenerator.Tests.ElementList_T;

namespace Yotei.ORM.Generators.InvariantGenerator.Tests;

// ========================================================
/// <summary>
/// <inheritdoc cref="IHost"/>
/// </summary>
[DebuggerDisplay("{ToDebugString(4)}")]
[InvariantList<IItem>(ReturnType = typeof(IHost))]
public partial class ElementList_T : IHost
{
    protected override Builder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    [SuppressMessage("", "IDE0290")]
    public ElementList_T(IEngine engine) => Items = new(engine);

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
    /// <param name="other"></param>
    protected ElementList_T(THost other) : this(other.Engine) => Items.Add(other);

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="InvariantList{T}.ToBuilder"/>
    /// </summary>
    /// <returns></returns>
    public override IHost.IBuilder ToBuilder() => Items.Clone();

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine => Items.Engine;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IItem? other)
    {
        if (ReferenceEquals(this,other)) return true;
        if (other is null) return false;
        if (other is not IHost valid) return false;

        if (!Engine.Equals(valid.Engine)) return false;
        if (Count != valid.Count) return false;

        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var temp = valid[i];
            var same = item is NamedElement xitem && temp is NamedElement xtemp
                ? xitem.Equals(xtemp, ignoreCase: !Engine.CaseSensitiveNames)
                : item.Equals(temp);

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

    public static bool operator ==(THost? host, IHost? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(THost? host, IHost? item) => !(host == item);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        var code = Engine.GetHashCode();
        code = HashCode.Combine(code, Count);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }
}
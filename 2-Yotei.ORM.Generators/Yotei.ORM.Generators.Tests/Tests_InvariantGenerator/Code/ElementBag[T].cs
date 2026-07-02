using IItem = Yotei.ORM.InvariantGenerator.Tests.IElement;
using IHost = Yotei.ORM.InvariantGenerator.Tests.IElementBag_T;
using THost = Yotei.ORM.InvariantGenerator.Tests.ElementBag_T;

namespace Yotei.ORM.InvariantGenerator.Tests;

// ========================================================
/// <summary>
/// <inheritdoc cref="InvariantBag{T}"/>
/// </summary>
[DebuggerDisplay("{ToDebugString(3)}")]
[InvariantBag<IItem>(ReturnType = typeof(IHost))]
public partial class ElementBag_T : IHost
{
    protected override Builder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    [SuppressMessage("", "IDE0290")]
    public ElementBag_T(IEngine engine) => Items = new(engine);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public ElementBag_T(
        IEngine engine, IEnumerable<IItem> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="other"></param>
    protected ElementBag_T(THost other) : this(other.Engine)
    {
        Items.AcceptDuplicates = other.AcceptDuplicates;
        Items.AddRange(other);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc cref="IInvariantBag{T}.ToBuilder"/>
    /// </summary>
    /// <returns></returns>
    public override IHost.IBuilder ToBuilder() => Items.Clone();

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IEngine Engine => Items.Engine;

    public bool AcceptDuplicates // For debug purposes...
    {
        get => Items.AcceptDuplicates;
        set => Items.AcceptDuplicates = value;
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(IItem? other) // IItem only if IHost is itself an item...
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not IHost valid) return false;

        if (!Engine.Equals(valid.Engine)) return false;
        if (Count != valid.Count) return false;

        foreach (var item in this)
        {
            var same = valid.Find(temp =>
                item is NamedElement xitem && temp is NamedElement xtemp
                ? xitem.Equals(xtemp, Engine.IgnoreCase)
                : item.EqualsEx(temp),
                out var value);

            if (!same) return false;
        }

        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(
        object? obj) => Equals(obj as IItem); // IItem only if IHost is itself an item...

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
        foreach (var item in this) code = HashCode.Combine(code, item);
        return code;
    }
}
using THost = Yotei.ORM.Generators.Invariant.Tests.ElementList_KT;
using IHost = Yotei.ORM.Generators.Invariant.Tests.IElementList_KT;
using IItem = Yotei.ORM.Generators.Invariant.Tests.IElement;
using TKey = string;

namespace Yotei.ORM.Generators.Invariant.Tests;

// ========================================================
/// <summary>
/// <inheritdoc cref="IHost"/>
/// </summary>
[InvariantList<TKey, IItem>(ReturnType = typeof(IHost))]
[DebuggerDisplay("{ToDebugString(5)}")]
public partial class ElementList_KT : IHost
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public ElementList_KT(IEngine engine) => Items = new Builder(engine);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public ElementList_KT(
        IEngine engine, IEnumerable<IItem> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected ElementList_KT(THost source) : this(source.Engine) => Items.AddRange(source);

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    /// Note that 'other' is 'IItem' instead of 'IHost' because the collection can be an element.
    public bool Equals(IItem? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not IHost valid) return false;

        if (!Engine.Equals(valid.Engine)) return false;
        if (Count != valid.Count) return false;

        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var temp = valid[i];
            var same = item is NamedElement xitem && temp is NamedElement xtemp
                ? xitem.Equals(xtemp, Engine.CaseSensitiveNames)
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
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    protected override Builder Items { get; }

    /// <summary>
    /// Returns a new builder base upon the contents of this instance.
    /// </summary>
    /// <returns></returns>
    public virtual IHost.IBuilder CreateBuilder() => Items.Clone();

    /// <summary>
    /// The engine this instance is associated with.
    /// </summary>
    public IEngine Engine => Items.Engine;
}
using THost = Yotei.ORM.Tests.InvariantListGenerator.ElementListKT;
using IHost = Yotei.ORM.Tests.InvariantListGenerator.IElementListKT;
using IItem = Yotei.ORM.Tests.InvariantListGenerator.IElement;
using TKey = string;

namespace Yotei.ORM.Tests.InvariantListGenerator;

// ========================================================
/// <inheritdoc cref="IHost"/>
[InvariantList<TKey, IItem>(ReturnType = typeof(IHost))]
[DebuggerDisplay("{Items.ToDebugString(5)}")]
public partial class ElementListKT : IHost, IItem
{
    protected override Builder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="engine"></param>
    public ElementListKT(IEngine engine) => Items = new Builder(engine);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="engine"></param>
    /// <param name="range"></param>
    public ElementListKT(
        IEngine engine, IEnumerable<IItem> range) : this(engine) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected ElementListKT(THost source) : this(source.Engine) => Items.AddRange(source);

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    /// We are using 'IItem' instead of 'IHost' because this collection may itself be an element.
    public virtual bool Equals(IItem? other)
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

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IItem);

    public static bool operator ==(THost? host, IHost? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(THost? host, IHost? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = Engine.GetHashCode();
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IHost.IBuilder CreateBuilder() => Items.Clone();

    /// <inheritdoc/>
    public IEngine Engine => Items.Engine;
}
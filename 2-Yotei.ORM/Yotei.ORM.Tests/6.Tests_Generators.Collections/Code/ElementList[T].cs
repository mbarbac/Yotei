using THost = Yotei.ORM.Tests.Tools.Generators.Collections.ElementList_T;
using IHost = Yotei.ORM.Tests.Tools.Generators.Collections.IElementList_T;
using IItem = Yotei.ORM.Tests.Tools.Generators.Collections.IElement;

namespace Yotei.ORM.Tests.Tools.Generators.Collections;

// ========================================================
/// <inheritdoc cref="IHost"/>
[InvariantList<IItem>]
[DebuggerDisplay("{Items.ToDebugString(5)}")]
public partial class ElementList_T : IHost, IItem
{
    protected override Builder Items { get; }

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="sensitive"></param>
    public ElementList_T(bool sensitive) => Items = new Builder(sensitive);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="range"></param>
    public ElementList_T(bool sensitive, IEnumerable<IItem> range) : this(sensitive) => Items.AddRange(range);
    
    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected ElementList_T(THost source) : this(source.CaseSensitive) => Items.AddRange(source);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IItem? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not IHost valid) return false;

        if (CaseSensitive != valid.CaseSensitive) return false;
        if (Count != valid.Count) return false;

        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var temp = valid[i];
            var same = item is NamedElement xitem && temp is NamedElement xtemp
                ? xitem.Equals(xtemp, CaseSensitive)
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
        var code = CaseSensitive.GetHashCode();
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual IHost.IBuilder CreateBuilder() => new Builder(CaseSensitive, this);

    /// <inheritdoc/>
    public bool CaseSensitive
    {
        get => Items.CaseSensitive;
        init => Items.CaseSensitive = value;
    }
}
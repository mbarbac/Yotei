using IHost = Yotei.ORM.Tests.Generators.IElementList_T;
using IItem = Yotei.ORM.Tests.Generators.IElement;

namespace Yotei.ORM.Tests.Generators;

// ========================================================
/// <inheritdoc cref="IHost"/>
[DebuggerDisplay("{ToDebugString(5)}")]
[InvariantList<IItem>]
public partial class ElementList_T : IHost
{
    protected override Builder Items { get; }
    protected ElementList_T(Builder items) => Items = items;

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="sensitive"></param>
    public ElementList_T(bool sensitive) : this(new Builder(sensitive)) { }

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="sensitive"></param>
    /// <param name="range"></param>
    public ElementList_T(
        bool sensitive, IEnumerable<IItem> range) : this(sensitive) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected ElementList_T(ElementList_T source) => Items = source.Items.Clone();

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IItem? other) // In this case, IHost inherits from IItem...
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not IHost valid) return false;

        if(CaseSensitive != valid.CaseSensitive) return false;
        if (Count != valid.Count) return false;

        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var temp = valid[i];
            var equal = item is INamedElement inamed && temp is INamedElement tnamed
                ? inamed.Equals(tnamed, CaseSensitive)
                : item.Equals(temp);

            if (!equal) return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IHost);

    public static bool operator ==(ElementList_T? host, IHost? item)
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(ElementList_T? host, IHost? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        code = HashCode.Combine(code, CaseSensitive);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc cref="IHost.CreateBuilder"/>
    public virtual Builder CreateBuilder() => Items.Clone();
    IHost.IBuilder IHost.CreateBuilder() => CreateBuilder();

    /// <inheritdoc/>
    public bool CaseSensitive { get; }
}
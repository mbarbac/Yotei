using IHost = Yotei.ORM.Tools.Code.Templates.IElementList_T;
using THost = Yotei.ORM.Tools.Code.Templates.ElementList_T;
using IItem = Yotei.ORM.Tools.Code.Templates.IElement;

namespace Yotei.ORM.Tools.Code.Templates;

// ========================================================
/// <inheritdoc cref="IHost"/>
[InvariantList<IItem>]
[DebuggerDisplay("{ToDebugString(5)}")]
public partial class ElementList_T : IHost
{
    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    public ElementList_T() => Items = new();

    /// <summary>
    /// Initializes a new empty instance with the given initial capacity.
    /// </summary>
    /// <param name="capacity"></param>
    public ElementList_T(int capacity) => Items = new(capacity);

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// </summary>
    /// <param name="range"></param>
    public ElementList_T(IEnumerable<IItem> range) : this() => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <param name="source"></param>
    protected ElementList_T(THost source) : this() => Items.AddRange(source);

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ----------------------------------------------------

    protected override Builder Items { get; }

    /// <inheritdoc cref="IHost.GetBuilder"/>
    public override Builder GetBuilder() => Items.Clone();
    IHost.IBuilder IHost.GetBuilder() => GetBuilder();

    // ----------------------------------------------------

    /// <inheritdoc/>
    public bool Equals(IHost? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other == null) return false;
        if (other is not IHost valid) return false;

        for (int i = 0; i < Count; i++)
        {
            var item = Items[i];
            var equal = item.Equals(valid[i]);

            if (!equal) return false;
        }

        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IHost);

    public static bool operator ==(THost? host, IHost? item) // Use 'is' instead of '=='...
    {
        if (host is null && item is null) return true;
        if (host is null || item is null) return false;

        return host.Equals(item);
    }

    public static bool operator !=(THost? host, IHost? item) => !(host == item);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var code = 0;
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }
}
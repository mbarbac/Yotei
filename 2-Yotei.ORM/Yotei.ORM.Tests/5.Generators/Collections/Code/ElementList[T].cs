using IHost = Yotei.ORM.Tests.Generators.IElementList_T;
using THost = Yotei.ORM.Tests.Generators.ElementList_T;
using IItem = Yotei.ORM.Tests.Generators.IElement;

namespace Yotei.ORM.Tests.Generators;

// ========================================================
/// <inheritdoc cref="IHost"/>
[DebuggerDisplay("{ToDebugString(5)}")]
[InvariantList<IItem>]
public partial class ElementList_T : IHost
{
    /// <inheritdoc/>
    protected override Builder Items { get; }

    /// <summary>
    /// Initializes a new empty instance with a <see cref="StringComparison.OrdinalIgnoreCase"/>
    /// comparison.
    /// </summary>
    public ElementList_T() : this(StringComparison.OrdinalIgnoreCase) { }

    /// <summary>
    /// Initializes a new instance with the elements of the given range, and a default
    /// <see cref="StringComparison.OrdinalIgnoreCase"/> comparison.
    /// </summary>
    /// <param name="range"></param>
    public ElementList_T(params IEnumerable<IItem> range) : this() => Items.AddRange(range);

    /// <inheritdoc/>
    public override string ToString() => Items.ToString();

    // ------------------------------------------------

    /// <summary>
    /// Initializes a new empty instance.
    /// </summary>
    /// <param name="comparison"></param>
    public ElementList_T(StringComparison comparison) => Items = new(comparison);

    /// <summary>
    /// Initializes a new instance with the elements from the given range.
    /// </summary>
    /// <param name="comparison"></param>
    /// <param name="range"></param>
    public ElementList_T(
        StringComparison comparison, IEnumerable<IItem> range) : this(comparison) => Items.AddRange(range);

    /// <summary>
    /// Copy constructor.
    /// </summary>
    /// <remarks>Needs to be protected for the 'IEnumerable(range)' constructor to work.</remarks>
    /// <param name="source"></param>
    protected ElementList_T(THost source) : this(source.Comparison) => Items.AddRange(source);

    // ----------------------------------------------------

    /// <inheritdoc/>
    public virtual bool Equals(IHost? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not IHost valid) return false;

        for (int i = 0; i < Items.Count; i++)
        {
            var item = Items[i];
            var equal = item.Equals(valid[i], Comparison);
            if (!equal) return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IHost);

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
        var code = 0;
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, Items[i]);
        return code;
    }

    // ----------------------------------------------------

    /// <inheritdoc cref="IHost.GetBuilder"/>
    public virtual Builder GetBuilder() => new(Comparison, this);
    IHost.IBuilder IHost.GetBuilder() => GetBuilder();

    /// <inheritdoc/>
    public StringComparison Comparison
    {
        get => Items.Comparison;
        init => Items.Comparison = value;
    }
}
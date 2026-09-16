using IItem = Yotei.ORM.InvariantGenerator.Tests.IElement;
using IHost = Yotei.ORM.InvariantGenerator.Tests.IElementList_T;
using THost = Yotei.ORM.InvariantGenerator.Tests.ElementList_T;

namespace Yotei.ORM.InvariantGenerator.Tests;

// ========================================================
/// <summary>
/// <inheritdoc cref="IHost"/>
/// </summary>
[InvariantList<IItem>(ReturnType = typeof(IHost))]
[DebuggerDisplay("{ToDebugString(3)}")]
public partial class ElementList_T : IHost
{
    /// <summary>
    /// Initializes a new instance.
    /// <br/> This method completely takes over the base one.
    /// </summary>
    /// <param name="ignoreCase"></param>
    public ElementList_T(bool ignoreCase) => IgnoreCase = ignoreCase;

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// <br/> This method completely takes over the base one.
    /// </summary>
    /// <param name="ignoreCase"></param>
    /// <param name="range"></param>
    public ElementList_T(bool ignoreCase, IEnumerable<IItem> range)
        : this(ignoreCase)
        => Items.AddRange(range.ThrowWhenNull());

    /// <summary>
    /// Copy constructor.
    /// <br/> This method completely takes over the base one.
    /// </summary>
    /// <param name="other"></param>
    protected ElementList_T(THost other)
    {
        ArgumentNullException.ThrowIfNull(other);
        IgnoreCase = other.IgnoreCase;
        AcceptDuplicates = other.AcceptDuplicates;
        FlattenElements = other.FlattenElements;
        Items.AddRange(other);
    }

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override Builder Items => (Builder)base.Items;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    protected override Builder CreateItems() => new(IgnoreCase)
    {
        AcceptDuplicates = AcceptDuplicates,
        FlattenElements = FlattenElements,
    };

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override IHost.IBuilder ToBuilder() => Items.Clone();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IgnoreCase
    {
        get => ItemsCreated ? Items.IgnoreCase : field;
        init
        {
            if (ItemsCreated) Items.IgnoreCase = value;
            else field = value;
        }
    }

    /// <summary>
    /// For DEBUG purposes only.
    /// </summary>
    public bool AcceptDuplicates
    {
        get => ItemsCreated ? Items.AcceptDuplicates : field;
        init
        {
            if (ItemsCreated) Items.AcceptDuplicates = value;
            else field = value;
        }
    }

    /// <summary>
    /// For DEBUG purposes only.
    /// </summary>
    public bool FlattenElements
    {
        get => ItemsCreated ? Items.FlattenElements : field;
        init
        {
            if (ItemsCreated) Items.FlattenElements = value;
            else field = value;
        }
    }
    = true;

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="other"></param>
    /// <returns><inheritdoc/></returns>
    public virtual bool Equals(IItem? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (other is null) return false;
        if (other is not IHost valid) return false;

        if (IgnoreCase != valid.IgnoreCase) return false;

        if (Count != valid.Count) return false;
        for (int i = 0; i < Count; i++)
        {
            var item = this[i];
            var temp = valid[i];
            var same = item is NamedElement xitem && temp is NamedElement xtemp
                ? xitem.Equals(xtemp, IgnoreCase)
                : item.EqualsEx(temp);

            if (!same) return false;
        }

        return true;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="obj"></param>
    /// <returns><inheritdoc/></returns>
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
    /// <returns><inheritdoc/></returns>
    public override int GetHashCode()
    {
        var code = IgnoreCase.GetHashCode();
        code = HashCode.Combine(code, Items);
        for (int i = 0; i < Count; i++) code = HashCode.Combine(code, this[i]);
        return code;
    }
}
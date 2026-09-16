using IItem = Yotei.ORM.InvariantGenerator.Tests.IElement;
using IHost = Yotei.ORM.InvariantGenerator.Tests.IElementBag_T;
using THost = Yotei.ORM.InvariantGenerator.Tests.ElementBag_T;

namespace Yotei.ORM.InvariantGenerator.Tests;

// ========================================================
/// <summary>
/// <inheritdoc cref="IHost"/>
/// </summary>
[InvariantBag<IItem>(ReturnType = typeof(IHost))]
[DebuggerDisplay("{ToDebugString(3)}")]
public partial class ElementBag_T : IHost
{
    /// <summary>
    /// Initializes a new instance.
    /// <br/> This method completely takes over the base one.
    /// </summary>
    /// <param name="ignoreCase"></param>
    public ElementBag_T(bool ignoreCase) => IgnoreCase = ignoreCase;

    /// <summary>
    /// Initializes a new instance with the elements of the given range.
    /// <br/> This method completely takes over the base one.
    /// </summary>
    /// <param name="ignoreCase"></param>
    /// <param name="range"></param>
    public ElementBag_T(bool ignoreCase, IEnumerable<IItem> range)
        : this(ignoreCase)
        => Items.AddRange(range.ThrowWhenNull());

    /// <summary>
    /// Copy constructor.
    /// <br/> This method completely takes over the base one.
    /// </summary>
    /// <param name="other"></param>
    protected ElementBag_T(THost other)
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
    protected override Builder CreateItems()
        => new(IgnoreCase) { AcceptDuplicates = AcceptDuplicates };

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns><inheritdoc/></returns>
    public override IHost.IBuilder ToBuilder() => Items.Clone();

    // ----------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool IgnoreCase { get; init; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool AcceptDuplicates { get; init; }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool FlattenElements
    {
        get => Items.FlattenElements;
        init => Items.FlattenElements = value;
    }

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
        var list = ToList();
        foreach (var temp in valid)
        {
            if (!TryFind(list, temp, out var value)) return false;
            list.Remove(value);
        }
        if (list.Count != 0) return false;

        return true;

        // Determines if the list contains the given target...
        static bool TryFind(List<IItem> list, IItem target, out IItem value)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];

                if (ReferenceEquals(item, target) || item.Equals(target))
                {
                    value = item;
                    return true;
                }
            }

            value = default!;
            return false;
        }
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
        foreach (var item in Items) code = HashCode.Combine(code, item);
        return code;
    }
}
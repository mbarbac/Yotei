#if NETSTANDARD2_0
namespace Yotei.Tools.Std20.Tests;
#else
namespace Yotei.Tools;
#endif

// ========================================================
/// <summary>
/// A reference type for testing purposes.
/// </summary>
public class Item<T>(T value) : ICloneable
{
    public T Value = value;

    public override string ToString() => Value?.ToString() ?? "";

    public Item<T> Clone() => new(Value);
    object ICloneable.Clone() => Clone();
}

// ========================================================
/// <summary>
/// A comparer for <see cref="Item{T}"/> instances. Comparisons are implemented by comparing the
/// values carried by the elements, not the elements themselves. If a custom comparison logic is
/// provided in the constructor, then it is used. Otherwise, if the type <c>T</c> implements
/// <see cref="IComparable"/>, then the default comparer for that type is used. Otherwise, the
/// respective string representations of the values are just compared.
/// </summary>
public class ItemComparer<T> : IEqualityComparer<Item<T>>, IComparer<Item<T>>
{
    /// <summary>
    /// Initializes a new instance with the given comparison logic or, if it is null, with a
    /// default one that, if the type <c>T</c> implements <c>IComparable</c>, is based on the
    /// default comparer of the type, or otherwise, in the comparison between the respective
    /// values carried by the elements.
    /// </summary>
    public ItemComparer(Func<T, T, int>? comparison = null)
    {
        if (comparison != null) _Compare = comparison;
        else
        {
            var type = typeof(T);
            var same = type.IsAssignableFrom(typeof(IComparable<T>)) || type.IsAssignableFrom(typeof(IComparable));
            if (same) _Compare = Comparer<T>.Default.Compare;
            else _Compare = (x, y) =>
            {
                if (x is null && y is null) return 0;
                if (x is null) return -1;
                if (y is null) return +1;
                return string.Compare(x.ToString(), y.ToString());
            };
        }
    }
    readonly Func<T, T, int> _Compare;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int Compare(Item<T>? x, Item<T>? y)
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return +1;
        return _Compare(x.Value, y.Value);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public bool Equals(Item<T>? x, Item<T>? y) => Compare(x, y) == 0;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public int GetHashCode([DisallowNull] Item<T> obj) => obj.Value?.GetHashCode() ?? 0;
}
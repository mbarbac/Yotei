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

    public static ComparerFactory.InnerComparer<Item<T>> Create(
        Func<Item<T>, Item<T>, int>? comparer = null)
        => comparer != null
        ? ComparerFactory.Create(comparer)
        : ComparerFactory.Create(comparer, StringComparison.Ordinal);
}
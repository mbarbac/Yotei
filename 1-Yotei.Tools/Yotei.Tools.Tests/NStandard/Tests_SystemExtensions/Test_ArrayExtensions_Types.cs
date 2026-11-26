#pragma warning disable IDE0079
#pragma warning disable CA1859

#if NETSTANDARD2_0
using System.Net.Sockets;

namespace Yotei.Tools.NStd20.Tests;
#else
namespace Yotei.Tools.Tests;
#endif

// ========================================================
public static partial class Test_ArrayExtensions
{
    static int[] IntArray() => [];
    static int[] IntArray(IEnumerable<int> values) => [.. values];

    static ReadOnlySpan<int> IntSpan() => [];
    static ReadOnlySpan<int> IntSpan(IEnumerable<int> values) => values.ToArray();

    static IEnumerable<int> IntEnumerable() => [];
    static IEnumerable<int> IntEnumerable(IEnumerable<int> values) => values;

    static Predicate<int> IntPredicate(int value) => x => x == value;
    static IEqualityComparer<int> IntEquality() => EqualityComparer<int>.Default;
    static IComparer<int> IntComparer() => Comparer<int>.Create((x, y) => x.CompareTo(y));

    // ----------------------------------------------------

    static Item<int> Item(int value) => new(value);
    static Item<int>[] ItemArray() => [];
    static Item<int>[] ItemArray(IEnumerable<int> values) => [.. values.Select(x => Item(x))];

    static ReadOnlySpan<Item<int>> ItemSpan() => [];
    static ReadOnlySpan<Item<int>> ItemSpan(IEnumerable<int> values) => values.Select(x => Item(x)).ToArray();

    static IEnumerable<Item<int>> ItemEnumerable() => [];
    static IEnumerable<Item<int>> ItemEnumerable(IEnumerable<int> values) => values.Select(x => Item(x));

    static Predicate<Item<int>> ItemPredicate(int value) => x => x.Value == value;
    static IEqualityComparer<Item<int>> ItemEquality() => EqualityComparerEx.New<Item<int>>((x, y) =>
    {
        if (x is null && y is null) return true;
        if (x is null) return false;
        if (y is null) return false;
        return x.Value == y.Value;
    });
    static IComparer<Item<int>> ItemComparer() => ComparerEx.New<Item<int>>((x, y) =>
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return +1;
        return x.Value.CompareTo(y.Value);
    });
}
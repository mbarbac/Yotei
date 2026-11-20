namespace Yotei.Tools.Tests;

// ========================================================
public static partial class Test_ReadOnlySpanExtensions
{
    static ReadOnlySpan<int> IntSpan(int[] values) => values.AsSpan();
    static Predicate<int> IntPredicate(int value) => x => x == value;
    static IEqualityComparer<int> IntEquality() => EqualityComparer<int>.Default;
    static IComparer<int> IntComparer() => Comparer<int>.Create((x, y) => x.CompareTo(y));

    // ----------------------------------------------------

    static Item<int> Item(int value) => new(value);
    static ReadOnlySpan<Item<int>> ItemSpan(IEnumerable<int> values)
    {
        return values.Select(x => Item(x)).ToArray();
    }
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

    // ----------------------------------------------------

    [Fact]
    public static void Inherit_OnInt_IsEmpty()
    {
        var source = IntSpan([]); Assert.True(source.IsEmpty);
        source = IntSpan([1, 2]); Assert.False(source.IsEmpty);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnInt_IndexOf()
    {
        int result;
        var source = IntSpan([]);
        result = source.IndexOf(2); Assert.Equal(-1, result);
        result = source.IndexOf(2, IntEquality()); Assert.Equal(-1, result);

        source = IntSpan([1, 2, 3, 4]);
        result = source.IndexOf(2); Assert.Equal(1, result);
        result = source.IndexOf(2, IntEquality()); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_OnItem_IndexOf()
    {
        int result;
        var source = ItemSpan([]);
        // source.IndexOf(Item(2)); => CS0311: Item<T> is not IEquatable
        result = source.IndexOf(Item(2), ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([1, 2, 3, 4]);
        result = source.IndexOf(Item(2), ItemEquality()); Assert.Equal(1, result);
    }
}
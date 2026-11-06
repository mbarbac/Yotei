#pragma warning disable IDE0079
#pragma warning disable CA1859

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
    static ReadOnlySpan<Item<int>> ItemSpan(Item<int>[] values) => values.AsSpan();
    static Predicate<Item<int>> ItemPredicate(int value) => x => x.Value == value;
    static IEqualityComparer<Item<int>> ItemEquality() => new ItemEqualityComparer();
    static IComparer<Item<int>> ItemComparer() => Comparer<Item<int>>.Create((x, y) =>
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return +1;
        return x.Value.CompareTo(y.Value);
    });

    readonly struct ItemEqualityComparer : IEqualityComparer<Item<int>>
    {
        public bool Equals(Item<int>? x, Item<int>? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;
            return x.Value == y.Value;
        }
        public int GetHashCode([DisallowNull] Item<int> obj) => obj.Value.GetHashCode();
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IntSpan_IndexOf()
    {
        int result;
        var span = IntSpan([]);
        result = span.IndexOf(9); Assert.Equal(-1, result);
        result = span.IndexOf(9, IntEquality()); Assert.Equal(-1, result);
        result = span.IndexOf(IntPredicate(9)); Assert.Equal(-1, result);

        span = [1, 2, 3];
        result = span.IndexOf(0); Assert.Equal(-1, result);
        result = span.IndexOf(0, IntEquality()); Assert.Equal(-1, result);
        result = span.IndexOf(IntPredicate(0)); Assert.Equal(-1, result);

        span = [1, 0, 2, 0, 4];
        result = span.IndexOf(0); Assert.Equal(1, result);
        result = span.IndexOf(0, IntEquality()); Assert.Equal(1, result);
        result = span.IndexOf(IntPredicate(0)); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_ItemSpan_IndexOf()
    {
        int result;
        var span = ItemSpan([]);
        result = span.IndexOf(Item(9)); Assert.Equal(-1, result);
        result = span.IndexOf(Item(9), ItemEquality()); Assert.Equal(-1, result);
        result = span.IndexOf(ItemPredicate(9)); Assert.Equal(-1, result);

        span = ItemSpan([new(1), new(2), new(3)]);
        result = span.IndexOf(Item(0)); Assert.Equal(-1, result);
        result = span.IndexOf(Item(0), ItemEquality()); Assert.Equal(-1, result);
        result = span.IndexOf(ItemPredicate(0)); Assert.Equal(-1, result);

        span = ItemSpan([new(1), new(0), new(2), new(0), new(4)]);
        result = span.IndexOf(Item(0)); Assert.Equal(-1, result);
        result = span.IndexOf(Item(0), ItemEquality()); Assert.Equal(1, result);
        result = span.IndexOf(ItemPredicate(0)); Assert.Equal(1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IntSpan_LastIndexOf()
    {
        int result;
        var span = IntSpan([]);
        result = span.LastIndexOf(9); Assert.Equal(-1, result);
        result = span.LastIndexOf(9, IntEquality()); Assert.Equal(-1, result);
        result = span.LastIndexOf(IntPredicate(9)); Assert.Equal(-1, result);

        span = [1, 2, 3];
        result = span.LastIndexOf(0); Assert.Equal(-1, result);
        result = span.LastIndexOf(0, IntEquality()); Assert.Equal(-1, result);
        result = span.LastIndexOf(IntPredicate(0)); Assert.Equal(-1, result);

        span = [1, 0, 2, 0, 4];
        result = span.LastIndexOf(0); Assert.Equal(3, result);
        result = span.LastIndexOf(0, IntEquality()); Assert.Equal(3, result);
        result = span.LastIndexOf(IntPredicate(0)); Assert.Equal(3, result);
    }

    [Fact]
    public static void Test_ItemSpan_LastIndexOf()
    {
        int result;
        var span = ItemSpan([]);
        result = span.LastIndexOf(Item(9)); Assert.Equal(-1, result);
        result = span.LastIndexOf(Item(9), ItemEquality()); Assert.Equal(-1, result);
        result = span.LastIndexOf(ItemPredicate(9)); Assert.Equal(-1, result);

        span = ItemSpan([new(1), new(2), new(3)]);
        result = span.LastIndexOf(Item(0)); Assert.Equal(-1, result);
        result = span.LastIndexOf(Item(0), ItemEquality()); Assert.Equal(-1, result);
        result = span.LastIndexOf(ItemPredicate(0)); Assert.Equal(-1, result);

        span = ItemSpan([new(1), new(0), new(2), new(0), new(4)]);
        result = span.LastIndexOf(Item(0)); Assert.Equal(-1, result);
        result = span.LastIndexOf(Item(0), ItemEquality()); Assert.Equal(3, result);
        result = span.LastIndexOf(ItemPredicate(0)); Assert.Equal(3, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IntSpan_IndexesOf()
    {
        List<int> result;
        var span = IntSpan([]);
        result = span.IndexesOf(9); Assert.True(result is []);
        result = span.IndexesOf(9, IntEquality()); Assert.True(result is []);
        result = span.IndexesOf(IntPredicate(9)); Assert.True(result is []);

        span = [1, 2, 3];
        result = span.IndexesOf(0); Assert.True(result is []);
        result = span.IndexesOf(0, IntEquality()); Assert.True(result is []);
        result = span.IndexesOf(IntPredicate(0)); Assert.True(result is []);

        span = [1, 0, 2, 0, 4];
        result = span.IndexesOf(0); Assert.True(result is [1, 3]);
        result = span.IndexesOf(0, IntEquality()); Assert.True(result is [1, 3]);
        result = span.IndexesOf(IntPredicate(0)); Assert.True(result is [1, 3]);
    }

    [Fact]
    public static void Test_ItemSpan_IndexesOf()
    {
        List<int> result;
        var span = ItemSpan([]);
        result = span.IndexesOf(Item(0)); Assert.True(result is []);
        result = span.IndexesOf(Item(9), ItemEquality()); Assert.True(result is []);
        result = span.IndexesOf(ItemPredicate(9)); Assert.True(result is []);

        span = ItemSpan([new(1), new(2), new(3)]);
        result = span.IndexesOf(Item(0)); Assert.True(result is []);
        result = span.IndexesOf(Item(0), ItemEquality()); Assert.True(result is []);
        result = span.IndexesOf(ItemPredicate(0)); Assert.True(result is []);

        span = ItemSpan([new(1), new(0), new(2), new(0), new(4)]);
        result = span.IndexesOf(Item(0)); Assert.True(result is []);
        result = span.IndexesOf(Item(0), ItemEquality()); Assert.True(result is [1, 3]);
        result = span.IndexesOf(ItemPredicate(0)); Assert.True(result is [1, 3]);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IntSpan_Contains()
    {
        bool result;
        var span = IntSpan([]);
        result = span.Contains(9); Assert.False(result);
        result = span.Contains(9, IntEquality()); Assert.False(result);
        result = span.Contains(IntPredicate(9)); Assert.False(result);

        span = [1, 2, 3];
        result = span.Contains(0); Assert.False(result);
        result = span.Contains(0, IntEquality()); Assert.False(result);
        result = span.Contains(IntPredicate(0)); Assert.False(result);

        span = [1, 0, 2, 0, 4];
        result = span.Contains(0); Assert.True(result);
        result = span.Contains(0, IntEquality()); Assert.True(result);
        result = span.Contains(IntPredicate(0)); Assert.True(result);
    }

    [Fact]
    public static void Test_ItemSpan_Contains()
    {
        bool result;
        var span = ItemSpan([]);
        result = span.Contains(Item(0)); Assert.False(result);
        result = span.Contains(Item(9), ItemEquality()); Assert.False(result);
        result = span.Contains(ItemPredicate(9)); Assert.False(result);

        span = ItemSpan([new(1), new(2), new(3)]);
        result = span.Contains(Item(0)); Assert.False(result);
        result = span.Contains(Item(0), ItemEquality()); Assert.False(result);
        result = span.Contains(ItemPredicate(0)); Assert.False(result);

        span = ItemSpan([new(1), new(0), new(2), new(0), new(4)]);
        result = span.Contains(Item(0)); Assert.False(result); // Reference type!
        result = span.Contains(Item(0), ItemEquality()); Assert.True(result);
        result = span.Contains(ItemPredicate(0)); Assert.True(result);
    }
}

// ========================================================
#if NETSTANDARD2_0
public static partial class Test_ReadOnlySpanExtensions
{
    //[Fact]
    //public static void Test_IntSpan_()
    //{
    //    bool result;
    //    var span = IntSpan([]);
    //    result = span.Contains(9); Assert.False(result);
    //    result = span.Contains(9, IntEquality()); Assert.False(result);
    //    result = span.Contains(IntPredicate(9)); Assert.False(result);

    //    span = [1, 2, 3];
    //    result = span.Contains(0); Assert.False(result);
    //    result = span.Contains(0, IntEquality()); Assert.False(result);
    //    result = span.Contains(IntPredicate(0)); Assert.False(result);

    //    span = [1, 0, 2, 0, 4];
    //    result = span.Contains(0); Assert.True(result);
    //    result = span.Contains(0, IntEquality()); Assert.True(result);
    //    result = span.Contains(IntPredicate(0)); Assert.True(result);
    //}

    //[Fact]
    //public static void Test_ItemSpan_()
    //{
    //    bool result;
    //    var span = ItemSpan([]);
    //    result = span.Contains(Item(0)); Assert.False(result);
    //    result = span.Contains(Item(9), ItemEquality()); Assert.False(result);
    //    result = span.Contains(ItemPredicate(9)); Assert.False(result);

    //    span = ItemSpan([new(1), new(2), new(3)]);
    //    result = span.Contains(Item(0)); Assert.False(result);
    //    result = span.Contains(Item(0), ItemEquality()); Assert.False(result);
    //    result = span.Contains(ItemPredicate(0)); Assert.False(result);

    //    span = ItemSpan([new(1), new(0), new(2), new(0), new(4)]);
    //    result = span.Contains(Item(0)); Assert.False(result); // Reference type!
    //    result = span.Contains(Item(0), ItemEquality()); Assert.True(result);
    //    result = span.Contains(ItemPredicate(0)); Assert.True(result);
    //}
}
#endif
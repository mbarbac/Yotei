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
        int data;
        var span = IntSpan([]);
        data = span.IndexOf(9); Assert.Equal(-1, data);
        data = span.IndexOf(9, IntEquality()); Assert.Equal(-1, data);
        data = span.IndexOf(IntPredicate(9)); Assert.Equal(-1, data);

        span = [1, 2, 3];
        data = span.IndexOf(0); Assert.Equal(-1, data);
        data = span.IndexOf(0, IntEquality()); Assert.Equal(-1, data);
        data = span.IndexOf(IntPredicate(0)); Assert.Equal(-1, data);

        span = [1, 0, 2, 0, 4];
        data = span.IndexOf(0); Assert.Equal(1, data);
        data = span.IndexOf(0, IntEquality()); Assert.Equal(1, data);
        data = span.IndexOf(IntPredicate(0)); Assert.Equal(1, data);
    }

    [Fact]
    public static void Test_ItemSpan_IndexOf()
    {
        int data;
        var span = ItemSpan([]);
        data = span.IndexOf(Item(9)); Assert.Equal(-1, data);
        data = span.IndexOf(Item(9), ItemEquality()); Assert.Equal(-1, data);
        data = span.IndexOf(ItemPredicate(9)); Assert.Equal(-1, data);

        span = ItemSpan([new(1), new(2), new(3)]);
        data = span.IndexOf(Item(0)); Assert.Equal(-1, data);
        data = span.IndexOf(Item(0), ItemEquality()); Assert.Equal(-1, data);
        data = span.IndexOf(ItemPredicate(0)); Assert.Equal(-1, data);

        span = ItemSpan([new(1), new(0), new(2), new(0), new(4)]);
        data = span.IndexOf(Item(0)); Assert.Equal(-1, data);
        data = span.IndexOf(Item(0), ItemEquality()); Assert.Equal(1, data);
        data = span.IndexOf(ItemPredicate(0)); Assert.Equal(1, data);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IntSpan_LastIndexOf()
    {
        int data;
        var span = IntSpan([]);
        data = span.LastIndexOf(9); Assert.Equal(-1, data);
        data = span.LastIndexOf(9, IntEquality()); Assert.Equal(-1, data);
        data = span.LastIndexOf(IntPredicate(9)); Assert.Equal(-1, data);

        span = [1, 2, 3];
        data = span.LastIndexOf(0); Assert.Equal(-1, data);
        data = span.LastIndexOf(0, IntEquality()); Assert.Equal(-1, data);
        data = span.LastIndexOf(IntPredicate(0)); Assert.Equal(-1, data);

        span = [1, 0, 2, 0, 4];
        data = span.LastIndexOf(0); Assert.Equal(3, data);
        data = span.LastIndexOf(0, IntEquality()); Assert.Equal(3, data);
        data = span.LastIndexOf(IntPredicate(0)); Assert.Equal(3, data);
    }

    [Fact]
    public static void Test_ItemSpan_LastIndexOf()
    {
        int data;
        var span = ItemSpan([]);
        data = span.LastIndexOf(Item(9)); Assert.Equal(-1, data);
        data = span.LastIndexOf(Item(9), ItemEquality()); Assert.Equal(-1, data);
        data = span.LastIndexOf(ItemPredicate(9)); Assert.Equal(-1, data);

        span = ItemSpan([new(1), new(2), new(3)]);
        data = span.LastIndexOf(Item(0)); Assert.Equal(-1, data);
        data = span.LastIndexOf(Item(0), ItemEquality()); Assert.Equal(-1, data);
        data = span.LastIndexOf(ItemPredicate(0)); Assert.Equal(-1, data);

        span = ItemSpan([new(1), new(0), new(2), new(0), new(4)]);
        data = span.LastIndexOf(Item(0)); Assert.Equal(-1, data);
        data = span.LastIndexOf(Item(0), ItemEquality()); Assert.Equal(3, data);
        data = span.LastIndexOf(ItemPredicate(0)); Assert.Equal(3, data);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IntSpan_IndexesOf()
    {
        List<int> data;
        var span = IntSpan([]);
        data = span.IndexesOf(9); Assert.True(data is []);
        data = span.IndexesOf(9, IntEquality()); Assert.True(data is []);
        data = span.IndexesOf(IntPredicate(9)); Assert.True(data is []);

        span = [1, 2, 3];
        data = span.IndexesOf(0); Assert.True(data is []);
        data = span.IndexesOf(0, IntEquality()); Assert.True(data is []);
        data = span.IndexesOf(IntPredicate(0)); Assert.True(data is []);

        span = [1, 0, 2, 0, 4];
        data = span.IndexesOf(0); Assert.True(data is [1, 3]);
        data = span.IndexesOf(0, IntEquality()); Assert.True(data is [1, 3]);
        data = span.IndexesOf(IntPredicate(0)); Assert.True(data is [1, 3]);
    }

    [Fact]
    public static void Test_ItemSpan_IndexesOf()
    {
        List<int> data;
        var span = ItemSpan([]);
        data = span.IndexesOf(Item(0)); Assert.True(data is []);
        data = span.IndexesOf(Item(9), ItemEquality()); Assert.True(data is []);
        data = span.IndexesOf(ItemPredicate(9)); Assert.True(data is []);

        span = ItemSpan([new(1), new(2), new(3)]);
        data = span.IndexesOf(Item(0)); Assert.True(data is []);
        data = span.IndexesOf(Item(0), ItemEquality()); Assert.True(data is []);
        data = span.IndexesOf(ItemPredicate(0)); Assert.True(data is []);

        span = ItemSpan([new(1), new(0), new(2), new(0), new(4)]);
        data = span.IndexesOf(Item(0)); Assert.True(data is []);
        data = span.IndexesOf(Item(0), ItemEquality()); Assert.True(data is [1, 3]);
        data = span.IndexesOf(ItemPredicate(0)); Assert.True(data is [1, 3]);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IntSpan_Contains()
    {
        bool data;
        var span = IntSpan([]);
        data = span.Contains(9); Assert.False(data);
        data = span.Contains(9, IntEquality()); Assert.False(data);
        data = span.Contains(IntPredicate(9)); Assert.False(data);

        span = [1, 2, 3];
        data = span.Contains(0); Assert.False(data);
        data = span.Contains(0, IntEquality()); Assert.False(data);
        data = span.Contains(IntPredicate(0)); Assert.False(data);

        span = [1, 0, 2, 0, 4];
        data = span.Contains(0); Assert.True(data);
        data = span.Contains(0, IntEquality()); Assert.True(data);
        data = span.Contains(IntPredicate(0)); Assert.True(data);
    }

    [Fact]
    public static void Test_ItemSpan_Contains()
    {
        bool data;
        var span = ItemSpan([]);
        data = span.Contains(Item(0)); Assert.False(data);
        data = span.Contains(Item(9), ItemEquality()); Assert.False(data);
        data = span.Contains(ItemPredicate(9)); Assert.False(data);

        span = ItemSpan([new(1), new(2), new(3)]);
        data = span.Contains(Item(0)); Assert.False(data);
        data = span.Contains(Item(0), ItemEquality()); Assert.False(data);
        data = span.Contains(ItemPredicate(0)); Assert.False(data);

        span = ItemSpan([new(1), new(0), new(2), new(0), new(4)]);
        data = span.Contains(Item(0)); Assert.False(data); // Reference type!
        data = span.Contains(Item(0), ItemEquality()); Assert.True(data);
        data = span.Contains(ItemPredicate(0)); Assert.True(data);
    }

    // ----------------------------------------------------
#if NETSTANDARD2_0
    //[Fact]
    //public static void Test_IntSpan_()
    //{
    //    bool data;
    //    var span = IntSpan([]);
    //    data = span.Contains(9); Assert.False(data);
    //    data = span.Contains(9, IntEquality()); Assert.False(data);
    //    data = span.Contains(IntPredicate(9)); Assert.False(data);

    //    span = [1, 2, 3];
    //    data = span.Contains(0); Assert.False(data);
    //    data = span.Contains(0, IntEquality()); Assert.False(data);
    //    data = span.Contains(IntPredicate(0)); Assert.False(data);

    //    span = [1, 0, 2, 0, 4];
    //    data = span.Contains(0); Assert.True(data);
    //    data = span.Contains(0, IntEquality()); Assert.True(data);
    //    data = span.Contains(IntPredicate(0)); Assert.True(data);
    //}

    //[Fact]
    //public static void Test_ItemSpan_()
    //{
    //    bool data;
    //    var span = ItemSpan([]);
    //    data = span.Contains(Item(0)); Assert.False(data);
    //    data = span.Contains(Item(9), ItemEquality()); Assert.False(data);
    //    data = span.Contains(ItemPredicate(9)); Assert.False(data);

    //    span = ItemSpan([new(1), new(2), new(3)]);
    //    data = span.Contains(Item(0)); Assert.False(data);
    //    data = span.Contains(Item(0), ItemEquality()); Assert.False(data);
    //    data = span.Contains(ItemPredicate(0)); Assert.False(data);

    //    span = ItemSpan([new(1), new(0), new(2), new(0), new(4)]);
    //    data = span.Contains(Item(0)); Assert.False(data); // Reference type!
    //    data = span.Contains(Item(0), ItemEquality()); Assert.True(data);
    //    data = span.Contains(ItemPredicate(0)); Assert.True(data);
    //}
#endif
}
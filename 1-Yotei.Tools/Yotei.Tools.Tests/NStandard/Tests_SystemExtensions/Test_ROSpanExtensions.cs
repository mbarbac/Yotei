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

    // ----------------------------------------------------

    [Fact]
    public static void Test_IntSpan_StartsWith()
    {
        bool result;
        var span = IntSpan([]);
        result = span.StartsWith(9); Assert.False(result);
        result = span.StartsWith(9, IntEquality()); Assert.False(result);
        result = span.StartsWith(IntPredicate(9)); Assert.False(result);

        span = [1, 2, 3];
        result = span.StartsWith(0); Assert.False(result);
        result = span.StartsWith(0, IntEquality()); Assert.False(result);
        result = span.StartsWith(IntPredicate(0)); Assert.False(result);

        span = [0, 2, 0, 4];
        result = span.StartsWith(0); Assert.True(result);
        result = span.StartsWith(0, IntEquality()); Assert.True(result);
        result = span.StartsWith(IntPredicate(0)); Assert.True(result);
    }

    [Fact]
    public static void Test_ItemSpan_StartsWith()
    {
        bool result;
        var span = ItemSpan([]);
        result = span.StartsWith(Item(0)); Assert.False(result);
        result = span.StartsWith(Item(9), ItemEquality()); Assert.False(result);
        result = span.StartsWith(ItemPredicate(9)); Assert.False(result);

        span = ItemSpan([new(1), new(2), new(3)]);
        result = span.StartsWith(Item(0)); Assert.False(result);
        result = span.StartsWith(Item(0), ItemEquality()); Assert.False(result);
        result = span.StartsWith(ItemPredicate(0)); Assert.False(result);

        span = ItemSpan([new(0), new(2), new(0), new(4)]);
        result = span.StartsWith(Item(0)); Assert.False(result); // Reference type!
        result = span.StartsWith(Item(0), ItemEquality()); Assert.True(result);
        result = span.StartsWith(ItemPredicate(0)); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IntSpan_EndsWith()
    {
        bool result;
        var span = IntSpan([]);
        result = span.EndsWith(9); Assert.False(result);
        result = span.EndsWith(9, IntEquality()); Assert.False(result);
        result = span.EndsWith(IntPredicate(9)); Assert.False(result);

        span = [1, 2, 3];
        result = span.EndsWith(0); Assert.False(result);
        result = span.EndsWith(0, IntEquality()); Assert.False(result);
        result = span.EndsWith(IntPredicate(0)); Assert.False(result);

        span = [1, 2, 0];
        result = span.EndsWith(0); Assert.True(result);
        result = span.EndsWith(0, IntEquality()); Assert.True(result);
        result = span.EndsWith(IntPredicate(0)); Assert.True(result);
    }

    [Fact]
    public static void Test_ItemSpan_EndsWith()
    {
        bool result;
        var span = ItemSpan([]);
        result = span.EndsWith(Item(0)); Assert.False(result);
        result = span.EndsWith(Item(9), ItemEquality()); Assert.False(result);
        result = span.EndsWith(ItemPredicate(9)); Assert.False(result);

        span = ItemSpan([new(1), new(2), new(3)]);
        result = span.EndsWith(Item(0)); Assert.False(result);
        result = span.EndsWith(Item(0), ItemEquality()); Assert.False(result);
        result = span.EndsWith(ItemPredicate(0)); Assert.False(result);

        span = ItemSpan([new(1), new(2), new(0)]);
        result = span.EndsWith(Item(0)); Assert.False(result); // Reference type!
        result = span.EndsWith(Item(0), ItemEquality()); Assert.True(result);
        result = span.EndsWith(ItemPredicate(0)); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IntSpan_IndexOfAnyExcept()
    {
        int result;
        var span = IntSpan([]);
        result = span.IndexOfAnyExcept(9); Assert.Equal(-1, result);
        result = span.IndexOfAnyExcept(9, IntEquality()); Assert.Equal(-1, result);
        result = span.IndexOfAnyExcept(IntPredicate(9)); Assert.Equal(-1, result);

        span = [1, 2, 3];
        result = span.IndexOfAnyExcept(0); Assert.Equal(0, result);
        result = span.IndexOfAnyExcept(0, IntEquality()); Assert.Equal(0, result);
        result = span.IndexOfAnyExcept(IntPredicate(0)); Assert.Equal(0, result);

        span = [0, 0];
        result = span.IndexOfAnyExcept(0); Assert.Equal(-1, result);
        result = span.IndexOfAnyExcept(0, IntEquality()); Assert.Equal(-1, result);
        result = span.IndexOfAnyExcept(IntPredicate(0)); Assert.Equal(-1, result);

        span = [0, 0, 1, 2];
        result = span.IndexOfAnyExcept(0); Assert.Equal(2, result);
        result = span.IndexOfAnyExcept(0, IntEquality()); Assert.Equal(2, result);
        result = span.IndexOfAnyExcept(IntPredicate(0)); Assert.Equal(2, result);
    }

    [Fact]
    public static void Test_ItemSpan_IndexOfAnyExcept()
    {
        int result;
        var span = ItemSpan([]);
        result = span.IndexOfAnyExcept(Item(9)); Assert.Equal(-1, result);
        result = span.IndexOfAnyExcept(Item(9), ItemEquality()); Assert.Equal(-1, result);
        result = span.IndexOfAnyExcept(ItemPredicate(9)); Assert.Equal(-1, result);

        span = ItemSpan([new(1), new(2), new(3)]);
        result = span.IndexOfAnyExcept(Item(0)); Assert.Equal(0, result);
        result = span.IndexOfAnyExcept(Item(0), ItemEquality()); Assert.Equal(0, result);
        result = span.IndexOfAnyExcept(ItemPredicate(0)); Assert.Equal(0, result);

        span = ItemSpan([new(0), new(0)]);
        result = span.IndexOfAnyExcept(Item(0)); Assert.Equal(0, result); // Reference type!
        result = span.IndexOfAnyExcept(Item(0), ItemEquality()); Assert.Equal(-1, result);
        result = span.IndexOfAnyExcept(ItemPredicate(0)); Assert.Equal(-1, result);

        span = ItemSpan([new(0), new(0), new(1), new(2)]);
        result = span.IndexOfAnyExcept(Item(0)); Assert.Equal(0, result); // Reference type!
        result = span.IndexOfAnyExcept(Item(0), ItemEquality()); Assert.Equal(2, result);
        result = span.IndexOfAnyExcept(ItemPredicate(0)); Assert.Equal(2, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IntSpan_LastIndexOfAnyExcept()
    {
        int result;
        var span = IntSpan([]);
        result = span.LastIndexOfAnyExcept(9); Assert.Equal(-1, result);
        result = span.LastIndexOfAnyExcept(9, IntEquality()); Assert.Equal(-1, result);
        result = span.LastIndexOfAnyExcept(IntPredicate(9)); Assert.Equal(-1, result);

        span = [1, 2, 3];
        result = span.LastIndexOfAnyExcept(0); Assert.Equal(2, result);
        result = span.LastIndexOfAnyExcept(0, IntEquality()); Assert.Equal(2, result);
        result = span.LastIndexOfAnyExcept(IntPredicate(0)); Assert.Equal(2, result);

        span = [0, 0, 0];
        result = span.LastIndexOfAnyExcept(0); Assert.Equal(-1, result);
        result = span.LastIndexOfAnyExcept(0, IntEquality()); Assert.Equal(-1, result);
        result = span.LastIndexOfAnyExcept(IntPredicate(0)); Assert.Equal(-1, result);

        span = [1, 2, 0, 0];
        result = span.LastIndexOfAnyExcept(0); Assert.Equal(1, result);
        result = span.LastIndexOfAnyExcept(0, IntEquality()); Assert.Equal(1, result);
        result = span.LastIndexOfAnyExcept(IntPredicate(0)); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_ItemSpan_LastIndexOfAnyExcept()
    {
        int result;
        var span = ItemSpan([]);
        result = span.LastIndexOfAnyExcept(Item(9)); Assert.Equal(-1, result);
        result = span.LastIndexOfAnyExcept(Item(9), ItemEquality()); Assert.Equal(-1, result);
        result = span.LastIndexOfAnyExcept(ItemPredicate(9)); Assert.Equal(-1, result);

        span = ItemSpan([new(1), new(2), new(3)]);
        result = span.LastIndexOfAnyExcept(Item(0)); Assert.Equal(2, result);
        result = span.LastIndexOfAnyExcept(Item(0), ItemEquality()); Assert.Equal(2, result);
        result = span.LastIndexOfAnyExcept(ItemPredicate(0)); Assert.Equal(2, result);

        span = ItemSpan([new(0), new(0), new(0)]);
        result = span.LastIndexOfAnyExcept(Item(0)); Assert.Equal(2, result); // Reference type!
        result = span.LastIndexOfAnyExcept(Item(0), ItemEquality()); Assert.Equal(-1, result);
        result = span.LastIndexOfAnyExcept(ItemPredicate(0)); Assert.Equal(-1, result);

        span = ItemSpan([new(1), new(2), new(0), new(0)]);
        result = span.LastIndexOfAnyExcept(Item(0)); Assert.Equal(3, result); // Reference type!
        result = span.LastIndexOfAnyExcept(Item(0), ItemEquality()); Assert.Equal(1, result);
        result = span.LastIndexOfAnyExcept(ItemPredicate(0)); Assert.Equal(1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IntSpan_ContainsAnyExcept()
    {
        bool result;
        var span = IntSpan([]);
        result = span.ContainsAnyExcept(9); Assert.False(result);
        result = span.ContainsAnyExcept(9, IntEquality()); Assert.False(result);
        result = span.ContainsAnyExcept(IntPredicate(9)); Assert.False(result);

        span = [1, 2, 3];
        result = span.ContainsAnyExcept(0); Assert.True(result);
        result = span.ContainsAnyExcept(0, IntEquality()); Assert.True(result);
        result = span.ContainsAnyExcept(IntPredicate(0)); Assert.True(result);

        span = [0, 0, 0];
        result = span.ContainsAnyExcept(0); Assert.False(result);
        result = span.ContainsAnyExcept(0, IntEquality()); Assert.False(result);
        result = span.ContainsAnyExcept(IntPredicate(0)); Assert.False(result);

        span = [1, 2, 0, 0];
        result = span.ContainsAnyExcept(0); Assert.True(result);
        result = span.ContainsAnyExcept(0, IntEquality()); Assert.True(result);
        result = span.ContainsAnyExcept(IntPredicate(0)); Assert.True(result);
    }

    [Fact]
    public static void Test_ItemSpan_ContainsAnyExcept()
    {
        bool result;
        var span = ItemSpan([]);
        result = span.ContainsAnyExcept(Item(9)); Assert.False(result);
        result = span.ContainsAnyExcept(Item(9), ItemEquality()); Assert.False(result);
        result = span.ContainsAnyExcept(ItemPredicate(9)); Assert.False(result);

        span = ItemSpan([new(1), new(2), new(3)]);
        result = span.ContainsAnyExcept(Item(0)); Assert.True(result);
        result = span.ContainsAnyExcept(Item(0), ItemEquality()); Assert.True(result);
        result = span.ContainsAnyExcept(ItemPredicate(0)); Assert.True(result);

        span = ItemSpan([new(0), new(0), new(0)]);
        result = span.ContainsAnyExcept(Item(0)); Assert.True(result); // Reference type!
        result = span.ContainsAnyExcept(Item(0), ItemEquality()); Assert.False(result);
        result = span.ContainsAnyExcept(ItemPredicate(0)); Assert.False(result);

        span = ItemSpan([new(1), new(2), new(0), new(0)]);
        result = span.ContainsAnyExcept(Item(0)); Assert.True(result); // Reference type!
        result = span.ContainsAnyExcept(Item(0), ItemEquality()); Assert.True(result);
        result = span.ContainsAnyExcept(ItemPredicate(0)); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IntSpan_TrimStart()
    {
        ReadOnlySpan<int> source;
        ReadOnlySpan<int> target;

        source = IntSpan([]);
        target = source.TrimStart(9); Assert.True(target.IsEmpty);
        target = source.TrimStart(9, IntEquality()); Assert.True(target.IsEmpty);
        target = source.TrimStart(IntPredicate(9)); Assert.True(target.IsEmpty);

        source = IntSpan([1, 2, 3]);
        target = source.TrimStart(0); Assert.True(target.SequenceEqual(source));
        target = source.TrimStart(0, IntEquality()); Assert.True(target.SequenceEqual(source));
        target = source.TrimStart(IntPredicate(0)); Assert.True(target.SequenceEqual(source));

        target = source.TrimStart(1); Assert.True(target is [2, 3]);
        target = source.TrimStart(1, IntEquality()); Assert.True(target is [2, 3]);
        target = source.TrimStart(IntPredicate(1)); Assert.True(target is [2, 3]);
    }

    [Fact]
    public static void Test_ItemSpan_TrimStart()
    {
        ReadOnlySpan<Item<int>> source;
        ReadOnlySpan<Item<int>> target;

        source = ItemSpan([]);
        target = source.TrimStart(Item(9)); Assert.True(target.IsEmpty);
        target = source.TrimStart(Item(9), ItemEquality()); Assert.True(target.IsEmpty);
        target = source.TrimStart(ItemPredicate(9)); Assert.True(target.IsEmpty);

        source = ItemSpan([new(1), new(2), new(3)]);
        target = source.TrimStart(Item(0)); Assert.True(target.SequenceEqual(source, ItemEquality()));
        target = source.TrimStart(Item(0), ItemEquality()); Assert.True(target.SequenceEqual(source, ItemEquality()));
        target = source.TrimStart(ItemPredicate(0)); Assert.True(target.SequenceEqual(source, ItemEquality()));

        var expected = ItemSpan([new(2), new(3)]);
        target = source.TrimStart(Item(1)); Assert.False(target.SequenceEqual(expected, ItemEquality()));
        target = source.TrimStart(Item(1), ItemEquality()); Assert.True(target.SequenceEqual(expected, ItemEquality()));
        target = source.TrimStart(ItemPredicate(1)); Assert.True(target.SequenceEqual(expected, ItemEquality()));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IntSpan_TryFind()
    {
        ReadOnlySpan<int> span;
        int value;

        span = [];
        Assert.False(span.TryFind(x => x != 0, out value));

        span = [1, 2, 3];
        Assert.True(span.TryFind(x => x > 1, out value));
        Assert.Equal(2, value);
    }

    [Fact]
    public static void Test_ItemSpan_TryFind()
    {
        ReadOnlySpan<Item<int>> span;
        Item<int> value;

        span = [];
        Assert.False(span.TryFind(x => x.Value != 0, out value));

        span = ItemSpan([new(1), new(2), new(3)]);
        Assert.True(span.TryFind(x => x.Value > 1, out value));
        Assert.Equal(2, value.Value);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IntSpan_TryFindLast()
    {
        ReadOnlySpan<int> span;
        int value;

        span = [];
        Assert.False(span.TryFindLast(x => x != 0, out value));

        span = [1, 2, 3];
        Assert.True(span.TryFindLast(x => x > 1, out value));
        Assert.Equal(3, value);
    }

    [Fact]
    public static void Test_ItemSpan_TryFindLast()
    {
        ReadOnlySpan<Item<int>> span;
        Item<int> value;

        span = [];
        Assert.False(span.TryFindLast(x => x.Value != 0, out value));

        span = ItemSpan([new(1), new(2), new(3)]);
        Assert.True(span.TryFindLast(x => x.Value > 1, out value));
        Assert.Equal(3, value.Value);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IntSpan_TryFindAll()
    {
        ReadOnlySpan<int> span;
        List<int> values;

        span = [];
        Assert.False(span.TryFindAll(x => x != 0, out values));

        span = [1, 2, 3];
        Assert.True(span.TryFindAll(x => x > 1, out values));
        Assert.Equal(2, values.Count);
        Assert.Equal(2, values[0]);
        Assert.Equal(3, values[1]);
    }

    [Fact]
    public static void Test_ItemSpan_TryFindAll()
    {
        ReadOnlySpan<Item<int>> span;
        List<Item<int>> values;

        span = [];
        Assert.False(span.TryFindAll(x => x.Value != 0, out values));

        span = ItemSpan([new(1), new(2), new(3)]);
        Assert.True(span.TryFindAll(x => x.Value > 1, out values));
        Assert.Equal(2, values.Count);
        Assert.Equal(2, values[0].Value);
        Assert.Equal(3, values[1].Value);
    }
}
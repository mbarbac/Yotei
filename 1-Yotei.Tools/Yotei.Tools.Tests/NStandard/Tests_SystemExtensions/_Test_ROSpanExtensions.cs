#pragma warning disable IDE0059, CA1859, IDE0301
namespace Yotei.Tools.Tests;

// TODO ROSpan: public static void Test_OnInt_Clone()
// TODO ROSpan: public static void Test_OnItem_Clone()
// TODO ROSpan: Add(value)
// TODO ROSpan: Insert(value)
// TODO ROSpan: RemoveAt(index)
// TODO ROSpan: RemoveRange(index, count)
// TODO ROSpan: Remove(value)
// TODO ROSpan: RemoveLast(value)
// TODO ROSpan: RemoveAll(value)
// TODO ROSpan: Remove(predicate)
// TODO ROSpan: RemoveLast(predicate)
// TODO ROSpan: RemoveAll(predicate)
// TODO ROSpan: Clear()

// ========================================================
public static partial class Test_ReadOnlySpanExtensions
{
    static ReadOnlySpan<int> IntSpan() => Array.Empty<int>();
    static ReadOnlySpan<int> IntSpan(int[] values) => values.AsSpan();
    static Predicate<int> IntPredicate(int value) => x => x == value;
    static IEqualityComparer<int> IntEquality() => EqualityComparer<int>.Default;
    static IComparer<int> IntComparer() => Comparer<int>.Create((x, y) => x.CompareTo(y));

    // ----------------------------------------------------

    static Item<int> Item(int value) => new(value);
    static ReadOnlySpan<Item<int>> ItemSpan() => [];
    static ReadOnlySpan<Item<int>> ItemSpan(IEnumerable<int> values)
    {
        return values.Select(x => Item(x)).ToArray();
    }
    static ReadOnlySpan<Item<int>> ItemSpan(IEnumerable<Item<int>> values) => values.ToArray();
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
        var source = IntSpan(); Assert.True(source.IsEmpty);
        source = IntSpan([1, 2]); Assert.False(source.IsEmpty);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnInt_IndexOf()
    {
        int result;
        var source = IntSpan();
        result = source.IndexOf(2); Assert.Equal(-1, result);
        result = source.IndexOf(2, IntEquality()); Assert.Equal(-1, result);
        result = source.IndexOf(IntPredicate(2)); Assert.Equal(-1, result);

        source = IntSpan([1, 2, 3, 4]);
        result = source.IndexOf(2); Assert.Equal(1, result);
        result = source.IndexOf(2, IntEquality()); Assert.Equal(1, result);
        result = source.IndexOf(IntPredicate(2)); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_OnItem_IndexOf()
    {
        int result;
        var source = ItemSpan();
        // source.IndexOf(Item(2)); // => CS0311: Item<T> is not IEquatable
        result = source.IndexOf(Item(2), ItemEquality()); Assert.Equal(-1, result);
        result = source.IndexOf(ItemPredicate(2)); Assert.Equal(-1, result);

        source = ItemSpan([1, 2, 3, 4]);
        result = source.IndexOf(Item(2), ItemEquality()); Assert.Equal(1, result);
        result = source.IndexOf(ItemPredicate(2)); Assert.Equal(1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnInt_LastIndexOf()
    {
        int result;
        var source = IntSpan();
        result = source.LastIndexOf(2); Assert.Equal(-1, result);
        result = source.LastIndexOf(2, IntEquality()); Assert.Equal(-1, result);
        result = source.LastIndexOf(IntPredicate(2)); Assert.Equal(-1, result);

        source = IntSpan([1, 2, 3, 4]);
        result = source.LastIndexOf(2); Assert.Equal(1, result);
        result = source.LastIndexOf(2, IntEquality()); Assert.Equal(1, result);
        result = source.LastIndexOf(IntPredicate(2)); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_OnItem_LastIndexOf()
    {
        int result;
        var source = ItemSpan();
        // source.LastIndexOf(Item(2)); // => CS0311: Item<T> is not IEquatable
        result = source.LastIndexOf(Item(2), ItemEquality()); Assert.Equal(-1, result);
        result = source.LastIndexOf(ItemPredicate(2)); Assert.Equal(-1, result);

        source = ItemSpan([1, 2, 3, 4]);
        result = source.LastIndexOf(Item(2), ItemEquality()); Assert.Equal(1, result);
        result = source.LastIndexOf(ItemPredicate(2)); Assert.Equal(1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnInt_IndexesOf()
    {
        List<int> result;
        var source = IntSpan();
        result = source.IndexesOf(2); Assert.Empty(result);
        result = source.IndexesOf(2, IntEquality()); Assert.Empty(result);
        result = source.IndexesOf(IntPredicate(2)); Assert.Empty(result);

        source = IntSpan([1, 2, 3, 2]);
        result = source.IndexesOf(2); Assert.True(result is [1, 3]);
        result = source.IndexesOf(2, IntEquality()); Assert.True(result is [1, 3]);
        result = source.IndexesOf(IntPredicate(2)); Assert.True(result is [1, 3]);
    }

    [Fact]
    public static void Test_OnItem_IndexesOf()
    {
        List<int> result;
        var source = ItemSpan();
        // source.IndexesOf(Item(2)); // => CS0311: Item<T> is not IEquatable
        result = source.IndexesOf(Item(2), ItemEquality()); Assert.Empty(result);
        result = source.IndexesOf(ItemPredicate(2)); Assert.Empty(result);

        source = ItemSpan([1, 2, 3, 2]);
        result = source.IndexesOf(Item(2), ItemEquality()); Assert.True(result is [1, 3]);
        result = source.IndexesOf(ItemPredicate(2)); Assert.True(result is [1, 3]);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnInt_Contains()
    {
        bool result;
        var source = IntSpan();
        result = source.Contains(2); Assert.False(result);
        result = source.Contains(2, IntEquality()); Assert.False(result);
        result = source.Contains(IntPredicate(2)); Assert.False(result);

        source = IntSpan([1, 2, 3, 4]);
        result = source.Contains(2); Assert.True(result);
        result = source.Contains(2, IntEquality()); Assert.True(result);
        result = source.Contains(IntPredicate(2)); Assert.True(result);
    }

    [Fact]
    public static void Test_OnItem_Contains()
    {
        bool result;
        var source = ItemSpan();
        // source.Contains(Item(2)); // => CS0311: Item<T> is not IEquatable
        result = source.Contains(Item(2), ItemEquality()); Assert.False(result);
        result = source.Contains(ItemPredicate(2)); Assert.False(result);

        source = ItemSpan([1, 2, 3, 4]);
        result = source.Contains(Item(2), ItemEquality()); Assert.True(result);
        result = source.Contains(ItemPredicate(2)); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnInt_StartsWith()
    {
        bool result;
        var source = IntSpan();
        result = source.StartsWith(2); Assert.False(result);
        result = source.StartsWith(2, IntEquality()); Assert.False(result);
        result = source.StartsWith(IntPredicate(2)); Assert.False(result);

        source = IntSpan([1, 2]);
        result = source.StartsWith(2); Assert.False(result);
        result = source.StartsWith(2, IntEquality()); Assert.False(result);
        result = source.StartsWith(IntPredicate(2)); Assert.False(result);

        source = IntSpan([2, 3]);
        result = source.StartsWith(2); Assert.True(result);
        result = source.StartsWith(2, IntEquality()); Assert.True(result);
        result = source.StartsWith(IntPredicate(2)); Assert.True(result);
    }

    [Fact]
    public static void Test_OnItem_StartsWith()
    {
        bool result;
        var source = ItemSpan();
        // source.StartsWith(Item(2)); // => CS0311: Item<T> is not IEquatable
        result = source.StartsWith(Item(2), ItemEquality()); Assert.False(result);
        result = source.StartsWith(ItemPredicate(2)); Assert.False(result);

        source = ItemSpan([1, 2]);
        result = source.StartsWith(Item(2), ItemEquality()); Assert.False(result);
        result = source.StartsWith(ItemPredicate(2)); Assert.False(result);

        source = ItemSpan([2, 3]);
        result = source.StartsWith(Item(2), ItemEquality()); Assert.True(result);
        result = source.StartsWith(ItemPredicate(2)); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnInt_EndsWith()
    {
        bool result;
        var source = IntSpan();
        result = source.EndsWith(2); Assert.False(result);
        result = source.EndsWith(2, IntEquality()); Assert.False(result);
        result = source.EndsWith(IntPredicate(2)); Assert.False(result);

        source = IntSpan([2, 1]);
        result = source.EndsWith(2); Assert.False(result);
        result = source.EndsWith(2, IntEquality()); Assert.False(result);
        result = source.EndsWith(IntPredicate(2)); Assert.False(result);

        source = IntSpan([3, 2]);
        result = source.EndsWith(2); Assert.True(result);
        result = source.EndsWith(2, IntEquality()); Assert.True(result);
        result = source.EndsWith(IntPredicate(2)); Assert.True(result);
    }

    [Fact]
    public static void Test_OnItem_EndsWith()
    {
        bool result;
        var source = ItemSpan();
        // source.EndsWith(Item(2)); // => CS0311: Item<T> is not IEquatable
        result = source.EndsWith(Item(2), ItemEquality()); Assert.False(result);
        result = source.EndsWith(ItemPredicate(2)); Assert.False(result);

        source = ItemSpan([2, 1]);
        result = source.EndsWith(Item(2), ItemEquality()); Assert.False(result);
        result = source.EndsWith(ItemPredicate(2)); Assert.False(result);

        source = ItemSpan([3, 2]);
        result = source.EndsWith(Item(2), ItemEquality()); Assert.True(result);
        result = source.EndsWith(ItemPredicate(2)); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnInt_TrimStart()
    {
        var result = IntSpan();
        var source = IntSpan();
        result = source.TrimStart(); Assert.True(result.IsEmpty);
        result = source.TrimStart(IntEquality()); Assert.True(result.IsEmpty);

        source = IntSpan([0, 0, 1]);
        result = source.TrimStart(); Assert.True(result is [1]);
        result = source.TrimStart(IntEquality()); Assert.True(result is [1]);

        source = IntSpan([1, 2]);
        result = source.TrimStart(9); Assert.True(result is [1, 2]);
        result = source.TrimStart(9, IntEquality()); Assert.True(result is [1, 2]);
        result = source.TrimStart(IntPredicate(9)); Assert.True(result is [1, 2]);

        source = IntSpan([9, 9, 1, 2, 9]);
        result = source.TrimStart(9); Assert.True(result is [1, 2, 9]);
        result = source.TrimStart(9, IntEquality()); Assert.True(result is [1, 2, 9]);
        result = source.TrimStart(IntPredicate(9)); Assert.True(result is [1, 2, 9]);
    }

    [Fact]
    public static void Test_OnItem_TrimStart()
    {
        var result = ItemSpan();
        var source = ItemSpan();
        // source.TrimStart(); // => CS0311: Item<T> is not IEquatable
        result = source.TrimStart(ItemEquality()); Assert.True(result.IsEmpty);

        source = ItemSpan([0, 0, 1]);
        result = source.TrimStart(ItemEquality()); Assert.False(result.SequenceEqual([new(1)], ItemEquality()));

        source = ItemSpan([null!, null!, new(1)]);
        result = source.TrimStart(ItemEquality()); Assert.True(result.SequenceEqual([new(1)], ItemEquality()));

        source = ItemSpan([1, 2]);
        result = source.TrimStart(Item(9), ItemEquality()); Assert.True(result.SequenceEqual([new(1), new(2)], ItemEquality()));
        result = source.TrimStart(ItemPredicate(9)); Assert.True(result.SequenceEqual([new(1), new(2)], ItemEquality()));

        source = ItemSpan([9, 9, 1, 2]);
        result = source.TrimStart(Item(9), ItemEquality()); Assert.True(result.SequenceEqual([new(1), new(2)], ItemEquality()));
        result = source.TrimStart(ItemPredicate(9)); Assert.True(result.SequenceEqual([new(1), new(2)], ItemEquality()));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnInt_TrimEnd()
    {
        var result = IntSpan();
        var source = IntSpan();
        result = source.TrimEnd(); Assert.True(result.IsEmpty);
        result = source.TrimEnd(IntEquality()); Assert.True(result.IsEmpty);

        source = IntSpan([1, 0, 0]);
        result = source.TrimEnd(); Assert.True(result is [1]);
        result = source.TrimEnd(IntEquality()); Assert.True(result is [1]);

        source = IntSpan([1, 2]);
        result = source.TrimEnd(9); Assert.True(result is [1, 2]);
        result = source.TrimEnd(9, IntEquality()); Assert.True(result is [1, 2]);
        result = source.TrimEnd(IntPredicate(9)); Assert.True(result is [1, 2]);

        source = IntSpan([9, 1, 2, 9]);
        result = source.TrimEnd(9); Assert.True(result is [9, 1, 2]);
        result = source.TrimEnd(9, IntEquality()); Assert.True(result is [9, 1, 2]);
        result = source.TrimEnd(IntPredicate(9)); Assert.True(result is [9, 1, 2]);
    }

    [Fact]
    public static void Test_OnItem_TrimEnd()
    {
        var result = ItemSpan();
        var source = ItemSpan();
        // source.TrimEnd(); // => CS0311: Item<T> is not IEquatable
        result = source.TrimEnd(ItemEquality()); Assert.True(result.IsEmpty);

        source = ItemSpan([0, 0, 1]);
        result = source.TrimEnd(ItemEquality()); Assert.False(result.SequenceEqual([new(1)], ItemEquality()));

        source = ItemSpan([new(1), null!, null!]);
        result = source.TrimEnd(ItemEquality()); Assert.True(result.SequenceEqual([new(1)], ItemEquality()));

        source = ItemSpan([1, 2]);
        result = source.TrimEnd(Item(9), ItemEquality()); Assert.True(result.SequenceEqual([new(1), new(2)], ItemEquality()));
        result = source.TrimEnd(ItemPredicate(9)); Assert.True(result.SequenceEqual([new(1), new(2)], ItemEquality()));

        source = ItemSpan([9, 1, 2, 9]);
        result = source.TrimEnd(Item(9), ItemEquality()); Assert.True(result.SequenceEqual([new(9), new(1), new(2)], ItemEquality()));
        result = source.TrimEnd(ItemPredicate(9)); Assert.True(result.SequenceEqual([new(9), new(1), new(2)], ItemEquality()));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnInt_Trim()
    {
        var result = IntSpan();
        var source = IntSpan();
        result = source.Trim(); Assert.True(result.IsEmpty);
        result = source.Trim(IntEquality()); Assert.True(result.IsEmpty);

        source = IntSpan([0, 0, 1, 0, 0]);
        result = source.Trim(); Assert.True(result is [1]);
        result = source.Trim(IntEquality()); Assert.True(result is [1]);

        source = IntSpan([1, 2]);
        result = source.Trim(9); Assert.True(result is [1, 2]);
        result = source.Trim(9, IntEquality()); Assert.True(result is [1, 2]);
        result = source.Trim(IntPredicate(9)); Assert.True(result is [1, 2]);

        source = IntSpan([9, 1, 2, 9]);
        result = source.Trim(9); Assert.True(result is [1, 2]);
        result = source.Trim(9, IntEquality()); Assert.True(result is [1, 2]);
        result = source.Trim(IntPredicate(9)); Assert.True(result is [1, 2]);
    }

    [Fact]
    public static void Test_OnItem_Trim()
    {
        var result = ItemSpan();
        var source = ItemSpan();
        // source.Trim(); // => CS0311: Item<T> is not IEquatable
        result = source.Trim(ItemEquality()); Assert.True(result.IsEmpty);

        source = ItemSpan([0, 0, 1, 0, 0]);
        result = source.Trim(ItemEquality()); Assert.False(result.SequenceEqual([new(1)], ItemEquality()));

        source = ItemSpan([null!, null!, new(1), null!, null!]);
        result = source.Trim(ItemEquality()); Assert.True(result.SequenceEqual([new(1)], ItemEquality()));

        source = ItemSpan([1, 2]);
        result = source.Trim(Item(9), ItemEquality()); Assert.True(result.SequenceEqual([new(1), new(2)], ItemEquality()));
        result = source.Trim(ItemPredicate(9)); Assert.True(result.SequenceEqual([new(1), new(2)], ItemEquality()));

        source = ItemSpan([9, 1, 2, 9]);
        result = source.Trim(Item(9), ItemEquality()); Assert.True(result.SequenceEqual([new(1), new(2)], ItemEquality()));
        result = source.Trim(ItemPredicate(9)); Assert.True(result.SequenceEqual([new(1), new(2)], ItemEquality()));
    }
}
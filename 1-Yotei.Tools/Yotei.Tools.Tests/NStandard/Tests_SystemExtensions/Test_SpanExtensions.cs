#pragma warning disable IDE0059, CA1859, IDE0301
namespace Yotei.Tools.Tests;

// ========================================================
public static partial class Test_SpanExtensions
{
    static Span<int> IntSpan() => [];
    static Span<int> IntSpan(int[] values) => values.AsSpan();
    static Predicate<int> IntPredicate(int value) => x => x == value;
    static IEqualityComparer<int> IntEquality() => EqualityComparer<int>.Default;
    static IComparer<int> IntComparer() => Comparer<int>.Create((x, y) => x.CompareTo(y));

    // ----------------------------------------------------

    static Item<int> Item(int value) => new(value);
    static Span<Item<int>> ItemSpan() => [];
    static Span<Item<int>> ItemSpan(IEnumerable<int> values)
    {
        return values.Select(x => Item(x)).ToArray();
    }
    static Span<Item<int>> ItemSpan(IEnumerable<Item<int>> values) => values.ToArray();
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
    public static void Inherit_IsEmpty()
    {
        var source = IntSpan(); Assert.True(source.IsEmpty);
        source = IntSpan([1, 2]); Assert.False(source.IsEmpty);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnInt_Clone()
    {
        var source = IntSpan();
        var target = source.Clone(); Assert.True(target.IsEmpty);

        source = IntSpan([1, 2]);
        target = source.Clone(); Assert.True(target is [1, 2]);
    }

    [Fact]
    public static void Test_OnItem_Clone()
    {
        var source = ItemSpan();
        var target = source.Clone(); Assert.True(target.IsEmpty);

        source = ItemSpan([1, 2]);
        target = source.Clone(); Assert.True(target.SequenceEqual(ItemEnumerable([1, 2]), ItemEquality()));
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

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnInt_Add()
    {
        var source = IntSpan();
        var target = source.Add(9); Assert.True(target is [9]);

        source = IntSpan([1, 2]);
        target = source.Add(9); Assert.True(target is [1, 2, 9]);
    }

    [Fact]
    public static void Test_OnItem_Add()
    {
        var source = ItemSpan();
        var target = source.Add(Item(9)); Assert.True(target.SequenceEqual(ItemEnumerable([9]), ItemEquality()));

        source = ItemSpan([1, 2]);
        target = source.Add(Item(9)); Assert.True(target.SequenceEqual(ItemEnumerable([1, 2, 9]), ItemEquality()));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnInt_Insert()
    {
        var source = IntSpan();
        var target = source.Insert(0, 9); Assert.True(target is [9]);

        source = IntSpan([1, 2]);
        target = source.Insert(1, 9); Assert.True(target is [1, 9, 2]);
        target = source.Insert(2, 9); Assert.True(target is [1, 2, 9]);

        try { source.Insert(-1, 0); Assert.Fail(""); } catch (ArgumentOutOfRangeException) { }
        try { source.Insert(3, 0); Assert.Fail(""); } catch (ArgumentOutOfRangeException) { }
    }

    [Fact]
    public static void Test_OnItem_Insert()
    {
        var source = ItemSpan();
        var target = source.Insert(0, Item(9)); target.SequenceEqual(ItemEnumerable([9]), ItemEquality());

        source = ItemSpan([1, 2]);
        target = source.Insert(1, Item(9)); Assert.True(target.SequenceEqual(ItemEnumerable([1, 9, 2]), ItemEquality()));
        target = source.Insert(2, Item(9)); Assert.True(target.SequenceEqual(ItemEnumerable([1, 2, 9]), ItemEquality()));

        try { source.Insert(-1, Item(9)); Assert.Fail(""); } catch (ArgumentOutOfRangeException) { }
        try { source.Insert(3, Item(9)); Assert.Fail(""); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_RemoveAt()
    {
        var source = IntSpan([1]);
        var target = source.RemoveAt(0); Assert.True(target.IsEmpty);

        source = IntSpan([1, 2]);
        target = source.RemoveAt(0); Assert.True(target is [2]);

        source = IntSpan([1, 2]);
        target = source.RemoveAt(1); Assert.True(target is [1]);

        try { source.RemoveAt(-1); Assert.Fail(""); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveAt(2); Assert.Fail(""); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_RemoveRange()
    {
        var source = IntSpan();
        var target = source.RemoveRange(0, 0); Assert.True(target.IsEmpty);

        source = IntSpan([1, 2, 3, 4]);
        try { source.RemoveRange(-1, 0); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(4, 0); } catch (ArgumentOutOfRangeException) { }

        target = source.RemoveRange(0, 0); Assert.True(target is [1, 2, 3, 4]);
        target = source.RemoveRange(0, 1); Assert.True(target is [2, 3, 4]);
        target = source.RemoveRange(0, 2); Assert.True(target is [3, 4]);
        target = source.RemoveRange(0, 3); Assert.True(target is [4]);
        target = source.RemoveRange(0, 4); Assert.True(target.IsEmpty);
        try { source.RemoveRange(0, 5); } catch (ArgumentOutOfRangeException) { }

        target = source.RemoveRange(1, 0); Assert.True(target is [1, 2, 3, 4]);
        target = source.RemoveRange(1, 1); Assert.True(target is [1, 3, 4]);
        target = source.RemoveRange(1, 2); Assert.True(target is [1, 4]);
        target = source.RemoveRange(1, 3); Assert.True(target is [1]);
        try { source.RemoveRange(1, 4); } catch (ArgumentOutOfRangeException) { }

        target = source.RemoveRange(2, 0); Assert.True(target is [1, 2, 3, 4]);
        target = source.RemoveRange(2, 1); Assert.True(target is [1, 2, 4]);
        target = source.RemoveRange(2, 2); Assert.True(target is [1, 2]);
        try { source.RemoveRange(2, 3); } catch (ArgumentOutOfRangeException) { }

        target = source.RemoveRange(3, 0); Assert.True(target is [1, 2, 3, 4]);
        target = source.RemoveRange(3, 1); Assert.True(target is [1, 2, 3]);
        try { source.RemoveRange(3, 2); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnInt_Remove_Value()
    {
        var source = IntSpan();
        var target = source.Remove(9); Assert.True(target.IsEmpty);

        source = IntSpan([1, 9, 2, 9]);
        target = source.Remove(0); Assert.True(target is [1, 9, 2, 9]);
        target = source.Remove(9); Assert.True(target is [1, 2, 9]);

        target = source.Remove(0, IntEquality()); Assert.True(target is [1, 9, 2, 9]);
        target = source.Remove(9, IntEquality()); Assert.True(target is [1, 2, 9]);

        target = source.Remove(IntPredicate(0)); Assert.True(target is [1, 9, 2, 9]);
        target = source.Remove(IntPredicate(9)); Assert.True(target is [1, 2, 9]);
    }

    [Fact]
    public static void Test_OnItem_Remove_Value()
    {
        var target = ItemSpan();
        var source = ItemSpan();
        // source.Remove(Item(9)); // => CS0311: Item<T> is not IEquatable

        source = ItemSpan([1, 9, 2, 9]);
        target = source.Remove(Item(0), ItemEquality()); Assert.True(target.SequenceEqual(ItemEnumerable([1, 9, 2, 9]), ItemEquality()));
        target = source.Remove(Item(9), ItemEquality()); Assert.True(target.SequenceEqual(ItemEnumerable([1, 2, 9]), ItemEquality()));

        target = source.Remove(ItemPredicate(0)); Assert.True(target.SequenceEqual(ItemEnumerable([1, 9, 2, 9]), ItemEquality()));
        target = source.Remove(ItemPredicate(9)); Assert.True(target.SequenceEqual(ItemEnumerable([1, 2, 9]), ItemEquality()));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnInt_RemoveLast_Value()
    {
        var source = IntSpan();
        var target = source.RemoveLast(9); Assert.True(target.IsEmpty);

        source = IntSpan([1, 9, 2, 9]);
        target = source.RemoveLast(0); Assert.True(target is [1, 9, 2, 9]);
        target = source.RemoveLast(9); Assert.True(target is [1, 9, 2]);

        target = source.RemoveLast(0, IntEquality()); Assert.True(target is [1, 9, 2, 9]);
        target = source.RemoveLast(9, IntEquality()); Assert.True(target is [1, 9, 2]);

        target = source.RemoveLast(IntPredicate(0)); Assert.True(target is [1, 9, 2, 9]);
        target = source.RemoveLast(IntPredicate(9)); Assert.True(target is [1, 9, 2]);
    }

    [Fact]
    public static void Test_OnItem_RemoveLast_Value()
    {
        var target = ItemSpan();
        var source = ItemSpan();
        // source.RemoveLast(Item(9)); // => CS0311: Item<T> is not IEquatable

        source = ItemSpan([1, 9, 2, 9]);
        target = source.RemoveLast(Item(0), ItemEquality()); Assert.True(target.SequenceEqual(ItemEnumerable([1, 9, 2, 9]), ItemEquality()));
        target = source.RemoveLast(Item(9), ItemEquality()); Assert.True(target.SequenceEqual(ItemEnumerable([1, 9, 2]), ItemEquality()));

        target = source.RemoveLast(ItemPredicate(0)); Assert.True(target.SequenceEqual(ItemEnumerable([1, 9, 2, 9]), ItemEquality()));
        target = source.RemoveLast(ItemPredicate(9)); Assert.True(target.SequenceEqual(ItemEnumerable([1, 9, 2]), ItemEquality()));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnInt_RemoveAll_Value()
    {
        var source = IntSpan();
        var target = source.RemoveAll(9); Assert.True(target.IsEmpty);

        source = IntSpan([1, 9, 2, 9]);
        target = source.RemoveAll(0); Assert.True(target is [1, 9, 2, 9]);
        target = source.RemoveAll(9); Assert.True(target is [1, 2]);

        target = source.RemoveAll(0, IntEquality()); Assert.True(target is [1, 9, 2, 9]);
        target = source.RemoveAll(9, IntEquality()); Assert.True(target is [1, 2]);

        target = source.RemoveAll(IntPredicate(0)); Assert.True(target is [1, 9, 2, 9]);
        target = source.RemoveAll(IntPredicate(9)); Assert.True(target is [1, 2]);
    }

    [Fact]
    public static void Test_OnItem_RemoveAll_Value()
    {
        var target = ItemSpan();
        var source = ItemSpan();
        // source.RemoveAll(Item(9)); // => CS0311: Item<T> is not IEquatable

        source = ItemSpan([1, 9, 2, 9]);
        target = source.RemoveAll(Item(0), ItemEquality()); Assert.True(target.SequenceEqual(ItemEnumerable([1, 9, 2, 9]), ItemEquality()));
        target = source.RemoveAll(Item(9), ItemEquality()); Assert.True(target.SequenceEqual(ItemEnumerable([1, 2]), ItemEquality()));

        target = source.RemoveAll(ItemPredicate(0)); Assert.True(target.SequenceEqual(ItemEnumerable([1, 9, 2, 9]), ItemEquality()));
        target = source.RemoveAll(ItemPredicate(9)); Assert.True(target.SequenceEqual(ItemEnumerable([1, 2]), ItemEquality()));
    }
}
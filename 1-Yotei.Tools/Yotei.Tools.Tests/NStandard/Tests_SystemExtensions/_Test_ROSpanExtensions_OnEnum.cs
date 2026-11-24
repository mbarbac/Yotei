namespace Yotei.Tools.Tests;

// TODO ROSpan: AddRange(enum)
// TODO ROSpan: InsertRange(enum)
// TODO ROSpan: Remove(enum)
// TODO ROSpan: RemoveLast(enum)
// TODO ROSpan: RemoveAll(enum)

// ========================================================
public static partial class Test_ReadOnlySpanExtensions
{
    static IEnumerable<int> IntEnumerable() => [];

    static IEnumerable<Item<int>> ItemEnumerable() => [];
    static IEnumerable<Item<int>> ItemEnumerable(IEnumerable<int> values) => values.Select(x => Item(x));

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnEnum_SequenceCompareTo_Ints()
    {
        int result;
        var source = IntSpan();
        var target = IntEnumerable();
        result = source.SequenceCompareTo(target); Assert.Equal(0, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(0, result);

        source = [1, 2];
        target = [1, 2];
        result = source.SequenceCompareTo(target); Assert.Equal(0, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(0, result);

        source = [1, 2, 3];
        target = [1, 2];
        result = source.SequenceCompareTo(target); Assert.Equal(+1, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(+1, result);

        source = IntSpan([1]);
        target = [1, 2];
        result = source.SequenceCompareTo(target); Assert.Equal(-1, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(-1, result);

        source = IntSpan([1, 3]);
        target = [1, 2];
        result = source.SequenceCompareTo(target); Assert.Equal(+1, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(+1, result);

        source = IntSpan([1, 1]);
        target = [1, 2];
        result = source.SequenceCompareTo(target); Assert.Equal(-1, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(-1, result);
    }

    [Fact]
    public static void Test_OnEnum_SequenceCompareTo_Items()
    {
        int result;
        var source = ItemSpan();
        var target = ItemEnumerable();
        // source.SequenceCompareTo(target); // => CS0311: Item<T> is not IEquatable
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(0, result);

        source = ItemSpan([1, 2]);
        target = [new(1), new(2)];
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(0, result);

        source = ItemSpan([1, 2, 3]);
        target = ItemEnumerable([1, 2]);
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(+1, result);

        source = ItemSpan([1]);
        target = ItemEnumerable([1, 2]);
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(-1, result);

        source = ItemSpan([1, 3]);
        target = ItemEnumerable([1, 2]);
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(+1, result);

        source = ItemSpan([1, 1]);
        target = ItemEnumerable([1, 2]);
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(-1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnEnum_SequenceEquals_Ints()
    {
        bool result;
        var source = IntSpan();
        var target = IntEnumerable();
        result = source.SequenceEqual(target); Assert.True(result);
        result = source.SequenceEqual(target, IntEquality()); Assert.True(result);

        source = IntSpan([1, 2]);
        target = [1, 2];
        result = source.SequenceEqual(target); Assert.True(result);
        result = source.SequenceEqual(target, IntEquality()); Assert.True(result);

        source = IntSpan([1]);
        target = [1, 2];
        result = source.SequenceEqual(target); Assert.False(result);
        result = source.SequenceEqual(target, IntEquality()); Assert.False(result);
    }

    [Fact]
    public static void Test_OnEnum_SequenceEquals_Items()
    {
        bool result;
        var source = ItemSpan();
        var target = ItemEnumerable();
        // source.SequenceEqual(target); // => CS0311: Item<T> is not IEquatable
        result = source.SequenceEqual(target, ItemEquality()); Assert.True(result);

        source = ItemSpan([1, 2]);
        target = ItemEnumerable([1, 2]);
        result = source.SequenceEqual(target, ItemEquality()); Assert.True(result);

        source = ItemSpan([1]);
        target = ItemEnumerable([1, 2]);
        result = source.SequenceEqual(target, ItemEquality()); Assert.False(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnEnum_IndexOf_Range_Ints()
    {
        int result;
        var source = IntSpan();
        var values = IntEnumerable();
        result = source.IndexOf(values); Assert.Equal(0, result); // Empties are ok!
        result = source.IndexOf(values, IntEquality()); Assert.Equal(0, result);

        source = IntSpan([3, 1, 2]);
        values = [3, 4];
        result = source.IndexOf(values); Assert.Equal(-1, result);
        result = source.IndexOf(values, IntEquality()); Assert.Equal(-1, result);

        source = IntSpan([1, 2, 3]);
        values = [3, 4];
        result = source.IndexOf(values); Assert.Equal(-1, result);
        result = source.IndexOf(values, IntEquality()); Assert.Equal(-1, result);

        source = IntSpan([1, 2, 3, 4]);
        values = [2, 3];
        result = source.IndexOf(values); Assert.Equal(1, result);
        result = source.IndexOf(values, IntEquality()); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_OnEnum_IndexOf_Range_Items()
    {
        int result;
        var source = ItemSpan();
        var values = ItemEnumerable();
        // source.IndexOf(values); // => CS0311: Item<T> is not IEquatable
        result = source.IndexOf(values, ItemEquality()); Assert.Equal(0, result);

        source = ItemSpan([3, 1, 2]);
        values = ItemEnumerable([3, 4]);
        result = source.IndexOf(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([1, 2, 3]);
        values = ItemEnumerable([3, 4]);
        result = source.IndexOf(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([1, 2, 3, 4]);
        values = ItemEnumerable([2, 3]);
        result = source.IndexOf(values, ItemEquality()); Assert.Equal(1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnEnum_LastIndexOf_Range_Ints()
    {
        int result;
        var source = IntSpan();
        var values = IntEnumerable();
        result = source.LastIndexOf(values); Assert.Equal(0, result); // Empties are ok!
        result = source.LastIndexOf(values, IntEquality()); Assert.Equal(0, result);

        source = IntSpan([3, 1, 2]);
        values = [3, 4];
        result = source.LastIndexOf(values); Assert.Equal(-1, result);
        result = source.LastIndexOf(values, IntEquality()); Assert.Equal(-1, result);

        source = IntSpan([1, 2, 3]);
        values = [3, 4];
        result = source.LastIndexOf(values); Assert.Equal(-1, result);
        result = source.LastIndexOf(values, IntEquality()); Assert.Equal(-1, result);

        source = IntSpan([1, 2, 3, 4, 5]);
        values = [3, 4];
        result = source.LastIndexOf(values); Assert.Equal(2, result);
        result = source.LastIndexOf(values, IntEquality()); Assert.Equal(2, result);
    }

    [Fact]
    public static void Test_OnEnum_LastIndexOf_Range_Items()
    {
        int result;
        var source = ItemSpan();
        var values = ItemEnumerable();
        // source.LastIndexOf(values); // => CS0311: Item<T> is not IEquatable
        result = source.LastIndexOf(values, ItemEquality()); Assert.Equal(0, result);

        source = ItemSpan([3, 1, 2]);
        values = ItemEnumerable([3, 4]);
        result = source.LastIndexOf(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([1, 2, 3]);
        values = ItemEnumerable([3, 4]);
        result = source.LastIndexOf(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([1, 2, 3, 4, 5]);
        values = ItemEnumerable([3, 4]);
        result = source.LastIndexOf(values, ItemEquality()); Assert.Equal(2, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnEnum_Contains_Range_Ints()
    {
        bool result;
        var source = IntSpan();
        var values = IntEnumerable();
        result = source.Contains(values); Assert.True(result); // Empties are ok!
        result = source.Contains(values, IntEquality()); Assert.True(result);

        source = IntSpan([3, 1, 2]);
        values = [3, 4];
        result = source.Contains(values); Assert.False(result);
        result = source.Contains(values, IntEquality()); Assert.False(result);

        source = IntSpan([1, 2, 3]);
        values = [3, 4];
        result = source.Contains(values); Assert.False(result);
        result = source.Contains(values, IntEquality()); Assert.False(result);

        source = IntSpan([1, 2, 3, 4, 5]);
        values = [3, 4];
        result = source.Contains(values); Assert.True(result);
        result = source.Contains(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_OnEnum_ContainsRange_Items()
    {
        bool result;
        var source = ItemSpan();
        var values = ItemEnumerable();
        // source.Contains(values); // => CS0311: Item<T> is not IEquatable
        result = source.Contains(values, ItemEquality()); Assert.True(result); // Empties are ok!

        source = ItemSpan([3, 1, 2]);
        values = ItemEnumerable([3, 4]);
        result = source.Contains(values, ItemEquality()); Assert.False(result);

        source = ItemSpan([1, 2, 3]);
        values = ItemEnumerable([3, 4]);
        result = source.Contains(values, ItemEquality()); Assert.False(result);

        source = ItemSpan([1, 2, 3, 4, 5]);
        values = ItemEnumerable([3, 4]);
        result = source.Contains(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnEnum_StartsWith_Range_Ints()
    {
        bool result;
        var source = IntSpan();
        var values = IntEnumerable();
        result = source.StartsWith(values); Assert.True(result); // Empties are ok!
        result = source.StartsWith(values, IntEquality()); Assert.True(result);

        source = IntSpan([1, 2, 3]);
        values = [1, 4];
        result = source.StartsWith(values); Assert.False(result);
        result = source.StartsWith(values, IntEquality()); Assert.False(result);

        source = IntSpan([1, 2, 3, 4]);
        values = [1, 2];
        result = source.StartsWith(values); Assert.True(result);
        result = source.StartsWith(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_OnEnum_StartsWith_Range_Items()
    {
        bool result;
        var source = ItemSpan();
        var values = ItemEnumerable();
        // source.StartsWith(values); // => CS0311: Item<T> is not IEquatable
        result = source.StartsWith(values, ItemEquality()); Assert.True(result);

        source = ItemSpan([1, 2, 3]);
        values = ItemEnumerable([1, 4]);
        result = source.StartsWith(values, ItemEquality()); Assert.False(result);

        source = ItemSpan([1, 2, 3, 4]);
        values = ItemEnumerable([1, 2]);
        result = source.StartsWith(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnEnum_EndsWith_Range_Ints()
    {
        bool result;
        var source = IntSpan();
        var values = IntEnumerable();
        result = source.EndsWith(values); Assert.True(result); // Empties are ok!
        result = source.EndsWith(values, IntEquality()); Assert.True(result);

        source = IntSpan([1, 2, 3]);
        values = [2, 4];
        result = source.EndsWith(values); Assert.False(result);
        result = source.EndsWith(values, IntEquality()); Assert.False(result);

        source = IntSpan([1, 2, 3]);
        values = [3, 4];
        result = source.EndsWith(values); Assert.False(result);
        result = source.EndsWith(values, IntEquality()); Assert.False(result);

        source = IntSpan([1, 2, 3, 4]);
        values = [3, 4];
        result = source.EndsWith(values); Assert.True(result);
        result = source.EndsWith(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_OnEnum_EndsWith_Range_Items()
    {
        bool result;
        var source = ItemSpan();
        var values = ItemEnumerable();
        // source.EndsWith(values); // => CS0311: Item<T> is not IEquatable
        result = source.EndsWith(values, ItemEquality()); Assert.True(result);

        source = ItemSpan([1, 2, 3]);
        values = ItemEnumerable([2, 4]);
        result = source.EndsWith(values, ItemEquality()); Assert.False(result);

        source = ItemSpan([1, 2, 3]);
        values = ItemEnumerable([3, 4]);
        result = source.EndsWith(values, ItemEquality()); Assert.False(result);

        source = ItemSpan([1, 2, 3, 4]);
        values = ItemEnumerable([3, 4]);
        result = source.EndsWith(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnEnum_IndexOfAny_Ints()
    {
        int result;
        var source = IntSpan();
        var values = IntEnumerable();
        result = source.IndexOfAny(values); Assert.Equal(-1, result);
        result = source.IndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        source = IntSpan([1, 2]);
        values = [3, 4];
        result = source.IndexOfAny(values); Assert.Equal(-1, result);
        result = source.IndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        source = IntSpan([0, 1, 2, 3]);
        values = [4, 1, 5];
        result = source.IndexOfAny(values); Assert.Equal(1, result);
        result = source.IndexOfAny(values, IntEquality()); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_OnEnum_IndexOfAny_Items()
    {
        int result;
        var source = ItemSpan();
        var values = ItemEnumerable();
        // source.IndexOfAny(values); // => CS0311: Item<T> is not IEquatable
        result = source.IndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([1, 2]);
        values = ItemEnumerable([3, 4]);
        result = source.IndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([0, 1, 2, 3]);
        values = ItemEnumerable([4, 1, 5]);
        result = source.IndexOfAny(values, ItemEquality()); Assert.Equal(1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnEnum_LastIndexOfAny_Ints()
    {
        int result;
        var source = IntSpan();
        var values = IntEnumerable();
        result = source.LastIndexOfAny(values); Assert.Equal(-1, result);
        result = source.LastIndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        source = IntSpan([1, 2]);
        values = [3, 4];
        result = source.LastIndexOfAny(values); Assert.Equal(-1, result);
        result = source.LastIndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        source = IntSpan([3, 2, 1, 0]);
        values = [4, 1, 5];
        result = source.LastIndexOfAny(values); Assert.Equal(2, result);
        result = source.LastIndexOfAny(values, IntEquality()); Assert.Equal(2, result);
    }

    [Fact]
    public static void Test_OnEnum_LastIndexOfAny_Items()
    {
        int result;
        var source = ItemSpan();
        var values = ItemEnumerable();
        // source.ContainsAny(values); // => CS0311: Item<T> is not IEquatable
        result = source.LastIndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([1, 2]);
        values = ItemEnumerable([3, 4]);
        result = source.LastIndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([3, 2, 1, 0]);
        values = ItemEnumerable([4, 1, 5]);
        result = source.LastIndexOfAny(values, ItemEquality()); Assert.Equal(2, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnEnum_ContainsAny_Ints()
    {
        bool result;
        var source = IntSpan();
        var values = IntEnumerable();
        result = source.ContainsAny(values); Assert.False(result);
        result = source.ContainsAny(values, IntEquality()); Assert.False(result);

        source = IntSpan([1, 2, 3]);
        values = [4, 5];
        result = source.ContainsAny(values); Assert.False(result);
        result = source.ContainsAny(values, IntEquality()); Assert.False(result);

        source = IntSpan([1, 2, 3]);
        values = [4, 1];
        result = source.ContainsAny(values); Assert.True(result);
        result = source.ContainsAny(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_OnEnum_ContainsAny_Items()
    {
        bool result;
        var source = ItemSpan();
        var values = ItemEnumerable();
        // source.ContainsAny(values); // => CS0311: Item<T> is not IEquatable
        result = source.ContainsAny(values, ItemEquality()); Assert.False(result);

        source = ItemSpan([1, 2, 3]);
        values = ItemEnumerable([4, 5]);
        result = source.ContainsAny(values, ItemEquality()); Assert.False(result);

        source = ItemSpan([1, 2, 3]);
        values = ItemEnumerable([4, 1]);
        result = source.ContainsAny(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnEnum_IndexOfAnyExcept_Ints()
    {
        int result;
        var source = IntSpan();
        var values = IntEnumerable();
        result = source.IndexOfAnyExcept(values); Assert.Equal(-1, result);
        result = source.IndexOfAnyExcept(values, IntEquality()); Assert.Equal(-1, result);

        source = [1, 2, 3, 4];
        values = [8, 9];
        result = source.IndexOfAnyExcept(values); Assert.Equal(0, result);
        result = source.IndexOfAnyExcept(values, IntEquality()); Assert.Equal(0, result);

        values = [2, 1];
        result = source.IndexOfAnyExcept(values); Assert.Equal(2, result);
        result = source.IndexOfAnyExcept(values, IntEquality()); Assert.Equal(2, result);
    }

    [Fact]
    public static void Test_OnEnum_IndexOfAnyExcept_Items()
    {
        int result;
        var source = ItemSpan();
        var values = ItemEnumerable();
        // source.IndexOfAny(values); // => CS0311: Item<T> is not IEquatable
        result = source.IndexOfAnyExcept(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([1, 2, 3, 4]);
        values = ItemEnumerable([8, 9]);
        result = source.IndexOfAnyExcept(values, ItemEquality()); Assert.Equal(0, result);

        values = ItemEnumerable([2, 1]);
        result = source.IndexOfAnyExcept(values, ItemEquality()); Assert.Equal(2, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnEnum_LastIndexOfAnyExcept_Ints()
    {
        int result;
        var source = IntSpan();
        var values = IntEnumerable();
        result = source.LastIndexOfAnyExcept(values); Assert.Equal(-1, result);
        result = source.LastIndexOfAnyExcept(values, IntEquality()); Assert.Equal(-1, result);

        source = [3, 4, 1, 2];
        values = [8, 9];
        result = source.LastIndexOfAnyExcept(values); Assert.Equal(3, result);
        result = source.LastIndexOfAnyExcept(values, IntEquality()); Assert.Equal(3, result);

        values = [2, 1];
        result = source.LastIndexOfAnyExcept(values); Assert.Equal(1, result);
        result = source.LastIndexOfAnyExcept(values, IntEquality()); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_OnEnum_LastIndexOfAnyExcept_Items()
    {
        int result;
        var source = ItemSpan();
        var values = ItemEnumerable();
        // source.IndexOfAny(values); // => CS0311: Item<T> is not IEquatable
        result = source.LastIndexOfAnyExcept(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([3, 4, 1, 2]);
        values = ItemEnumerable([8, 9]);
        result = source.LastIndexOfAnyExcept(values, ItemEquality()); Assert.Equal(3, result);

        values = ItemEnumerable([2, 1]);
        result = source.LastIndexOfAnyExcept(values, ItemEquality()); Assert.Equal(1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnEnum_ContainsAnyExcept_Ints()
    {
        bool result;
        var source = IntSpan();
        var values = IntEnumerable();
        result = source.ContainsAnyExcept(values); Assert.False(result);
        result = source.ContainsAnyExcept(values, IntEquality()); Assert.False(result);

        source = [8, 9, 1];
        values = [8, 9];
        result = source.ContainsAnyExcept(values); Assert.True(result);
        result = source.ContainsAnyExcept(values, IntEquality()); Assert.True(result);

        source = [9];
        values = [8, 9];
        result = source.ContainsAnyExcept(values); Assert.False(result);
        result = source.ContainsAnyExcept(values, IntEquality()); Assert.False(result);
    }

    [Fact]
    public static void Test_OnEnum_ContainsAnyExcept_Items()
    {
        bool result;
        var source = ItemSpan();
        var values = ItemEnumerable();
        // source.ContainsAnyExcept(values); // => CS0311: Item<T> is not IEquatable
        result = source.ContainsAnyExcept(values, ItemEquality()); Assert.False(result);

        source = ItemSpan([8, 9, 1]);
        values = ItemEnumerable([8, 9]);
        result = source.ContainsAnyExcept(values, ItemEquality()); Assert.True(result);

        source = ItemSpan([9]);
        values = ItemEnumerable([8, 9]);
        result = source.ContainsAnyExcept(values, ItemEquality()); Assert.False(result);
    }
}
namespace Yotei.Tools.Tests;

// ========================================================
public static partial class Test_ReadOnlySpanExtensions
{
    [Fact]
    public static void Test_OnSpan_SequenceCompareTo_Ints()
    {
        int result;
        var source = IntSpan();
        var target = IntSpan();
        result = source.SequenceCompareTo(target); Assert.Equal(0, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(0, result);

        source = IntSpan([1, 2]);
        target = IntSpan([1, 2]);
        result = source.SequenceCompareTo(target); Assert.Equal(0, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(0, result);

        source = IntSpan([1, 2, 3]);
        target = IntSpan([1, 2]);
        result = source.SequenceCompareTo(target); Assert.Equal(+1, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(+1, result);

        source = IntSpan([1]);
        target = IntSpan([1, 2]);
        result = source.SequenceCompareTo(target); Assert.Equal(-1, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(-1, result);

        source = IntSpan([1, 3]);
        target = IntSpan([1, 2]);
        result = source.SequenceCompareTo(target); Assert.Equal(+1, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(+1, result);

        source = IntSpan([1, 1]);
        target = IntSpan([1, 2]);
        result = source.SequenceCompareTo(target); Assert.Equal(-1, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(-1, result);
    }

    [Fact]
    public static void Test_OnSpan_SequenceCompareTo_Items()
    {
        int result;
        var source = ItemSpan();
        var target = ItemSpan();
        // source.SequenceCompareTo(target); // => CS0311: Item<T> is not IEquatable
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(0, result);

        source = ItemSpan([1, 2]);
        target = ItemSpan([1, 2]);
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(0, result);

        source = ItemSpan([1, 2, 3]);
        target = ItemSpan([1, 2]);
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(+1, result);

        source = ItemSpan([1]);
        target = ItemSpan([1, 2]);
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(-1, result);

        source = ItemSpan([1, 3]);
        target = ItemSpan([1, 2]);
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(+1, result);

        source = ItemSpan([1, 1]);
        target = ItemSpan([1, 2]);
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(-1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnSpan_SequenceEqual_Ints()
    {
        bool result;
        var source = IntSpan();
        var target = IntSpan();
        result = source.SequenceEqual(target); Assert.True(result);
        result = source.SequenceEqual(target, IntEquality()); Assert.True(result);

        source = IntSpan([1, 2]);
        target = IntSpan([1, 2]);
        result = source.SequenceEqual(target); Assert.True(result);
        result = source.SequenceEqual(target, IntEquality()); Assert.True(result);

        source = IntSpan([1]);
        target = IntSpan([1, 2]);
        result = source.SequenceEqual(target); Assert.False(result);
        result = source.SequenceEqual(target, IntEquality()); Assert.False(result);
    }

    [Fact]
    public static void Test_OnSpan_SequenceEqual_Items()
    {
        bool result;
        var source = ItemSpan();
        var target = ItemSpan();
        // source.SequenceEqual(target); // => CS0311: Item<T> is not IEquatable
        result = source.SequenceEqual(target, ItemEquality()); Assert.True(result);

        source = ItemSpan([1, 2]);
        target = ItemSpan([1, 2]);
        result = source.SequenceEqual(target, ItemEquality()); Assert.True(result);

        source = ItemSpan([1]);
        target = ItemSpan([1, 2]);
        result = source.SequenceEqual(target, ItemEquality()); Assert.False(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnSpan_IndexOf_Range_Ints()
    {
        int result;
        var source = IntSpan();
        var values = IntSpan();
        result = source.IndexOf(values); Assert.Equal(0, result); // Empties are ok!
        result = source.IndexOf(values, IntEquality()); Assert.Equal(0, result);

        source = IntSpan([3, 1, 2]);
        values = IntSpan([3, 4]);
        result = source.IndexOf(values); Assert.Equal(-1, result);
        result = source.IndexOf(values, IntEquality()); Assert.Equal(-1, result);

        source = IntSpan([1, 2, 3]);
        values = IntSpan([3, 4]);
        result = source.IndexOf(values); Assert.Equal(-1, result);
        result = source.IndexOf(values, IntEquality()); Assert.Equal(-1, result);

        source = IntSpan([1, 2, 3, 4]);
        values = IntSpan([2, 3]);
        result = source.IndexOf(values); Assert.Equal(1, result);
        result = source.IndexOf(values, IntEquality()); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_OnSpan_IndexOf_Range_Items()
    {
        int result;
        var source = ItemSpan();
        var values = ItemSpan();
        // source.IndexOf(values); // => CS0311: Item<T> is not IEquatable
        result = source.IndexOf(values, ItemEquality()); Assert.Equal(0, result);

        source = ItemSpan([3, 1, 2]);
        values = ItemSpan([3, 4]);
        result = source.IndexOf(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([1, 2, 3]);
        values = ItemSpan([3, 4]);
        result = source.IndexOf(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([1, 2, 3, 4]);
        values = ItemSpan([2, 3]);
        result = source.IndexOf(values, ItemEquality()); Assert.Equal(1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnSpan_LastIndexOf_Range_Ints()
    {
        int result;
        var source = IntSpan();
        var values = IntSpan();
        result = source.LastIndexOf(values); Assert.Equal(0, result); // Empties are ok!
        result = source.LastIndexOf(values, IntEquality()); Assert.Equal(0, result);

        source = IntSpan([3, 1, 2]);
        values = IntSpan([3, 4]);
        result = source.LastIndexOf(values); Assert.Equal(-1, result);
        result = source.LastIndexOf(values, IntEquality()); Assert.Equal(-1, result);

        source = IntSpan([1, 2, 3]);
        values = IntSpan([3, 4]);
        result = source.LastIndexOf(values); Assert.Equal(-1, result);
        result = source.LastIndexOf(values, IntEquality()); Assert.Equal(-1, result);

        source = IntSpan([1, 2, 3, 4, 5]);
        values = IntSpan([3, 4]);
        result = source.LastIndexOf(values); Assert.Equal(2, result);
        result = source.LastIndexOf(values, IntEquality()); Assert.Equal(2, result);
    }

    [Fact]
    public static void Test_OnSpan_LastIndexOf_Range_Items()
    {
        int result;
        var source = ItemSpan();
        var values = ItemSpan();
        // source.LastIndexOf(values); // => CS0311: Item<T> is not IEquatable
        result = source.LastIndexOf(values, ItemEquality()); Assert.Equal(0, result);

        source = ItemSpan([3, 1, 2]);
        values = ItemSpan([3, 4]);
        result = source.LastIndexOf(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([1, 2, 3]);
        values = ItemSpan([3, 4]);
        result = source.LastIndexOf(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([1, 2, 3, 4, 5]);
        values = ItemSpan([3, 4]);
        result = source.LastIndexOf(values, ItemEquality()); Assert.Equal(2, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnSpan_Contains_Range_Ints()
    {
        bool result;
        var source = IntSpan();
        var values = IntSpan();
        result = source.Contains(values); Assert.True(result); // Empties are ok!
        result = source.Contains(values, IntEquality()); Assert.True(result);

        source = IntSpan([3, 1, 2]);
        values = IntSpan([3, 4]);
        result = source.Contains(values); Assert.False(result);
        result = source.Contains(values, IntEquality()); Assert.False(result);

        source = IntSpan([1, 2, 3]);
        values = IntSpan([3, 4]);
        result = source.Contains(values); Assert.False(result);
        result = source.Contains(values, IntEquality()); Assert.False(result);

        source = IntSpan([1, 2, 3, 4, 5]);
        values = IntSpan([3, 4]);
        result = source.Contains(values); Assert.True(result);
        result = source.Contains(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_OnSpan_ContainsRange_Items()
    {
        bool result;
        var source = ItemSpan();
        var values = ItemSpan();
        // source.Contains(values); // => CS0311: Item<T> is not IEquatable
        result = source.Contains(values, ItemEquality()); Assert.True(result); // Empties are ok!

        source = ItemSpan([3, 1, 2]);
        values = ItemSpan([3, 4]);
        result = source.Contains(values, ItemEquality()); Assert.False(result);

        source = ItemSpan([1, 2, 3]);
        values = ItemSpan([3, 4]);
        result = source.Contains(values, ItemEquality()); Assert.False(result);

        source = ItemSpan([1, 2, 3, 4, 5]);
        values = ItemSpan([3, 4]);
        result = source.Contains(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnSpan_StartsWith_Range_Ints()
    {
        bool result;
        var source = IntSpan();
        var values = IntSpan();
        result = source.StartsWith(values); Assert.True(result); // Empties are ok!
        result = source.StartsWith(values, IntEquality()); Assert.True(result);

        source = IntSpan([1, 2, 3]);
        values = IntSpan([1, 4]);
        result = source.StartsWith(values); Assert.False(result);
        result = source.StartsWith(values, IntEquality()); Assert.False(result);

        source = IntSpan([1, 2, 3, 4]);
        values = IntSpan([1, 2]);
        result = source.StartsWith(values); Assert.True(result);
        result = source.StartsWith(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_OnSpan_StartsWith_Range_Items()
    {
        bool result;
        var source = ItemSpan();
        var values = ItemSpan();
        // source.StartsWith(values); // => CS0311: Item<T> is not IEquatable
        result = source.StartsWith(values, ItemEquality()); Assert.True(result);

        source = ItemSpan([1, 2, 3]);
        values = ItemSpan([1, 4]);
        result = source.StartsWith(values, ItemEquality()); Assert.False(result);

        source = ItemSpan([1, 2, 3, 4]);
        values = ItemSpan([1, 2]);
        result = source.StartsWith(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnSpan_EndsWith_Range_Ints()
    {
        bool result;
        var source = IntSpan();
        var values = IntSpan();
        result = source.EndsWith(values); Assert.True(result); // Empties are ok!
        result = source.EndsWith(values, IntEquality()); Assert.True(result);

        source = IntSpan([1, 2, 3]);
        values = IntSpan([2, 4]);
        result = source.EndsWith(values); Assert.False(result);
        result = source.EndsWith(values, IntEquality()); Assert.False(result);

        source = IntSpan([1, 2, 3]);
        values = IntSpan([3, 4]);
        result = source.EndsWith(values); Assert.False(result);
        result = source.EndsWith(values, IntEquality()); Assert.False(result);

        source = IntSpan([1, 2, 3, 4]);
        values = IntSpan([3, 4]);
        result = source.EndsWith(values); Assert.True(result);
        result = source.EndsWith(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_OnSpan_EndsWith_Range_Items()
    {
        bool result;
        var source = ItemSpan();
        var values = ItemSpan();
        // source.EndsWith(values); // => CS0311: Item<T> is not IEquatable
        result = source.EndsWith(values, ItemEquality()); Assert.True(result);

        source = ItemSpan([1, 2, 3]);
        values = ItemSpan([2, 4]);
        result = source.EndsWith(values, ItemEquality()); Assert.False(result);

        source = ItemSpan([1, 2, 3]);
        values = ItemSpan([3, 4]);
        result = source.EndsWith(values, ItemEquality()); Assert.False(result);

        source = ItemSpan([1, 2, 3, 4]);
        values = ItemSpan([3, 4]);
        result = source.EndsWith(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnSpan_IndexOfAny_Ints()
    {
        int result;
        var source = IntSpan();
        var values = IntSpan();
        result = source.IndexOfAny(values); Assert.Equal(-1, result);
        result = source.IndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        source = IntSpan([1, 2]);
        values = IntSpan([3, 4]);
        result = source.IndexOfAny(values); Assert.Equal(-1, result);
        result = source.IndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        source = IntSpan([0, 1, 2, 3]);
        values = IntSpan([4, 1, 5]);
        result = source.IndexOfAny(values); Assert.Equal(1, result);
        result = source.IndexOfAny(values, IntEquality()); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_OnSpan_IndexOfAny_Items()
    {
        int result;
        var source = ItemSpan();
        var values = ItemSpan();
        // source.IndexOfAny(values); // => CS0311: Item<T> is not IEquatable
        result = source.IndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([1, 2]);
        values = ItemSpan([3, 4]);
        result = source.IndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([0, 1, 2, 3]);
        values = ItemSpan([4, 1, 5]);
        result = source.IndexOfAny(values, ItemEquality()); Assert.Equal(1, result);
    }


    // ----------------------------------------------------

    [Fact]
    public static void Test_OnSpan_LastIndexOfAny_Ints()
    {
        int result;
        var source = IntSpan();
        var values = IntSpan();
        result = source.LastIndexOfAny(values); Assert.Equal(-1, result);
        result = source.LastIndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        source = IntSpan([1, 2]);
        values = IntSpan([3, 4]);
        result = source.LastIndexOfAny(values); Assert.Equal(-1, result);
        result = source.LastIndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        source = IntSpan([3, 2, 1, 0]);
        values = IntSpan([4, 1, 5]);
        result = source.LastIndexOfAny(values); Assert.Equal(2, result);
        result = source.LastIndexOfAny(values, IntEquality()); Assert.Equal(2, result);
    }

    [Fact]
    public static void Test_OnSpan_LastIndexOfAny_Items()
    {
        int result;
        var source = ItemSpan();
        var values = ItemSpan();
        // source.ContainsAny(values); // => CS0311: Item<T> is not IEquatable
        result = source.LastIndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([1, 2]);
        values = ItemSpan([3, 4]);
        result = source.LastIndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([3, 2, 1, 0]);
        values = ItemSpan([4, 1, 5]);
        result = source.LastIndexOfAny(values, ItemEquality()); Assert.Equal(2, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnSpan_ContainsAny_Ints()
    {
        bool result;
        var source = IntSpan();
        var values = IntSpan();
        result = source.ContainsAny(values); Assert.False(result);
        result = source.ContainsAny(values, IntEquality()); Assert.False(result);

        source = IntSpan([1, 2, 3]);
        values = IntSpan([4, 5]);
        result = source.ContainsAny(values); Assert.False(result);
        result = source.ContainsAny(values, IntEquality()); Assert.False(result);

        source = IntSpan([1, 2, 3]);
        values = IntSpan([4, 1]);
        result = source.ContainsAny(values); Assert.True(result);
        result = source.ContainsAny(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_OnSpan_ContainsAny_Items()
    {
        bool result;
        var source = ItemSpan();
        var values = ItemSpan();
        // source.ContainsAny(values); // => CS0311: Item<T> is not IEquatable
        result = source.ContainsAny(values, ItemEquality()); Assert.False(result);

        source = ItemSpan([1, 2, 3]);
        values = ItemSpan([4, 5]);
        result = source.ContainsAny(values, ItemEquality()); Assert.False(result);

        source = ItemSpan([1, 2, 3]);
        values = ItemSpan([4, 1]);
        result = source.ContainsAny(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnSpan_IndexOfAnyExcept_Ints()
    {
        int result;
        var source = IntSpan();
        var values = IntSpan();
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
    public static void Test_OnSpan_IndexOfAnyExcept_Items()
    {
        int result;
        var source = ItemSpan();
        var values = ItemSpan();
        // source.IndexOfAny(values); // => CS0311: Item<T> is not IEquatable
        result = source.IndexOfAnyExcept(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([1, 2, 3, 4]);
        values = ItemSpan([8, 9]);
        result = source.IndexOfAnyExcept(values, ItemEquality()); Assert.Equal(0, result);

        values = ItemSpan([2, 1]);
        result = source.IndexOfAnyExcept(values, ItemEquality()); Assert.Equal(2, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnSpan_LastIndexOfAnyExcept_Ints()
    {
        int result;
        var source = IntSpan();
        var values = IntSpan();
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
    public static void Test_OnSpan_LastIndexOfAnyExcept_Items()
    {
        int result;
        var source = ItemSpan();
        var values = ItemSpan();
        // source.IndexOfAny(values); // => CS0311: Item<T> is not IEquatable
        result = source.LastIndexOfAnyExcept(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemSpan([3, 4, 1, 2]);
        values = ItemSpan([8, 9]);
        result = source.LastIndexOfAnyExcept(values, ItemEquality()); Assert.Equal(3, result);

        values = ItemSpan([2, 1]);
        result = source.LastIndexOfAnyExcept(values, ItemEquality()); Assert.Equal(1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnSpan_ContainsAnyExcept_Ints()
    {
        bool result;
        var source = IntSpan();
        var values = IntSpan();
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
    public static void Test_OnSpan_ContainsAnyExcept_Items()
    {
        bool result;
        var source = ItemSpan();
        var values = ItemSpan();
        // source.ContainsAnyExcept(values); // => CS0311: Item<T> is not IEquatable
        result = source.ContainsAnyExcept(values, ItemEquality()); Assert.False(result);

        source = ItemSpan([8, 9, 1]);
        values = ItemSpan([8, 9]);
        result = source.ContainsAnyExcept(values, ItemEquality()); Assert.True(result);

        source = ItemSpan([9]);
        values = ItemSpan([8, 9]);
        result = source.ContainsAnyExcept(values, ItemEquality()); Assert.False(result);
    }
}
#pragma warning disable IDE0079
#pragma warning disable CA1859

namespace Yotei.Tools.Tests;

// ========================================================
public static partial class Test_ReadOnlySpanExtensions
{
    [Fact]
    public static void Test_SpanOnEnum_SequenceCompareTo()
    {
        int result;
        var span = IntSpan([]);
        var values = Array.Empty<int>();
        result = span.SequenceCompareTo(values); Assert.Equal(0, result);
        result = span.SequenceCompareTo(values, IntComparer()); Assert.Equal(0, result);

        span = IntSpan([1, 2, 3]);
        values = [1, 2, 3];
        result = span.SequenceCompareTo(values); Assert.Equal(0, result);
        result = span.SequenceCompareTo(values, IntComparer()); Assert.Equal(0, result);

        values = [1, 2];
        result = span.SequenceCompareTo(values); Assert.Equal(+1, result);
        result = span.SequenceCompareTo(values, IntComparer()); Assert.Equal(+1, result);

        values = [1, 2, 3, 4];
        result = span.SequenceCompareTo(values); Assert.Equal(-1, result);
        result = span.SequenceCompareTo(values, IntComparer()); Assert.Equal(-1, result);

        span = IntSpan([1, 2, 4]);
        values = [1, 2, 3];
        result = span.SequenceCompareTo(values); Assert.Equal(+1, result);
        result = span.SequenceCompareTo(values, IntComparer()); Assert.Equal(+1, result);

        span = IntSpan([1, 2, 2]);
        values = [1, 2, 3];
        result = span.SequenceCompareTo(values); Assert.Equal(-1, result);
        result = span.SequenceCompareTo(values, IntComparer()); Assert.Equal(-1, result);
    }

    [Fact]
    public static void Test_SpanOnEnum_SequenceEqual()
    {
        bool result;
        var span = IntSpan([]);
        var values = Array.Empty<int>();
        result = span.SequenceEqual(values); Assert.True(result);
        result = span.SequenceEqual(values, IntEquality()); Assert.True(result);

        span = IntSpan([1, 2, 3]);
        values = [1, 2, 3];
        result = span.SequenceEqual(values); Assert.True(result);
        result = span.SequenceEqual(values, IntEquality()); Assert.True(result);

        values = [1, 2];
        result = span.SequenceEqual(values); Assert.False(result);
        result = span.SequenceEqual(values, IntEquality()); Assert.False(result);

        values = [1, 2, 3, 4];
        result = span.SequenceEqual(values); Assert.False(result);
        result = span.SequenceEqual(values, IntEquality()); Assert.False(result);

        span = IntSpan([1, 2, 4]);
        values = [1, 2, 3];
        result = span.SequenceEqual(values); Assert.False(result);
        result = span.SequenceEqual(values, IntEquality()); Assert.False(result);

        span = IntSpan([1, 2, 2]);
        values = [1, 2, 3];
        result = span.SequenceEqual(values); Assert.False(result);
        result = span.SequenceEqual(values, IntEquality()); Assert.False(result);
    }

    // ----------------------------------------------------
    [Fact]
    public static void Test_SpanOnEnum_StartsWithAny()
    {
        bool result;
        var span = IntSpan([]);
        var values = Array.Empty<int>();
        result = span.StartsWithAny(values); Assert.False(result);
        result = span.StartsWithAny(values, IntEquality()); Assert.False(result);

        values = [8, 9];
        result = span.StartsWithAny(values); Assert.False(result);
        result = span.StartsWithAny(values, IntEquality()); Assert.False(result);

        span = IntSpan([1, 2]);
        values = [8, 9, 1];
        result = span.StartsWithAny(values); Assert.True(result);
        result = span.StartsWithAny(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_SpanOnEnum_EndsWithAny()
    {
        bool result;
        var span = IntSpan([]);
        var values = Array.Empty<int>();
        result = span.EndsWithAny(values); Assert.False(result);
        result = span.EndsWithAny(values, IntEquality()); Assert.False(result);

        values = [8, 9];
        result = span.EndsWithAny(values); Assert.False(result);
        result = span.EndsWithAny(values, IntEquality()); Assert.False(result);

        span = IntSpan([1, 2]);
        values = [8, 9, 2];
        result = span.EndsWithAny(values); Assert.True(result);
        result = span.EndsWithAny(values, IntEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_SpanOnEnum_IndexOfAny()
    {
        int result;
        var span = IntSpan([]);
        var values = Array.Empty<int>();
        result = span.IndexOfAny(values); Assert.Equal(-1, result);
        result = span.IndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        values = [9, 8, 2];
        result = span.IndexOfAny(values); Assert.Equal(-1, result);
        result = span.IndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        span = [1, 2, 3, 4];
        result = span.IndexOfAny(values); Assert.Equal(1, result);
        result = span.IndexOfAny(values, IntEquality()); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_SpanOnEnum_LastIndexOfAny()
    {
        int result;
        var span = IntSpan([]);
        var values = Array.Empty<int>();
        result = span.LastIndexOfAny(values); Assert.Equal(-1, result);
        result = span.LastIndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        values = [9, 8, 2];
        result = span.LastIndexOfAny(values); Assert.Equal(-1, result);
        result = span.LastIndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        span = [1, 2, 3, 4];
        result = span.LastIndexOfAny(values); Assert.Equal(1, result);
        result = span.LastIndexOfAny(values, IntEquality()); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_SpanOnEnum_ContainsAny()
    {
        bool result;
        var span = IntSpan([]);
        var values = Array.Empty<int>();
        result = span.ContainsAny(values); Assert.False(result);
        result = span.ContainsAny(values, IntEquality()); Assert.False(result);

        values = [9, 8, 2];
        result = span.ContainsAny(values); Assert.False(result);
        result = span.ContainsAny(values, IntEquality()); Assert.False(result);

        span = [1, 2, 3, 4];
        result = span.ContainsAny(values); Assert.True(result);
        result = span.ContainsAny(values, IntEquality()); Assert.True(result);
    }

    // ----------------------------------------------------
    
    [Fact]
    public static void Test_SpanOnEnum_IndexOfAnyExcept()
    {
        int result;
        var span = IntSpan([]);
        var values = Array.Empty<int>();
        result = span.IndexOfAnyExcept(values); Assert.Equal(-1, result);
        result = span.IndexOfAnyExcept(values, IntEquality()); Assert.Equal(-1, result);

        span = IntSpan([]);
        values = [9, 8, 2];
        result = span.IndexOfAnyExcept(values); Assert.Equal(-1, result);
        result = span.IndexOfAnyExcept(values, IntEquality()); Assert.Equal(-1, result);

        span = [1, 2, 3, 4];
        values = [1, 2];
        result = span.IndexOfAnyExcept(values); Assert.Equal(2, result);
        result = span.IndexOfAnyExcept(values, IntEquality()); Assert.Equal(2, result);
    }

    [Fact]
    public static void Test_SpanOnEnum_LastIndexOfAnyExcept()
    {
        int result;
        var span = IntSpan([]);
        var values = Array.Empty<int>();
        result = span.LastIndexOfAnyExcept(values); Assert.Equal(-1, result);
        result = span.LastIndexOfAnyExcept(values, IntEquality()); Assert.Equal(-1, result);

        span = IntSpan([]);
        values = [9, 8, 2];
        result = span.LastIndexOfAnyExcept(values); Assert.Equal(-1, result);
        result = span.LastIndexOfAnyExcept(values, IntEquality()); Assert.Equal(-1, result);

        span = [1, 2, 3, 4];
        values = [3, 4];
        result = span.LastIndexOfAnyExcept(values); Assert.Equal(1, result);
        result = span.LastIndexOfAnyExcept(values, IntEquality()); Assert.Equal(1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_SpanOnEnum_TrimStart()
    {
        int[] values;
        ReadOnlySpan<int> source;
        ReadOnlySpan<int> target;

        source = [];
        values = [];
        target = source.TrimStart(values); Assert.True(target.IsEmpty);
        target = source.TrimStart(values, IntEquality()); Assert.True(target.IsEmpty);

        values = [1, 2];
        target = source.TrimStart(values); Assert.True(target.IsEmpty);
        target = source.TrimStart(values, IntEquality()); Assert.True(target.IsEmpty);

        source = [1, 2];
        values = [];
        target = source.TrimStart(values); Assert.False(target.IsEmpty);
        target = source.TrimStart(values, IntEquality()); Assert.False(target.IsEmpty);

        source = [1, 2, 3];
        values = [1];
        target = source.TrimStart(values); Assert.True(target is [2, 3]);
        target = source.TrimStart(values, IntEquality()); Assert.True(target is [2, 3]);
    }

    [Fact]
    public static void Test_SpanOnEnum_TrimEnd()
    {
        Item<int>[] values = [];
        ReadOnlySpan<Item<int>> source = [];
        var target = source.TrimEnd(values); Assert.True(target.IsEmpty);
        target = source.TrimEnd(values, ItemEquality()); Assert.True(target.IsEmpty);

        values = [new(1), new(2)];
        target = source.TrimEnd(values); Assert.True(target.IsEmpty);
        target = source.TrimEnd(values, ItemEquality()); Assert.True(target.IsEmpty);

        source = ItemSpan([new(1), new(2)]);
        values = [];
        target = source.TrimEnd(values); Assert.False(target.IsEmpty);
        target = source.TrimEnd(values, ItemEquality()); Assert.False(target.IsEmpty);

        source = ItemSpan([new(1), new(2), new(3)]);
        values = [new(3)];
        var expected = ItemSpan([new(1), new(2)]);
        target = source.TrimEnd(values); Assert.False(target.SequenceEqual(expected, ItemEquality()));
        target = source.TrimEnd(values, ItemEquality()); Assert.True(target.SequenceEqual(expected, ItemEquality()));
    }

    [Fact]
    public static void Test_SpanOnEnum_Trim()
    {
        ReadOnlySpan<Item<int>> values = [];
        ReadOnlySpan<Item<int>> source = [];
        var target = source.Trim(values); Assert.True(target.IsEmpty);
        target = source.Trim(values, ItemEquality()); Assert.True(target.IsEmpty);

        values = ItemSpan([new(1), new(2)]);
        target = source.Trim(values); Assert.True(target.IsEmpty);
        target = source.Trim(values, ItemEquality()); Assert.True(target.IsEmpty);

        source = ItemSpan([new(1), new(2)]);
        values = [];
        target = source.Trim(values); Assert.False(target.IsEmpty);
        target = source.Trim(values, ItemEquality()); Assert.False(target.IsEmpty);

        source = ItemSpan([new(1), new(2), new(3)]);
        values = ItemSpan([new(3), new(1)]);
        var expected = ItemSpan([new(2)]);
        target = source.Trim(values); Assert.False(target.SequenceEqual(expected, ItemEquality()));
        target = source.Trim(values, ItemEquality()); Assert.True(target.SequenceEqual(expected, ItemEquality()));
    }
}
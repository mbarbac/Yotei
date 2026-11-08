#pragma warning disable IDE0079
#pragma warning disable CA1859

namespace Yotei.Tools.Tests;

// ========================================================
public static partial class Test_ReadOnlySpanExtensions
{
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
}

// ========================================================
#if !NETSTANDARD2_0
public static partial class Test_ReadOnlySpanExtensions
{
    
}
#endif
#pragma warning disable IDE0079
#pragma warning disable CA1859

namespace Yotei.Tools.Tests;

// ========================================================
public static partial class Test_ReadOnlySpanExtensions
{
    [Fact]
    public static void Test_SpanOnSpan_IndexOfAny()
    {
        int result;
        var span = ItemSpan([]);
        var values = ItemSpan([]);
        result = span.IndexOfAny(values); Assert.Equal(-1, result);
        result = span.IndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        values = ItemSpan([new(9), new(8), new(2)]);
        result = span.IndexOfAny(values); Assert.Equal(-1, result);
        result = span.IndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        span = ItemSpan([new(1), new(2), new(3), new(4)]);
        result = span.IndexOfAny(values); Assert.Equal(-1, result);
        result = span.IndexOfAny(values, ItemEquality()); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_SpanOnSpan_LastIndexOfAny()
    {
        int result;
        var span = ItemSpan([]);
        var values = ItemSpan([]);
        result = span.LastIndexOfAny(values); Assert.Equal(-1, result);
        result = span.LastIndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        values = ItemSpan([new(9), new(8), new(2)]);
        result = span.LastIndexOfAny(values); Assert.Equal(-1, result);
        result = span.LastIndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        span = ItemSpan([new(1), new(2), new(3), new(4)]);
        result = span.LastIndexOfAny(values); Assert.Equal(-1, result);
        result = span.LastIndexOfAny(values, ItemEquality()); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_SpanOnSpan_ContainsAny()
    {
        bool result;
        var span = ItemSpan([]);
        var values = ItemSpan([]);
        result = span.ContainsAny(values); Assert.False(result);
        result = span.ContainsAny(values, ItemEquality()); Assert.False(result);

        values = ItemSpan([new(9), new(8), new(2)]);
        result = span.ContainsAny(values); Assert.False(result);
        result = span.ContainsAny(values, ItemEquality()); Assert.False(result);

        span = ItemSpan([new(1), new(2), new(3), new(4)]);
        result = span.ContainsAny(values); Assert.False(result);
        result = span.ContainsAny(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_SpanOnSpan_SequenceCompareTo()
    {
        int result;
        var span = ItemSpan([]);
        var values = ItemSpan([]);
        result = span.SequenceCompareTo(values); Assert.Equal(0, result);
        result = span.SequenceCompareTo(values, ItemComparer()); Assert.Equal(0, result);

        span = ItemSpan([new(1)]);
        values = ItemSpan([new(1)]);
        try { result = span.SequenceCompareTo(values); Assert.Fail(""); }
        catch (ArgumentException e) when (e.Message.Contains("IComparable")) { }

        span = ItemSpan([new(1), new(2), new(3)]);
        values = ItemSpan([new(1), new(2), new(3)]);
        result = span.SequenceCompareTo(values, ItemComparer()); Assert.Equal(0, result);

        values = ItemSpan([new(1), new(2)]);
        result = span.SequenceCompareTo(values, ItemComparer()); Assert.Equal(+1, result);

        values = ItemSpan([new(1), new(2), new(3), new(4)]);
        result = span.SequenceCompareTo(values, ItemComparer()); Assert.Equal(-1, result);

        span = ItemSpan([new(1), new(2), new(4)]);
        values = ItemSpan([new(1), new(2), new(3)]);
        result = span.SequenceCompareTo(values, ItemComparer()); Assert.Equal(+1, result);

        span = ItemSpan([new(1), new(2), new(2)]);
        values = ItemSpan([new(1), new(2), new(3)]);
        result = span.SequenceCompareTo(values, ItemComparer()); Assert.Equal(-1, result);
    }

    [Fact]
    public static void Test_SpanOnSpan_SequenceEqual()
    {
        bool result;
        var span = ItemSpan([]);
        var values = ItemSpan([]);
        result = span.SequenceEqual(values); Assert.True(result);
        result = span.SequenceEqual(values, ItemEquality()); Assert.True(result);

        span = ItemSpan([new(1), new(2), new(3)]);
        values = ItemSpan([new(1), new(2), new(3)]);
        result = span.SequenceEqual(values); Assert.False(result); // Reference type!
        result = span.SequenceEqual(values, ItemEquality()); Assert.True(result);

        values = ItemSpan([new(1), new(2)]);
        result = span.SequenceEqual(values); Assert.False(result);
        result = span.SequenceEqual(values, ItemEquality()); Assert.False(result);

        values = ItemSpan([new(1), new(2), new(3), new(4)]);
        result = span.SequenceEqual(values); Assert.False(result);
        result = span.SequenceEqual(values, ItemEquality()); Assert.False(result);

        span = ItemSpan([new(1), new(2), new(4)]);
        values = ItemSpan([new(1), new(2), new(3)]);
        result = span.SequenceEqual(values); Assert.False(result);
        result = span.SequenceEqual(values, ItemEquality()); Assert.False(result);

        span = ItemSpan([new(1), new(2), new(2)]);
        values = ItemSpan([new(1), new(2), new(3)]);
        result = span.SequenceEqual(values); Assert.False(result);
        result = span.SequenceEqual(values, ItemEquality()); Assert.False(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_SpanOnSpan_StartsWithAny()
    {
        bool result;
        var span = ItemSpan([]);
        var values = ItemSpan([]);
        result = span.StartsWithAny(values); Assert.False(result);
        result = span.StartsWithAny(values, ItemEquality()); Assert.False(result);

        values = ItemSpan([new(8), new(9)]);
        result = span.StartsWithAny(values); Assert.False(result);
        result = span.StartsWithAny(values, ItemEquality()); Assert.False(result);

        span = ItemSpan([new(1), new(2)]);
        values = ItemSpan([new(8), new(9), new(1)]);
        result = span.StartsWithAny(values); Assert.False(result); // Reference type
        result = span.StartsWithAny(values, ItemEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_SpanOnSpan_EndsWithAny()
    {
        bool result;
        var span = ItemSpan([]);
        var values = ItemSpan([]);
        result = span.EndsWithAny(values); Assert.False(result);
        result = span.EndsWithAny(values, ItemEquality()); Assert.False(result);

        values = ItemSpan([new(8), new(9)]);
        result = span.EndsWithAny(values); Assert.False(result);
        result = span.EndsWithAny(values, ItemEquality()); Assert.False(result);

        span = ItemSpan([new(1), new(2)]);
        values = ItemSpan([new(8), new(9), new(2)]);
        result = span.EndsWithAny(values); Assert.False(result); // Reference type
        result = span.EndsWithAny(values, ItemEquality()); Assert.True(result);
    }
}

// ========================================================
#if !NETSTANDARD2_0
public static partial class Test_ReadOnlySpanExtensions
{
    
}
#endif
#pragma warning disable IDE0079
#pragma warning disable CA1859

namespace Yotei.Tools.Tests;

// ========================================================
public static partial class Test_ReadOnlySpanExtensions
{
    [Fact]
    public static void Test_ItemSpan_IndexOfAny()
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
    public static void Test_ItemSpan_LastIndexOfAny()
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
    public static void Test_ItemSpan_ContainsAny()
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
    public static void Test_ItemSpan_SequenceCompareTo()
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
}

// ========================================================
#if NETSTANDARD2_0
public static partial class Test_ReadOnlySpanExtensions
{
    //[Fact]
    //public static void Test_ItemSpan_()
    //{
    //    int result;
    //    var span = ItemSpan([]);
    //    var values = ItemSpan([]);
    //    result = span.SequenceCompareTo(values); Assert.Equal(0, result);
    //    result = span.SequenceCompareTo(values, ItemComparer()); Assert.Equal(0, result);

    //    span = ItemSpan([new(1)]);
    //    values = ItemSpan([new(1)]);
    //    try { result = span.SequenceCompareTo(values); Assert.Fail(""); }
    //    catch (ArgumentException e) when (e.Message.Contains("IComparable")) { }

    //    span = ItemSpan([new(1), new(2), new(3)]);
    //    values = ItemSpan([new(1), new(2), new(3)]);
    //    result = span.SequenceCompareTo(values, ItemComparer()); Assert.Equal(0, result);

    //    values = ItemSpan([new(1), new(2)]);
    //    result = span.SequenceCompareTo(values, ItemComparer()); Assert.Equal(+1, result);

    //    values = ItemSpan([new(1), new(2), new(3), new(4)]);
    //    result = span.SequenceCompareTo(values, ItemComparer()); Assert.Equal(-1, result);

    //    span = ItemSpan([new(1), new(2), new(4)]);
    //    values = ItemSpan([new(1), new(2), new(3)]);
    //    result = span.SequenceCompareTo(values, ItemComparer()); Assert.Equal(+1, result);

    //    span = ItemSpan([new(1), new(2), new(2)]);
    //    values = ItemSpan([new(1), new(2), new(3)]);
    //    result = span.SequenceCompareTo(values, ItemComparer()); Assert.Equal(-1, result);
    //}
}
#endif
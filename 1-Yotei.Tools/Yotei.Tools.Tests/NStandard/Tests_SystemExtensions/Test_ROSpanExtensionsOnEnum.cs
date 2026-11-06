#pragma warning disable IDE0079
#pragma warning disable CA1859

namespace Yotei.Tools.Tests;

// ========================================================
public static partial class Test_ReadOnlySpanExtensions
{
    [Fact]
    public static void Test_IntSpan_IndexOfAny()
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
    public static void Test_IntSpan_LastIndexOfAny()
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
    public static void Test_IntSpan_ContainsAny()
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
}

// ========================================================
#if !NETSTANDARD2_0
public static partial class Test_ReadOnlySpanExtensions
{
    //[Fact]
    //public static void Test_IntSpan_()
    //{
    //    int result;
    //    var span = IntSpan([]);
    //    var values = Array.Empty<int>();
    //    result = span.IndexOfAny(values); Assert.Equal(-1, result);
    //    result = span.IndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

    //    values = [9, 8, 2];
    //    result = span.IndexOfAny(values); Assert.Equal(-1, result);
    //    result = span.IndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

    //    span = [1, 2, 3, 4];
    //    result = span.IndexOfAny(values); Assert.Equal(1, result);
    //    result = span.IndexOfAny(values, IntEquality()); Assert.Equal(1, result);
    //}
}
#endif
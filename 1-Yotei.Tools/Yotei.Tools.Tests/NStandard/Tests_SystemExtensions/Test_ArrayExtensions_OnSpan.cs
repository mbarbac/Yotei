#pragma warning disable IDE0079
#pragma warning disable CA1859

#if NETSTANDARD2_0
using System.Net.Sockets;

namespace Yotei.Tools.NStd20.Tests;
#else
namespace Yotei.Tools.Tests;
#endif

// ========================================================
public static partial class Test_ArrayExtensions
{
    [Fact]
    public static void Test_SequenceEqual_Span_Ints()
    {
        bool result;
        var source = Array.Empty<int>();
        var target = IntSpan();
        result = source.SequenceEqual(target); Assert.True(result);
        result = source.SequenceEqual(target, IntEquality()); Assert.True(result);

        source = [1, 2];
        result = source.SequenceEqual(target); Assert.False(result);
        result = source.SequenceEqual(target, IntEquality()); Assert.False(result);

        target = [1, 2];
        result = source.SequenceEqual(target); Assert.True(result);
        result = source.SequenceEqual(target, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_SequenceEqual_Span_Items()
    {
        bool result;
        var source = Array.Empty<Item<int>>();
        var target = ItemSpan();
        result = source.SequenceEqual(target); Assert.True(result);
        result = source.SequenceEqual(target, ItemEquality()); Assert.True(result);

        source = ItemArray([1, 2]);
        result = source.SequenceEqual(target); Assert.False(result);
        result = source.SequenceEqual(target, ItemEquality()); Assert.False(result);

        target = ItemSpan([1, 2]);
        result = source.SequenceEqual(target); Assert.False(result); // elements are references
        result = source.SequenceEqual(target, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    // TODO ArrayEx: IndexOf_Range span
    // TODO ArrayEx: LastIndexOf_Range span
    // TODO ArrayEx: Contains_Range span
    // TODO ArrayEx: Contains_Range span
    // TODO ArrayEx: StartsWith_Range span
    // TODO ArrayEx: EndsWith_Range span

    // ----------------------------------------------------

    [Fact]
    public static void Test_IndexOfAny_Span_Ints()
    {
        int result;
        var source = IntArray();
        var values = IntSpan();
        result = source.IndexOfAny(values); Assert.Equal(-1, result);
        result = source.IndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        source = [1, 2];
        values = IntSpan([8, 9]);
        result = source.IndexOfAny(values); Assert.Equal(-1, result);
        result = source.IndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        source = [1, 9, 2];
        values = IntSpan([8, 9]);
        result = source.IndexOfAny(values); Assert.Equal(1, result);
        result = source.IndexOfAny(values, IntEquality()); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_IndexOfAny_Span_Items()
    {
        int result;
        var source = ItemArray();
        var values = ItemSpan();
        // result = source.IndexOfAny(values); // Item<> not IEquatable
        result = source.IndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemArray([1, 2]);
        values = ItemSpan([8, 9]);
        result = source.IndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemArray([1, 9, 2]);
        values = ItemSpan([8, 9]);
        result = source.IndexOfAny(values, ItemEquality()); Assert.Equal(1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_LastIndexOf_Span_Ints()
    {
        int result;
        var source = IntArray();
        var values = IntSpan();
        result = source.LastIndexOfAny(values); Assert.Equal(-1, result);
        result = source.LastIndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        source = [1, 2];
        values = IntSpan([8, 9]);
        result = source.LastIndexOfAny(values); Assert.Equal(-1, result);
        result = source.LastIndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        source = [1, 9, 2];
        values = IntSpan([8, 9]);
        result = source.LastIndexOfAny(values); Assert.Equal(1, result);
        result = source.LastIndexOfAny(values, IntEquality()); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_LastIndexOf_Span_Items()
    {
        int result;
        var source = ItemArray();
        var values = ItemSpan();
        // result = source.LastIndexOfAny(values); // Item<> not IEquatable
        result = source.LastIndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemArray([1, 2]);
        values = ItemSpan([8, 9]);
        result = source.LastIndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemArray([1, 9, 2]);
        values = ItemSpan([8, 9]);
        result = source.LastIndexOfAny(values, ItemEquality()); Assert.Equal(1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IndexesOfAny_Span_Ints()
    {
        List<int> result;
        var source = IntArray();
        var values = IntSpan();
        result = source.IndexesOfAny(values); Assert.Empty(result);
        result = source.IndexesOfAny(values, IntEquality()); Assert.Empty(result);

        source = [1, 2];
        values = IntSpan([8, 9]);
        result = source.IndexesOfAny(values); Assert.Empty(result);
        result = source.IndexesOfAny(values, IntEquality()); Assert.Empty(result);

        source = [1, 9, 8, 2];
        values = IntSpan([8, 9]);
        result = source.IndexesOfAny(values); Assert.True(result is [1, 2]);
        result = source.IndexesOfAny(values, IntEquality()); Assert.True(result is [1, 2]);
    }

    [Fact]
    public static void Test_IndexesOfAny_Span_Items()
    {
        List<int> result;
        var source = ItemArray();
        var values = ItemSpan();
        // result = source.IndexesOfAny(values); // Item<> not IEquatable
        result = source.IndexesOfAny(values, ItemEquality()); Assert.Empty(result);

        source = ItemArray([1, 2]);
        values = ItemSpan([8, 9]);
        result = source.IndexesOfAny(values, ItemEquality()); Assert.Empty(result);

        source = ItemArray([1, 9, 8, 2]);
        values = ItemSpan([8, 9]);
        result = source.IndexesOfAny(values, ItemEquality()); Assert.True(result is [1, 2]);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_ContainsAny_Span_Ints()
    {
        bool result;
        var source = IntArray();
        var values = IntSpan();
        result = source.ContainsAny(values); Assert.False(result);
        result = source.ContainsAny(values, IntEquality()); Assert.False(result);

        source = [1, 2];
        values = IntSpan([8, 9]);
        result = source.ContainsAny(values); Assert.False(result);
        result = source.ContainsAny(values, IntEquality()); Assert.False(result);

        source = [1, 9, 2];
        values = IntSpan([8, 9]);
        result = source.ContainsAny(values); Assert.True(result);
        result = source.ContainsAny(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_ContainsAny_Span_Items()
    {
        bool result;
        var source = ItemArray();
        var values = ItemSpan();
        //result = source.ContainsAny(values); // Item<> not IEquatable
        result = source.ContainsAny(values, ItemEquality()); Assert.False(result);

        source = ItemArray([1, 2]);
        values = ItemSpan([8, 9]);
        result = source.ContainsAny(values, ItemEquality()); Assert.False(result);

        source = ItemArray([1, 9, 2]);
        values = ItemSpan([8, 9]);
        result = source.ContainsAny(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_StartsWithAny_Span_Ints()
    {
        bool result;
        var source = IntArray();
        var values = IntSpan();
        result = source.StartsWithAny(values); Assert.False(result);
        result = source.StartsWithAny(values, IntEquality()); Assert.False(result);

        source = [1, 2];
        values = IntSpan([8, 9]);
        result = source.StartsWithAny(values); Assert.False(result);
        result = source.StartsWithAny(values, IntEquality()); Assert.False(result);

        source = [9, 2];
        values = IntSpan([8, 9]);
        result = source.StartsWithAny(values); Assert.True(result);
        result = source.StartsWithAny(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_StartsWithAny_Span_Items()
    {
        bool result;
        var source = ItemArray();
        var values = ItemSpan();
        //result = source.StartsWithAny(values); // Item<> not IEquatable
        result = source.StartsWithAny(values, ItemEquality()); Assert.False(result);

        source = ItemArray([1, 2]);
        values = ItemSpan([8, 9]);
        result = source.StartsWithAny(values, ItemEquality()); Assert.False(result);

        source = ItemArray([9, 2]);
        values = ItemSpan([8, 9]);
        result = source.StartsWithAny(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_EndsWithAny_Span_Ints()
    {
        bool result;
        var source = IntArray();
        var values = IntSpan();
        result = source.EndsWithAny(values); Assert.False(result);
        result = source.EndsWithAny(values, IntEquality()); Assert.False(result);

        source = [1, 2];
        values = IntSpan([8, 9]);
        result = source.EndsWithAny(values); Assert.False(result);
        result = source.EndsWithAny(values, IntEquality()); Assert.False(result);

        source = [1, 9];
        values = IntSpan([8, 9]);
        result = source.EndsWithAny(values); Assert.True(result);
        result = source.EndsWithAny(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_EndsWithAny_Span_Items()
    {
        bool result;
        var source = ItemArray();
        var values = ItemSpan();
        //result = source.EndsWithAny(values); // Item<> not IEquatable
        result = source.EndsWithAny(values, ItemEquality()); Assert.False(result);

        source = ItemArray([1, 2]);
        values = ItemSpan([8, 9]);
        result = source.EndsWithAny(values, ItemEquality()); Assert.False(result);

        source = ItemArray([1, 9]);
        values = ItemSpan([8, 9]);
        result = source.EndsWithAny(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    // TODO ArrayEx: IndexOfAnyExcept span
    // TODO ArrayEx: LastIndexOfAnyExcept span
    // TODO ArrayEx: IndexesOfAnyExcept span
    // TODO ArrayEx: ContainsAnyExcept span
    // TODO ArrayEx: StartsWithAnyExcept span
    // TODO ArrayEx: EndsWithAnyExcept span

    // ----------------------------------------------------

    // TODO ArrayEx: TrimStart span
    // TODO ArrayEx: TrimEnd span
    // TODO ArrayEx: Trim span

    // ----------------------------------------------------

    // TODO ArrayEx: AddRange span
    // TODO ArrayEx: InsertRange span
    // TODO ArrayEx: Remove span
    // TODO ArrayEx: RemoveLast span
    // TODO ArrayEx: RemoveAll span
}
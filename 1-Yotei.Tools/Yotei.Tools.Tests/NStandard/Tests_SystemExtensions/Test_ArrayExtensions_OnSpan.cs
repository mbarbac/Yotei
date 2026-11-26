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

    // TODO ArraEx: IndexOf_Range span
    // TODO ArraEx: LastIndexOf_Range span
    // TODO ArraEx: Contains_Range span
    // TODO ArraEx: Contains_Range span
    // TODO ArraEx: StartsWith_Range span
    // TODO ArraEx: EndsWith_Range span

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

    // TODO ArraEx: StartsWithAny span
    // TODO ArraEx: EndsWithAny span

    // ----------------------------------------------------

    // TODO ArraEx: IndexOfAnyExcept span
    // TODO ArraEx: LastIndexOfAnyExcept span
    // TODO ArraEx: IndexesOfAnyExcept span
    // TODO ArraEx: ContainsAnyExcept span
    // TODO ArraEx: StartsWithAnyExcept span
    // TODO ArraEx: EndsWithAnyExcept span

    // ----------------------------------------------------

    // TODO ArraEx: TrimStart span
    // TODO ArraEx: TrimEnd span
    // TODO ArraEx: Trim span

    // ----------------------------------------------------

    // TODO ArraEx: AddRange span
    // TODO ArraEx: InsertRange span
    // TODO ArraEx: Remove span
    // TODO ArraEx: RemoveLast span
    // TODO ArraEx: RemoveAll span
}
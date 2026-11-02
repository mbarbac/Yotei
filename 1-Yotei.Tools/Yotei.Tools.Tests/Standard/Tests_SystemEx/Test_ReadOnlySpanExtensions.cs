#if NETSTANDARD2_0
namespace Yotei.Tools.Std20.Tests;
#else
using Xunit.Internal;

namespace Yotei.Tools;
#endif

// ========================================================
public static class Test_ReadOnlySpanExtensions
{
    static Item<int> ToItem(int value) => new(value);
    static ReadOnlySpan<Item<int>> ToSpan(Item<int>[] values) => values.AsSpan();
    static ItemComparer<int> ToComparer() => new();

    // ----------------------------------------------------

    [Fact]
    public static void Test_SequenceEquals()
    {
        ReadOnlySpan<Item<int>> span;
        bool same;

        span = ToSpan([]); same = span.SequenceEqual([]); Assert.True(same);
        span = ToSpan([new(1)]); same = span.SequenceEqual([]); Assert.False(same);
        span = ToSpan([new(1)]); same = span.SequenceEqual([new(1)]); Assert.True(same);
        span = ToSpan([new(1), new(2)]); same = span.SequenceEqual([new(1)]); Assert.False(same);
        span = ToSpan([new(1), new(2)]); same = span.SequenceEqual([new(1), new(2)]); Assert.True(same);

        span = ToSpan([]); same = span.SequenceEqual([], ToComparer()); Assert.True(same);
        span = ToSpan([new(1)]); same = span.SequenceEqual([], ToComparer()); Assert.False(same);
        span = ToSpan([new(1)]); same = span.SequenceEqual([new(1)], ToComparer()); Assert.True(same);
        span = ToSpan([new(1), new(2)]); same = span.SequenceEqual([new(1)], ToComparer()); Assert.False(same);
        span = ToSpan([new(1), new(2)]); same = span.SequenceEqual([new(1), new(2)], ToComparer()); Assert.True(same);
    }

    [Fact]
    public static void Test_SequenceCompareTo()
    {
        ReadOnlySpan<Item<int>> span;
        int same;

        span = ToSpan([]); same = span.SequenceCompareTo([]); Assert.Equal(0, same);
        span = ToSpan([new(1), new(2)]); same = span.SequenceCompareTo([]); Assert.Equal(+1, same);
        span = ToSpan([]); same = span.SequenceCompareTo([new(1), new(2)]); Assert.Equal(-1, same);
        span = ToSpan([new(1), new(2)]); same = span.SequenceCompareTo([new(1), new(2)]); Assert.Equal(0, same);

        span = ToSpan([]); same = span.SequenceCompareTo([], ToComparer()); Assert.Equal(0, same);
        span = ToSpan([new(1), new(2)]); same = span.SequenceCompareTo([], ToComparer()); Assert.Equal(+1, same);
        span = ToSpan([]); same = span.SequenceCompareTo([new(1), new(2)], ToComparer()); Assert.Equal(-1, same);
        span = ToSpan([new(1), new(2)]); same = span.SequenceCompareTo([new(1), new(2)], ToComparer()); Assert.Equal(0, same);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_Contains()
    {
        Predicate<Item<int>> Predicate(int value) => x => x.Value == value;
        ReadOnlySpan<Item<int>> span;
        bool same;

        span = ToSpan([]);
        same = span.Contains(ToItem(9)); Assert.False(same);
        same = span.Contains(new(9), ToComparer()); Assert.False(same);
        same = span.Contains(Predicate(9));

        span = ToSpan([new(1), new(2)]);
        same = span.Contains(ToItem(9)); Assert.False(same);
        same = span.Contains(new(9), ToComparer()); Assert.False(same);
        same = span.Contains(Predicate(9)); Assert.False(same);

        span = ToSpan([new(1), new(2), new(1), new(2)]);
        same = span.Contains(ToItem(2)); Assert.False(same);
        same = span.Contains(new(2), ToComparer()); Assert.True(same);
        same = span.Contains(Predicate(2)); Assert.True(same);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IndexOf()
    {
        Predicate<Item<int>> Predicate(int value) => x => x.Value == value;
        ReadOnlySpan<Item<int>> span;
        int index;

        span = ToSpan([]);
        index = span.IndexOf(ToItem(9)); Assert.Equal(-1, index);
        index = span.IndexOf(new(9), ToComparer()); Assert.Equal(-1, index);
        index = span.IndexOf(Predicate(9));

        span = ToSpan([new(1), new(2)]);
        index = span.IndexOf(ToItem(9)); Assert.Equal(-1, index);
        index = span.IndexOf(new(9), ToComparer()); Assert.Equal(-1, index);
        index = span.IndexOf(Predicate(9)); Assert.Equal(-1, index);

        span = ToSpan([new(1), new(2), new(1), new(2)]);
        index = span.IndexOf(ToItem(2)); Assert.Equal(-1, index);
        index = span.IndexOf(new(2), ToComparer()); Assert.Equal(1, index);
        index = span.IndexOf(Predicate(2)); Assert.Equal(1, index);
    }

    [Fact]
    public static void Test_LastIndexOf()
    {
        Predicate<Item<int>> Predicate(int value) => x => x.Value == value;
        ReadOnlySpan<Item<int>> span;
        int index;

        span = ToSpan([]);
        index = span.LastIndexOf(ToItem(9)); Assert.Equal(-1, index);
        index = span.LastIndexOf(new(9), ToComparer()); Assert.Equal(-1, index);
        index = span.LastIndexOf(Predicate(9));

        span = ToSpan([new(1), new(2)]);
        index = span.LastIndexOf(ToItem(9)); Assert.Equal(-1, index);
        index = span.LastIndexOf(new(9), ToComparer()); Assert.Equal(-1, index);
        index = span.LastIndexOf(Predicate(9)); Assert.Equal(-1, index);

        span = ToSpan([new(1), new(2), new(1), new(2)]);
        index = span.LastIndexOf(ToItem(2)); Assert.Equal(-1, index);
        index = span.LastIndexOf(new(2), ToComparer()); Assert.Equal(3, index);
        index = span.LastIndexOf(Predicate(2)); Assert.Equal(3, index);
    }

    [Fact]
    public static void Test_IndexesOf()
    {
        Predicate<Item<int>> Predicate(int value) => x => x.Value == value;
        ReadOnlySpan<Item<int>> span;
        List<int> indexes;

        span = ToSpan([]);
        indexes = span.IndexesOf(ToItem(9)); Assert.Empty(indexes);
        indexes = span.IndexesOf(new(9), ToComparer()); Assert.Empty(indexes);
        indexes = span.IndexesOf(Predicate(9)); Assert.Empty(indexes);
        
        span = ToSpan([new(1), new(2)]);
        indexes = span.IndexesOf(ToItem(9)); Assert.Empty(indexes);
        indexes = span.IndexesOf(new(9), ToComparer()); Assert.Empty(indexes);
        indexes = span.IndexesOf(Predicate(9)); Assert.Empty(indexes);

        span = ToSpan([new(1), new(2), new(1), new(2)]);
        indexes = span.IndexesOf(ToItem(2)); Assert.Empty(indexes);
        indexes = span.IndexesOf(ToItem(2), ToComparer()); Assert.True(indexes is [1, 3]);
        indexes = span.IndexesOf(Predicate(2)); Assert.True(indexes is [1, 3]);
    }
}
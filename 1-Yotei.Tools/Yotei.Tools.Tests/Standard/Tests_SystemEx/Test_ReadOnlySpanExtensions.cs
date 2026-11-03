#if NETSTANDARD2_0
namespace Yotei.Tools.Std20.Tests;
#else
namespace Yotei.Tools;
#endif
#pragma warning disable IDE0079
#pragma warning disable IDE0059, CA1859

/* Note:
* - Two instances of the Item<T> type are not equal to each other, even if they cary the same
*   value, because it is a reference type. This is on purpose to check reference types with custom
*   comparers and predicate. Reference types with value semantics are easily implemented.
*/
// ========================================================
public static class Test_ReadOnlySpanExtensions
{
    static Item<int> Item(int value) => new(value);
    static ReadOnlySpan<Item<int>> Span(Item<int>[] values) => values.AsSpan();
    static ComparerFactory.InnerComparer<Item<int>> Comparer() => Item<int>.Create();
    static Predicate<Item<int>> Predicate(int value) => x => x.Value == value;

    // ----------------------------------------------------

    [Fact]
    public static void Test_SequenceEquals()
    {
        ReadOnlySpan<Item<int>> span = Span([]);        
        Assert.True(span.SequenceEqual([]));
        Assert.True(span.SequenceEqual([], Comparer()));

        span = Span([Item(1)]);        
        Assert.False(span.SequenceEqual([]));
        Assert.False(span.SequenceEqual([Item(1)]));

        Assert.False(span.SequenceEqual([], Comparer()));
        Assert.True(span.SequenceEqual([Item(1)], Comparer()));

        span = Span([Item(1), Item(2)]);        
        Assert.False(span.SequenceEqual([]));
        Assert.False(span.SequenceEqual([Item(1)]));
        Assert.False(span.SequenceEqual([Item(1), Item(2)]));

        Assert.False(span.SequenceEqual([], Comparer()));
        Assert.False(span.SequenceEqual([Item(1)], Comparer()));
        Assert.True(span.SequenceEqual([Item(1), Item(2)], Comparer()));
    }

    [Fact]
    public static void Test_SequenceCompareTo()
    {
        ReadOnlySpan<Item<int>> span = Span([]);
        Assert.Equal(0, span.SequenceCompareTo([]));
        Assert.Equal(-1, span.SequenceCompareTo([Item(1)]));

        Assert.Equal(0, span.SequenceCompareTo([], Comparer()));        
        Assert.Equal(-1, span.SequenceCompareTo([Item(1)], Comparer()));

        span = Span([Item(5)]);
        Assert.Equal(+1, span.SequenceCompareTo([]));
        Assert.Equal(-1, span.SequenceCompareTo([Item(1)]));
        Assert.Equal(-1, span.SequenceCompareTo([Item(9)]));
        Assert.Equal(-1, span.SequenceCompareTo([Item(5)]));

        Assert.Equal(+1, span.SequenceCompareTo([], Comparer()));
        Assert.Equal(+1, span.SequenceCompareTo([Item(1)], Comparer()));
        Assert.Equal(-1, span.SequenceCompareTo([Item(9)], Comparer()));
        Assert.Equal(0, span.SequenceCompareTo([Item(5)], Comparer()));

        span = Span([Item(5), Item(6)]);
        Assert.Equal(+1, span.SequenceCompareTo([]));
        Assert.Equal(-1, span.SequenceCompareTo([Item(5)]));
        Assert.Equal(-1, span.SequenceCompareTo([Item(4), Item(6)]));
        Assert.Equal(-1, span.SequenceCompareTo([Item(5), Item(7)]));
        Assert.Equal(-1, span.SequenceCompareTo([Item(5), Item(6), Item(7)]));

        Assert.Equal(+1, span.SequenceCompareTo([], Comparer()));
        Assert.Equal(+1, span.SequenceCompareTo([Item(5)], Comparer()));
        Assert.Equal(+1, span.SequenceCompareTo([Item(4), Item(6)], Comparer()));
        Assert.Equal(-1, span.SequenceCompareTo([Item(5), Item(7)], Comparer()));
        Assert.Equal(-1, span.SequenceCompareTo([Item(5), Item(6), Item(7)], Comparer()));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_Contains()
    {
        ReadOnlySpan<Item<int>> span = Span([]);
        Assert.False(span.Contains(Item(9)));
        Assert.False(span.Contains(Item(9), Comparer()));
        Assert.False(span.Contains(Predicate(9)));

        span = Span([Item(1), Item(2)]);
        Assert.False(span.Contains(Item(9)));
        Assert.False(span.Contains(Item(9), Comparer()));
        Assert.False(span.Contains(Predicate(9)));

        span = Span([Item(1), Item(2), Item(1), Item(2)]);
        Assert.False(span.Contains(Item(2)));
        Assert.True(span.Contains(Item(2), Comparer()));
        Assert.True(span.Contains(Predicate(2)));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IndexOf()
    {
        ReadOnlySpan<Item<int>> span = Span([]);
        Assert.Equal(-1, span.IndexOf(Item(9)));
        Assert.Equal(-1, span.IndexOf(Item(9), Comparer()));
        Assert.Equal(-1, span.IndexOf(Predicate(9)));

        span = Span([Item(1), Item(2)]);
        Assert.Equal(-1, span.IndexOf(Item(9)));
        Assert.Equal(-1, span.IndexOf(Item(9), Comparer()));
        Assert.Equal(-1, span.IndexOf(Predicate(9)));

        span = Span([Item(1), Item(2), Item(1), Item(2)]);
        Assert.Equal(-1, span.IndexOf(Item(2)));
        Assert.Equal(1, span.IndexOf(Item(2), Comparer()));
        Assert.Equal(1, span.IndexOf(Predicate(2)));
    }

    [Fact]
    public static void Test_LastIndexOf()
    {
        ReadOnlySpan<Item<int>> span = Span([]);
        Assert.Equal(-1, span.LastIndexOf(Item(9)));
        Assert.Equal(-1, span.LastIndexOf(Item(9), Comparer()));
        Assert.Equal(-1, span.LastIndexOf(Predicate(9)));

        span = Span([Item(1), Item(2)]);
        Assert.Equal(-1, span.LastIndexOf(Item(9)));
        Assert.Equal(-1, span.LastIndexOf(Item(9), Comparer()));
        Assert.Equal(-1, span.LastIndexOf(Predicate(9)));

        span = Span([Item(1), Item(2), Item(1), Item(2)]);
        Assert.Equal(-1, span.LastIndexOf(Item(2)));
        Assert.Equal(3, span.LastIndexOf(Item(2), Comparer()));
        Assert.Equal(3, span.LastIndexOf(Predicate(2)));
    }

    [Fact]
    public static void Test_IndexesOf()
    {
        ReadOnlySpan<Item<int>> span = Span([]);
        Assert.Empty(span.IndexesOf(Item(9)));
        Assert.Empty(span.IndexesOf(Item(9), Comparer()));
        Assert.Empty(span.IndexesOf(Predicate(9)));

        span = Span([Item(1), Item(2)]);
        Assert.Empty(span.IndexesOf(Item(9)));
        Assert.Empty(span.IndexesOf(Item(9), Comparer()));
        Assert.Empty(span.IndexesOf(Predicate(9)));

        span = Span([Item(1), Item(2), Item(1), Item(2)]);
        Assert.Empty(span.IndexesOf(Item(2)));
        Assert.True(span.IndexesOf(Item(2), Comparer()) is [1,3]);
        Assert.True(span.IndexesOf(Predicate(2)) is [1, 3]);
    }

    // ----------------------------------------------------
#if !NETSTANDARD2_0
    [Fact]
    public static void Test()
    {
        ReadOnlySpan<Item<int>> span = Span([]);
        Assert.Equal(-1, span.IndexOfAnyExcept(Item(9)));
        Assert.Equal(-1, span.IndexOfAnyExcept(Item(9), Comparer()));
        Assert.Equal(-1, span.IndexOfAnyExcept(Predicate(9)));


        //span = Span([]); index = span.IndexOfAnyExcept(Item(9)); Assert.Equal(-1, index);
        //span = Span([Item(9)]); index = span.IndexOfAnyExcept(Item(9)); Assert.Equal(0, index);
        //span = Span([Item(9), Item(9), Item(9)]); index = span.IndexOfAnyExcept(Item(9)); Assert.Equal(0, index);

        //span = Span([]); index = span.IndexOfAnyExcept(Item(9), Comparer()); Assert.Equal(-1, index);
        //span = Span([Item(9)]); index = span.IndexOfAnyExcept(Item(9), Comparer()); Assert.Equal(-1, index);
        //span = Span([Item(9), Item(9), Item(9)]); index = span.IndexOfAnyExcept(Item(9), Comparer()); Assert.Equal(-1, index);
        //span = Span([Item(1), Item(2)]); index = span.IndexOfAnyExcept(Item(9), Comparer()); Assert.Equal(0, index);
        //span = Span([Item(9), Item(9), Item(1), Item(2)]); Assert.Equal(2, index = span.IndexOfAnyExcept(Item(9), Comparer()));

        //span = Span([]); index = span.IndexOfAnyExcept(Predicate(9));
    }
#endif
}
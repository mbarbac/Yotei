#if NETSTANDARD2_0
namespace Yotei.Tools.Std20.Tests;
#else
namespace Yotei.Tools;
#endif

// ========================================================
public static class Test_ReadOnlySpanExtensions
{
    [Fact]
    public static void Test_Contains()
    {
        ReadOnlySpan<int> span;
        Predicate<int> Predicate(int value) => x => x == value;
        IEqualityComparer<int> Comparer(
            int value) => EqualityComparer<int>.Create((x, y) => x == value && y == value);

        span = []; Assert.False(span.Contains(9));
        span = [9]; Assert.True(span.Contains(9));
        span = [0, 9]; Assert.True(span.Contains(9));
        span = [9, 0]; Assert.True(span.Contains(9));

        span = []; Assert.False(span.Contains(9, Comparer(9)));
        span = [9]; Assert.True(span.Contains(9, Comparer(9)));
        span = [0, 9]; Assert.True(span.Contains(9, Comparer(9)));
        span = [9, 0]; Assert.True(span.Contains(9, Comparer(9)));

        span = []; Assert.False(span.Contains(Predicate(9)));
        span = [9]; Assert.True(span.Contains(Predicate(9)));
        span = [0, 9]; Assert.True(span.Contains(Predicate(9)));
        span = [9, 0]; Assert.True(span.Contains(Predicate(9)));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IndexOf()
    {
        List<int> indexes;
        int index;
        ReadOnlySpan<int> span;
        Predicate<int> Predicate(int value) => x => x == value;
        IEqualityComparer<int> Comparer(int value)
            => EqualityComparer<int>.Create((x, y)
            => x == value && y == value);

        span = [1, 2, 3, 1, 2];

        index = span.IndexOf(99); Assert.Equal(-1, index);
        index = span.IndexOf(2); Assert.Equal(1, index);
        index = span.IndexOf(2, Comparer(2)); Assert.Equal(1, index);
        index = span.IndexOf(Predicate(2)); Assert.Equal(1, index);

        index = span.LastIndexOf(99); Assert.Equal(-1, index);
        index = span.LastIndexOf(1); Assert.Equal(3, index);
        index = span.LastIndexOf(1, Comparer(1)); Assert.Equal(3, index);
        index = span.LastIndexOf(Predicate(1)); Assert.Equal(3, index);

        indexes = span.IndexesOf(99); Assert.Empty(indexes);
        indexes = span.IndexesOf(1); Assert.True(indexes is [0, 3]);
        indexes = span.IndexesOf(1, Comparer(1)); Assert.True(indexes is [0, 3]);
        indexes = span.IndexesOf(Predicate(1)); Assert.True(indexes is [0, 3]);
    }
}
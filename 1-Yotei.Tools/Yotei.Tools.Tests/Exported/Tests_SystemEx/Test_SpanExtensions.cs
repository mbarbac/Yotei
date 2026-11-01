namespace Yotei.Tools.Tests;

// ========================================================
public static class Test_SpanExtensions
{
    [Fact]
    public static void Test_IndexOf()
    {
        List<int> indexes;
        int index;
        Span<int> span = [1, 2, 3, 1, 2];
        Predicate<int> Predicate(int value) => x => x == value;
        IEqualityComparer<int> Comparer(
            int value) => EqualityComparer<int>.Create((x, y) => x == value && y == value);

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
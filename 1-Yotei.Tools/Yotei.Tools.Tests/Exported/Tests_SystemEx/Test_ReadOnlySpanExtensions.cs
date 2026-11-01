namespace Yotei.Tools.Tests;

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
        IEqualityComparer<int> Comparer(
            int value) => EqualityComparer<int>.Create((x, y) => x == value && y == value);

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

    // ----------------------------------------------------

    [Fact]
    public static void Test_FirstCropIndex()
    {
        int index;
        ReadOnlySpan<int> span;
        Predicate<int> Predicate(int value) => x => x == value;
        IEqualityComparer<int> Comparer(
            int value) => EqualityComparer<int>.Create((x, y) => x == value && y == value);

        span = []; index = span.FirstCropIndex(9); Assert.Equal(-1, index);
        span = [9]; index = span.FirstCropIndex(9); Assert.Equal(-1, index);
        span = [9, 9, 9]; index = span.FirstCropIndex(9); Assert.Equal(-1, index);
        span = [0, 1]; index = span.FirstCropIndex(9); Assert.Equal(0, index);
        span = [9, 0, 1]; index = span.FirstCropIndex(9); Assert.Equal(1, index);
        span = [9, 9, 0, 1]; index = span.FirstCropIndex(9); Assert.Equal(2, index);
        span = [0, 9, 0, 1]; index = span.FirstCropIndex(9); Assert.Equal(0, index);

        span = []; index = span.FirstCropIndex(9, Comparer(9)); Assert.Equal(-1, index);
        span = [9]; index = span.FirstCropIndex(9, Comparer(9)); Assert.Equal(-1, index);
        span = [9, 9, 9]; index = span.FirstCropIndex(9, Comparer(9)); Assert.Equal(-1, index);
        span = [0, 1]; index = span.FirstCropIndex(9, Comparer(9)); Assert.Equal(0, index);
        span = [9, 0, 1]; index = span.FirstCropIndex(9, Comparer(9)); Assert.Equal(1, index);
        span = [9, 9, 0, 1]; index = span.FirstCropIndex(9, Comparer(9)); Assert.Equal(2, index);
        span = [0, 9, 0, 1]; index = span.FirstCropIndex(9, Comparer(9)); Assert.Equal(0, index);

        span = []; index = span.FirstCropIndex(Predicate(9)); Assert.Equal(-1, index);
        span = [9]; index = span.FirstCropIndex(Predicate(9)); Assert.Equal(-1, index);
        span = [9, 9, 9]; index = span.FirstCropIndex(Predicate(9)); Assert.Equal(-1, index);
        span = [0, 1]; index = span.FirstCropIndex(Predicate(9)); Assert.Equal(0, index);
        span = [9, 0, 1]; index = span.FirstCropIndex(Predicate(9)); Assert.Equal(1, index);
        span = [9, 9, 0, 1]; index = span.FirstCropIndex(Predicate(9)); Assert.Equal(2, index);
        span = [0, 9, 0, 1]; index = span.FirstCropIndex(Predicate(9)); Assert.Equal(0, index);
    }

    [Fact]
    public static void Test_LastCropIndex()
    {
        int index;
        ReadOnlySpan<int> span;
        Predicate<int> Predicate(int value) => x => x == value;
        IEqualityComparer<int> Comparer(
            int value) => EqualityComparer<int>.Create((x, y) => x == value && y == value);

        span = []; index = span.LastCropIndex(9); Assert.Equal(-1, index);
        span = [9]; index = span.LastCropIndex(9); Assert.Equal(-1, index);
        span = [9, 9, 9]; index = span.LastCropIndex(9); Assert.Equal(-1, index);
        span = [0, 1]; index = span.LastCropIndex(9); Assert.Equal(1, index);
        span = [0, 1, 9]; index = span.LastCropIndex(9); Assert.Equal(1, index);
        span = [0, 1, 9, 9]; index = span.LastCropIndex(9); Assert.Equal(1, index);
        span = [0, 1, 9, 0]; index = span.LastCropIndex(9); Assert.Equal(3, index);

        span = []; index = span.LastCropIndex(0, Comparer(9)); Assert.Equal(-1, index);
        span = [9]; index = span.LastCropIndex(9, Comparer(9)); Assert.Equal(-1, index);
        span = [9, 9, 9]; index = span.LastCropIndex(9, Comparer(9)); Assert.Equal(-1, index);
        span = [0, 1]; index = span.LastCropIndex(9, Comparer(9)); Assert.Equal(1, index);
        span = [0, 1, 9]; index = span.LastCropIndex(9, Comparer(9)); Assert.Equal(1, index);
        span = [0, 1, 9, 9]; index = span.LastCropIndex(9, Comparer(9)); Assert.Equal(1, index);
        span = [0, 1, 9, 0]; index = span.LastCropIndex(9, Comparer(9)); Assert.Equal(3, index);

        span = []; index = span.LastCropIndex(Predicate(9)); Assert.Equal(-1, index);
        span = [9]; index = span.LastCropIndex(Predicate(9)); Assert.Equal(-1, index);
        span = [9, 9, 9]; index = span.LastCropIndex(Predicate(9)); Assert.Equal(-1, index);
        span = [0, 1]; index = span.LastCropIndex(Predicate(9)); Assert.Equal(1, index);
        span = [0, 1, 9]; index = span.LastCropIndex(Predicate(9)); Assert.Equal(1, index);
        span = [0, 1, 9, 9]; index = span.LastCropIndex(Predicate(9)); Assert.Equal(1, index);
        span = [0, 1, 9, 0]; index = span.LastCropIndex(Predicate(9)); Assert.Equal(3, index);
    }

    // ----------------------------------------------------

    [Fact]
    public static void TrimStart()
    {
        ReadOnlySpan<int> source;
        ReadOnlySpan<int> target;
        Predicate<int> Predicate(int value) => x => x == value;
        IEqualityComparer<int> Comparer(
            int value) => EqualityComparer<int>.Create((x, y) => x == value && y == value);

        source = []; target = source.TrimStart(); Assert.True(target.IsEmpty);
        source = [9]; target = source.TrimStart(9); Assert.True(target.IsEmpty);
        source = [9, 0]; target = source.TrimStart(9); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.TrimStart(9); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.TrimStart(9); Assert.True(target is [0, 9, 9]);

        source = [9]; target = source.TrimStart(9, Comparer(9)); Assert.True(target.IsEmpty);
        source = [9, 0]; target = source.TrimStart(9, Comparer(9)); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.TrimStart(9, Comparer(9)); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.TrimStart(9, Comparer(9)); Assert.True(target is [0, 9, 9]);

        source = [9]; target = source.TrimStart(Predicate(9)); Assert.True(target.IsEmpty);
        source = [9, 0]; target = source.TrimStart(Predicate(9)); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.TrimStart(Predicate(9)); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.TrimStart(Predicate(9)); Assert.True(target is [0, 9, 9]);
    }

    [Fact]
    public static void TrimEnd()
    {
        ReadOnlySpan<int> source;
        ReadOnlySpan<int> target;
        Predicate<int> Predicate(int value) => x => x == value;
        IEqualityComparer<int> Comparer(
            int value) => EqualityComparer<int>.Create((x, y) => x == value && y == value);

        source = []; target = source.TrimEnd(); Assert.True(target.IsEmpty);
        source = [9]; target = source.TrimEnd(9); Assert.True(target.IsEmpty);
        source = [0, 9]; target = source.TrimEnd(9); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.TrimEnd(9); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.TrimEnd(9); Assert.True(target is [9, 9, 0]);

        source = [9]; target = source.TrimEnd(9, Comparer(9)); Assert.True(target.IsEmpty);
        source = [0, 9]; target = source.TrimEnd(9, Comparer(9)); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.TrimEnd(9, Comparer(9)); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.TrimEnd(9, Comparer(9)); Assert.True(target is [9, 9, 0]);

        source = [9]; target = source.TrimEnd(Predicate(9)); Assert.True(target.IsEmpty);
        source = [0, 9]; target = source.TrimEnd(Predicate(9)); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.TrimEnd(Predicate(9)); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.TrimEnd(Predicate(9)); Assert.True(target is [9, 9, 0]);
    }

    [Fact]
    public static void Trim()
    {
        ReadOnlySpan<int> source;
        ReadOnlySpan<int> target;
        Predicate<int> Predicate(int value) => x => x == value;
        IEqualityComparer<int> Comparer(
            int value) => EqualityComparer<int>.Create((x, y) => x == value && y == value);

        source = []; target = source.Trim(); Assert.True(target.IsEmpty);
        source = [9]; target = source.Trim(9); Assert.True(target.IsEmpty);
        source = [0, 9]; target = source.Trim(9); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.Trim(9); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.Trim(9); Assert.True(target is [0]);
        source = [9, 0, 0, 9]; target = source.Trim(9); Assert.True(target is [0, 0]);
        source = [0, 9, 9, 0]; target = source.Trim(9); Assert.True(target is [0, 9, 9, 0]);

        source = [9]; target = source.Trim(9, Comparer(9)); Assert.True(target.IsEmpty);
        source = [0, 9]; target = source.Trim(9, Comparer(9)); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.Trim(9, Comparer(9)); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.Trim(9, Comparer(9)); Assert.True(target is [0]);
        source = [9, 0, 0, 9]; target = source.Trim(9, Comparer(9)); Assert.True(target is [0, 0]);
        source = [0, 9, 9, 0]; target = source.Trim(9, Comparer(9)); Assert.True(target is [0, 9, 9, 0]);

        source = [9]; target = source.Trim(Predicate(9)); Assert.True(target.IsEmpty);
        source = [0, 9]; target = source.Trim(Predicate(9)); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.Trim(Predicate(9)); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.Trim(Predicate(9)); Assert.True(target is [0]);
        source = [9, 0, 0, 9]; target = source.Trim(Predicate(9)); Assert.True(target is [0, 0]);
        source = [0, 9, 9, 0]; target = source.Trim(Predicate(9)); Assert.True(target is [0, 9, 9, 0]);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_StartsWith()
    {
        ReadOnlySpan<int> span;
        Predicate<int> Predicate(int value) => x => x == value;
        IEqualityComparer<int> Comparer(
            int value) => EqualityComparer<int>.Create((x, y) => x == value && y == value);

        span = []; Assert.False(span.StartsWith(9));
        span = [9]; Assert.True(span.StartsWith(9));
        span = [0, 9]; Assert.False(span.StartsWith(9));
        span = [9, 0]; Assert.True(span.StartsWith(9));

        span = []; Assert.False(span.StartsWith(9, Comparer(9)));
        span = [9]; Assert.True(span.StartsWith(9, Comparer(9)));
        span = [0, 9]; Assert.False(span.StartsWith(9, Comparer(9)));
        span = [9, 0]; Assert.True(span.StartsWith(9, Comparer(9)));

        span = []; Assert.False(span.StartsWith(Predicate(9)));
        span = [9]; Assert.True(span.StartsWith(Predicate(9)));
        span = [0, 9]; Assert.False(span.StartsWith(Predicate(9)));
        span = [9, 0]; Assert.True(span.StartsWith(Predicate(9)));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_EndsWith()
    {
        ReadOnlySpan<int> span;
        Predicate<int> Predicate(int value) => x => x == value;
        IEqualityComparer<int> Comparer(
            int value) => EqualityComparer<int>.Create((x, y) => x == value && y == value);

        span = []; Assert.False(span.EndsWith(9));
        span = [9]; Assert.True(span.EndsWith(9));
        span = [0, 9]; Assert.True(span.EndsWith(9));
        span = [9, 0]; Assert.False(span.EndsWith(9));

        span = []; Assert.False(span.EndsWith(9, Comparer(9)));
        span = [9]; Assert.True(span.EndsWith(9, Comparer(9)));
        span = [0, 9]; Assert.True(span.EndsWith(9, Comparer(9)));
        span = [9, 0]; Assert.False(span.EndsWith(9, Comparer(9)));

        span = []; Assert.False(span.EndsWith(Predicate(9)));
        span = [9]; Assert.True(span.EndsWith(Predicate(9)));
        span = [0, 9]; Assert.True(span.EndsWith(Predicate(9)));
        span = [9, 0]; Assert.False(span.EndsWith(Predicate(9)));
    }
}
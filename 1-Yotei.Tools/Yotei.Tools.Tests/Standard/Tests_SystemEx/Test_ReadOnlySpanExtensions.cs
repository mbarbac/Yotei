#if NETSTANDARD2_0
namespace Yotei.Tools.Std20.Tests;
#else
using Xunit.Runner.Common;

namespace Yotei.Tools;
#endif

// ========================================================
public static class Test_ReadOnlySpanExtensions
{
    readonly struct MyComparer<T> : IComparer<T>, IEqualityComparer<T> where T : IComparable
    {
        public int Compare(T? x, T? y)
        {
            if (x is null && y is null) return 0;
            if (x is null) return -1;
            if (y is null) return +1;
            return x.CompareTo(y);
        }
        public bool Equals(T? x, T? y)
        {
            if (x is null && y is null) return true;
            if (x is null || y is null) return false;
            return x.CompareTo(y) == 0;
        }
        public int GetHashCode([DisallowNull] T obj) => obj.GetHashCode();
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_SequenceCompareTo()
    {
        int comp;
        ReadOnlySpan<int> span;

        span = []; comp = span.SequenceCompareTo([]); Assert.Equal(0, comp);
        span = [1, 2]; comp = span.SequenceCompareTo([1, 2]); Assert.Equal(0, comp);
        span = [1, 2]; comp = span.SequenceCompareTo([1, 2, 3]); Assert.Equal(-1, comp);
        span = [1, 2, 3]; comp = span.SequenceCompareTo([1, 2]); Assert.Equal(+1, comp);
        span = [1, 1]; comp = span.SequenceCompareTo([1, 2]); Assert.Equal(-1, comp);
        span = [1, 2]; comp = span.SequenceCompareTo([1, 1]); Assert.Equal(+1, comp);

        span = []; comp = span.SequenceCompareTo([], new MyComparer<int>()); Assert.Equal(0, comp);
        span = [1, 2]; comp = span.SequenceCompareTo([1, 2], new MyComparer<int>()); Assert.Equal(0, comp);
        span = [1, 2]; comp = span.SequenceCompareTo([1, 2, 3], new MyComparer<int>()); Assert.Equal(-1, comp);
        span = [1, 2, 3]; comp = span.SequenceCompareTo([1, 2], new MyComparer<int>()); Assert.Equal(+1, comp);
        span = [1, 1]; comp = span.SequenceCompareTo([1, 2], new MyComparer<int>()); Assert.Equal(-1, comp);
        span = [1, 2]; comp = span.SequenceCompareTo([1, 1], new MyComparer<int>()); Assert.Equal(+1, comp);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_Contains()
    {
        ReadOnlySpan<int> span;
        Predicate<int> Predicate(int value) => x => x == value;

        span = []; Assert.False(span.Contains(9));
        span = [9]; Assert.True(span.Contains(9));
        span = [0, 9]; Assert.True(span.Contains(9));
        span = [9, 0]; Assert.True(span.Contains(9));

        span = []; Assert.False(span.Contains(9, new MyComparer<int>()));
        span = [9]; Assert.True(span.Contains(9, new MyComparer<int>()));
        span = [0, 9]; Assert.True(span.Contains(9, new MyComparer<int>()));
        span = [9, 0]; Assert.True(span.Contains(9, new MyComparer<int>()));

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

        span = [1, 2, 3, 1, 2];

        index = span.IndexOf(99); Assert.Equal(-1, index);
        index = span.IndexOf(2); Assert.Equal(1, index);
        index = span.IndexOf(2, new MyComparer<int>()); Assert.Equal(1, index);
        index = span.IndexOf(Predicate(2)); Assert.Equal(1, index);

        index = span.LastIndexOf(99); Assert.Equal(-1, index);
        index = span.LastIndexOf(1); Assert.Equal(3, index);
        index = span.LastIndexOf(1, new MyComparer<int>()); Assert.Equal(3, index);
        index = span.LastIndexOf(Predicate(1)); Assert.Equal(3, index);

        indexes = span.IndexesOf(99); Assert.Empty(indexes);
        indexes = span.IndexesOf(1); Assert.True(indexes is [0, 3]);
        indexes = span.IndexesOf(1, new MyComparer<int>()); Assert.True(indexes is [0, 3]);
        indexes = span.IndexesOf(Predicate(1)); Assert.True(indexes is [0, 3]);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_FirstCropIndex()
    {
        int index;
        ReadOnlySpan<int> span;
        Predicate<int> Predicate(int value) => x => x == value;

        span = []; index = span.FirstCropIndex(9); Assert.Equal(-1, index);
        span = [9]; index = span.FirstCropIndex(9); Assert.Equal(-1, index);
        span = [9, 9, 9]; index = span.FirstCropIndex(9); Assert.Equal(-1, index);
        span = [0, 1]; index = span.FirstCropIndex(9); Assert.Equal(0, index);
        span = [9, 0, 1]; index = span.FirstCropIndex(9); Assert.Equal(1, index);
        span = [9, 9, 0, 1]; index = span.FirstCropIndex(9); Assert.Equal(2, index);
        span = [0, 9, 0, 1]; index = span.FirstCropIndex(9); Assert.Equal(0, index);

        span = []; index = span.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(-1, index);
        span = [9]; index = span.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(-1, index);
        span = [9, 9, 9]; index = span.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(-1, index);
        span = [0, 1]; index = span.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(0, index);
        span = [9, 0, 1]; index = span.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(1, index);
        span = [9, 9, 0, 1]; index = span.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(2, index);
        span = [0, 9, 0, 1]; index = span.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(0, index);

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

        span = []; index = span.LastCropIndex(9); Assert.Equal(-1, index);
        span = [9]; index = span.LastCropIndex(9); Assert.Equal(-1, index);
        span = [9, 9, 9]; index = span.LastCropIndex(9); Assert.Equal(-1, index);
        span = [0, 1]; index = span.LastCropIndex(9); Assert.Equal(1, index);
        span = [0, 1, 9]; index = span.LastCropIndex(9); Assert.Equal(1, index);
        span = [0, 1, 9, 9]; index = span.LastCropIndex(9); Assert.Equal(1, index);
        span = [0, 1, 9, 0]; index = span.LastCropIndex(9); Assert.Equal(3, index);

        span = []; index = span.LastCropIndex(0, new MyComparer<int>()); Assert.Equal(-1, index);
        span = [9]; index = span.LastCropIndex(9, new MyComparer<int>()); Assert.Equal(-1, index);
        span = [9, 9, 9]; index = span.LastCropIndex(9, new MyComparer<int>()); Assert.Equal(-1, index);
        span = [0, 1]; index = span.LastCropIndex(9, new MyComparer<int>()); Assert.Equal(1, index);
        span = [0, 1, 9]; index = span.LastCropIndex(9, new MyComparer<int>()); Assert.Equal(1, index);
        span = [0, 1, 9, 9]; index = span.LastCropIndex(9, new MyComparer<int>()); Assert.Equal(1, index);
        span = [0, 1, 9, 0]; index = span.LastCropIndex(9, new MyComparer<int>()); Assert.Equal(3, index);

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

        source = []; target = source.TrimStart(); Assert.True(target.IsEmpty);
        source = [9]; target = source.TrimStart(9); Assert.True(target.IsEmpty);
        source = [9, 0]; target = source.TrimStart(9); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.TrimStart(9); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.TrimStart(9); Assert.True(target is [0, 9, 9]);

        source = [9]; target = source.TrimStart(9, new MyComparer<int>()); Assert.True(target.IsEmpty);
        source = [9, 0]; target = source.TrimStart(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.TrimStart(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.TrimStart(9, new MyComparer<int>()); Assert.True(target is [0, 9, 9]);

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

        source = []; target = source.TrimEnd(); Assert.True(target.IsEmpty);
        source = [9]; target = source.TrimEnd(9); Assert.True(target.IsEmpty);
        source = [0, 9]; target = source.TrimEnd(9); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.TrimEnd(9); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.TrimEnd(9); Assert.True(target is [9, 9, 0]);

        source = [9]; target = source.TrimEnd(9, new MyComparer<int>()); Assert.True(target.IsEmpty);
        source = [0, 9]; target = source.TrimEnd(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.TrimEnd(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.TrimEnd(9, new MyComparer<int>()); Assert.True(target is [9, 9, 0]);

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

        source = []; target = source.Trim(); Assert.True(target.IsEmpty);
        source = [9]; target = source.Trim(9); Assert.True(target.IsEmpty);
        source = [0, 9]; target = source.Trim(9); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.Trim(9); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.Trim(9); Assert.True(target is [0]);
        source = [9, 0, 0, 9]; target = source.Trim(9); Assert.True(target is [0, 0]);
        source = [0, 9, 9, 0]; target = source.Trim(9); Assert.True(target is [0, 9, 9, 0]);

        source = [9]; target = source.Trim(9, new MyComparer<int>()); Assert.True(target.IsEmpty);
        source = [0, 9]; target = source.Trim(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = [0, 9, 9]; target = source.Trim(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = [9, 9, 0]; target = source.Trim(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = [9, 0, 0, 9]; target = source.Trim(9, new MyComparer<int>()); Assert.True(target is [0, 0]);
        source = [0, 9, 9, 0]; target = source.Trim(9, new MyComparer<int>()); Assert.True(target is [0, 9, 9, 0]);

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

        span = []; Assert.False(span.StartsWith(9));
        span = [9]; Assert.True(span.StartsWith(9));
        span = [0, 9]; Assert.False(span.StartsWith(9));
        span = [9, 0]; Assert.True(span.StartsWith(9));

        span = []; Assert.False(span.StartsWith(9, new MyComparer<int>()));
        span = [9]; Assert.True(span.StartsWith(9, new MyComparer<int>()));
        span = [0, 9]; Assert.False(span.StartsWith(9, new MyComparer<int>()));
        span = [9, 0]; Assert.True(span.StartsWith(9, new MyComparer<int>()));

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

        span = []; Assert.False(span.EndsWith(9));
        span = [9]; Assert.True(span.EndsWith(9));
        span = [0, 9]; Assert.True(span.EndsWith(9));
        span = [9, 0]; Assert.False(span.EndsWith(9));

        span = []; Assert.False(span.EndsWith(9, new MyComparer<int>()));
        span = [9]; Assert.True(span.EndsWith(9, new MyComparer<int>()));
        span = [0, 9]; Assert.True(span.EndsWith(9, new MyComparer<int>()));
        span = [9, 0]; Assert.False(span.EndsWith(9, new MyComparer<int>()));

        span = []; Assert.False(span.EndsWith(Predicate(9)));
        span = [9]; Assert.True(span.EndsWith(Predicate(9)));
        span = [0, 9]; Assert.True(span.EndsWith(Predicate(9)));
        span = [9, 0]; Assert.False(span.EndsWith(Predicate(9)));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_ContainsAny()
    {
        ReadOnlySpan<int> span = [1, 2, 3, 1, 2];

        Assert.False(span.ContainsAny([98, 99]));
        Assert.False(span.ContainsAny([98, 99], new MyComparer<int>()));
        Assert.True(span.ContainsAny([98, 99, 1, 2]));
        Assert.True(span.ContainsAny([98, 99, 1, 2], new MyComparer<int>()));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IndexOfAny()
    {
        List<int> indexes;
        int index;
        ReadOnlySpan<int> span;

        span = [1, 2, 3, 1, 2];

        index = span.IndexOfAny([98, 99]); Assert.Equal(-1, index);
        index = span.IndexOfAny([9, 3]); Assert.Equal(2, index);
        index = span.IndexOfAny([9, 3], new MyComparer<int>()); Assert.Equal(2, index);

        index = span.LastIndexOfAny([98, 99]); Assert.Equal(-1, index);
        index = span.LastIndexOfAny([9, 3]); Assert.Equal(2, index);
        index = span.LastIndexOfAny([9, 1], new MyComparer<int>()); Assert.Equal(3, index);

        indexes = span.IndexesOfAny([98, 99]); Assert.Empty(indexes);
        indexes = span.IndexesOfAny([9, 3, 1]);
        Assert.Equal(3, indexes.Count);
        Assert.Equal(0, indexes[0]);
        Assert.Equal(2, indexes[1]);
        Assert.Equal(3, indexes[2]);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_StartsWithAny()
    {
        bool found;
        ReadOnlySpan<int> span;

        span = []; found = span.StartsWithAny([0, 99]); Assert.False(found);
        span = [1, 2]; found = span.StartsWithAny([0, 3]); Assert.False(found);
        span = [1, 2]; found = span.StartsWithAny([0, 1]); Assert.True(found);
        span = [1, 2]; found = span.StartsWithAny([0, 1], new MyComparer<int>()); Assert.True(found);
    }

    [Fact]
    public static void Test_EndsWithAny()
    {
        bool found;
        ReadOnlySpan<int> span;

        span = []; found = span.EndsWithAny([0, 99]); Assert.False(found);
        span = [1, 2]; found = span.EndsWithAny([0, 3]); Assert.False(found);
        span = [2, 1]; found = span.EndsWithAny([0, 1]); Assert.True(found);
        span = [2, 1]; found = span.EndsWithAny([0, 1], new MyComparer<int>()); Assert.True(found);
    }
}
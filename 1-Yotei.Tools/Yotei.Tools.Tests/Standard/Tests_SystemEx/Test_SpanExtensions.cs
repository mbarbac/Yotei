#if NETSTANDARD2_0
namespace Yotei.Tools.Std20.Tests;
#else
namespace Yotei.Tools;
#endif

// ========================================================
public static class Test_SpanExtensions
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

    [SuppressMessage("", "IDE0300")]
    [Fact]
    public static void Inherit_SequenceEqual()
    {
        Span<int> span;
        bool same;

        span = []; same = span.SequenceEqual([]); Assert.True(same);

        span = new int[] { 1, 2 };
        same = span.SequenceEqual([]); Assert.False(same);
        same = span.SequenceEqual([1]); Assert.False(same);
        same = span.SequenceEqual([1, 2, 3]); Assert.False(same);
        same = span.SequenceEqual([1, 2]); Assert.True(same);
        same = span.SequenceEqual([1, 2], new MyComparer<int>()); Assert.True(same);
    }

    [SuppressMessage("", "IDE0300")]
    [Fact]
    public static void Inherit_SequenceCompareTo()
    {
        int comp;
        Span<int> span;

        span = []; comp = span.SequenceCompareTo([]); Assert.Equal(0, comp);
        span = new int[] { 1, 2 }; comp = span.SequenceCompareTo([1, 2]); Assert.Equal(0, comp);
        span = new int[] { 1, 2 }; comp = span.SequenceCompareTo([1, 2, 3]); Assert.Equal(-1, comp);
        span = new int[] { 1, 2, 3 }; comp = span.SequenceCompareTo([1, 2]); Assert.Equal(+1, comp);
        span = new int[] { 1, 1 }; comp = span.SequenceCompareTo([1, 2]); Assert.Equal(-1, comp);
        span = new int[] { 1, 2 }; comp = span.SequenceCompareTo([1, 1]); Assert.Equal(+1, comp);

        span = []; comp = span.SequenceCompareTo([], new MyComparer<int>()); Assert.Equal(0, comp);
        span = new int[] { 1, 2 }; comp = span.SequenceCompareTo([1, 2], new MyComparer<int>()); Assert.Equal(0, comp);
        span = new int[] { 1, 2 }; comp = span.SequenceCompareTo([1, 2, 3], new MyComparer<int>()); Assert.Equal(-1, comp);
        span = new int[] { 1, 2, 3 }; comp = span.SequenceCompareTo([1, 2], new MyComparer<int>()); Assert.Equal(+1, comp);
        span = new int[] { 1, 1 }; comp = span.SequenceCompareTo([1, 2], new MyComparer<int>()); Assert.Equal(-1, comp);
        span = new int[] { 1, 2 }; comp = span.SequenceCompareTo([1, 1], new MyComparer<int>()); Assert.Equal(+1, comp);
    }

    // ----------------------------------------------------

    [SuppressMessage("", "IDE0300")]
    [Fact]
    public static void Inherit_Contains()
    {
        Span<int> span;
        Predicate<int> Predicate(int value) => x => x == value;

        span = []; Assert.False(span.Contains(9));
        span = new int[] { 9 }; Assert.True(span.Contains(9));
        span = new int[] { 0, 9 }; Assert.True(span.Contains(9));
        span = new int[] { 9, 0 }; Assert.True(span.Contains(9));

        span = []; Assert.False(span.Contains(9, new MyComparer<int>()));
        span = new int[] { 9 }; Assert.True(span.Contains(9, new MyComparer<int>()));
        span = new int[] { 0, 9 }; Assert.True(span.Contains(9, new MyComparer<int>()));
        span = new int[] { 9, 0 }; Assert.True(span.Contains(9, new MyComparer<int>()));

        span = []; Assert.False(span.Contains(Predicate(9)));
        span = new int[] { 9 }; Assert.True(span.Contains(Predicate(9)));
        span = new int[] { 0, 9 }; Assert.True(span.Contains(Predicate(9)));
        span = new int[] { 9, 0 }; Assert.True(span.Contains(Predicate(9)));
    }

    // ----------------------------------------------------

    [SuppressMessage("", "IDE0300")]
    [Fact]
    public static void Inherit_IndexOf()
    {
        List<int> indexes;
        int index;
        Span<int> span;
        Predicate<int> Predicate(int value) => x => x == value;

        span = new int[] { 1, 2, 3, 1, 2 };

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

    [SuppressMessage("", "IDE0300")]
    [Fact]
    public static void Inherit_FirstCropIndex()
    {
        int index;
        Span<int> span;
        Predicate<int> Predicate(int value) => x => x == value;

        span = []; index = span.FirstCropIndex(9); Assert.Equal(-1, index);
        span = new int[] { 9 }; index = span.FirstCropIndex(9); Assert.Equal(-1, index);
        span = new int[] { 9, 9, 9 }; index = span.FirstCropIndex(9); Assert.Equal(-1, index);
        span = new int[] { 0, 1 }; index = span.FirstCropIndex(9); Assert.Equal(0, index);
        span = new int[] { 9, 0, 1 }; index = span.FirstCropIndex(9); Assert.Equal(1, index);
        span = new int[] { 9, 9, 0, 1 }; index = span.FirstCropIndex(9); Assert.Equal(2, index);
        span = new int[] { 0, 9, 0, 1 }; index = span.FirstCropIndex(9); Assert.Equal(0, index);

        span = []; index = span.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(-1, index);
        span = new int[] { 9 }; index = span.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(-1, index);
        span = new int[] { 9, 9, 9 }; index = span.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(-1, index);
        span = new int[] { 0, 1 }; index = span.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(0, index);
        span = new int[] { 9, 0, 1 }; index = span.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(1, index);
        span = new int[] { 9, 9, 0, 1 }; index = span.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(2, index);
        span = new int[] { 0, 9, 0, 1 }; index = span.FirstCropIndex(9, new MyComparer<int>()); Assert.Equal(0, index);

        span = []; index = span.FirstCropIndex(Predicate(9)); Assert.Equal(-1, index);
        span = new int[] { 9 }; index = span.FirstCropIndex(Predicate(9)); Assert.Equal(-1, index);
        span = new int[] { 9, 9, 9 }; index = span.FirstCropIndex(Predicate(9)); Assert.Equal(-1, index);
        span = new int[] { 0, 1 }; index = span.FirstCropIndex(Predicate(9)); Assert.Equal(0, index);
        span = new int[] { 9, 0, 1 }; index = span.FirstCropIndex(Predicate(9)); Assert.Equal(1, index);
        span = new int[] { 9, 9, 0, 1 }; index = span.FirstCropIndex(Predicate(9)); Assert.Equal(2, index);
        span = new int[] { 0, 9, 0, 1 }; index = span.FirstCropIndex(Predicate(9)); Assert.Equal(0, index);
    }

    [SuppressMessage("", "IDE0300")]
    [Fact]
    public static void Inherit_LastCropIndex()
    {
        int index;
        Span<int> span;
        Predicate<int> Predicate(int value) => x => x == value;

        span = []; index = span.LastCropIndex(9); Assert.Equal(-1, index);
        span = new int[] { 9 }; index = span.LastCropIndex(9); Assert.Equal(-1, index);
        span = new int[] { 9, 9, 9 }; index = span.LastCropIndex(9); Assert.Equal(-1, index);
        span = new int[] { 0, 1 }; index = span.LastCropIndex(9); Assert.Equal(1, index);
        span = new int[] { 0, 1, 9 }; index = span.LastCropIndex(9); Assert.Equal(1, index);
        span = new int[] { 0, 1, 9, 9 }; index = span.LastCropIndex(9); Assert.Equal(1, index);
        span = new int[] { 0, 1, 9, 0 }; index = span.LastCropIndex(9); Assert.Equal(3, index);

        span = []; index = span.LastCropIndex(0, new MyComparer<int>()); Assert.Equal(-1, index);
        span = new int[] { 9 }; index = span.LastCropIndex(9, new MyComparer<int>()); Assert.Equal(-1, index);
        span = new int[] { 9, 9, 9 }; index = span.LastCropIndex(9, new MyComparer<int>()); Assert.Equal(-1, index);
        span = new int[] { 0, 1 }; index = span.LastCropIndex(9, new MyComparer<int>()); Assert.Equal(1, index);
        span = new int[] { 0, 1, 9 }; index = span.LastCropIndex(9, new MyComparer<int>()); Assert.Equal(1, index);
        span = new int[] { 0, 1, 9, 9 }; index = span.LastCropIndex(9, new MyComparer<int>()); Assert.Equal(1, index);
        span = new int[] { 0, 1, 9, 0 }; index = span.LastCropIndex(9, new MyComparer<int>()); Assert.Equal(3, index);

        span = []; index = span.LastCropIndex(Predicate(9)); Assert.Equal(-1, index);
        span = new int[] { 9 }; index = span.LastCropIndex(Predicate(9)); Assert.Equal(-1, index);
        span = new int[] { 9, 9, 9 }; index = span.LastCropIndex(Predicate(9)); Assert.Equal(-1, index);
        span = new int[] { 0, 1 }; index = span.LastCropIndex(Predicate(9)); Assert.Equal(1, index);
        span = new int[] { 0, 1, 9 }; index = span.LastCropIndex(Predicate(9)); Assert.Equal(1, index);
        span = new int[] { 0, 1, 9, 9 }; index = span.LastCropIndex(Predicate(9)); Assert.Equal(1, index);
        span = new int[] { 0, 1, 9, 0 }; index = span.LastCropIndex(Predicate(9)); Assert.Equal(3, index);
    }

    // ----------------------------------------------------

    [SuppressMessage("", "IDE0300")]
    [Fact]
    public static void Test_TrimStart() // Not inherit from ReadOnlySpan<T>
    {
        Span<int> source;
        Span<int> target;
        Predicate<int> Predicate(int value) => x => x == value;

        source = []; target = source.TrimStart(); Assert.True(target.IsEmpty);
        source = new int[] { 9 }; target = source.TrimStart(9); Assert.True(target.IsEmpty);
        source = new int[] { 9, 0 }; target = source.TrimStart(9); Assert.True(target is [0]);
        source = new int[] { 9, 9, 0 }; target = source.TrimStart(9); Assert.True(target is [0]);
        source = new int[] { 0, 9, 9 }; target = source.TrimStart(9); Assert.True(target is [0, 9, 9]);

        source = new int[] { 9 }; target = source.TrimStart(9, new MyComparer<int>()); Assert.True(target.IsEmpty);
        source = new int[] { 9, 0 }; target = source.TrimStart(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = new int[] { 9, 9, 0 }; target = source.TrimStart(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = new int[] { 0, 9, 9 }; target = source.TrimStart(9, new MyComparer<int>()); Assert.True(target is [0, 9, 9]);

        source = new int[] { 9 }; target = source.TrimStart(Predicate(9)); Assert.True(target.IsEmpty);
        source = new int[] { 9, 0 }; target = source.TrimStart(Predicate(9)); Assert.True(target is [0]);
        source = new int[] { 9, 9, 0 }; target = source.TrimStart(Predicate(9)); Assert.True(target is [0]);
        source = new int[] { 0, 9, 9 }; target = source.TrimStart(Predicate(9)); Assert.True(target is [0, 9, 9]);
    }

    [SuppressMessage("", "IDE0300")]
    [Fact]
    public static void Test_TrimEnd() // Not inherit from ReadOnlySpan<T>
    {
        Span<int> source;
        Span<int> target;
        Predicate<int> Predicate(int value) => x => x == value;

        source = []; target = source.TrimEnd(); Assert.True(target.IsEmpty);
        source = new int[] { 9 }; target = source.TrimEnd(9); Assert.True(target.IsEmpty);
        source = new int[] { 0, 9 }; target = source.TrimEnd(9); Assert.True(target is [0]);
        source = new int[] { 0, 9, 9 }; target = source.TrimEnd(9); Assert.True(target is [0]);
        source = new int[] { 9, 9, 0 }; target = source.TrimEnd(9); Assert.True(target is [9, 9, 0]);

        source = new int[] { 9 }; target = source.TrimEnd(9, new MyComparer<int>()); Assert.True(target.IsEmpty);
        source = new int[] { 0, 9 }; target = source.TrimEnd(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = new int[] { 0, 9, 9 }; target = source.TrimEnd(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = new int[] { 9, 9, 0 }; target = source.TrimEnd(9, new MyComparer<int>()); Assert.True(target is [9, 9, 0]);

        source = new int[] { 9 }; target = source.TrimEnd(Predicate(9)); Assert.True(target.IsEmpty);
        source = new int[] { 0, 9 }; target = source.TrimEnd(Predicate(9)); Assert.True(target is [0]);
        source = new int[] { 0, 9, 9 }; target = source.TrimEnd(Predicate(9)); Assert.True(target is [0]);
        source = new int[] { 9, 9, 0 }; target = source.TrimEnd(Predicate(9)); Assert.True(target is [9, 9, 0]);
    }

    [SuppressMessage("", "IDE0300")]
    [Fact]
    public static void Test_Trim() // Not inherit from ReadOnlySpan<T>
    {
        Span<int> source;
        Span<int> target;
        Predicate<int> Predicate(int value) => x => x == value;

        source = []; target = source.Trim(); Assert.True(target.IsEmpty);
        source = new int[] { 9 }; target = source.Trim(9); Assert.True(target.IsEmpty);
        source = new int[] { 0, 9 }; target = source.Trim(9); Assert.True(target is [0]);
        source = new int[] { 0, 9, 9 }; target = source.Trim(9); Assert.True(target is [0]);
        source = new int[] { 9, 9, 0 }; target = source.Trim(9); Assert.True(target is [0]);
        source = new int[] { 9, 0, 0, 9 }; target = source.Trim(9); Assert.True(target is [0, 0]);
        source = new int[] { 0, 9, 9, 0 }; target = source.Trim(9); Assert.True(target is [0, 9, 9, 0]);

        source = new int[] { 9 }; target = source.Trim(9, new MyComparer<int>()); Assert.True(target.IsEmpty);
        source = new int[] { 0, 9 }; target = source.Trim(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = new int[] { 0, 9, 9 }; target = source.Trim(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = new int[] { 9, 9, 0 }; target = source.Trim(9, new MyComparer<int>()); Assert.True(target is [0]);
        source = new int[] { 9, 0, 0, 9 }; target = source.Trim(9, new MyComparer<int>()); Assert.True(target is [0, 0]);
        source = new int[] { 0, 9, 9, 0 }; target = source.Trim(9, new MyComparer<int>()); Assert.True(target is [0, 9, 9, 0]);

        source = new int[] { 9 }; target = source.Trim(Predicate(9)); Assert.True(target.IsEmpty);
        source = new int[] { 0, 9 }; target = source.Trim(Predicate(9)); Assert.True(target is [0]);
        source = new int[] { 0, 9, 9 }; target = source.Trim(Predicate(9)); Assert.True(target is [0]);
        source = new int[] { 9, 9, 0 }; target = source.Trim(Predicate(9)); Assert.True(target is [0]);
        source = new int[] { 9, 0, 0, 9 }; target = source.Trim(Predicate(9)); Assert.True(target is [0, 0]);
        source = new int[] { 0, 9, 9, 0 }; target = source.Trim(Predicate(9)); Assert.True(target is [0, 9, 9, 0]);
    }

    // ----------------------------------------------------

    [SuppressMessage("", "IDE0300")]
    [Fact]
    public static void Inherit_StartsWith()
    {
        Span<int> span;
        Predicate<int> Predicate(int value) => x => x == value;

        span = []; Assert.False(span.StartsWith(9));
        span = new int[] { 9 }; Assert.True(span.StartsWith(9));
        span = new int[] { 0, 9 }; Assert.False(span.StartsWith(9));
        span = new int[] { 9, 0 }; Assert.True(span.StartsWith(9));

        span = []; Assert.False(span.StartsWith(9, new MyComparer<int>()));
        span = new int[] { 9 }; Assert.True(span.StartsWith(9, new MyComparer<int>()));
        span = new int[] { 0, 9 }; Assert.False(span.StartsWith(9, new MyComparer<int>()));
        span = new int[] { 9, 0 }; Assert.True(span.StartsWith(9, new MyComparer<int>()));

        span = []; Assert.False(span.StartsWith(Predicate(9)));
        span = new int[] { 9 }; Assert.True(span.StartsWith(Predicate(9)));
        span = new int[] { 0, 9 }; Assert.False(span.StartsWith(Predicate(9)));
        span = new int[] { 9, 0 }; Assert.True(span.StartsWith(Predicate(9)));
    }

    // ----------------------------------------------------

    [SuppressMessage("", "IDE0300")]
    [Fact]
    public static void Inherit_EndsWith()
    {
        Span<int> span;
        Predicate<int> Predicate(int value) => x => x == value;

        span = []; Assert.False(span.EndsWith(9));
        span = new int[] { 9 }; Assert.True(span.EndsWith(9));
        span = new int[] { 0, 9 }; Assert.True(span.EndsWith(9));
        span = new int[] { 9, 0 }; Assert.False(span.EndsWith(9));

        span = []; Assert.False(span.EndsWith(9, new MyComparer<int>()));
        span = new int[] { 9 }; Assert.True(span.EndsWith(9, new MyComparer<int>()));
        span = new int[] { 0, 9 }; Assert.True(span.EndsWith(9, new MyComparer<int>()));
        span = new int[] { 9, 0 }; Assert.False(span.EndsWith(9, new MyComparer<int>()));

        span = []; Assert.False(span.EndsWith(Predicate(9)));
        span = new int[] { 9 }; Assert.True(span.EndsWith(Predicate(9)));
        span = new int[] { 0, 9 }; Assert.True(span.EndsWith(Predicate(9)));
        span = new int[] { 9, 0 }; Assert.False(span.EndsWith(Predicate(9)));
    }

    // ----------------------------------------------------

    [SuppressMessage("", "IDE0300")]
    [Fact]
    public static void Inherit_ContainsAny()
    {
        Span<int> span = new int[] { 1, 2, 3, 1, 2 };

        Assert.False(span.ContainsAny([98, 99]));
        Assert.False(span.ContainsAny([98, 99], new MyComparer<int>()));
        Assert.True(span.ContainsAny([98, 99, 1, 2]));
        Assert.True(span.ContainsAny([98, 99, 1, 2], new MyComparer<int>()));
    }

    // ----------------------------------------------------

    [SuppressMessage("", "IDE0300")]
    [Fact]
    public static void Inherit_IndexOfAny()
    {
        List<int> indexes;
        int index;
        Span<int> span;

        span = new int[] { 1, 2, 3, 1, 2 };

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

    [SuppressMessage("", "IDE0300")]
    [Fact]
    public static void Inherit_StartsWithAny()
    {
        bool found;
        Span<int> span;

        span = []; found = span.StartsWithAny([0, 99]); Assert.False(found);
        span = new int[] { 1, 2 }; found = span.StartsWithAny([0, 3]); Assert.False(found);
        span = new int[] { 1, 2 }; found = span.StartsWithAny([0, 1]); Assert.True(found);
        span = new int[] { 1, 2 }; found = span.StartsWithAny([0, 1], new MyComparer<int>()); Assert.True(found);
    }

    [SuppressMessage("", "IDE0300")]
    [Fact]
    public static void Inherit_EndsWithAny()
    {
        bool found;
        Span<int> span;

        span = []; found = span.EndsWithAny([0, 99]); Assert.False(found);
        span = new int[] { 1, 2 }; found = span.EndsWithAny([0, 3]); Assert.False(found);
        span = new int[] { 2, 1 }; found = span.EndsWithAny([0, 1]); Assert.True(found);
        span = new int[] { 2, 1 }; found = span.EndsWithAny([0, 1], new MyComparer<int>()); Assert.True(found);
    }

    // ----------------------------------------------------

    [SuppressMessage("", "IDE0300")]
    [Fact]
    [SuppressMessage("", "IDE0018")]
    public static void Inherit_TryFind()
    {
        int value;
        List<int> list;
        Span<int> span;

        span = []; Assert.False(span.TryFind(x => x != 0, out value)); Assert.Equal(0, value);
        span = new int[] { 1, 3, 2 }; Assert.True(span.TryFind(x => x > 1, out value)); Assert.Equal(3, value);

        span = []; Assert.False(span.TryFindLast(x => x != 0, out value)); Assert.Equal(0, value);
        span = new int[] { 3, 2, 1 }; Assert.True(span.TryFindLast(x => x > 1, out value)); Assert.Equal(2, value);

        span = []; Assert.False(span.TryFindAll(x => x != 0, out list)); Assert.Empty(list);
        span = new int[] { 1, 3, 2, 1 }; Assert.True(span.TryFindAll(x => x > 1, out list));
        Assert.Equal(2, list.Count);
        Assert.Equal(3, list[0]);
        Assert.Equal(2, list[1]);
    }
}
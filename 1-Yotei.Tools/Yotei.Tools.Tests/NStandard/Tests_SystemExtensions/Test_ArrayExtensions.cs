#pragma warning disable IDE0079
#pragma warning disable CA1859, IDE0018
namespace Yotei.Tools.Tests;

// ========================================================
public static class Test_ArrayExtensions
{
    static Predicate<int> IntPredicate(int value) => x => x == value;
    static IEqualityComparer<int> IntEquality() => EqualityComparer<int>.Default;
    static IComparer<int> IntComparer() => Comparer<int>.Create((x, y) => x.CompareTo(y));

    // ----------------------------------------------------

    [Fact]
    public static void Test_Enumerators()
    {
        int[] nums = [1, 2, 3];

        var stditer = nums.GetEnumerator();
        var type = stditer.GetType();
        Assert.False(type.IsGenericType);

        var xtriter = nums.GetTypedEnumerator();
        type = xtriter.GetType();
        Assert.True(type.IsGenericType);
        type = type.GetGenericArguments()[0];
        Assert.Equal(typeof(int), type);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_Clone()
    {
        Item<int>[] source = [];
        Item<int>[] target = (Item<int>[])source.Clone(); Assert.NotSame(source, target);
        Assert.Empty(target);

        source = [new(1), new(2)];
        target = (Item<int>[])source.Clone(); Assert.NotSame(source, target);
        Assert.Equal(2, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);

        target = source.Clone(false); Assert.NotSame(source, target);
        Assert.Equal(2, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);

        target = source.Clone(true); Assert.NotSame(source, target);
        Assert.Equal(2, target.Length);
        Assert.NotSame(source[0], target[0]); Assert.Equal(source[0].Value, target[0].Value);
        Assert.NotSame(source[1], target[1]); Assert.Equal(source[1].Value, target[1].Value);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Inherit_IndexOf()
    {
        int result;
        var span = Array.Empty<int>();
        result = span.IndexOf(9); Assert.Equal(-1, result);
        result = span.IndexOf(9, IntEquality()); Assert.Equal(-1, result);
        result = span.IndexOf(IntPredicate(9)); Assert.Equal(-1, result);

        span = [1, 2, 3];
        result = span.IndexOf(0); Assert.Equal(-1, result);
        result = span.IndexOf(0, IntEquality()); Assert.Equal(-1, result);
        result = span.IndexOf(IntPredicate(0)); Assert.Equal(-1, result);

        span = [1, 0, 2, 0, 4];
        result = span.IndexOf(0); Assert.Equal(1, result);
        result = span.IndexOf(0, IntEquality()); Assert.Equal(1, result);
        result = span.IndexOf(IntPredicate(0)); Assert.Equal(1, result);
    }

    [Fact]
    public static void Inherit_LastIndexOf()
    {
        int result;
        var span = Array.Empty<int>();
        result = span.LastIndexOf(9); Assert.Equal(-1, result);
        result = span.LastIndexOf(9, IntEquality()); Assert.Equal(-1, result);
        result = span.LastIndexOf(IntPredicate(9)); Assert.Equal(-1, result);

        span = [1, 2, 3];
        result = span.LastIndexOf(0); Assert.Equal(-1, result);
        result = span.LastIndexOf(0, IntEquality()); Assert.Equal(-1, result);
        result = span.LastIndexOf(IntPredicate(0)); Assert.Equal(-1, result);

        span = [1, 0, 2, 0, 4];
        result = span.LastIndexOf(0); Assert.Equal(3, result);
        result = span.LastIndexOf(0, IntEquality()); Assert.Equal(3, result);
        result = span.LastIndexOf(IntPredicate(0)); Assert.Equal(3, result);
    }

    [Fact]
    public static void Inherit_IndexesOf()
    {
        List<int> result;
        var span = Array.Empty<int>();
        result = span.IndexesOf(9); Assert.True(result is []);
        result = span.IndexesOf(9, IntEquality()); Assert.True(result is []);
        result = span.IndexesOf(IntPredicate(9)); Assert.True(result is []);

        span = [1, 2, 3];
        result = span.IndexesOf(0); Assert.True(result is []);
        result = span.IndexesOf(0, IntEquality()); Assert.True(result is []);
        result = span.IndexesOf(IntPredicate(0)); Assert.True(result is []);

        span = [1, 0, 2, 0, 4];
        result = span.IndexesOf(0); Assert.True(result is [1, 3]);
        result = span.IndexesOf(0, IntEquality()); Assert.True(result is [1, 3]);
        result = span.IndexesOf(IntPredicate(0)); Assert.True(result is [1, 3]);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Inherit_Contains()
    {
        bool result;
        var span = Array.Empty<int>();
        result = span.Contains(9); Assert.False(result);
        result = span.Contains(9, IntEquality()); Assert.False(result);
        result = span.Contains(IntPredicate(9)); Assert.False(result);

        span = [1, 2, 3];
        result = span.Contains(0); Assert.False(result);
        result = span.Contains(0, IntEquality()); Assert.False(result);
        result = span.Contains(IntPredicate(0)); Assert.False(result);

        span = [1, 0, 2, 0, 4];
        result = span.Contains(0); Assert.True(result);
        result = span.Contains(0, IntEquality()); Assert.True(result);
        result = span.Contains(IntPredicate(0)); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Inherit_StartsWith()
    {
        bool result;
        var span = Array.Empty<int>();
        result = span.StartsWith(9); Assert.False(result);
        result = span.StartsWith(9, IntEquality()); Assert.False(result);
        result = span.StartsWith(IntPredicate(9)); Assert.False(result);

        span = [1, 2, 3];
        result = span.StartsWith(0); Assert.False(result);
        result = span.StartsWith(0, IntEquality()); Assert.False(result);
        result = span.StartsWith(IntPredicate(0)); Assert.False(result);

        span = [0, 2, 0, 4];
        result = span.StartsWith(0); Assert.True(result);
        result = span.StartsWith(0, IntEquality()); Assert.True(result);
        result = span.StartsWith(IntPredicate(0)); Assert.True(result);
    }

    [Fact]
    public static void Inherit_EndsWith()
    {
        bool result;
        var span = Array.Empty<int>();
        result = span.EndsWith(9); Assert.False(result);
        result = span.EndsWith(9, IntEquality()); Assert.False(result);
        result = span.EndsWith(IntPredicate(9)); Assert.False(result);

        span = [1, 2, 3];
        result = span.EndsWith(0); Assert.False(result);
        result = span.EndsWith(0, IntEquality()); Assert.False(result);
        result = span.EndsWith(IntPredicate(0)); Assert.False(result);

        span = [1, 2, 0];
        result = span.EndsWith(0); Assert.True(result);
        result = span.EndsWith(0, IntEquality()); Assert.True(result);
        result = span.EndsWith(IntPredicate(0)); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Inherit_IndexOfAny()
    {
        int result;
        var span = Array.Empty<int>();
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
    public static void Inherit_LastIndexOfAny()
    {
        int result;
        var span = Array.Empty<int>();
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
    public static void Inherit_ContainsAny()
    {
        bool result;
        var span = Array.Empty<int>();
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

    // ----------------------------------------------------

    [Fact]
    public static void Inherit_IndexOfAnyExcept()
    {
        int result;
        var span = Array.Empty<int>();
        result = span.IndexOfAnyExcept(9); Assert.Equal(-1, result);
        result = span.IndexOfAnyExcept(9, IntEquality()); Assert.Equal(-1, result);
        result = span.IndexOfAnyExcept(IntPredicate(9)); Assert.Equal(-1, result);

        span = [1, 2, 3];
        result = span.IndexOfAnyExcept(0); Assert.Equal(0, result);
        result = span.IndexOfAnyExcept(0, IntEquality()); Assert.Equal(0, result);
        result = span.IndexOfAnyExcept(IntPredicate(0)); Assert.Equal(0, result);

        span = [0, 0];
        result = span.IndexOfAnyExcept(0); Assert.Equal(-1, result);
        result = span.IndexOfAnyExcept(0, IntEquality()); Assert.Equal(-1, result);
        result = span.IndexOfAnyExcept(IntPredicate(0)); Assert.Equal(-1, result);

        span = [0, 0, 1, 2];
        result = span.IndexOfAnyExcept(0); Assert.Equal(2, result);
        result = span.IndexOfAnyExcept(0, IntEquality()); Assert.Equal(2, result);
        result = span.IndexOfAnyExcept(IntPredicate(0)); Assert.Equal(2, result);
    }

    [Fact]
    public static void Inherit_LastIndexOfAnyExcept()
    {
        int result;
        var span = Array.Empty<int>();
        result = span.LastIndexOfAnyExcept(9); Assert.Equal(-1, result);
        result = span.LastIndexOfAnyExcept(9, IntEquality()); Assert.Equal(-1, result);
        result = span.LastIndexOfAnyExcept(IntPredicate(9)); Assert.Equal(-1, result);

        span = [1, 2, 3];
        result = span.LastIndexOfAnyExcept(0); Assert.Equal(2, result);
        result = span.LastIndexOfAnyExcept(0, IntEquality()); Assert.Equal(2, result);
        result = span.LastIndexOfAnyExcept(IntPredicate(0)); Assert.Equal(2, result);

        span = [0, 0, 0];
        result = span.LastIndexOfAnyExcept(0); Assert.Equal(-1, result);
        result = span.LastIndexOfAnyExcept(0, IntEquality()); Assert.Equal(-1, result);
        result = span.LastIndexOfAnyExcept(IntPredicate(0)); Assert.Equal(-1, result);

        span = [1, 2, 0, 0];
        result = span.LastIndexOfAnyExcept(0); Assert.Equal(1, result);
        result = span.LastIndexOfAnyExcept(0, IntEquality()); Assert.Equal(1, result);
        result = span.LastIndexOfAnyExcept(IntPredicate(0)); Assert.Equal(1, result);
    }

    [Fact]
    public static void Inherit_ContainsAnyExcept()
    {
        bool result;
        var span = Array.Empty<int>();
        result = span.ContainsAnyExcept(9); Assert.False(result);
        result = span.ContainsAnyExcept(9, IntEquality()); Assert.False(result);
        result = span.ContainsAnyExcept(IntPredicate(9)); Assert.False(result);

        span = [1, 2, 3];
        result = span.ContainsAnyExcept(0); Assert.True(result);
        result = span.ContainsAnyExcept(0, IntEquality()); Assert.True(result);
        result = span.ContainsAnyExcept(IntPredicate(0)); Assert.True(result);

        span = [0, 0, 0];
        result = span.ContainsAnyExcept(0); Assert.False(result);
        result = span.ContainsAnyExcept(0, IntEquality()); Assert.False(result);
        result = span.ContainsAnyExcept(IntPredicate(0)); Assert.False(result);

        span = [1, 2, 0, 0];
        result = span.ContainsAnyExcept(0); Assert.True(result);
        result = span.ContainsAnyExcept(0, IntEquality()); Assert.True(result);
        result = span.ContainsAnyExcept(IntPredicate(0)); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Inherit_SequenceCompareTo()
    {
        int result;
        var span = Array.Empty<int>();
        var values = Array.Empty<int>();
        result = span.SequenceCompareTo(values); Assert.Equal(0, result);
        result = span.SequenceCompareTo(values, IntComparer()); Assert.Equal(0, result);

        span = [1, 2, 3];
        values = [1, 2, 3];
        result = span.SequenceCompareTo(values); Assert.Equal(0, result);
        result = span.SequenceCompareTo(values, IntComparer()); Assert.Equal(0, result);

        values = [1, 2];
        result = span.SequenceCompareTo(values); Assert.Equal(+1, result);
        result = span.SequenceCompareTo(values, IntComparer()); Assert.Equal(+1, result);

        values = [1, 2, 3, 4];
        result = span.SequenceCompareTo(values); Assert.Equal(-1, result);
        result = span.SequenceCompareTo(values, IntComparer()); Assert.Equal(-1, result);

        span = [1, 2, 4];
        values = [1, 2, 3];
        result = span.SequenceCompareTo(values); Assert.Equal(+1, result);
        result = span.SequenceCompareTo(values, IntComparer()); Assert.Equal(+1, result);

        span = [1, 2, 2];
        values = [1, 2, 3];
        result = span.SequenceCompareTo(values); Assert.Equal(-1, result);
        result = span.SequenceCompareTo(values, IntComparer()); Assert.Equal(-1, result);
    }

    [Fact]
    public static void Inherit_SequenceEqual()
    {
        bool result;
        var span = Array.Empty<int>();
        var values = Array.Empty<int>();
        result = span.SequenceEqual(values); Assert.True(result);
        result = span.SequenceEqual(values, IntEquality()); Assert.True(result);

        span = [1, 2, 3];
        values = [1, 2, 3];
        result = span.SequenceEqual(values); Assert.True(result);
        result = span.SequenceEqual(values, IntEquality()); Assert.True(result);

        values = [1, 2];
        result = span.SequenceEqual(values); Assert.False(result);
        result = span.SequenceEqual(values, IntEquality()); Assert.False(result);

        values = [1, 2, 3, 4];
        result = span.SequenceEqual(values); Assert.False(result);
        result = span.SequenceEqual(values, IntEquality()); Assert.False(result);

        span = [1, 2, 4];
        values = [1, 2, 3];
        result = span.SequenceEqual(values); Assert.False(result);
        result = span.SequenceEqual(values, IntEquality()); Assert.False(result);

        span = [1, 2, 2];
        values = [1, 2, 3];
        result = span.SequenceEqual(values); Assert.False(result);
        result = span.SequenceEqual(values, IntEquality()); Assert.False(result);
    }

    // ----------------------------------------------------
    [Fact]
    public static void Inherit_StartsWithAny()
    {
        bool result;
        var span = Array.Empty<int>();
        var values = Array.Empty<int>();
        result = span.StartsWithAny(values); Assert.False(result);
        result = span.StartsWithAny(values, IntEquality()); Assert.False(result);

        values = [8, 9];
        result = span.StartsWithAny(values); Assert.False(result);
        result = span.StartsWithAny(values, IntEquality()); Assert.False(result);

        span = [1, 2];
        values = [8, 9, 1];
        result = span.StartsWithAny(values); Assert.True(result);
        result = span.StartsWithAny(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Inherit_EndsWithAny()
    {
        bool result;
        var span = Array.Empty<int>();
        var values = Array.Empty<int>();
        result = span.EndsWithAny(values); Assert.False(result);
        result = span.EndsWithAny(values, IntEquality()); Assert.False(result);

        values = [8, 9];
        result = span.EndsWithAny(values); Assert.False(result);
        result = span.EndsWithAny(values, IntEquality()); Assert.False(result);

        span = [1, 2];
        values = [8, 9, 2];
        result = span.EndsWithAny(values); Assert.True(result);
        result = span.EndsWithAny(values, IntEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Inherit_TrimStart()
    {
        int[] source;
        int[] target;

        source = [];
        target = source.TrimStart(9); Assert.True(target.IsEmpty);
        target = source.TrimStart(9, IntEquality()); Assert.True(target.IsEmpty);
        target = source.TrimStart(IntPredicate(9)); Assert.True(target.IsEmpty);

        source = [1, 2, 3];
        target = source.TrimStart(0); Assert.True(target.SequenceEqual(source));
        target = source.TrimStart(0, IntEquality()); Assert.True(target.SequenceEqual(source));
        target = source.TrimStart(IntPredicate(0)); Assert.True(target.SequenceEqual(source));

        target = source.TrimStart(1); Assert.True(target is [2, 3]);
        target = source.TrimStart(1, IntEquality()); Assert.True(target is [2, 3]);
        target = source.TrimStart(IntPredicate(1)); Assert.True(target is [2, 3]);
    }

    [Fact]
    public static void Inherit_TrimEnd()
    {
        int[] values = [];
        int[] source = [];
        var target = source.TrimEnd(values); Assert.True(target.IsEmpty);
        target = source.TrimEnd(values, IntEquality()); Assert.True(target.IsEmpty);

        values = [1, 2];
        target = source.TrimEnd(values); Assert.True(target.IsEmpty);
        target = source.TrimEnd(values, IntEquality()); Assert.True(target.IsEmpty);

        source = [1, 2];
        values = [];
        target = source.TrimEnd(values); Assert.False(target.IsEmpty);
        target = source.TrimEnd(values, IntEquality()); Assert.False(target.IsEmpty);

        source = [1, 2, 3];
        values = [3];
        var expected = new int[] { 1, 2 };
        target = source.TrimEnd(values); Assert.True(target.SequenceEqual(expected));
        target = source.TrimEnd(values, IntEquality()); Assert.True(target.SequenceEqual(expected));
    }

    [Fact]
    public static void Inherit_Trim()
    {
        int[] values = [];
        int[] source = [];
        var target = source.Trim(values); Assert.True(target.IsEmpty);
        target = source.Trim(values, IntEquality()); Assert.True(target.IsEmpty);

        values = [1, 2];
        target = source.Trim(values); Assert.True(target.IsEmpty);
        target = source.Trim(values, IntEquality()); Assert.True(target.IsEmpty);

        source = [1, 2];
        values = [];
        target = source.Trim(values); Assert.False(target.IsEmpty);
        target = source.Trim(values, IntEquality()); Assert.False(target.IsEmpty);

        source = [1, 2, 3];
        values = [3, 1];
        var expected = new int[] { 2 };
        target = source.Trim(values); Assert.True(target.SequenceEqual(expected));
        target = source.Trim(values, IntEquality()); Assert.True(target.SequenceEqual(expected));
    }

    // ----------------------------------------------------

    [Fact]
    public static void Inherit_TryFind()
    {
        int[] span;
        int value;

        span = [];
        Assert.False(span.TryFind(x => x != 0, out value));

        span = [1, 2, 3];
        Assert.True(span.TryFind(x => x > 1, out value));
        Assert.Equal(2, value);
    }

    [Fact]
    public static void Inherit_TryFindLast()
    {
        int[] span;
        int value;

        span = [];
        Assert.False(span.TryFindLast(x => x != 0, out value));

        span = [1, 2, 3];
        Assert.True(span.TryFindLast(x => x > 1, out value));
        Assert.Equal(3, value);
    }

    [Fact]
    public static void Inherit_TryFindAll()
    {
        int[] span;
        List<int> values;

        span = [];
        Assert.False(span.TryFindAll(x => x != 0, out values));

        span = [1, 2, 3];
        Assert.True(span.TryFindAll(x => x > 1, out values));
        Assert.Equal(2, values.Count);
        Assert.Equal(2, values[0]);
        Assert.Equal(3, values[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ResizeHead()
    {
        var source = new int[] { 1, 2, 3, 4 };
        var target = source.ResizeHead(3);
        Assert.Equal(3, target.Length);
        Assert.Equal(2, target[0]);
        Assert.Equal(3, target[1]);
        Assert.Equal(4, target[2]);

        target = source.ResizeHead(5, 99);
        Assert.Equal(5, target.Length);
        Assert.Equal(99, target[0]);
        Assert.Equal(1, target[1]);
        Assert.Equal(2, target[2]);
        Assert.Equal(3, target[3]);
        Assert.Equal(4, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_ResizeTail()
    {
        var source = new int[] { 1, 2, 3, 4 };
        var target = source.ResizeTail(3);
        Assert.Equal(3, target.Length);
        Assert.Equal(1, target[0]);
        Assert.Equal(2, target[1]);
        Assert.Equal(3, target[2]);

        target = source.ResizeTail(5, 99);
        Assert.Equal(5, target.Length);
        Assert.Equal(1, target[0]);
        Assert.Equal(2, target[1]);
        Assert.Equal(3, target[2]);
        Assert.Equal(4, target[3]);
        Assert.Equal(99, target[4]);
    }
}
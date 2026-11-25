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
    static int[] IntArray() => [];
    static int[] IntArray(IEnumerable<int> values) => values.ToArray();

    static ReadOnlySpan<int> IntSpan() => [];
    static ReadOnlySpan<int> IntSpan(IEnumerable<int> values) => values.ToArray();

    static Predicate<int> IntPredicate(int value) => x => x == value;
    static IEqualityComparer<int> IntEquality() => EqualityComparer<int>.Default;
    static IComparer<int> IntComparer() => Comparer<int>.Create((x, y) => x.CompareTo(y));

    // ----------------------------------------------------

    static Item<int> Item(int value) => new(value);
    static Item<int>[] ItemArray() => [];
    static Item<int>[] ItemArray(IEnumerable<int> values) => [.. values.Select(x => Item(x))];

    static ReadOnlySpan<Item<int>> ItemSpan() => [];
    static ReadOnlySpan<Item<int>> ItemSpan(IEnumerable<int> values) => values.Select(x => Item(x)).ToArray();

    static Predicate<Item<int>> ItemPredicate(int value) => x => x.Value == value;
    static IEqualityComparer<Item<int>> ItemEquality() => EqualityComparerEx.New<Item<int>>((x, y) =>
    {
        if (x is null && y is null) return true;
        if (x is null) return false;
        if (y is null) return false;
        return x.Value == y.Value;
    });
    static IComparer<Item<int>> ItemComparer() => ComparerEx.New<Item<int>>((x, y) =>
    {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return +1;
        return x.Value.CompareTo(y.Value);
    });

    // ----------------------------------------------------

    [Fact]
    public static void Test_IsEmpty()
    {
        int[] nums = []; Assert.True(nums.IsEmpty);
        nums = [1, 2, 3]; Assert.False(nums.IsEmpty);
    }

    [Fact]
    public static void Test_TypedEnumerator()
    {
        int[] nums = [1, 2, 3];

        var iter = nums.GetEnumerator();
        var type = iter.GetType();
        Assert.False(type.IsGenericType);

        var iterex = nums.GetTypedEnumerator();
        type = iterex.GetType();
        Assert.True(type.IsGenericType);
        type = type.GetGenericArguments()[0];
        Assert.Equal(typeof(int), type);
    }

    [Fact]
    public static void Test_Clone()
    {
        var source = Array.Empty<Item<int>>();
        var result = (Item<int>[])source.Clone();
        Assert.NotSame(source, result);
        Assert.True(result.IsEmpty);

        source = ItemArray([1, 2, 3]);
        result = (Item<int>[])source.Clone();
        Assert.NotSame(source, result);
        Assert.Equal(3, result.Length);
        for (int i = 0; i < 3; i++) Assert.Same(source[i], result[i]);

        result = source.Clone(deep: true);
        Assert.NotSame(source, result);
        Assert.Equal(3, result.Length);
        for (int i = 0; i < 3; i++) Assert.NotSame(source[i], result[i]);
        for (int i = 0; i < 3; i++) Assert.Equal(source[i].Value, result[i].Value);
    }

    // ----------------------------------------------------

    // TODO ArraEx: IndexOf value
    // TODO ArraEx: LastIndexOf value
    // TODO ArraEx: IndexesOf value
    // TODO ArraEx: Contains value
    // TODO ArraEx: StartsWith value
    // TODO ArraEx: EndsWith value

    // ----------------------------------------------------

    // TODO ArraEx: IndexOfAny value
    // TODO ArraEx: LastIndexOfAny value
    // TODO ArraEx: IndexesOfAny value
    // TODO ArraEx: ContainsAny value
    // TODO ArraEx: StartsWithAny value
    // TODO ArraEx: EndsWithAny value

    // ----------------------------------------------------

    // TODO ArraEx: IndexOfAnyExcept value
    // TODO ArraEx: LastIndexOfAnyExcept value
    // TODO ArraEx: IndexesOfAnyExcept value
    // TODO ArraEx: ContainsAnyExcept value
    // TODO ArraEx: StartsWithAnyExcept value
    // TODO ArraEx: EndsWithAnyExcept value

    // ----------------------------------------------------

    // TODO ArraEx: ResizeStart
    // TODO ArraEx: ResizeEnd

    // ----------------------------------------------------

    // TODO ArraEx: TrimStart value
    // TODO ArraEx: TrimEnd value
    // TODO ArraEx: Trim value

    // ----------------------------------------------------

    // TODO ArraEx: Add value
    // TODO ArraEx: Insert value
    // TODO ArraEx: RemoveAt
    // TODO ArraEx: RemoveRange index count
    // TODO ArraEx: Remove value
    // TODO ArraEx: RemoveLast value
    // TODO ArraEx: RemoveAll value
    // TODO ArraEx: Clear
}
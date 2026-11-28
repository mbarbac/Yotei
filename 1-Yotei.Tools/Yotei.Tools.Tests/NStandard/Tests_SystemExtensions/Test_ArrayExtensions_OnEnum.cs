#pragma warning disable IDE0079
#pragma warning disable CA1859

#if NETSTANDARD2_0
namespace Yotei.Tools.NStd20.Tests;
#else
namespace Yotei.Tools.Tests;
#endif

// ========================================================
public static partial class Test_ArrayExtensions
{
    [Fact]
    public static void Test_SequenceEqual_Enum_Ints()
    {
        bool result;
        var source = Array.Empty<int>();
        var target = IntEnumerable();
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
    public static void Test_SequenceEqual_Enum_Items()
    {
        bool result;
        var source = Array.Empty<Item<int>>();
        var target = ItemEnumerable();
        result = source.SequenceEqual(target); Assert.True(result);
        result = source.SequenceEqual(target, ItemEquality()); Assert.True(result);

        source = ItemArray([1, 2]);
        result = source.SequenceEqual(target); Assert.False(result);
        result = source.SequenceEqual(target, ItemEquality()); Assert.False(result);

        target = ItemEnumerable([1, 2]);
        result = source.SequenceEqual(target); Assert.False(result); // elements are references
        result = source.SequenceEqual(target, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_SequenceCompareTo_Enum_Ints()
    {
        int result;
        var source = IntArray();
        var values = IntEnumerable();
        result = source.SequenceCompareTo(values); Assert.Equal(0, result);
        result = source.SequenceCompareTo(values, IntComparer()); Assert.Equal(0, result);

        source = [1, 2, 3];
        values = [1, 2, 3];
        result = source.SequenceCompareTo(values); Assert.Equal(0, result);
        result = source.SequenceCompareTo(values, IntComparer()); Assert.Equal(0, result);

        values = [1, 2];
        result = source.SequenceCompareTo(values); Assert.Equal(+1, result);
        result = source.SequenceCompareTo(values, IntComparer()); Assert.Equal(+1, result);

        values = [1, 2, 3, 4];
        result = source.SequenceCompareTo(values); Assert.Equal(-1, result);
        result = source.SequenceCompareTo(values, IntComparer()); Assert.Equal(-1, result);

        source = [1, 2, 4];
        values = [1, 2, 3];
        result = source.SequenceCompareTo(values); Assert.Equal(+1, result);
        result = source.SequenceCompareTo(values, IntComparer()); Assert.Equal(+1, result);

        source = [1, 2, 2];
        values = [1, 2, 3];
        result = source.SequenceCompareTo(values); Assert.Equal(-1, result);
        result = source.SequenceCompareTo(values, IntComparer()); Assert.Equal(-1, result);
    }

    [Fact]
    public static void Test_SequenceCompareTo_Enum_Items()
    {
        int result;
        var source = ItemArray();
        var values = ItemEnumerable();
        // result = source.SequenceCompareTo(values); // Item<> not IEquatable
        result = source.SequenceCompareTo(values, ItemComparer()); Assert.Equal(0, result);

        source = ItemArray([1, 2, 3]);
        values = ItemEnumerable([1, 2, 3]);
        result = source.SequenceCompareTo(values, ItemComparer()); Assert.Equal(0, result);

        values = ItemEnumerable([1, 2]);
        result = source.SequenceCompareTo(values, ItemComparer()); Assert.Equal(+1, result);

        values = ItemEnumerable([1, 2, 3, 4]);
        result = source.SequenceCompareTo(values, ItemComparer()); Assert.Equal(-1, result);

        source = ItemArray([1, 2, 4]);
        values = ItemEnumerable([1, 2, 3]);
        result = source.SequenceCompareTo(values, ItemComparer()); Assert.Equal(+1, result);

        source = ItemArray([1, 2, 2]);
        values = ItemEnumerable([1, 2, 3]);
        result = source.SequenceCompareTo(values, ItemComparer()); Assert.Equal(-1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IndexOf_Enum_Ints()
    {
        int result;
        var source = IntArray();
        var values = IntEnumerable();
        result = source.IndexOf(values); Assert.Equal(0, result);
        result = source.IndexOf(values, IntEquality()); Assert.Equal(0, result);

        source = [1, 2, 3, 4];
        values = [4, 5];
        result = source.IndexOf(values); Assert.Equal(-1, result);
        result = source.IndexOf(values, IntEquality()); Assert.Equal(-1, result);

        values = [2, 3];
        result = source.IndexOf(values); Assert.Equal(1, result);
        result = source.IndexOf(values, IntEquality()); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_IndexOf_Enum_Items()
    {
        int result;
        var source = ItemArray();
        var values = ItemEnumerable();
        // result = source.IndexOf(values); // Item<> not IEquatable
        result = source.IndexOf(values, ItemEquality()); Assert.Equal(0, result);

        source = ItemArray([1, 2, 3, 4]);
        values = ItemEnumerable([4, 5]);
        result = source.IndexOf(values, ItemEquality()); Assert.Equal(-1, result);

        values = ItemEnumerable([2, 3]);
        result = source.IndexOf(values, ItemEquality()); Assert.Equal(1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_LastIndexOf_Enum_Ints()
    {
        int result;
        var source = IntArray();
        var values = IntEnumerable();
        result = source.LastIndexOf(values); Assert.Equal(0, result);
        result = source.LastIndexOf(values, IntEquality()); Assert.Equal(0, result);

        source = [1, 2, 3, 4];
        values = [4, 5];
        result = source.LastIndexOf(values); Assert.Equal(-1, result);
        result = source.LastIndexOf(values, IntEquality()); Assert.Equal(-1, result);

        values = [2, 3];
        result = source.LastIndexOf(values); Assert.Equal(1, result);
        result = source.LastIndexOf(values, IntEquality()); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_LastIndexOf_Enum_Items()
    {
        int result;
        var source = ItemArray();
        var values = ItemEnumerable();
        // result = source.LastIndexOf(values); // Item<> not IEquatable
        result = source.LastIndexOf(values, ItemEquality()); Assert.Equal(0, result);

        source = ItemArray([1, 2, 3, 4]);
        values = ItemEnumerable([4, 5]);
        result = source.LastIndexOf(values, ItemEquality()); Assert.Equal(-1, result);

        values = ItemEnumerable([2, 3]);
        result = source.LastIndexOf(values, ItemEquality()); Assert.Equal(1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_Contains_Enum_Ints()
    {
        bool result;
        var source = IntArray();
        var values = IntEnumerable();
        result = source.Contains(values); Assert.True(result); // Standard behavior
        result = source.Contains(values, IntEquality()); Assert.True(result);

        source = [1, 2, 3, 4];
        values = [4, 5];
        result = source.Contains(values); Assert.False(result);
        result = source.Contains(values, IntEquality()); Assert.False(result);

        values = [2, 3];
        result = source.Contains(values); Assert.True(result);
        result = source.Contains(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_Contains_Enum_Items()
    {
        bool result;
        var source = ItemArray();
        var values = ItemEnumerable();
        // result = source.Contains(values); // Item<> not IEquatable
        result = source.Contains(values, ItemEquality()); Assert.True(result);

        source = ItemArray([1, 2, 3, 4]);
        values = ItemEnumerable([4, 5]);
        result = source.Contains(values, ItemEquality()); Assert.False(result);

        values = ItemEnumerable([2, 3]);
        result = source.Contains(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_StartsWith_Enum_Ints()
    {
        bool result;
        var source = IntArray();
        var values = IntEnumerable();
        result = source.StartsWith(values); Assert.True(result); // Standard behavior
        result = source.StartsWith(values, IntEquality()); Assert.True(result);

        source = [1, 2, 3, 4];
        values = [4, 5];
        result = source.StartsWith(values); Assert.False(result);
        result = source.StartsWith(values, IntEquality()); Assert.False(result);

        values = [1, 2];
        result = source.StartsWith(values); Assert.True(result);
        result = source.StartsWith(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_StartsWith_Enum_Items()
    {
        bool result;
        var source = ItemArray();
        var values = ItemEnumerable();
        // result = source.StartsWith(values); // Item<> not IEquatable
        result = source.StartsWith(values, ItemEquality()); Assert.True(result);

        source = ItemArray([1, 2, 3, 4]);
        values = ItemEnumerable([4, 5]);
        result = source.StartsWith(values, ItemEquality()); Assert.False(result);

        values = ItemEnumerable([1, 2]);
        result = source.StartsWith(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_EndsWith_Enum_Ints()
    {
        bool result;
        var source = IntArray();
        var values = IntEnumerable();
        result = source.EndsWith(values); Assert.False(result);
        result = source.EndsWith(values, IntEquality()); Assert.False(result);

        source = [1, 2, 3, 4];
        values = [4, 5];
        result = source.EndsWith(values); Assert.False(result);
        result = source.EndsWith(values, IntEquality()); Assert.False(result);

        values = [3, 4];
        result = source.EndsWith(values); Assert.True(result);
        result = source.EndsWith(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_EndsWith_Enum_Items()
    {
        bool result;
        var source = ItemArray();
        var values = ItemEnumerable();
        // result = source.EndsWith(values); // Item<> not IEquatable
        result = source.EndsWith(values, ItemEquality()); Assert.False(result);

        source = ItemArray([1, 2, 3, 4]);
        values = ItemEnumerable([4, 5]);
        result = source.EndsWith(values, ItemEquality()); Assert.False(result);

        values = ItemEnumerable([3, 4]);
        result = source.EndsWith(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IndexOfAny_Enum_Ints()
    {
        int result;
        var source = IntArray();
        var values = IntEnumerable();
        result = source.IndexOfAny(values); Assert.Equal(-1, result);
        result = source.IndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        source = [1, 2];
        values = IntEnumerable([8, 9]);
        result = source.IndexOfAny(values); Assert.Equal(-1, result);
        result = source.IndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        source = [1, 9, 2];
        values = IntEnumerable([8, 9]);
        result = source.IndexOfAny(values); Assert.Equal(1, result);
        result = source.IndexOfAny(values, IntEquality()); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_IndexOfAny_Enum_Items()
    {
        int result;
        var source = ItemArray();
        var values = ItemEnumerable();
        //result = source.IndexOfAny(values); // Item<> not IEquatable
        result = source.IndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemArray([1, 2]);
        values = ItemEnumerable([8, 9]);
        result = source.IndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemArray([1, 9, 2]);
        values = ItemEnumerable([8, 9]);
        result = source.IndexOfAny(values, ItemEquality()); Assert.Equal(1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_LastIndexOfAny_Enum_Ints()
    {
        int result;
        var source = IntArray();
        var values = IntEnumerable();
        result = source.LastIndexOfAny(values); Assert.Equal(-1, result);
        result = source.LastIndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        source = [1, 2];
        values = IntEnumerable([8, 9]);
        result = source.LastIndexOfAny(values); Assert.Equal(-1, result);
        result = source.LastIndexOfAny(values, IntEquality()); Assert.Equal(-1, result);

        source = [1, 9, 2];
        values = IntEnumerable([8, 9]);
        result = source.LastIndexOfAny(values); Assert.Equal(1, result);
        result = source.LastIndexOfAny(values, IntEquality()); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_LastIndexOfAny_Enum_Items()
    {
        int result;
        var source = ItemArray();
        var values = ItemEnumerable();
        //result = source.LastIndexOfAny(values); // Item<> not IEquatable
        result = source.LastIndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemArray([1, 2]);
        values = ItemEnumerable([8, 9]);
        result = source.LastIndexOfAny(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemArray([1, 9, 2]);
        values = ItemEnumerable([8, 9]);
        result = source.LastIndexOfAny(values, ItemEquality()); Assert.Equal(1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IndexesOfAny_Enum_Ints()
    {
        List<int> result;
        var source = IntArray();
        var values = IntEnumerable();
        result = source.IndexesOfAny(values); Assert.Empty(result);
        result = source.IndexesOfAny(values, IntEquality()); Assert.Empty(result);

        source = [1, 2];
        values = IntEnumerable([8, 9]);
        result = source.IndexesOfAny(values); Assert.Empty(result);
        result = source.IndexesOfAny(values, IntEquality()); Assert.Empty(result);

        source = [1, 9, 8, 2];
        values = IntEnumerable([8, 9]);
        result = source.IndexesOfAny(values); Assert.True(result is [1, 2]);
        result = source.IndexesOfAny(values, IntEquality()); Assert.True(result is [1, 2]);
    }

    [Fact]
    public static void Test_IndexesOfAny_Enum_Items()
    {
        List<int> result;
        var source = ItemArray();
        var values = ItemEnumerable();
        // result = source.IndexesOfAny(values); // Item<> not IEquatable
        result = source.IndexesOfAny(values, ItemEquality()); Assert.Empty(result);

        source = ItemArray([1, 2]);
        values = ItemEnumerable([8, 9]);
        result = source.IndexesOfAny(values, ItemEquality()); Assert.Empty(result);

        source = ItemArray([1, 9, 8, 2]);
        values = ItemEnumerable([8, 9]);
        result = source.IndexesOfAny(values, ItemEquality()); Assert.True(result is [1, 2]);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_ContainsAny_Enum_Ints()
    {
        bool result;
        var source = IntArray();
        var values = IntEnumerable();
        result = source.ContainsAny(values); Assert.False(result);
        result = source.ContainsAny(values, IntEquality()); Assert.False(result);

        source = [1, 2];
        values = IntEnumerable([8, 9]);
        result = source.ContainsAny(values); Assert.False(result);
        result = source.ContainsAny(values, IntEquality()); Assert.False(result);

        source = [1, 9, 2];
        values = IntEnumerable([8, 9]);
        result = source.ContainsAny(values); Assert.True(result);
        result = source.ContainsAny(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_ContainsAny_Enum_Items()
    {
        bool result;
        var source = ItemArray();
        var values = ItemEnumerable();
        //result = source.ContainsAny(values); // Item<> not IEquatable
        result = source.ContainsAny(values, ItemEquality()); Assert.False(result);

        source = ItemArray([1, 2]);
        values = ItemEnumerable([8, 9]);
        result = source.ContainsAny(values, ItemEquality()); Assert.False(result);

        source = ItemArray([1, 9, 2]);
        values = ItemEnumerable([8, 9]);
        result = source.ContainsAny(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_StartsWithAny_Enum_Ints()
    {
        bool result;
        var source = IntArray();
        var values = IntEnumerable();
        result = source.StartsWithAny(values); Assert.False(result);
        result = source.StartsWithAny(values, IntEquality()); Assert.False(result);

        source = [1, 2];
        values = IntEnumerable([8, 9]);
        result = source.StartsWithAny(values); Assert.False(result);
        result = source.StartsWithAny(values, IntEquality()); Assert.False(result);

        source = [9, 2];
        values = IntEnumerable([8, 9]);
        result = source.StartsWithAny(values); Assert.True(result);
        result = source.StartsWithAny(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_StartsWithAny_Enum_Items()
    {
        bool result;
        var source = ItemArray();
        var values = ItemEnumerable();
        //result = source.StartsWithAny(values); // Item<> not IEquatable
        result = source.StartsWithAny(values, ItemEquality()); Assert.False(result);

        source = ItemArray([1, 2]);
        values = ItemEnumerable([8, 9]);
        result = source.StartsWithAny(values, ItemEquality()); Assert.False(result);

        source = ItemArray([9, 2]);
        values = ItemEnumerable([8, 9]);
        result = source.StartsWithAny(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_EndsWithAny_Enum_Ints()
    {
        bool result;
        var source = IntArray();
        var values = IntEnumerable();
        result = source.EndsWithAny(values); Assert.False(result);
        result = source.EndsWithAny(values, IntEquality()); Assert.False(result);

        source = [1, 2];
        values = IntEnumerable([8, 9]);
        result = source.EndsWithAny(values); Assert.False(result);
        result = source.EndsWithAny(values, IntEquality()); Assert.False(result);

        source = [1, 9];
        values = IntEnumerable([8, 9]);
        result = source.EndsWithAny(values); Assert.True(result);
        result = source.EndsWithAny(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_EndsWithAny_Enum_Items()
    {
        bool result;
        var source = ItemArray();
        var values = ItemEnumerable();
        //result = source.EndsWithAny(values); // Item<> not IEquatable
        result = source.EndsWithAny(values, ItemEquality()); Assert.False(result);

        source = ItemArray([1, 2]);
        values = ItemEnumerable([8, 9]);
        result = source.EndsWithAny(values, ItemEquality()); Assert.False(result);

        source = ItemArray([1, 9]);
        values = ItemEnumerable([8, 9]);
        result = source.EndsWithAny(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IndexOfAnyExcept_Enum_Ints()
    {
        int result;
        var source = IntArray();
        var values = IntEnumerable();
        result = source.IndexOfAnyExcept(values); Assert.Equal(-1, result);
        result = source.IndexOfAnyExcept(values, IntEquality()); Assert.Equal(-1, result);

        source = [1, 2, 3, 4];
        values = IntEnumerable([8, 9]);
        result = source.IndexOfAnyExcept(values); Assert.Equal(0, result);
        result = source.IndexOfAnyExcept(values, IntEquality()); Assert.Equal(0, result);

        source = [1, 2, 3, 4];
        values = IntEnumerable([8, 1, 2]);
        result = source.IndexOfAnyExcept(values); Assert.Equal(2, result);
        result = source.IndexOfAnyExcept(values, IntEquality()); Assert.Equal(2, result);
    }

    [Fact]
    public static void Test_IndexOfAnyExcept_Enum_Items()
    {
        int result;
        var source = ItemArray();
        var values = ItemEnumerable();
        // result = source.IndexOfAnyExcept(values); // Item<> not IEquatable
        result = source.IndexOfAnyExcept(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemArray([1, 2, 3, 4]);
        values = ItemEnumerable([8, 9]);
        result = source.IndexOfAnyExcept(values, ItemEquality()); Assert.Equal(0, result);

        source = ItemArray([1, 2, 3, 4]);
        values = ItemEnumerable([8, 1, 2]);
        result = source.IndexOfAnyExcept(values, ItemEquality()); Assert.Equal(2, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_LastIndexOfAnyExcept_Enum_Ints()
    {
        int result;
        var source = IntArray();
        var values = IntEnumerable();
        result = source.LastIndexOfAnyExcept(values); Assert.Equal(-1, result);
        result = source.LastIndexOfAnyExcept(values, IntEquality()); Assert.Equal(-1, result);

        source = [1, 2, 3, 4];
        values = IntEnumerable([8, 9]);
        result = source.LastIndexOfAnyExcept(values); Assert.Equal(3, result);
        result = source.LastIndexOfAnyExcept(values, IntEquality()); Assert.Equal(3, result);

        source = [1, 2, 3, 4];
        values = IntEnumerable([8, 3, 4]);
        result = source.LastIndexOfAnyExcept(values); Assert.Equal(1, result);
        result = source.LastIndexOfAnyExcept(values, IntEquality()); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_LastIndexOfAnyExcept_Enum_Items()
    {
        int result;
        var source = ItemArray();
        var values = ItemEnumerable();
        // result = source.LastIndexOfAnyExcept(values); // Item<> not IEquatable
        result = source.LastIndexOfAnyExcept(values, ItemEquality()); Assert.Equal(-1, result);

        source = ItemArray([1, 2, 3, 4]);
        values = ItemEnumerable([8, 9]);
        result = source.LastIndexOfAnyExcept(values, ItemEquality()); Assert.Equal(3, result);

        source = ItemArray([1, 2, 3, 4]);
        values = ItemEnumerable([8, 3, 4]);
        result = source.LastIndexOfAnyExcept(values, ItemEquality()); Assert.Equal(1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IndexesOfAnyExcept_Enum_Ints()
    {
        List<int> result;
        var source = IntArray();
        var values = IntEnumerable();
        result = source.IndexesOfAnyExcept(values); Assert.Empty(result);
        result = source.IndexesOfAnyExcept(values, IntEquality()); Assert.Empty(result);

        source = [1, 2, 3, 4];
        values = IntEnumerable([1, 4]);
        result = source.IndexesOfAnyExcept(values); Assert.True(result is [1, 2]);
        result = source.IndexesOfAnyExcept(values, IntEquality()); Assert.True(result is [1, 2]);
    }

    [Fact]
    public static void Test_IndexesOfAnyExcept_Enum_Items()
    {
        List<int> result;
        var source = ItemArray();
        var values = ItemEnumerable();
        // result = source.IndexesOfAnyExcept(values); // Item<> not IEquatable
        result = source.IndexesOfAnyExcept(values, ItemEquality()); Assert.Empty(result);

        source = ItemArray([1, 2, 3, 4]);
        values = ItemEnumerable([1, 4]);
        result = source.IndexesOfAnyExcept(values, ItemEquality()); Assert.True(result is [1, 2]);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_ContainsAnyExcept_Enum_Ints()
    {
        bool result;
        var source = IntArray();
        var values = IntEnumerable();
        result = source.ContainsAnyExcept(values); Assert.False(result);
        result = source.ContainsAnyExcept(values, IntEquality()); Assert.False(result);

        source = [1, 2, 3];
        values = IntEnumerable([1, 2, 3]);
        result = source.ContainsAnyExcept(values); Assert.False(result);
        result = source.ContainsAnyExcept(values, IntEquality()); Assert.False(result);

        source = [1, 2, 3];
        values = IntEnumerable([1, 2]);
        result = source.ContainsAnyExcept(values); Assert.True(result);
        result = source.ContainsAnyExcept(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_ContainsAnyExcept_Enum_Items()
    {
        bool result;
        var source = ItemArray();
        var values = ItemEnumerable();
        // result = source.ContainsAnyExcept(values); // Item<> not IEquatable
        result = source.ContainsAnyExcept(values, ItemEquality()); Assert.False(result);

        source = ItemArray([1, 2, 3]);
        values = ItemEnumerable([1, 2, 3]);
        result = source.ContainsAnyExcept(values, ItemEquality()); Assert.False(result);

        source = ItemArray([1, 2, 3]);
        values = ItemEnumerable([1, 2]);
        result = source.ContainsAnyExcept(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_StartsWithAnyExcept_Enum_Ints()
    {
        bool result;
        var source = IntArray();
        var values = IntEnumerable();
        result = source.StartsWithAnyExcept(values); Assert.False(result);
        result = source.StartsWithAnyExcept(values, IntEquality()); Assert.False(result);

        source = [1, 2, 3];
        values = IntEnumerable([1, 2, 3]);
        result = source.StartsWithAnyExcept(values); Assert.False(result);
        result = source.StartsWithAnyExcept(values, IntEquality()); Assert.False(result);

        source = [1, 2, 3];
        values = IntEnumerable([8, 2, 3]);
        result = source.StartsWithAnyExcept(values); Assert.True(result);
        result = source.StartsWithAnyExcept(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_StartsWithAnyExcept_Enum_Items()
    {
        bool result;
        var source = ItemArray();
        var values = ItemEnumerable();
        // result = source.StartsWithAnyExcept(values); // Item<> not IEquatable
        result = source.StartsWithAnyExcept(values, ItemEquality()); Assert.False(result);

        source = ItemArray([1, 2, 3]);
        values = ItemEnumerable([1, 2, 3]);
        result = source.StartsWithAnyExcept(values, ItemEquality()); Assert.False(result);

        source = ItemArray([1, 2, 3]);
        values = ItemEnumerable([8, 2, 3]);
        result = source.StartsWithAnyExcept(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_EndsWithAnyExcept_Enum_Ints()
    {
        bool result;
        var source = IntArray();
        var values = IntEnumerable();
        result = source.EndsWithAnyExcept(values); Assert.False(result);
        result = source.EndsWithAnyExcept(values, IntEquality()); Assert.False(result);

        source = [1, 2, 3];
        values = IntEnumerable([1, 2, 3]);
        result = source.EndsWithAnyExcept(values); Assert.False(result);
        result = source.EndsWithAnyExcept(values, IntEquality()); Assert.False(result);

        source = [1, 2, 3];
        values = IntEnumerable([1, 2]);
        result = source.EndsWithAnyExcept(values); Assert.True(result);
        result = source.EndsWithAnyExcept(values, IntEquality()); Assert.True(result);
    }

    [Fact]
    public static void Test_EndsWithAnyExcept_Enum_Items()
    {
        bool result;
        var source = ItemArray();
        var values = ItemEnumerable();
        // result = source.EndsWithAnyExcept(values); // Item<> not IEquatable
        result = source.EndsWithAnyExcept(values, ItemEquality()); Assert.False(result);

        source = ItemArray([1, 2, 3]);
        values = ItemEnumerable([1, 2, 3]);
        result = source.EndsWithAnyExcept(values, ItemEquality()); Assert.False(result);

        source = ItemArray([1, 2, 3]);
        values = ItemEnumerable([1, 2]);
        result = source.EndsWithAnyExcept(values, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    // TODO ArrayEx: Test_TrimStart_Enum
    // TODO ArrayEx: Test_TrimEnd_Enum
    // TODO ArrayEx: Test_Trim_Enum
    // TODO ArrayEx: Test_TrimStartAny_Enum
    // TODO ArrayEx: Test_TrimEndAny_Enum
    // TODO ArrayEx: Test_TrimAny_Enum
    // TODO ArrayEx: Test_AddRange_Enum
    // TODO ArrayEx: Test_InsertRange_Enum
    // TODO ArrayEx: Test_Remove_Enum
    // TODO ArrayEx: Test_RemoveLast_Enum
    // TODO ArrayEx: Test_RemoveAll_Enum
}
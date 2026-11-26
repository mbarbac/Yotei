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
    [Fact]
    public static void Test_SequenceEqual_Enum_Ints()
    {
        bool result;
        var source = Array.Empty<int>();
        var target = IntArray();
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
        var target = ItemArray();
        result = source.SequenceEqual(target); Assert.True(result);
        result = source.SequenceEqual(target, ItemEquality()); Assert.True(result);

        source = ItemArray([1, 2]);
        result = source.SequenceEqual(target); Assert.False(result);
        result = source.SequenceEqual(target, ItemEquality()); Assert.False(result);

        target = ItemArray([1, 2]);
        result = source.SequenceEqual(target); Assert.False(result); // elements are references
        result = source.SequenceEqual(target, ItemEquality()); Assert.True(result);
    }

    // ----------------------------------------------------

    // TODO ArrayEx: IndexOf_Range enum
    // TODO ArrayEx: LastIndexOf_Range enum
    // TODO ArrayEx: IndexesOf_Range enum
    // TODO ArrayEx: Contains_Range enum
    // TODO ArrayEx: StartsWith_Range enum
    // TODO ArrayEx: EndsWith_Range enum

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

    // TODO ArrayEx: IndexOfAnyExcept enum
    // TODO ArrayEx: LastIndexOfAnyExcept enum
    // TODO ArrayEx: IndexesOfAnyExcept enum
    // TODO ArrayEx: ContainsAnyExcept enum
    // TODO ArrayEx: StartsWithAnyExcept enum
    // TODO ArrayEx: EndsWithAnyExcept enum

    // ----------------------------------------------------

    // TODO ArrayEx: TrimStart enum
    // TODO ArrayEx: TrimEnd enum
    // TODO ArrayEx: Trim enum

    // ----------------------------------------------------

    // TODO ArrayEx: AddRange enum
    // TODO ArrayEx: InsertRange enum
    // TODO ArrayEx: Remove enum
    // TODO ArrayEx: RemoveLast enum
    // TODO ArrayEx: RemoveAll enum
}
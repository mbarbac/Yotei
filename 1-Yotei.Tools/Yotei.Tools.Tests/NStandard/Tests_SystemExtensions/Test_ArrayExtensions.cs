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

    [Fact]
    public static void Test_IndexOf_Value_Int()
    {
        int result;
        var source = Array.Empty<int>();
        result = source.IndexOf(9); Assert.Equal(-1, result);
        result = source.IndexOf(9, IntEquality()); Assert.Equal(-1, result);
        result = source.IndexOf(IntPredicate(9)); Assert.Equal(-1, result);

        source = [1, 9, 3, 4];
        result = source.IndexOf(9); Assert.Equal(1, result);
        result = source.IndexOf(9, IntEquality()); Assert.Equal(1, result);
        result = source.IndexOf(IntPredicate(9)); Assert.Equal(1, result);
    }

    [Fact]
    public static void Test_IndexOf_Value_Item()
    {
        int result;
        var source = Array.Empty<Item<int>>();
        // result = source.IndexOf(Item(9)); // Item<> not IEquatable
        result = source.IndexOf(Item(9), ItemEquality()); Assert.Equal(-1, result);
        result = source.IndexOf(ItemPredicate(9)); Assert.Equal(-1, result);

        source = ItemArray([1, 9, 3, 4]);
        result = source.IndexOf(Item(9), ItemEquality()); Assert.Equal(1, result);
        result = source.IndexOf(ItemPredicate(9)); Assert.Equal(1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_LastIndexOf_Value_Int()
    {
        int result;
        var source = Array.Empty<int>();
        result = source.LastIndexOf(9); Assert.Equal(-1, result);
        result = source.LastIndexOf(9, IntEquality()); Assert.Equal(-1, result);
        result = source.LastIndexOf(IntPredicate(9)); Assert.Equal(-1, result);

        source = [1, 2, 9, 4];
        result = source.LastIndexOf(9); Assert.Equal(2, result);
        result = source.LastIndexOf(9, IntEquality()); Assert.Equal(2, result);
        result = source.LastIndexOf(IntPredicate(9)); Assert.Equal(2, result);
    }

    [Fact]
    public static void Test_LastIndexOf_Value_Item()
    {
        int result;
        var source = Array.Empty<Item<int>>();
        // result = source.LastIndexOf(Item(9)); // Item<> not IEquatable
        result = source.LastIndexOf(Item(9), ItemEquality()); Assert.Equal(-1, result);
        result = source.LastIndexOf(ItemPredicate(9)); Assert.Equal(-1, result);

        source = ItemArray([1, 2, 9, 4]);
        result = source.LastIndexOf(Item(9), ItemEquality()); Assert.Equal(2, result);
        result = source.LastIndexOf(ItemPredicate(9)); Assert.Equal(2, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IndexesOf_Value_Int()
    {
        List<int> result;
        var source = Array.Empty<int>();
        result = source.IndexesOf(9); Assert.True(result.IsEmpty);
        result = source.IndexesOf(9, IntEquality()); Assert.True(result.IsEmpty);
        result = source.IndexesOf(IntPredicate(9)); Assert.True(result.IsEmpty);

        source = [1, 9, 9, 4];
        result = source.IndexesOf(9); Assert.True(result is [1, 2]);
        result = source.IndexesOf(9, IntEquality()); Assert.True(result is [1, 2]);
        result = source.IndexesOf(IntPredicate(9)); Assert.True(result is [1, 2]);
    }

    [Fact]
    public static void Test_IndexesOf_Value_Item()
    {
        List<int> result;
        var source = Array.Empty<Item<int>>();
        // result = source.IndexesOf(Item(9)); // Item<> not IEquatable
        result = source.IndexesOf(Item(9), ItemEquality()); Assert.True(result.IsEmpty);
        result = source.IndexesOf(ItemPredicate(9)); Assert.True(result.IsEmpty);

        source = ItemArray([1, 9, 9, 4]);
        result = source.IndexesOf(Item(9), ItemEquality()); Assert.True(result is [1, 2]);
        result = source.IndexesOf(ItemPredicate(9)); Assert.True(result is [1, 2]);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_Contains_Value_Int()
    {
        bool result;
        var source = Array.Empty<int>();
        result = source.Contains(9); Assert.False(result);
        result = source.Contains(9, IntEquality()); Assert.False(result);
        result = source.Contains(IntPredicate(9)); Assert.False(result);

        source = [1, 9, 3];
        result = source.Contains(9); Assert.True(result);
        result = source.Contains(9, IntEquality()); Assert.True(result);
        result = source.Contains(IntPredicate(9)); Assert.True(result);
    }

    [Fact]
    public static void Test_Contains_Value_Item()
    {
        bool result;
        var source = Array.Empty<Item<int>>();
        result = source.Contains(Item(9)); Assert.False(result); // Not needs IEquatable!
        result = source.Contains(Item(9), ItemEquality()); Assert.False(result);
        result = source.Contains(ItemPredicate(9)); Assert.False(result);

        source = ItemArray([1, 9, 3]);
        result = source.Contains(Item(9)); Assert.False(result);
        result = source.Contains(Item(9), ItemEquality()); Assert.True(result);
        result = source.Contains(ItemPredicate(9)); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_StartsWith_Value_Int()
    {
        bool result;
        var source = Array.Empty<int>();
        result = source.StartsWith(9); Assert.False(result);
        result = source.StartsWith(9, IntEquality()); Assert.False(result);
        result = source.StartsWith(IntPredicate(9)); Assert.False(result);

        source = [1, 9];
        result = source.StartsWith(9); Assert.False(result);
        result = source.StartsWith(9, IntEquality()); Assert.False(result);
        result = source.StartsWith(IntPredicate(9)); Assert.False(result);

        source = [9, 1];
        result = source.StartsWith(9); Assert.True(result);
        result = source.StartsWith(9, IntEquality()); Assert.True(result);
        result = source.StartsWith(IntPredicate(9)); Assert.True(result);
    }

    [Fact]
    public static void Test_StartsWith_Value_Item()
    {
        bool result;
        var source = Array.Empty<Item<int>>();
        // result = source.StartsWith(9); Assert.False(result); // Needs IEquatable!
        result = source.StartsWith(Item(9), ItemEquality()); Assert.False(result);
        result = source.StartsWith(ItemPredicate(9)); Assert.False(result);

        source = ItemArray([1, 9]);
        result = source.StartsWith(Item(9), ItemEquality()); Assert.False(result);
        result = source.StartsWith(ItemPredicate(9)); Assert.False(result);

        source = ItemArray([9, 1]);
        result = source.StartsWith(Item(9), ItemEquality()); Assert.True(result);
        result = source.StartsWith(ItemPredicate(9)); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_EndsWith_Value_Int()
    {
        bool result;
        var source = Array.Empty<int>();
        result = source.EndsWith(9); Assert.False(result);
        result = source.EndsWith(9, IntEquality()); Assert.False(result);
        result = source.EndsWith(IntPredicate(9)); Assert.False(result);

        source = [9, 1];
        result = source.EndsWith(9); Assert.False(result);
        result = source.EndsWith(9, IntEquality()); Assert.False(result);
        result = source.EndsWith(IntPredicate(9)); Assert.False(result);

        source = [1, 9];
        result = source.EndsWith(9); Assert.True(result);
        result = source.EndsWith(9, IntEquality()); Assert.True(result);
        result = source.EndsWith(IntPredicate(9)); Assert.True(result);
    }

    [Fact]
    public static void Test_EndsWith_Value_Item()
    {
        bool result;
        var source = Array.Empty<Item<int>>();
        // result = source.EndsWith(9); Assert.False(result); // Needs IEquatable!
        result = source.EndsWith(Item(9), ItemEquality()); Assert.False(result);
        result = source.EndsWith(ItemPredicate(9)); Assert.False(result);

        source = ItemArray([9, 1]);
        result = source.EndsWith(Item(9), ItemEquality()); Assert.False(result);
        result = source.EndsWith(ItemPredicate(9)); Assert.False(result);

        source = ItemArray([1, 9]);
        result = source.EndsWith(Item(9), ItemEquality()); Assert.True(result);
        result = source.EndsWith(ItemPredicate(9)); Assert.True(result);
    }

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
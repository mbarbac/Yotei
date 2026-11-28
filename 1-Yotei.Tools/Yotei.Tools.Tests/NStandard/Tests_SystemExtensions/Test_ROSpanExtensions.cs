#if NETSTANDARD2_0
namespace Yotei.Tools.NStd20.Tests;
#else
namespace Yotei.Tools.Tests;
#endif

// ========================================================
public static partial class Test_ROSpanExtensions
{
    [Fact]
    public static void Test_IsEmpty()
    {
        var source = IntSpan(); Assert.True(source.IsEmpty);
        source = [1, 2, 3]; Assert.False(source.IsEmpty);
    }

    [Fact]
    public static void Test_Clone()
    {
        var source = IntSpan();
        var result = source.Clone(); Assert.True(result.IsEmpty);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IndexOf_Value_Int()
    {
        int result;
        var source = IntSpan();
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
        var source = ItemSpan();
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
        var source = IntSpan();
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
        var source = ItemSpan();
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
        var source = IntSpan();
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
        var source = ItemSpan();
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
        var source = IntSpan();
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
        var source = ItemSpan();
        // result = source.Contains(Item(9)); // Item<> not IEquatable
        result = source.Contains(Item(9), ItemEquality()); Assert.False(result);
        result = source.Contains(ItemPredicate(9)); Assert.False(result);

        source = ItemArray([1, 9, 3]);
        result = source.Contains(Item(9), ItemEquality()); Assert.True(result);
        result = source.Contains(ItemPredicate(9)); Assert.True(result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_StartsWith_Value_Int()
    {
        bool result;
        var source = IntSpan();
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
        var source = ItemSpan();
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
        var source = IntSpan();
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
        var source = ItemSpan();
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

    /*

    

    // ----------------------------------------------------

    [Fact]
    public static void Test_TrimStart_Default_Int()
    {
        var source = IntSpan();
        var result = source.TrimStart(); Assert.True(result.IsEmpty);

        source = [1, 2];
        result = source.TrimStart(); Assert.True(result is [1, 2]);

        source = [0, 0, 1, 2];
        result = source.TrimStart(); Assert.True(result is [1, 2]);
    }

    [Fact]
    public static void Test_TrimStart_Default_Item()
    {
        var source = ItemSpan();
        var result = source.TrimStart(); Assert.True(result.IsEmpty);

        source = ItemArray([1, 2]);
        result = source.TrimStart(); result.SequenceEqual(ItemArray([1, 2]), ItemEquality());

        source = [null!, null!, Item(1), Item(2)];
        result = source.TrimStart(); result.SequenceEqual(ItemArray([1, 2]), ItemEquality());
    }

    [Fact]
    public static void Test_TrimStart_Value_Int()
    {
        var source = IntSpan([1, 2]);
        var result = source.TrimStart(9); Assert.True(result is [1, 2]);
        result = source.TrimStart(9, IntEquality()); Assert.True(result is [1, 2]);

        source = [1, 2];
        result = source.TrimStart(9); Assert.True(result is [1, 2]);
        result = source.TrimStart(9, IntEquality()); Assert.True(result is [1, 2]);

        source = [9, 9, 1, 2];
        result = source.TrimStart(9); Assert.True(result is [1, 2]);
        result = source.TrimStart(9, IntEquality()); Assert.True(result is [1, 2]);
    }

    [Fact]
    public static void Test_TrimStart_Value_Item()
    {
        Item<int>[] result;
        var source = ItemSpan([1, 2]);
        // result = source.TrimStart(Item(9)); // Item<> not IEquatable
        result = source.TrimStart(Item(9), ItemEquality());
        result.SequenceEqual(ItemArray([1, 2]), ItemEquality());

        source = ItemArray([1, 2]);
        result = source.TrimStart(Item(9), ItemEquality());
        result.SequenceEqual(ItemArray([1, 2]), ItemEquality());

        source = ItemArray([9, 9, 1, 2]);
        result = source.TrimStart(Item(9), ItemEquality());
        result.SequenceEqual(ItemArray([1, 2]), ItemEquality());
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_TrimEnd_Default_Int()
    {
        var source = IntSpan():
        var result = source.TrimEnd(); Assert.Empty(result);

        source = [1, 2];
        result = source.TrimEnd(); Assert.True(result is [1, 2]);

        source = [1, 2, 0, 0];
        result = source.TrimEnd(); Assert.True(result is [1, 2]);
    }

    [Fact]
    public static void Test_TrimEnd_Default_Item()
    {
        var source = ItemSpan();
        var result = source.TrimEnd(); Assert.Empty(result);

        source = ItemArray([1, 2]);
        result = source.TrimEnd(); result.SequenceEqual(ItemArray([1, 2]), ItemEquality());

        source = [Item(1), Item(2), null!, null!];
        result = source.TrimEnd(); result.SequenceEqual(ItemArray([1, 2]), ItemEquality());
    }

    [Fact]
    public static void Test_TrimEnd_Value_Int()
    {
        var source = IntArray([1, 2]);
        var result = source.TrimEnd(9); Assert.True(result is [1, 2]);
        result = source.TrimEnd(9, IntEquality()); Assert.True(result is [1, 2]);

        source = [1, 2];
        result = source.TrimEnd(9); Assert.True(result is [1, 2]);
        result = source.TrimEnd(9, IntEquality()); Assert.True(result is [1, 2]);

        source = [1, 2, 9, 9];
        result = source.TrimEnd(9); Assert.True(result is [1, 2]);
        result = source.TrimEnd(9, IntEquality()); Assert.True(result is [1, 2]);
    }

    [Fact]
    public static void Test_TrimEnd_Value_Item()
    {
        Item<int>[] result;
        var source = ItemArray([1, 2]);
        // result = source.TrimEnd(Item(9)); // Item<> not IEquatable
        result = source.TrimEnd(Item(9), ItemEquality());
        result.SequenceEqual(ItemArray([1, 2]), ItemEquality());

        source = ItemArray([1, 2]);
        result = source.TrimEnd(Item(9), ItemEquality());
        result.SequenceEqual(ItemArray([1, 2]), ItemEquality());

        source = ItemArray([1, 2, 9, 9]);
        result = source.TrimEnd(Item(9), ItemEquality());
        result.SequenceEqual(ItemArray([1, 2]), ItemEquality());
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_Trim_Default_Int()
    {
        var source = IntSpan():
        var result = source.Trim(); Assert.Empty(result);

        source = [1, 2];
        result = source.Trim(); Assert.True(result is [1, 2]);

        source = [0, 0, 1, 2, 0, 0];
        result = source.Trim(); Assert.True(result is [1, 2]);
    }

    [Fact]
    public static void Test_Trim_Default_Item()
    {
        var source = ItemSpan();
        var result = source.Trim(); Assert.Empty(result);

        source = ItemArray([1, 2]);
        result = source.Trim(); result.SequenceEqual(ItemArray([1, 2]), ItemEquality());

        source = [null!, null!, Item(1), Item(2), null!, null!];
        result = source.Trim(); result.SequenceEqual(ItemArray([1, 2]), ItemEquality());
    }

    [Fact]
    public static void Test_Trim_Value_Int()
    {
        var source = IntArray([1, 2]);
        var result = source.Trim(9); Assert.True(result is [1, 2]);
        result = source.Trim(9, IntEquality()); Assert.True(result is [1, 2]);

        source = [1, 2];
        result = source.Trim(9); Assert.True(result is [1, 2]);
        result = source.Trim(9, IntEquality()); Assert.True(result is [1, 2]);

        source = [9, 9, 1, 2, 9, 9];
        result = source.Trim(9); Assert.True(result is [1, 2]);
        result = source.Trim(9, IntEquality()); Assert.True(result is [1, 2]);
    }

    [Fact]
    public static void Test_Trim_Value_Item()
    {
        Item<int>[] result;
        var source = ItemArray([1, 2]);
        // result = source.Trim(Item(9)); // Item<> not IEquatable
        result = source.Trim(Item(9), ItemEquality());
        result.SequenceEqual(ItemArray([1, 2]), ItemEquality());

        source = ItemArray([1, 2]);
        result = source.Trim(Item(9), ItemEquality());
        result.SequenceEqual(ItemArray([1, 2]), ItemEquality());

        source = ItemArray([9, 9, 1, 2, 9, 9]);
        result = source.Trim(Item(9), ItemEquality());
        result.SequenceEqual(ItemArray([1, 2]), ItemEquality());
    }
     */

    // ----------------------------------------------------

    // TODO ROSpanEx: Test_Add_Value
    // TODO ROSpanEx: Test_Insert_Value
    // TODO ROSpanEx: Test_RemoveAt
    // TODO ROSpanEx: Test_RemoveRange
    // TODO ROSpanEx: Test_Remove_Value
    // TODO ROSpanEx: Test_RemoveLast_Value
    // TODO ROSpanEx: Test_RemoveAll_Value
    // TODO ROSpanEx: Test_Remove_Predicate
    // TODO ROSpanEx: Test_RemoveLast_Predicate
    // TODO ROSpanEx: Test_RemoveAll_Predicate
    // TODO ROSpanEx: Test_Clear
}
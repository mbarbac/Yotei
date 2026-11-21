namespace Yotei.Tools.Tests;

// ========================================================
public static partial class Test_ReadOnlySpanExtensions
{
    [Fact]
    public static void Test_OnSpan_SequenceCompareTo_Ints()
    {
        int result;
        var source = IntSpan();
        var target = IntSpan();
        result = source.SequenceCompareTo(target); Assert.Equal(0, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(0, result);

        source = IntSpan([1, 2]);
        target = IntSpan([1, 2]);
        result = source.SequenceCompareTo(target); Assert.Equal(0, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(0, result);

        source = IntSpan([1, 2, 3]);
        target = IntSpan([1, 2]);
        result = source.SequenceCompareTo(target); Assert.Equal(+1, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(+1, result);

        source = IntSpan([1]);
        target = IntSpan([1, 2]);
        result = source.SequenceCompareTo(target); Assert.Equal(-1, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(-1, result);

        source = IntSpan([1, 3]);
        target = IntSpan([1, 2]);
        result = source.SequenceCompareTo(target); Assert.Equal(+1, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(+1, result);

        source = IntSpan([1, 1]);
        target = IntSpan([1, 2]);
        result = source.SequenceCompareTo(target); Assert.Equal(-1, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(-1, result);
    }

    [Fact]
    public static void Test_OnSpan_SequenceCompareTo_Items()
    {
        int result;
        var source = ItemSpan();
        var target = ItemSpan();
        // source.SequenceCompareTo(target); // => CS0311: Item<T> is not IEquatable
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(0, result);

        source = ItemSpan([1, 2]);
        target = ItemSpan([1, 2]);
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(0, result);

        source = ItemSpan([1, 2, 3]);
        target = ItemSpan([1, 2]);
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(+1, result);

        source = ItemSpan([1]);
        target = ItemSpan([1, 2]);
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(-1, result);

        source = ItemSpan([1, 3]);
        target = ItemSpan([1, 2]);
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(+1, result);

        source = ItemSpan([1, 1]);
        target = ItemSpan([1, 2]);
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(-1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnSpan_SequenceEqual_Ints()
    {
        bool result;
        var source = IntSpan();
        var target = IntSpan();
        result = source.SequenceEqual(target); Assert.True(result);
        result = source.SequenceEqual(target, IntEquality()); Assert.True(result);

        source = IntSpan([1, 2]);
        target = IntSpan([1, 2]);
        result = source.SequenceEqual(target); Assert.True(result);
        result = source.SequenceEqual(target, IntEquality()); Assert.True(result);

        source = IntSpan([1]);
        target = IntSpan([1, 2]);
        result = source.SequenceEqual(target); Assert.False(result);
        result = source.SequenceEqual(target, IntEquality()); Assert.False(result);
    }

    [Fact]
    public static void Test_OnSpan_SequenceEqual_Items()
    {
        bool result;
        var source = ItemSpan();
        var target = ItemSpan();
        // source.SequenceEqual(target); // => CS0311: Item<T> is not IEquatable
        result = source.SequenceEqual(target, ItemEquality()); Assert.True(result);

        source = ItemSpan([1, 2]);
        target = ItemSpan([1, 2]);
        result = source.SequenceEqual(target, ItemEquality()); Assert.True(result);

        source = ItemSpan([1]);
        target = ItemSpan([1, 2]);
        result = source.SequenceEqual(target, ItemEquality()); Assert.False(result);
    }
}
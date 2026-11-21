namespace Yotei.Tools.Tests;

// ========================================================
public static partial class Test_ReadOnlySpanExtensions
{
    [Fact]
    public static void Test_OnEnum_SequenceCompareTo_Ints()
    {
        int result;
        var source = IntSpan();
        IEnumerable<int> target = [];
        result = source.SequenceCompareTo(target); Assert.Equal(0, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(0, result);

        source = [1, 2];
        target = [1, 2];
        result = source.SequenceCompareTo(target); Assert.Equal(0, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(0, result);

        source = [1, 2, 3];
        target = [1, 2];
        result = source.SequenceCompareTo(target); Assert.Equal(+1, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(+1, result);

        source = IntSpan([1]);
        target = [1, 2];
        result = source.SequenceCompareTo(target); Assert.Equal(-1, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(-1, result);

        source = IntSpan([1, 3]);
        target = [1, 2];
        result = source.SequenceCompareTo(target); Assert.Equal(+1, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(+1, result);

        source = IntSpan([1, 1]);
        target = [1, 2];
        result = source.SequenceCompareTo(target); Assert.Equal(-1, result);
        result = source.SequenceCompareTo(target, IntComparer()); Assert.Equal(-1, result);
    }

    [Fact]
    public static void Test_OnEnum_SequenceCompareTo_Items()
    {
        int result;
        var source = ItemSpan();
        IEnumerable<Item<int>> target = [];
        // source.SequenceCompareTo(target); // => CS0311: Item<T> is not IEquatable
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(0, result);

        source = ItemSpan([1, 2]);
        target = [new(1), new(2)];
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(0, result);

        source = ItemSpan([1, 2, 3]);
        target = [new(1), new(2)];
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(+1, result);

        source = ItemSpan([1]);
        target = [new(1), new(2)];
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(-1, result);

        source = ItemSpan([1, 3]);
        target = [new(1), new(2)];
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(+1, result);

        source = ItemSpan([1, 1]);
        target = [new(1), new(2)];
        result = source.SequenceCompareTo(target, ItemComparer()); Assert.Equal(-1, result);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_OnEnum_SequenceEquals_Ints()
    {
        bool result;
        var source = IntSpan();
        IEnumerable<int> target = [];
        result = source.SequenceEqual(target); Assert.True(result);
        result = source.SequenceEqual(target, IntEquality()); Assert.True(result);

        source = IntSpan([1, 2]);
        target = [1, 2];
        result = source.SequenceEqual(target); Assert.True(result);
        result = source.SequenceEqual(target, IntEquality()); Assert.True(result);

        source = IntSpan([1]);
        target = [1, 2];
        result = source.SequenceEqual(target); Assert.False(result);
        result = source.SequenceEqual(target, IntEquality()); Assert.False(result);
    }

    [Fact]
    public static void Test_OnEnum_SequenceEquals_Items()
    {
        bool result;
        var source = ItemSpan();
        IEnumerable<Item<int>> target = [];
        // source.SequenceEqual(target); // => CS0311: Item<T> is not IEquatable
        result = source.SequenceEqual(target, ItemEquality()); Assert.True(result);

        source = ItemSpan([1, 2]);
        target = [new(1), new(2)];
        result = source.SequenceEqual(target, ItemEquality()); Assert.True(result);

        source = ItemSpan([1]);
        target = [new(1), new(2)];
        result = source.SequenceEqual(target, ItemEquality()); Assert.False(result);
    }
}
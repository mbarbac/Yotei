namespace Yotei.Tools.Tests;

// ========================================================
public static class Test_ArrayExtensions
{
    [Fact]
    public static void Test_GetEnumerator_ValueType()
    {
        int[] source = [1, 2];
        var iter = source.GetTypedEnumerator();
        Assert.IsType<IEnumerator<int>>(iter, false);
    }

    [Fact]
    public static void Test_GetEnumerator_ValueType_Nullable()
    {
        int[]? source = [1, 2];
        var iter = source.GetTypedEnumerator();
        Assert.IsType<IEnumerator<int>>(iter, false);
    }

    [Fact]
    public static void Test_GetEnumerator_ValueType_NullableElement()
    {
        int?[] source = [null, 1, 2];
        var iter = source.GetTypedEnumerator();
        Assert.IsType<IEnumerator<int?>>(iter, false);
    }

    [Fact]
    public static void Test_GetEnumerator_ReferenceType()
    {
        string[] source = ["1", "2"];
        var iter = source.GetTypedEnumerator();
        Assert.IsType<IEnumerator<string>>(iter, false);
    }

    [Fact]
    public static void Test_GetEnumerator_ReferenceType_Nullable()
    {
        string[]? source = [null!, "1", "2"];
        var iter = source.GetTypedEnumerator();
        Assert.IsType<IEnumerator<string>>(iter, false);
    }

    [Fact]
    public static void Test_GetEnumerator_ReferenceType_NullableElement()
    {
        string?[] source = [null, "1", "2"];
        var iter = source.GetTypedEnumerator();
        Assert.IsType<IEnumerator<string?>>(iter, false);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_Clone_ValueType()
    {
        int[] source = [1, 2, 3];
        var target = (int[])source.Clone();
        Assert.NotSame(source, target);
        for (int i = 0; i < 3; i++) Assert.Equal(source[i], target[i]);

        target = source.Clone(false);
        Assert.NotSame(source, target);
        for (int i = 0; i < 3; i++) Assert.Equal(source[i], target[i]);

        target = source.Clone(true);
        Assert.NotSame(source, target);
        for (int i = 0; i < 3; i++) Assert.Equal(source[i], target[i]);
    }

    [Fact]
    public static void Test_Clone_ValueType_Nullable()
    {
        int?[] source = [null, 1, 2];
        var target = (int?[])source.Clone();
        Assert.NotSame(source, target);
        for (int i = 0; i < 3; i++) Assert.Equal(source[i], target[i]);

        target = source.Clone(false);
        Assert.NotSame(source, target);
        for (int i = 0; i < 3; i++) Assert.Equal(source[i], target[i]);

        target = source.Clone(true);
        Assert.NotSame(source, target);
        for (int i = 0; i < 3; i++) Assert.Equal(source[i], target[i]);
    }

    [Fact]
    public static void Test_Clone_ReferenceType()
    {
        Element<int>[] source = [new(1), new(2), new(3)];
        var target = (Element<int>[])source.Clone();
        Assert.NotSame(source, target);
        for (int i = 0; i < 3; i++) Assert.Same(source[i], target[i]);

        target = source.Clone(false)!;
        Assert.NotSame(source, target);
        for (int i = 0; i < 3; i++) Assert.Same(source[i], target[i]);

        target = source.Clone(true)!;
        Assert.NotSame(source, target);
        for (int i = 0; i < 3; i++) Assert.NotSame(source[i], target[i]);
    }

    [Fact]
    public static void Test_Clone_ReferenceType_Nullable()
    {
        Element<int>?[] source = [new(1), new(2), new(3)];
        var target = (Element<int>?[])source.Clone();
        Assert.NotSame(source, target);
        for (int i = 0; i < 3; i++) Assert.Same(source[i], target[i]);

        target = source.Clone(false);
        Assert.NotSame(source, target);
        for (int i = 0; i < 3; i++) Assert.Same(source[i], target[i]);

        target = source.Clone(true);
        Assert.NotSame(source, target);
        for (int i = 0; i < 3; i++) Assert.NotSame(source[i], target[i]);
    }

    [Fact]
    public static void Test_Clone_ReferenceType_NullableElement()
    {
        Element<int?>[] source = [new(1), new(2), new(3)];
        var target = (Element<int?>[])source.Clone();
        Assert.NotSame(source, target);
        for (int i = 0; i < 3; i++) Assert.Same(source[i], target[i]);

        target = source.Clone(false)!;
        Assert.NotSame(source, target);
        for (int i = 0; i < 3; i++) Assert.Same(source[i], target[i]);

        target = source.Clone(true)!;
        Assert.NotSame(source, target);
        for (int i = 0; i < 3; i++) Assert.NotSame(source[i], target[i]);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_IndexOf_Value()
    {
        var aa = new Element<string>("aa");
        var bb = new Element<string>("bb");
        var cc = new Element<string>("cc");
        Element<string>[] source = [aa, bb, cc, aa];

        var index = source.IndexOf(new Element<string>("aa"));
        Assert.Equal(-1, index);

        index = source.IndexOf(aa); Assert.Equal(0, index);
        index = source.LastIndexOf(aa); Assert.Equal(3, index);
        var indexes = source.IndexesOf(aa);
        Assert.Equal(2, indexes.Count);
        Assert.Equal(0, indexes[0]);
        Assert.Equal(3, indexes[1]);
    }

    [Fact]
    public static void Test_IndexOf_Value_Comparer()
    {
        var aa = new Element<string>("aa");
        var bb = new Element<string>("bb");
        var cc = new Element<string>("cc");
        Element<string>[] source = [aa, bb, cc, aa];

        var comparer = new ElementComparer<string>()
        { InnerComparer = StringComparer.OrdinalIgnoreCase };

        var index = source.IndexOf(new Element<string>("other"), comparer);
        Assert.Equal(-1, index);

        index = source.IndexOf(new Element<string>("aa"), comparer); Assert.Equal(0, index);
        index = source.LastIndexOf(new Element<string>("aa"), comparer); Assert.Equal(3, index);
        var indexes = source.IndexesOf(new Element<string>("aa"), comparer);
        Assert.Equal(2, indexes.Count);
        Assert.Equal(0, indexes[0]);
        Assert.Equal(3, indexes[1]);
    }

    // ----------------------------------------------------

    //[Fact]
    //public static void Test_()
    //{
    //}
}
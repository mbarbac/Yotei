namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_ArrayExtensions
{
    //[Enforced]
    [Fact]
    public static void Test_Duplicate()
    {
        var source = new[] { "1", "2", "3" };
        var target = source.Duplicate();
        Assert.NotSame(source, target);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);

        source = [];
        target = source.Duplicate();
        Assert.Empty(target);
    }

    //[Enforced]
    [Fact]
    public static void Test_TypedEnumerator()
    {
        var source = new[] { "1", "2", "3" };
        var iter = source.GetTypedEnumerator();
        Assert.True(iter.MoveNext()); Assert.Same(source[0], iter.Current);
        Assert.True(iter.MoveNext()); Assert.Same(source[1], iter.Current);
        Assert.True(iter.MoveNext()); Assert.Same(source[2], iter.Current);
        Assert.False(iter.MoveNext());

        source = [];
        iter = source.GetTypedEnumerator();
        Assert.False(iter.MoveNext());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var source = new[] { "1", "2", "3" };
        var target = source.GetRange(1, 2);
        Assert.Equal(2, target.Length);
        Assert.Same(source[1], target[0]);
        Assert.Same(source[2], target[1]);

        target = source.GetRange(1, 0);
        Assert.Empty(target);
    }

    //[Enforced]
    [Fact]
    public static void Test_ReplaceItem()
    {
        var source = new[] { "1", "2", "3" };
        var target = source.ReplaceItem(1, "4");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same("4", target[1]);
        Assert.Same(source[2], target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new[] { "1", "2", "3" };
        var target = source.Add("4");
        Assert.Equal(4, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
        Assert.Same("4", target[3]);

        source = [];
        target = source.Add("4");
        Assert.Single(target);
        Assert.Same("4", target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new[] { "1", "2", "3" };
        var target = source.AddRange(["4", "5"]);
        Assert.Equal(5, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
        Assert.Same("4", target[3]);
        Assert.Same("5", target[4]);

        target = source.AddRange(Array.Empty<string>());
        Assert.Equal(3, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);

        source = [];
        target = source.AddRange(["4", "5"]);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Length);
        Assert.Same("4", target[0]);
        Assert.Same("5", target[1]);

        source = [];
        target = source.AddRange(Array.Empty<string>());
        Assert.Empty(target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new[] { "1", "2", "3" };
        var target = source.Insert(0, "4");
        Assert.Equal(4, target.Length);
        Assert.Same("4", target[0]);
        Assert.Same(source[0], target[1]);
        Assert.Same(source[1], target[2]);
        Assert.Same(source[2], target[3]);

        source = [];
        target = source.Insert(0, "4");
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same("4", target[0]);

        try { target = source.Insert(1, "4"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { target = source.Insert(4, "4"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var source = new[] { "1", "2", "3" };
        var target = source.InsertRange(3, ["4", "5"]);
        Assert.Equal(5, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
        Assert.Same("4", target[3]);
        Assert.Same("5", target[4]);

        target = source.InsertRange(0, Array.Empty<string>());
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);

        source = [];
        target = source.InsertRange(0, ["4", "5"]);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Length);
        Assert.Same("4", target[0]);
        Assert.Same("5", target[1]);

        source = [];
        target = source.InsertRange(0, Array.Empty<string>());
        Assert.Empty(target);

        try { target = source.InsertRange(1, Array.Empty<string>()); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = new[] { "1", "2", "3" };
        var target = source.RemoveAt(1);
        Assert.Equal(2, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[2], target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var source = new[] { "1", "2", "3" };
        var target = source.RemoveRange(1, 2);
        Assert.Single(target);
        Assert.Same(source[0], target[0]);

        target = source.RemoveRange(0, 3);
        Assert.Empty(target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var source = new[] { "1", "2", "3", "2" };
        var target = source.Remove("2");
        Assert.Equal(3, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[2], target[1]);
        Assert.Same(source[3], target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast()
    {
        var source = new[] { "1", "2", "3", "2" };
        var target = source.RemoveLast("2");
        Assert.Equal(3, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll()
    {
        var source = new[] { "1", "2", "3", "2" };
        var target = source.RemoveAll("2");
        Assert.Equal(2, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[2], target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new[] { "1", "2", "3", "2" };
        var target = source.Remove(x => x == "2");
        Assert.Equal(3, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[2], target[1]);
        Assert.Same(source[3], target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveLast_Predicate()
    {
        var source = new[] { "1", "2", "3", "2" };
        var target = source.RemoveLast(x => x == "2");
        Assert.Equal(3, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAll_Predicate()
    {
        var source = new[] { "1", "2", "3", "2" };
        var target = source.RemoveAll(x => x == "2");
        Assert.Equal(2, target.Length);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[2], target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new[] { "1", "2", "3", "2" };
        var target = source.Clear();
        Assert.Empty(target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ResizeHead()
    {
        var source = new[] { "1", "2", "3" };
        var target = source.ResizeHead(2);
        Assert.Equal(2, target.Length);
        Assert.Equal(source[1], target[0]);
        Assert.Equal(source[2], target[1]);

        target = source.ResizeHead(5, "x");
        Assert.Equal(5, target.Length);
        Assert.Equal("x", target[0]);
        Assert.Equal("x", target[1]);
        Assert.Equal(source[0], target[2]);
        Assert.Equal(source[1], target[3]);
        Assert.Equal(source[2], target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_ResizeTail()
    {
        var source = new[] { "1", "2", "3" };
        var target = source.ResizeTail(2);
        Assert.Equal(2, target.Length);
        Assert.Equal(source[0], target[0]);
        Assert.Equal(source[1], target[1]);

        target = source.ResizeTail(5, "x");
        Assert.Equal(5, target.Length);
        Assert.Equal(source[0], target[0]);
        Assert.Equal(source[1], target[1]);
        Assert.Equal(source[2], target[2]);
        Assert.Equal("x", target[3]);
        Assert.Equal("x", target[4]);
    }
}
using THost = Yotei.ORM.Records.Code.KnownIdentifierTags;

namespace Yotei.ORM.Core.Tests;

// ========================================================
//[Enforced]
public static class Test_KnownIdentifierTags
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var items = new THost(false);
        Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var items = new THost(false, "one.two.three");
        Assert.Equal(3, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Equal("two", items[1]);
        Assert.Equal("three", items[2]);

        Assert.Equal(0, items.IndexOf("ONE"));
        Assert.Equal(1, items.IndexOf("TWO"));
        Assert.Equal(2, items.IndexOf("THREE"));

        items = new THost(true, "one.ONE");
        Assert.Equal(2, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Equal("ONE", items[1]);

        try { _ = new THost(false, "one.ONE"); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Methods()
    {
        var source = new THost(true, "one.ONE");
        var target = source.WithCaseSensitiveTags(true);
        Assert.Same(source, target);

        try { target = source.WithCaseSensitiveTags(false); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Get_Range()
    {
        var source = new THost(false, "one.two.three");
        var target = source.GetRange(0, 0);
        Assert.Same(source, target);

        target = source.GetRange(1, 2);
        Assert.Equal(2, target.Count);
        Assert.Equal("two", target[0]);
        Assert.Equal("three", target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Item()
    {
        var source = new THost(false, "one.two.three");
        var target = source.ReplaceItem(0, "ONE");
        Assert.Same(source, target);

        target = source.ReplaceItem(1, "x");
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("x", target[1]);
        Assert.Equal("three", target[2]);

        target = source.ReplaceItem(1, "x.y");
        Assert.Equal(4, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("x", target[1]);
        Assert.Equal("y", target[2]);
        Assert.Equal("three", target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new THost(false, "one.two.three");
        var target = source.Add("x");
        Assert.Equal(4, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
        Assert.Equal("x", target[3]);

        target = source.Add("x.y");
        Assert.Equal(5, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
        Assert.Equal("x", target[3]);
        Assert.Equal("y", target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new THost(false, "one.two.three");
        var target = source.AddRange(new[] { "x", "y" });
        Assert.Equal(5, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
        Assert.Equal("x", target[3]);
        Assert.Equal("y", target[4]);

        target = source.AddRange(new[] { "a.b", "c.d" });
        Assert.Equal(7, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
        Assert.Equal("a", target[3]);
        Assert.Equal("b", target[4]);
        Assert.Equal("c", target[5]);
        Assert.Equal("d", target[6]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new THost(false, "one.two.three");
        var target = source.Add("x");
        Assert.Equal(4, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
        Assert.Equal("x", target[3]);

        target = source.Add("x.y");
        Assert.Equal(5, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
        Assert.Equal("x", target[3]);
        Assert.Equal("y", target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var source = new THost(false, "one.two.three");
        var target = source.InsertRange(0, new[] { "x", "y" });
        Assert.Equal(5, target.Count);
        Assert.Equal("x", target[0]);
        Assert.Equal("y", target[1]);
        Assert.Equal("one", target[2]);
        Assert.Equal("two", target[3]);
        Assert.Equal("three", target[4]);        

        target = source.InsertRange(0, new[] { "a.b", "c.d" });
        Assert.Equal(7, target.Count);
        Assert.Equal("a", target[0]);
        Assert.Equal("b", target[1]);
        Assert.Equal("c", target[2]);
        Assert.Equal("d", target[3]);
        Assert.Equal("one", target[4]);
        Assert.Equal("two", target[5]);
        Assert.Equal("three", target[6]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_At()
    {
        var source = new THost(false, "one.two.three");
        var target = source.RemoveAt(1);
        Assert.Equal(2, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("three", target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var source = new THost(false, "one.two.three");
        var target = source.Remove("two");
        Assert.Equal(2, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("three", target[1]);

        target = source.Remove("three.one");
        Assert.Equal(1, target.Count);
        Assert.Equal("two", target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new THost(false, "one.two.three");
        var target = source.Clear();
        Assert.Empty(target);
    }
}
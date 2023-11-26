using THost = Yotei.ORM.Records.Code.IdentifierTags;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_IdentifierTags
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
    public static void Test_Create_Single()
    {
        var items = new THost(false, "one");
        Assert.Single(items);
        Assert.Same("one", items[0]);

        try { _ = new THost(false, (string?)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new THost(false, " "); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var items = new THost(false, ["one", "two", "three"]);
        Assert.Equal(3, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Equal("two", items[1]);
        Assert.Equal("three", items[2]);

        items = new THost(false, ["one", "two.three"]);
        Assert.Equal(3, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Equal("two", items[1]);
        Assert.Equal("three", items[2]);

        items = new THost(false, "one.two.three");
        Assert.Equal(3, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Equal("two", items[1]);
        Assert.Equal("three", items[2]);

        try { _ = new THost(false, "one."); Assert.Fail(); }
        catch (EmptyException) { }

        try { _ = new THost(false, "one..three"); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many_With_Duplicates()
    {
        var items = new THost(true, "one.ONE");
        Assert.Equal(2, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Equal("ONE", items[1]);

        try { _ = new THost(false, "one.ONE"); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Equality()
    {
        var source = new THost(false, "one.two.three");
        var target = source.Clone();
        Assert.True(source.Equals(target));

        target = new THost(false, "one.two.three");
        Assert.True(source.Equals(target));

        target = new THost(false, "one.TWO.three");
        Assert.True(source.Equals(target));

        target = new THost(false, "one.x.three");
        Assert.False(source.Equals(target));
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var items = new THost(false, "one.two.three");
        Assert.Equal(0, items.IndexOf("ONE"));
        Assert.Equal(1, items.IndexOf("TWO"));
        Assert.Equal(2, items.IndexOf("THREE"));

        Assert.Equal(0, items.IndexOf(x => x.Contains('e')));
        Assert.Equal(2, items.LastIndexOf(x => x.Contains('e')));

        var list = items.IndexesOf(x => x.Contains('e'));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(2, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new THost(false, "one.two.three");

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Methods()
    {
        var source = new THost(false, "one.two.three");

        var target = source.WithCaseSensitiveTags(false);
        Assert.Same(source, target);

        target = source.WithCaseSensitiveTags(true);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);

        source = new THost(true, ["one", "two", "ONE"]);
        try { source.WithCaseSensitiveTags(false); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var source = new THost(false, "one.two.three");

        var target = source.GetRange(0, 0);
        Assert.Empty(target);

        target = source.GetRange(0, source.Count);
        Assert.Same(source, target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("two", target[0]);
        Assert.Equal("three", target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new THost(false, "one.two.three");

        var target = source.Replace(0, "one");
        Assert.Same(source, target);

        target = source.Replace(0, "ONE");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("ONE", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);

        target = source.Replace(0, "x.y");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("x", target[0]);
        Assert.Equal("y", target[1]);
        Assert.Equal("two", target[2]);
        Assert.Equal("three", target[3]);

        try { _ = source.Replace(0, "THREE"); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Many()
    {
        var source = new THost(false, ["one", "two", "three"]);

        var target = source.Replace(0, "four.five");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("four", target[0]);
        Assert.Equal("five", target[1]);
        Assert.Equal("two", target[2]);
        Assert.Equal("three", target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new THost(false, "one.two.three");

        var target = source.Add("x");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
        Assert.Equal("x", target[3]);

        target = source.Add("x.y");
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
        Assert.Equal("x", target[3]);
        Assert.Equal("y", target[4]);

        try { _ = source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.Add("TWO"); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new THost(false, "one.two.three");

        var target = source.AddRange(["x", "y"]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
        Assert.Equal("x", target[3]);
        Assert.Equal("y", target[4]);

        target = source.AddRange(["x", "y.z"]);
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);
        Assert.Equal("three", target[2]);
        Assert.Equal("x", target[3]);
        Assert.Equal("y", target[4]);
        Assert.Equal("z", target[5]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new THost(false, "one.two.three");

        var target = source.Insert(0, "x");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("x", target[0]);
        Assert.Equal("one", target[1]);
        Assert.Equal("two", target[2]);
        Assert.Equal("three", target[3]);

        target = source.Insert(0, "x.y");
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("x", target[0]);
        Assert.Equal("y", target[1]);
        Assert.Equal("one", target[2]);
        Assert.Equal("two", target[3]);
        Assert.Equal("three", target[4]);

        try { _ = source.Insert(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.Insert(1, "TWO"); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var source = new THost(false, "one.two.three");

        var target = source.InsertRange(0, ["x", "y"]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("x", target[0]);
        Assert.Equal("y", target[1]);
        Assert.Equal("one", target[2]);
        Assert.Equal("two", target[3]);
        Assert.Equal("three", target[4]);

        target = source.InsertRange(0, ["x", "y.z"]);
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.Equal("x", target[0]);
        Assert.Equal("y", target[1]);
        Assert.Equal("z", target[2]);
        Assert.Equal("one", target[3]);
        Assert.Equal("two", target[4]);
        Assert.Equal("three", target[5]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = new THost(false, "one.two.three");

        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("two", target[0]);
        Assert.Equal("three", target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var source = new THost(false, "one.two.three");

        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, source.Count);
        Assert.Empty(target);

        target = source.RemoveRange(0, 2);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("three", target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var source = new THost(false, "one.two.three");

        var target = source.Remove("x");
        Assert.Same(source, target);

        target = source.Remove("ONE");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("two", target[0]);
        Assert.Equal("three", target[1]);

        try { source.Remove("x.y"); Assert.Fail(); } // No embedded dots!
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new THost(false, "one.two.three");

        var target = source.Remove(x => x == "x");
        Assert.Same(source, target);

        target = source.Remove(x => x.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("two", target[0]);
        Assert.Equal("three", target[1]);

        target = source.RemoveLast(x => x.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("one", target[0]);
        Assert.Equal("two", target[1]);

        target = source.RemoveAll(x => x.Contains('e'));
        Assert.NotSame(source, target);
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
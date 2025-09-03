using Chain = Yotei.ORM.Internals.StrTokenChain;
using Text = Yotei.ORM.Internals.StrTokenText;
using Literal = Yotei.ORM.Internals.StrTokenLiteral;

namespace Yotei.ORM.Tests.StrTokens;

// ========================================================
//[Enforced]
public static class Test_StrTokenChain
{
    static readonly Text xone = new("one");
    static readonly Text xtwo = new("two");
    static readonly Text xthree = new("three");
    static readonly Text xfour = new("four");
    static readonly Text xfive = new("five");

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var items = new Chain();
        Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range()
    {
        var items = new Chain([]);
        Assert.Empty(items);

        items = new Chain([xone, xtwo, xthree]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        try { _ = new Chain(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Chain([null!]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Duplicates()
    {
        var items = new Chain([xone, xone, new Text("ONE")]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xone, items[1]);
        Assert.Equal("ONE", items[2].Payload);

        items = new Chain([xone, xtwo, xone, new Text("one")]);
        Assert.Equal(4, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xone, items[2]);
        Assert.Equal("one", items[2].Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Extended()
    {
        var other = new Chain([xtwo, xthree]);
        var items = new Chain([xone, other]);

        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new Chain([xone, xtwo, xthree, xone]);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xone, target[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var items = new Chain([xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf(xfive));

        Assert.Equal(0, items.IndexOf(xone));
        Assert.Equal(-1, items.IndexOf(new Text("ONE")));
        Assert.Equal(0, items.IndexOf(new Text("one")));

        Assert.Equal(3, items.LastIndexOf(xone));
        Assert.Equal(-1, items.LastIndexOf(new Text("ONE")));
        Assert.Equal(3, items.LastIndexOf(new Text("one")));

        var list = items.IndexesOf(xone);
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);

        list = items.IndexesOf(new Text("ONE"));
        Assert.Empty(list);

        list = items.IndexesOf(new Text("one"));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var items = new Chain([xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf(x => ((Text)x).Payload.Contains('z')));

        Assert.Equal(0, items.IndexOf(x => ((Text)x).Payload.Contains('n')));
        Assert.Equal(3, items.LastIndexOf(x => ((Text)x).Payload.Contains('n')));

        var list = items.IndexesOf(x => ((Text)x).Payload.Contains('n'));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var source = new Chain([xone, xtwo, xthree, xfour]);
        var target = source.GetRange(0, 0);
        Assert.Empty(target);

        target = source.GetRange(1, 2);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        try { source.GetRange(-1, 0); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { source.GetRange(0, -1); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { source.GetRange(5, 0); Assert.Fail(); }
        catch (ArgumentException) { }

        try { source.GetRange(0, 5); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var target = source.Replace(1, xtwo);
        Assert.Same(source, target);

        target = source.Replace(1, xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xone, target[1]);
        Assert.Same(xthree, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Extended()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var other = new Chain([xfour, xfive]);

        var target = source.Replace(1, other);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xfour, target[1]);
        Assert.Same(xfive, target[2]);
        Assert.Same(xthree, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Extended_Empty()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var other = new Chain();

        var target = source.Replace(1, other);
        Assert.Same(source, target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new Chain([xone, xtwo]);
        var target = source.Add(xthree);

        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        try { _ = source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Duplicates()
    {
        var source = new Chain([xone, xtwo]);
        var target = source.Add(xone);

        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Extended()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var other = new Chain([]);

        var target = source.Add(other);
        Assert.Same(source, target);

        other = new Chain([xfour, xfive]);
        target = source.Add(other);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new Chain([xone, xtwo]);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange([xthree, xfour]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        try { _ = source.AddRange([xfive, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Extended()
    {
        var source = new Chain([xone, xtwo]);
        var other = new Chain([]);

        var target = source.AddRange([other]);
        Assert.Same(source, target);

        other = new Chain([xfour, xfive]);
        target = source.AddRange([xthree, other]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new Chain([xone, xtwo]);
        var target = source.Insert(2, xthree);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        try { _ = source.Insert(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Duplicates()
    {
        var source = new Chain([xone, xtwo]);
        var target = source.Insert(2, xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Extended()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var other = new Chain([]);

        var target = source.Insert(3, other);
        Assert.Same(source, target);

        other = new Chain([xfour, xfive]);
        target = source.Insert(3, other);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var source = new Chain([xone, xtwo]);
        var target = source.InsertRange(2, []);
        Assert.Same(source, target);

        target = source.InsertRange(2, [xthree, xfour]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        try { _ = source.InsertRange(0, [xfive, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Extended()
    {
        var source = new Chain([xone, xtwo]);
        var other = new Chain([]);

        var target = source.InsertRange(2, [other]);
        Assert.Same(source, target);

        other = new Chain([xfour, xfive]);
        target = source.InsertRange(2, [xthree, other]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = new Chain([xone, xtwo, xthree, xone]);

        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        try { source.RemoveAt(999); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var source = new Chain([xone, xtwo, xthree, xone]);
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 4);
        Assert.NotSame(source, target);
        Assert.Empty(target);

        target = source.RemoveRange(0, 1);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        try { _ = source.RemoveRange(0, -1); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { _ = source.RemoveRange(-1, 0); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { _ = source.RemoveRange(5, 0); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = source.RemoveRange(0, 5); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var source = new Chain([xone, xtwo, xthree, xone]);
        var target = source.Remove(xfour);
        Assert.Same(source, target);

        target = source.Remove(xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.Remove(new Text("ONE"));
        Assert.Same(source, target);

        target = source.RemoveLast(xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveLast(new Text("ONE"));
        Assert.Same(source, target);

        target = source.RemoveAll(xone);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveAll(new Text("ONE"));
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item_Extended()
    {
        var source = new Chain([xone, xtwo, xthree, xone]);
        var other = new Chain([xone, xthree]);

        var target = source.Remove(other);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xone, target[1]);

        target = source.RemoveLast(other);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        target = source.RemoveAll(other);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xtwo, target[0]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new Chain([xone, xtwo, xthree, xone]);
        var target = source.Remove(x => ((Text)x).Payload.Contains('z'));
        Assert.Same(source, target);

        target = source.Remove(x => ((Text)x).Payload.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveLast(x => ((Text)x).Payload.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll(x => ((Text)x).Payload.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new Chain();
        var target = source.Clear();
        Assert.Same(source, target);

        source = new Chain([xone, xtwo, xthree, xone]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Reduce_Text()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var source = new Chain([xone, xtwo, xthree]);
        var target = source.Reduce(comparison);

        Assert.NotSame(source, target);
        var text = Assert.IsType<Text>(target);
        Assert.Equal("onetwothree", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Reduce_Literal()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var source = new Chain([xone, new Literal("two"), xthree]);
        var target = source.Reduce(comparison);
        Assert.Same(source, target);

        source = new Chain([xone, xtwo, new Literal("three")]);
        target = source.Reduce(comparison);
        Assert.NotSame(source, target);
        var chain = Assert.IsType<Chain>(target);
        Assert.Equal(2, chain.Count);
        var text = Assert.IsType<Text>(chain[0]);
        Assert.Equal("onetwo", text.Payload);
        var literal = Assert.IsType<Literal>(chain[1]);
        Assert.Equal("three", literal.Payload);
    }
}
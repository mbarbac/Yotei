using Chain = Yotei.ORM.Internal.StrTokenChain;
using Text = Yotei.ORM.Internal.StrTokenText;
using Fixed = Yotei.ORM.Internal.StrTokenFixed;

namespace Yotei.ORM.Tests.Internal;

// ========================================================
//[Enforced]
public static class Test_StrTokenChain
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var chain = new Chain();
        Assert.Empty(chain);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single()
    {
        var xone = new Text("one");
        var chain = new Chain(xone);
        Assert.Single(chain);
        Assert.Same(xone, chain[0]);

        try { chain = new Chain((Text)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var xone = new Text("one");
        var xtwo = new Text("two");
        var xthree = new Text("three");
        var chain = new Chain([xone, xtwo, xthree]);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        chain = new Chain([xone, xone]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xone, chain[1]);

        try { chain = new Chain((IEnumerable<IStrToken>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { chain = new Chain([xone, null!]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Token_Reduce()
    {
        var xone = new Text("one");
        var xtwo = new Text("two");
        var xthree = new Text("three");
        var source = new Chain([xone, xtwo, xthree]);

        var target = source.Reduce(StringComparison.Ordinal);
        var text = Assert.IsType<Text>(target);
        Assert.Equal("onetwothree", text.Payload);

        var xfixed = new Fixed("xx");
        source = new Chain([xone, xfixed, xtwo]);
        target = source.Reduce(StringComparison.Ordinal);
        Assert.Same(source, target);

        source = new Chain();
        target = source.Reduce(StringComparison.Ordinal);
        text = Assert.IsType<Text>(target);
        Assert.Empty(text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var xone = new Text("one");
        var xtwo = new Text("two");
        var xthree = new Text("three");
        var xfour = new Text("four");

        var chain = new Chain([xone, xtwo, xthree]);
        Assert.True(chain.Contains(xone));
        Assert.False(chain.Contains(xfour));

        var list = chain.IndexesOf(x => ((string)x.Payload!).Contains('e'));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(2, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var xone = new Text("one");
        var xtwo = new Text("two");
        var xthree = new Text("three");
        var chain = new Chain([xone, xtwo, xthree]);

        var range = chain.ToList(0, 0);
        Assert.Empty(range);

        range = chain.ToList(1, 1);
        Assert.Single(range);
        Assert.Same(xtwo, range[0]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var xone = new Text("one");
        var xtwo = new Text("two");
        var xthree = new Text("three");
        var source = new Chain([xone, xtwo, xthree]);

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_FrozenList_Reduce()
    {
        var xone = new Text("one");
        var xtwo = new Text("two");
        var xthree = new Text("three");
        var source = new Chain([xone, xtwo, xthree]);

        var target = source.GetRange(0, 3);
        Assert.Same(source, target);

        target = source.GetRange(1, 0);
        Assert.Empty(target);

        target = source.GetRange(1, 2);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var xone = new Text("one");
        var xtwo = new Text("two");
        var xthree = new Text("three");
        var xfour = new Text("four");
        var xfive = new Text("five");

        var source = new Chain([xone, xtwo, xthree]);
        var target = source.Replace(0, xfour);
        Assert.Equal(3, target.Count);
        Assert.Same(xfour, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        var other = new Chain([xfour, xfive]);
        target = source.Replace(1, other);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xfour, target[1]);
        Assert.Same(xfive, target[2]);
        Assert.Same(xthree, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var xone = new Text("one");
        var xtwo = new Text("two");
        var xthree = new Text("three");
        var xfour = new Text("four");
        var xfive = new Text("five");

        var source = new Chain([xone, xtwo, xthree]);
        var target = source.Add(xfour);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        var other = new Chain([xfour, xfive]);
        target = source.Add(other);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);

        try { source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var xone = new Text("one");
        var xtwo = new Text("two");
        var xthree = new Text("three");
        var xfour = new Text("four");
        var xfive = new Text("five");

        var source = new Chain([xone, xtwo, xthree]);
        var target = source.AddRange([xfour, xfive]);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);

        try { source.AddRange(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.AddRange([xfour, null!]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var xone = new Text("one");
        var xtwo = new Text("two");
        var xthree = new Text("three");
        var xfour = new Text("four");
        var xfive = new Text("five");

        var source = new Chain([xone, xtwo, xthree]);
        var target = source.Insert(3, xfour);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        var other = new Chain([xfour, xfive]);
        target = source.Insert(3, other);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);

        try { source.Insert(3, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var xone = new Text("one");
        var xtwo = new Text("two");
        var xthree = new Text("three");
        var xfour = new Text("four");
        var xfive = new Text("five");

        var source = new Chain([xone, xtwo, xthree]);
        var target = source.InsertRange(3, [xfour, xfive]);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);

        try { source.InsertRange(3, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.InsertRange(3, [xfour, null!]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var xone = new Text("one");
        var xtwo = new Text("two");
        var xthree = new Text("three");
        var source = new Chain([xone, xtwo, xthree]);

        var target = source.RemoveAt(0);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var xone = new Text("one");
        var xtwo = new Text("two");
        var xthree = new Text("three");
        var source = new Chain([xone, xtwo, xthree]);

        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(2, 1);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var xone = new Text("one");
        var xtwo = new Text("two");
        var xthree = new Text("three");
        var source = new Chain([xone, xtwo, xthree, xone]);

        var target = source.Remove(new Text("one"));
        Assert.Same(source, target);

        target = source.Remove(xone);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        var other = new Chain([xone, xthree, new Text("four")]);
        target = source.Remove(other);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xone, target[1]);

        target = source.RemoveLast(other);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var xone = new Text("one");
        var xtwo = new Text("two");
        var xthree = new Text("three");
        var source = new Chain([xone, xtwo, xthree]);

        var target = source.Remove(x => ((Text)x).Payload.Contains('e'));
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveLast(x => ((Text)x).Payload.Contains('e'));
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        target = source.RemoveAll(x => ((Text)x).Payload.Contains('e'));
        Assert.Single(target);
        Assert.Same(xtwo, target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new Chain();
        var target = source.Clear();
        Assert.Same(source, target);

        var xone = new Text("one");
        var xtwo = new Text("two");
        var xthree = new Text("three");
        source = new Chain([xone, xtwo, xthree]);
        target = source.Clear();
        Assert.Empty(target);
    }
}
using Chain = Yotei.ORM.Internal.StrTokenChain;
using Text = Yotei.ORM.Internal.StrTokenText;
using Fixed = Yotei.ORM.Internal.StrTokenFixed;

namespace Yotei.ORM.Tests;

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
    public static void Test_Reduce()
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
}
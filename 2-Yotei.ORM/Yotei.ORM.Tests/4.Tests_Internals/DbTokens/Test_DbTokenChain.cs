using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

using Chain = Yotei.ORM.Internals.DbTokenChain;
using Literal = Yotei.ORM.Internals.DbTokenLiteral;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_DbTokenChain
{
    static readonly Literal xone = new("One");
    static readonly Literal xtwo = new("Two");
    static readonly Literal xthree = new("Three");
    static readonly Literal xfour = new("Four");
    static readonly Literal xfive = new("Five");

    // ----------------------------------------------------

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
        var chain = new Chain([xone]);
        Assert.Single(chain);
        Assert.Same(xone, chain[0]);

        try { _ = new Chain(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var chain = new Chain([]);
        Assert.Empty(chain);

        chain = new Chain([xone, xtwo]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);

        try { _ = new Chain([null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Chain([xone, null!]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Flatten()
    {
        var source = new Chain([xone, xtwo]);
        var chain = new Chain([source]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Duplicates()
    {
        var chain = new Chain([xone, xtwo, xone]);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xone, chain[2]);

        var other = new Chain([xone, xtwo]);
        chain = new Chain([xone, xtwo, other]);
        Assert.Equal(4, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xone, chain[2]);
        Assert.Same(xtwo, chain[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var target = source.Clone();
        Assert.Equal(3, target.Count);
        Assert.Equal(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Equal(xthree, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var chain = new Chain([xone, xtwo, xthree, xone]);

        Assert.Equal(-1, chain.IndexOf(xfive));

        Assert.Equal(0, chain.IndexOf(xone));
        Assert.Equal(0, chain.IndexOf(new Literal("One")));
        Assert.Equal(-1, chain.IndexOf(new Literal("ONE")));

        Assert.Equal(3, chain.LastIndexOf(xone));
        Assert.Equal(3, chain.LastIndexOf(new Literal("One")));
        Assert.Equal(-1, chain.LastIndexOf(new Literal("ONE")));

        var list = chain.IndexesOf(xone);
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);

        list = chain.IndexesOf(new Literal("One"));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);

        list = chain.IndexesOf(new Literal("ONE"));
        Assert.Empty(list);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var chain = new Chain([xone, xtwo, xthree, xone]);

        Assert.Equal(-1, chain.IndexOf(x => ((Literal)x).Value.Contains('z')));

        Assert.Equal(0, chain.IndexOf(x => ((Literal)x).Value.Contains('n')));
        Assert.Equal(3, chain.LastIndexOf(x => ((Literal)x).Value.Contains('n')));

        var list = chain.IndexesOf(x => ((Literal)x).Value.Contains('n'));
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

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var target = source.Replace(1, xtwo);
        Assert.Same(source, target);

        target = source.Replace(1, new Literal("Two"));
        Assert.Same(source, target);

        target = source.Replace(1, xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal(xone, target[0]);
        Assert.Equal(xone, target[1]);
        Assert.Equal(xthree, target[2]);
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
        Assert.Equal(xone, target[0]);
        Assert.Equal(xfour, target[1]);
        Assert.Equal(xfive, target[2]);
        Assert.Equal(xthree, target[3]);
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
        Assert.Equal(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Equal(xthree, target[2]);

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
        Assert.Equal(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Equal(xone, target[2]);
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
        Assert.Equal(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Equal(xthree, target[2]);
        Assert.Equal(xfour, target[3]);
        Assert.Equal(xfive, target[4]);
    }

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
        Assert.Equal(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Equal(xthree, target[2]);
        Assert.Equal(xfour, target[3]);

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
        Assert.Equal(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Equal(xthree, target[2]);
        Assert.Equal(xfour, target[3]);
        Assert.Equal(xfive, target[4]);
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
        Assert.Equal(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Equal(xthree, target[2]);

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
        Assert.Equal(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Equal(xone, target[2]);
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
        Assert.Equal(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Equal(xthree, target[2]);
        Assert.Equal(xfour, target[3]);
        Assert.Equal(xfive, target[4]);
    }

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
        Assert.Equal(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Equal(xthree, target[2]);
        Assert.Equal(xfour, target[3]);

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
        Assert.Equal(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Equal(xthree, target[2]);
        Assert.Equal(xfour, target[3]);
        Assert.Equal(xfive, target[4]);
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
        Assert.Equal(xtwo, target[0]);
        Assert.Equal(xthree, target[1]);
        Assert.Equal(xone, target[2]);

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
        Assert.Equal(xtwo, target[0]);
        Assert.Equal(xthree, target[1]);
        Assert.Equal(xone, target[2]);

        try { _ = source.RemoveRange(0, -1); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { _ = source.RemoveRange(-1, 0); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { _ = source.RemoveRange(5, 0); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = source.RemoveRange(0, 5); Assert.Fail(); }
        catch (ArgumentException) { }
    }

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
        Assert.Equal(xtwo, target[0]);
        Assert.Equal(xthree, target[1]);
        Assert.Equal(xone, target[2]);

        target = source.Remove(new Literal("One"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal(xtwo, target[0]);
        Assert.Equal(xthree, target[1]);
        Assert.Equal(xone, target[2]);

        target = source.Remove(new Literal("ONE"));
        Assert.Same(source, target);

        target = source.RemoveLast(xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Equal(xthree, target[2]);

        target = source.RemoveLast(new Literal("One"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Equal(xthree, target[2]);

        target = source.RemoveLast(new Literal("ONE"));
        Assert.Same(source, target);

        target = source.RemoveAll(xone);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal(xtwo, target[0]);
        Assert.Equal(xthree, target[1]);

        target = source.RemoveAll(new Literal("One"));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal(xtwo, target[0]);
        Assert.Equal(xthree, target[1]);

        target = source.RemoveAll(new Literal("ONE"));
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Extended()
    {
        var source = new Chain([xone, xtwo, xthree, xone]);
        var other = new Chain([xone, xthree]);

        var target = source.Remove(other);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal(xtwo, target[0]);
        Assert.Equal(xone, target[1]);

        target = source.RemoveLast(other);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal(xone, target[0]);
        Assert.Equal(xtwo, target[1]);

        target = source.RemoveAll(other);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal(xtwo, target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new Chain([xone, xtwo, xthree, xone]);
        var target = source.Remove(x => ((Literal)x).Value.Contains('z'));
        Assert.Same(source, target);

        target = source.Remove(x => ((Literal)x).Value.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal(xtwo, target[0]);
        Assert.Equal(xthree, target[1]);
        Assert.Equal(xone, target[2]);

        target = source.RemoveLast(x => ((Literal)x).Value.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Equal(xthree, target[2]);

        target = source.RemoveAll(x => ((Literal)x).Value.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal(xtwo, target[0]);
        Assert.Equal(xthree, target[1]);
    }

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
    public static void Test_Reduce()
    {
        var source = new Chain();
        var target = source.Reduce();
        Assert.NotSame(source, target);
        var text = Assert.IsType<Literal>(target); Assert.Empty(text.Value);

        source = new Chain([xone]);
        target = source.Reduce();
        Assert.NotSame(source, target);
        text = Assert.IsType<Literal>(target); Assert.Same(xone, text);

        source = new Chain([xone, xtwo, xthree, xone]);
        target = source.Reduce();
        Assert.Same(source, target);
    }
}
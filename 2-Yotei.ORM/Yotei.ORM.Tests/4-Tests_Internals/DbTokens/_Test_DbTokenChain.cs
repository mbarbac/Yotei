/*using Chain = Yotei.ORM.Internals.DbTokenChain;
using Literal = Yotei.ORM.Internals.DbTokenLiteral;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_DbTokenChain
{
    readonly static Literal xone = new("one");
    readonly static Literal xtwo = new("two");
    readonly static Literal xthree = new("three");
    readonly static Literal xfour = new("four");
    readonly static Literal xfive = new("five");

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
    public static void Test_Create_Range()
    {
        var chain = new Chain([xone, xtwo, xthree]);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        try { _ = new Chain([xone, null!]); Assert.Fail(); } catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range_With_Duplicates()
    {
        var chain = new Chain([xone, xone, new Literal("ONE")]);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xone, chain[1]);
        Assert.Same("ONE", ((DbTokenLiteral)chain[2]).Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Flatten()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var target = new Chain(source);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        var other = new Chain([source]);
        Assert.Equal(3, other.Count);
        Assert.Same(xone, other[0]);
        Assert.Same(xtwo, other[1]);
        Assert.Same(xthree, other[2]);
    }

    // ----------------------------------------------------

    // This test is inherited from the core collection's capability, although is not intended
    // for production usage.
    //[Enforced]
    [Fact]
    [SuppressMessage("", "IDE0028")]
    public static void Test_Clone()
    {
        var source = new Chain();
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Empty(target);

        source = new Chain([xone, xtwo, xthree]);
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Value()
    {
        var source = new Chain([xone, xtwo, xthree, xone]);

        var index = source.IndexOf(xfour); Assert.Equal(-1, index);

        index = source.IndexOf(xone); Assert.Equal(0, index);
        index = source.LastIndexOf(xone); Assert.Equal(3, index);

        var nums = source.IndexesOf(xone);
        Assert.Equal(2, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(3, nums[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Predicate()
    {
        var source = new Chain([xone, xtwo, xone, xthree]);

        var index = source.IndexOf(x => x is null); Assert.Equal(-1, index);

        index = source.IndexOf(x => x is Literal Literal && Literal.Value.Contains('n'));
        Assert.Equal(0, index);

        index = source.LastIndexOf(x => x is Literal Literal && Literal.Value.Contains('n'));
        Assert.Equal(2, index);

        var nums = source.IndexesOf(x => x is Literal Literal && Literal.Value.Contains('e'));
        Assert.Equal(3, nums.Count);
        Assert.Equal(0, nums[0]);
        Assert.Equal(2, nums[1]);
        Assert.Equal(3, nums[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        IDbToken item;
        List<IDbToken> range;
        var source = new Chain([xone, xtwo, xone, xthree]);

        Assert.False(source.TryFind(x => x is null, out item));
        Assert.Null(item);

        Assert.True(source.TryFind(x => x is Literal Literal && Literal.Value.Contains('e'), out item));
        Assert.NotNull(item);
        Assert.Same(xone, item);

        Assert.True(source.TryFindLast(x => x is Literal Literal && Literal.Value.Contains('e'), out item));
        Assert.NotNull(item);
        Assert.Same(xthree, item);

        Assert.True(source.TryFindAll(x => x is Literal Literal && Literal.Value.Contains('e'), out range));
        Assert.Equal(3, range.Count);
        Assert.Same(xone, range[0]);
        Assert.Same(xone, range[1]);
        Assert.Same(xthree, range[2]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var target = source.Replace(0, xone);
        Assert.Same(source, target);

        target = source.Replace(0, new Literal("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("ONE", ((Literal)target[0]).Value);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        var xother = new Literal("other");
        try { source.Replace(-1, xother); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.Replace(3, xother); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Same()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var target = source.Replace(0, xone);
        Assert.Same(source, target);

        target = source.Replace(1, xtwo);
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Empty_Nested()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var target = source.Replace(0, new Chain());
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Range_Nested()
    {
        var xalpha = new Literal("alpha");
        var xbeta = new Literal("beta");
        var source = new Chain([xone, xtwo, xthree]);

        var target = source.Replace(0, new Chain([xalpha, xbeta]));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xalpha, target[0]);
        Assert.Same(xbeta, target[1]);
        Assert.Same(xtwo, target[2]);
        Assert.Same(xthree, target[3]);

        target = source.Replace(1, new Chain([xalpha, xbeta]));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xalpha, target[1]);
        Assert.Same(xbeta, target[2]);
        Assert.Same(xthree, target[3]);

        target = source.Replace(2, new Chain([xalpha, xbeta]));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xalpha, target[2]);
        Assert.Same(xbeta, target[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new Chain([xone]);
        var target = source.Add(xtwo);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        try { source.Add(null!); Assert.Fail(); } catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Duplicated()
    {
        var source = new Chain([xone]);
        var target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xone, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Nested()
    {
        var source = new Chain([xone, xtwo]);
        var target = source.Add(new Chain([xthree, xfour]));

        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
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
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Nested()
    {
        var source = new Chain([xone, xtwo]);
        var target = source.AddRange([xthree, new Chain([xfour, xfive])]);

        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new Chain();
        var target = source.Insert(0, xone);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xone, target[0]);

        source = target;
        target = source.Insert(1, xtwo);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        source = target;
        target = source.Insert(0, xthree);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xthree, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xtwo, target[2]);

        try { source.Insert(0, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Insert(-1, xfive); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.Insert(4, xfive); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Duplicated()
    {
        var source = new Chain([xone]);
        var target = source.Insert(1, xone);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xone, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Nested()
    {
        var source = new Chain([xone, xtwo]);
        var target = source.Insert(2, new Chain([xthree, xfour]));

        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var source = new Chain([xone, xtwo]);
        var target = source.InsertRange(0, []);
        Assert.Same(source, target);

        target = source.InsertRange(2, [xthree, xfour]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Nested()
    {
        var source = new Chain([xone, xtwo]);
        var target = source.InsertRange(1, [xthree, new Chain([xfour, xfive])]);

        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xfour, target[2]);
        Assert.Same(xfive, target[3]);
        Assert.Same(xtwo, target[4]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var target = source.RemoveAt(0);

        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        try { source.RemoveAt(-1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveAt(3); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange_Empty()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var source = new Chain([xone, xtwo, xthree]);
        var target = source.RemoveRange(0, 1);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xone, target[0]);

        try { source.RemoveRange(-1, 0); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(3, 1); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(0, 4); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(1, 3); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.RemoveRange(2, 2); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Value()
    {
        var source = new Chain([xone, xtwo, xone, xthree]);

        var target = source.Remove(xfour);
        Assert.Same(source, target);

        target = source.Remove(xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xthree, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Value_Nested()
    {
        var source = new Chain([xone, xtwo, xone, xthree]);

        var target = source.Remove(new Chain([xtwo, xone]));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveAll(new Chain([xtwo, xone]));
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xthree, target[0]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new Chain([xone, xtwo, xone, xthree]);

        var target = source.Remove(x => x is Literal Literal && Literal.Value is null);
        Assert.Same(source, target);

        target = source.Remove(x => x is Literal Literal && Literal.Value.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveLast(x => x is Literal Literal && Literal.Value.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll(x => x is Literal Literal && Literal.Value.Contains('n'));
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

        source = new Chain([xone, xtwo, xthree]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}*/
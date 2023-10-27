using THost = Yotei.Collections.ElementChain;
using TItem = Yotei.Collections.NameElement;

namespace Experimental.Tests;

// ========================================================
//[Enforced]
public static class Test_THost
{
    static readonly TItem xone = new("one");
    static readonly TItem xtwo = new("two");
    static readonly TItem xthree = new("three");
    static readonly TItem xfour = new("four");
    static readonly TItem xfive = new("five");
    static readonly TItem xsix = new("six");

    // ----------------------------------------------------

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
        var items = new THost(false, xone);
        Assert.Single(items);
        Assert.Same(xone, items[0]);

        try { _ = new THost(false, (TItem?)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many()
    {
        var items = new THost(false, [xone, xtwo, xthree]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        try { _ = new THost(false, (IEnumerable<IElement>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new THost(false, [xone, new TItem("ONE")]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many_Same_Duplicated()
    {
        var items = new THost(false, [xone, xtwo, xone]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xone, items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Many_CaseSensitive()
    {
        var items = new THost(true, [xone, xtwo, new TItem("ONE")]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Equal("ONE", ((TItem)items[2]).Name);

        items = new THost(true, [xone, xtwo, xone]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xone, items[2]);

        try { _ = new THost(true, [xone, xtwo, new TItem("one")]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Key()
    {
        var items = new THost(false, [xone, xtwo, xone]);
        Assert.Equal(0, items.IndexOf("ONE"));
        Assert.Equal(1, items.IndexOf("TWO"));
        Assert.Equal(-1, items.IndexOf("x"));

        Assert.Equal(2, items.LastIndexOf("ONE"));

        var list = items.IndexesOf("ONE");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(2, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var items = new THost(false, [xone, xtwo, xone]);

        var list = items.IndexesOf(x => x is TItem named && named.Name.Contains('e'));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(2, list[1]);

        list = items.IndexesOf(x => x is TItem named && named.Name == "x");
        Assert.Empty(list);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new THost(false, [xone, xtwo, xone]);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone_Sensitive()
    {
        var source = new THost(true, [xone, xtwo, new TItem("ONE")]);
        var target = source.Clone();

        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);
        Assert.Same(source[2], target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Change_Behaviors()
    {
        var source = new THost(false, [xone, xtwo, xone]);
        var target = source.WithCaseSensitive(false);
        Assert.Same(source, target);

        target = source.WithCaseSensitive(true);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);

        source = new THost(true, [xone, xtwo, new TItem("ONE")]);
        try { source.WithCaseSensitive(false); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var source = new THost(false, new[] { xone, xtwo, xthree });
        var target = source.GetRange(1, 0);
        Assert.Empty(target);

        target = source.GetRange(0, source.Count);
        Assert.Same(source, target);

        target = source.GetRange(1, 2);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var source = new THost(false, new[] { xone, xtwo, xthree });
        var target = source.Replace(0, xone);
        Assert.Same(source, target);

        target = source.Replace(0, new TItem("one"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.NotSame(xone, target[0]);
        Assert.Equal("one", ((TItem)target[0]).Name);

        try { _ = source.Replace(2, new TItem("TWO")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Many()
    {
        var source = new THost(false, [xone, xtwo, xthree]);
        var target = source.Replace(0, new THost(false, [xfour, xfive]));

        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xfour, target[0]);
        Assert.Same(xfive, target[1]);
        Assert.Same(xtwo, target[2]);
        Assert.Same(xthree, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var source = new THost(false, [xone, xtwo, xthree]);
        var target = source.Add(xfour);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        try { source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Add(new TItem("TWO")); Assert.Fail(); }
        catch (DuplicateException) { }

        target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xone, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Many()
    {
        var source = new THost(false, [xone, xtwo, xthree]);
        var target = source.Add(new THost(false));
        Assert.Same(source, target);

        target = source.Add(new THost(false, [xfour, xone]));
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xone, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var source = new THost(false, [xone, xtwo, xthree]);
        var target = source.AddRange([xfour, xone]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xone, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Many()
    {
        var source = new THost(false, [xone, xtwo, xthree]);
        var target = source.AddRange([xfour, new THost(false, [xfive, xone])]);
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);
        Assert.Same(xfive, target[4]);
        Assert.Same(xone, target[5]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var source = new THost(false, [xone, xtwo, xthree]);
        var target = source.Insert(0, xfour);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xfour, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xtwo, target[2]);
        Assert.Same(xthree, target[3]);

        try { source.Insert(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { source.Insert(0, new TItem("TWO")); Assert.Fail(); }
        catch (DuplicateException) { }

        source = new THost(false, [xone, xtwo, xthree]);
        target = source.Insert(3, xone);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xone, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Many()
    {
        var source = new THost(false, [xone, xtwo, xthree]);
        var target = source.Insert(0, new THost(false));
        Assert.Same(source, target);

        target = source.Insert(0, new THost(false, [xfour, xfive]));
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xfour, target[0]);
        Assert.Same(xfive, target[1]);
        Assert.Same(xone, target[2]);
        Assert.Same(xtwo, target[3]);
        Assert.Same(xthree, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var source = new THost(false, [xone, xtwo, xthree]);
        var target = source.InsertRange(0, new[] { xfour, xfive });
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xfour, target[0]);
        Assert.Same(xfive, target[1]);
        Assert.Same(xone, target[2]);
        Assert.Same(xtwo, target[3]);
        Assert.Same(xthree, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Many()
    {
        var source = new THost(false, [xone, xtwo, xthree]);
        var target = source.InsertRange(0, [xfour, new THost(false, [xfive, xsix])]);
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.Same(xfour, target[0]);
        Assert.Same(xfive, target[1]);
        Assert.Same(xsix, target[2]);
        Assert.Same(xone, target[3]);
        Assert.Same(xtwo, target[4]);
        Assert.Same(xthree, target[5]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        var source = new THost(false, new[] { xone, xtwo, xthree });
        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var source = new THost(false, new[] { xone, xtwo, xthree });
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 2);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xthree, target[0]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        var source = new THost(false, new[] { xone, xtwo, xthree, xone });
        var target = source.Remove("any");
        Assert.Same(source, target);

        target = source.Remove("ONE");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveLast("ONE");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll("ONE");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new THost(false, new[] { xone, xtwo, xthree, xfour });
        var target = source.Remove(x => x is not IElement);
        Assert.Same(source, target);

        target = source.Remove(x => x is TItem named && named.Name.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xfour, target[2]);

        target = source.RemoveLast(x => x is TItem named && named.Name.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xfour, target[2]);

        target = source.RemoveAll(x => x is TItem named && named.Name.Contains('e'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xfour, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var source = new THost(false, new[] { xone, xtwo, xthree });
        var target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}
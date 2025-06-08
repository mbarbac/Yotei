using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

using TItem = Yotei.ORM.Tests.Generators.Element;
using Chain = Yotei.ORM.Tests.Generators.ElementList_KT;

namespace Yotei.ORM.Tests.Generators;

// ========================================================
//[Enforced]
public static class Test_ElementList_KT
{
    static readonly TItem xone = new("one");
    static readonly TItem xtwo = new("two");
    static readonly TItem xthree = new("three");
    static readonly TItem xfour = new("four");
    static readonly TItem xfive = new("five");

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

        try { _ = new Chain([new TItem("")]); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Duplicates()
    {
        var items = new Chain([xone, xone]);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xone, items[1]);

        items = new Chain([xone, xtwo, xone]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xone, items[2]);

        try { _ = new Chain([xone, new TItem("ONE")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = new Chain(StringComparison.Ordinal, [xone, new TItem("one")]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    //[Fact]
    //public static void Test_Create_Extended()
    //{
    //    var other = new Chain([xtwo, xthree]);
    //    var items = new Chain([xone, other]);

    //    Assert.Equal(3, items.Count);
    //    Assert.Same(xone, items[0]);
    //    Assert.Same(xtwo, items[1]);
    //    Assert.Same(xthree, items[2]);
    //}

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var items = new Chain([xone, xtwo, xthree, xone]);
        var target = items.Clone();

        Assert.NotSame(items, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xone, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        var items = new Chain([xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf("five"));

        Assert.Equal(0, items.IndexOf("one"));
        Assert.Equal(0, items.IndexOf("ONE"));

        Assert.Equal(3, items.LastIndexOf("one"));
        Assert.Equal(3, items.LastIndexOf("ONE"));

        var list = items.IndexesOf("one");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);

        list = items.IndexesOf("ONE");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        var items = new Chain([xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf(x => ((TItem)x).Name.Contains('z')));

        Assert.Equal(0, items.IndexOf(x => ((TItem)x).Name.Contains('n')));
        Assert.Equal(3, items.LastIndexOf(x => ((TItem)x).Name.Contains('n')));

        var list = items.IndexesOf(x => ((TItem)x).Name.Contains('n'));
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

        target = source.Replace(1, xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xone, target[1]);
        Assert.Same(xthree, target[2]);

        try { source.Replace(1, new TItem("one")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    //[Fact]
    //public static void Test_Replace_Extended()
    //{
    //    var source = new Chain([xone, xtwo, xthree]);
    //    var other = new Chain([xfour, xfive]);

    //    var target = source.Replace(1, other);
    //    Assert.NotSame(source, target);
    //    Assert.Equal(4, target.Count);
    //    Assert.Same(xone, target[0]);
    //    Assert.Equal(xfour, target[1]);
    //    Assert.Same(xfive, target[2]);
    //    Assert.Same(xthree, target[3]);
    //}

    //[Enforced]
    //[Fact]
    //public static void Test_Replace_Extended_Empty()
    //{
    //    var source = new Chain([xone, xtwo, xthree]);
    //    var other = new Chain(StringComparison.Ordinal);

    //    var target = source.Replace(1, other);
    //    Assert.Same(source, target);
    //}

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

        try { _ = source.Add(new TItem("")); Assert.Fail(); }
        catch (EmptyException) { }
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

        source = new Chain([xone, xtwo]);
        try { _ = source.Add(new TItem("one")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    //[Fact]
    //public static void Test_Add_Extended()
    //{
    //    var source = new Chain([xone, xtwo, xthree]);
    //    var other = new Chain([]);

    //    var target = source.Add(other);
    //    Assert.Same(source, target);

    //    other = new Chain([xfour, xfive]);
    //    target = source.Add(other);
    //    Assert.NotSame(source, target);
    //    Assert.Equal(5, target.Count);
    //    Assert.Same(xone, target[0]);
    //    Assert.Equal(xtwo, target[1]);
    //    Assert.Same(xthree, target[2]);
    //    Assert.Same(xfour, target[3]);
    //    Assert.Same(xfive, target[4]);
    //}

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

        try { _ = source.AddRange([new TItem("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.AddRange([xfive, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.AddRange([xfive, new TItem("")]); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    //[Fact]
    //public static void Test_AddRange_Extended()
    //{
    //    var source = new Chain([xone, xtwo]);
    //    var other = new Chain([]);

    //    var target = source.AddRange([other]);
    //    Assert.Same(source, target);

    //    other = new Chain([xfour, xfive]);
    //    target = source.AddRange([xthree, other]);
    //    Assert.NotSame(source, target);
    //    Assert.Equal(5, target.Count);
    //    Assert.Same(xone, target[0]);
    //    Assert.Equal(xtwo, target[1]);
    //    Assert.Same(xthree, target[2]);
    //    Assert.Same(xfour, target[3]);
    //    Assert.Same(xfive, target[4]);
    //}

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

        try { _ = source.Insert(0, new TItem("")); Assert.Fail(); }
        catch (EmptyException) { }
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

        source = new Chain([xone, xtwo]);
        try { _ = source.Insert(0, new TItem("one")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    //[Fact]
    //public static void Test_Insert_Extended()
    //{
    //    var source = new Chain([xone, xtwo, xthree]);
    //    var other = new Chain([]);

    //    var target = source.Insert(3, other);
    //    Assert.Same(source, target);

    //    other = new Chain([xfour, xfive]);
    //    target = source.Insert(3, other);
    //    Assert.NotSame(source, target);
    //    Assert.Equal(5, target.Count);
    //    Assert.Same(xone, target[0]);
    //    Assert.Equal(xtwo, target[1]);
    //    Assert.Same(xthree, target[2]);
    //    Assert.Same(xfour, target[3]);
    //    Assert.Same(xfive, target[4]);
    //}

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

        try { _ = source.InsertRange(0, [new TItem("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.InsertRange(0, [xfive, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.InsertRange(0, [xfive, new TItem("")]); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    //[Fact]
    //public static void Test_InsertRange_Extended()
    //{
    //    var source = new Chain([xone, xtwo]);
    //    var other = new Chain([]);

    //    var target = source.InsertRange(2, [other]);
    //    Assert.Same(source, target);

    //    other = new Chain([xfour, xfive]);
    //    target = source.InsertRange(2, [xthree, other]);
    //    Assert.NotSame(source, target);
    //    Assert.Equal(5, target.Count);
    //    Assert.Same(xone, target[0]);
    //    Assert.Equal(xtwo, target[1]);
    //    Assert.Same(xthree, target[2]);
    //    Assert.Same(xfour, target[3]);
    //    Assert.Same(xfive, target[4]);
    //}

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

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        var source = new Chain([xone, xtwo, xthree, xone]);
        var target = source.Remove("four");
        Assert.Same(source, target);

        target = source.Remove("one");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.Remove("ONE");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveLast("one");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveLast("ONE");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll("one");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveAll("ONE");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    //[Fact]
    //public static void Test_Remove_Extended()
    //{
    //    var source = new Chain([xone, xtwo, xthree, xone]);
    //    var other = new Chain([xone, xthree]);

    //    var target = source.Remove(other);
    //    Assert.NotSame(source, target);
    //    Assert.Equal(2, target.Count);
    //    Assert.Same(xtwo, target[0]);
    //    Assert.Same(xone, target[1]);

    //    target = source.RemoveLast(other);
    //    Assert.NotSame(source, target);
    //    Assert.Equal(2, target.Count);
    //    Assert.Same(xone, target[0]);
    //    Assert.Same(xtwo, target[1]);

    //    target = source.RemoveAll(other);
    //    Assert.NotSame(source, target);
    //    Assert.Single(target);
    //    Assert.Same(xtwo, target[0]);
    //}

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var source = new Chain([xone, xtwo, xthree, xone]);
        var target = source.Remove(x => ((TItem)x).Name.Contains('z'));
        Assert.Same(source, target);

        target = source.Remove(x => ((TItem)x).Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveLast(x => ((TItem)x).Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll(x => ((TItem)x).Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
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
}
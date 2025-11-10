using Yotei.ORM.Code;
using Chain = Yotei.ORM.Generators.Invariant.Tests.ElementList_T;
using Element = Yotei.ORM.Generators.Invariant.Tests.NamedElement;

namespace Yotei.ORM.Generators.Invariant.Tests;

// ========================================================
////[Enforced]
public class Test_ElementList_T
{
    static readonly Element xone = new("one");
    static readonly Element xtwo = new("two");
    static readonly Element xthree = new("three");
    static readonly Element xfour = new("four");
    static readonly Element xfive = new("five");

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public void Test_Create_Empty()
    {
        IEngine engine = new FakeEngine();
        var items = new Chain(engine); Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public void Test_Create_Range()
    {
        IEngine engine = new FakeEngine();
        var items = new Chain(engine, []);
        Assert.Empty(items);

        items = new Chain(engine.WithCaseSensitiveNames(true), [xone, xtwo, xthree]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xthree, items[2]);

        try { _ = new Chain(engine, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Chain(engine, [null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Chain(engine, [new Element("")]); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public void Test_Create_With_Duplicates()
    {
        IEngine engine = new FakeEngine();
        var items = new Chain(engine, [xone, xone]);
        Assert.Equal(2, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xone, items[1]);

        items = new Chain(engine, [xone, xtwo, xone]);
        Assert.Equal(3, items.Count);
        Assert.Same(xone, items[0]);
        Assert.Same(xtwo, items[1]);
        Assert.Same(xone, items[2]);

        try { _ = new Chain(engine, [xone, new Element("ONE")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = new Chain(engine.WithCaseSensitiveNames(true), [xone, new Element("one")]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Embedded()
    {
        IEngine engine = new FakeEngine();
        var other = new Chain(engine, [xtwo, xthree]);
        var items = new Chain(engine, [xone, other]);

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
        IEngine engine = new FakeEngine();
        var items = new Chain(engine, [xone, xtwo, xthree, xone]);
        var target = items.Clone();

        Assert.NotSame(items, target);
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
        IEngine engine = new FakeEngine();
        var items = new Chain(engine, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf(xfive));

        Assert.Equal(0, items.IndexOf(xone));
        Assert.Equal(0, items.IndexOf(new Element("ONE")));

        Assert.Equal(3, items.LastIndexOf(xone));
        Assert.Equal(3, items.LastIndexOf(new Element("ONE")));

        var list = items.AllIndexesOf(xone);
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);

        list = items.AllIndexesOf(new Element("ONE"));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        IEngine engine = new FakeEngine();
        var items = new Chain(engine, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf(x => ((Element)x).Name.Contains('z')));

        Assert.Equal(0, items.IndexOf(x => ((Element)x).Name.Contains('n')));
        Assert.Equal(3, items.LastIndexOf(x => ((Element)x).Name.Contains('n')));

        var list = items.AllIndexesOf(x => ((Element)x).Name.Contains('n'));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine);
        var target = source.GetRange(0, 0); Assert.Same(source, target);

        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        target = source.GetRange(0, 4);
        Assert.Same(source, target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        try { source.GetRange(-1, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

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
    public static void Test_Replace_Empty()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree]);
        var target = source.Replace(1, new Chain(engine, []));
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Same()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree]);
        var target = source.Replace(1, xtwo);
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Standard()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree]);
        var target = source.Replace(1, xone);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xone, target[1]);
        Assert.Same(xthree, target[2]);

        try { source.Replace(1, new Element("one")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Embedded()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree]);
        var other = new Chain(engine, [xfour, xfive]);

        var target = source.Replace(1, other);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xfour, target[1]);
        Assert.Same(xfive, target[2]);
        Assert.Same(xthree, target[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);
        var target = source.Add(xthree);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        try { _ = source.Add(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.Add(new Element("")); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Duplicates()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);
        var target = source.Add(xone);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);

        try { _ = source.Add(new Element("one")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Extended()
    {
        IEngine engine = new FakeEngine();
        var other = new Chain(engine, []);
        var source = new Chain(engine, [xone, xtwo, xthree]);
        var target = source.Add(other);
        Assert.Same(source, target);

        other = new Chain(engine, [xfour, xfive]);
        target = source.Add(other);
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
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange([xthree, xfour]);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        try { _ = source.AddRange([new Element("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.AddRange([xfive, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.AddRange([xfive, new Element("")]); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Extended()
    {
        IEngine engine = new FakeEngine();
        var other = new Chain(engine, []);
        var source = new Chain(engine, [xone, xtwo]);
        var target = source.AddRange([other]);
        Assert.Same(source, target);

        other = new Chain(engine, [xfour, xfive]);
        target = source.AddRange([xthree, other]);
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
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);
        var target = source.Insert(2, xthree);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        try { _ = source.Insert(0, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.Insert(0, new Element("")); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Duplicates()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);
        var target = source.Insert(2, xone);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);

        try { _ = source.Insert(0, new Element("one")); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Extended()
    {
        IEngine engine = new FakeEngine();
        var other = new Chain(engine, []);
        var source = new Chain(engine, [xone, xtwo, xthree]);
        var target = source.Insert(3, other);
        Assert.Same(source, target);

        other = new Chain(engine, [xfour, xfive]);
        target = source.Insert(3, other);
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
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);
        var target = source.InsertRange(2, []);
        Assert.Same(source, target);

        target = source.InsertRange(2, [xthree, xfour]);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        try { _ = source.InsertRange(0, [new Element("one")]); Assert.Fail(); }
        catch (DuplicateException) { }

        try { _ = source.InsertRange(0, [xfive, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.InsertRange(0, [xfive, new Element("")]); Assert.Fail(); }
        catch (EmptyException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Extended()
    {
        IEngine engine = new FakeEngine();
        var other = new Chain(engine, []);
        var source = new Chain(engine, [xone, xtwo]);
        var target = source.InsertRange(2, [other]);
        Assert.Same(source, target);

        other = new Chain(engine, [xfour, xfive]);
        target = source.InsertRange(2, [xthree, other]);
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
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);
        var target = source.RemoveAt(0);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveAt(2);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);

        try { source.RemoveAt(999); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 4);
        Assert.Empty(target);

        target = source.RemoveRange(0, 1);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveRange(3, 1);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        try { _ = source.RemoveRange(0, -1); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { _ = source.RemoveRange(-1, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

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
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);
        var target = source.Remove(xfour);
        Assert.Same(source, target);

        target = source.Remove(xone);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.Remove(new Element("ONE"));
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item_Last()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);
        var target = source.RemoveLast(xone);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveLast(new Element("ONE"));
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item_All()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);
        var target = source.RemoveAll(xone);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.RemoveAll(new Element("ONE"));
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item_Extended()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);
        var other = new Chain(engine, [xone, xthree]);

        var target = source.Remove(other);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xone, target[1]);

        target = source.RemoveLast(other);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        target = source.RemoveAll(other);
        Assert.Single(target);
        Assert.Same(xtwo, target[0]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);
        var target = source.Remove(x => ((Element)x).Name.Contains('z'));
        Assert.Same(source, target);

        target = source.Remove(x => ((Element)x).Name.Contains('n'));
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate_Last()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);
        var target = source.RemoveLast(x => ((Element)x).Name.Contains('n'));
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate_All()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);
        var target = source.RemoveAll(x => ((Element)x).Name.Contains('n'));
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        target = source.Clear();
        Assert.Empty(target);
    }
}
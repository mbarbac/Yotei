using IChain = Yotei.ORM.IParameterList;
using Chain = Yotei.ORM.Code.ParameterList;
using IItem = Yotei.ORM.IParameter;
using Item = Yotei.ORM.Code.Parameter;
using System.Xml.Linq;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_ParameterList
{
    readonly static Item x007 = new("Id", "007");
    readonly static Item xJames = new("FirstName", "James");
    readonly static Item xBond = new("LastName", "Bond");
    readonly static Item xMi6 = new("Organization", "Mi6");
    readonly static Item xAge = new("Age", 55);

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var chain = new Chain(engine);
        Assert.Empty(chain);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range()
    {
        IEngine engine = new FakeEngine();
        var items = new Chain(engine, []);
        Assert.Empty(items);

        items = new Chain(engine, [x007, xJames, xBond]);
        Assert.Equal(3, items.Count);
        Assert.Same(x007, items[0]);
        Assert.Same(xJames, items[1]);
        Assert.Same(xBond, items[2]);

        try { _ = new Chain(engine, null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Chain(engine, [null!]); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Chain(engine, [x007, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range_With_Duplicates()
    {
        var engine = new FakeEngine();
        var chain = new Chain(engine, [x007, x007]);
        Assert.Equal(2, chain.Count);
        Assert.Same(x007, chain[0]);
        Assert.Same(x007, chain[1]);

        // Duplicates accepted only if they are the same instance.
        var item = x007.Clone();
        try { chain = new Chain(engine, [x007, item]); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        IEngine engine = new FakeEngine();
        var items = new Chain(engine, [x007, xJames, xBond, x007]);
        var target = items.Clone();

        Assert.NotSame(items, target);
        Assert.Equal(4, target.Count);
        Assert.Same(x007, target[0]);
        Assert.Same(xJames, target[1]);
        Assert.Same(xBond, target[2]);
        Assert.Same(x007, target[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Name()
    {
        IEngine engine = new FakeEngine() { IgnoreCase = true };
        var items = new Chain(engine, [x007, xJames, xBond, x007]);

        Assert.Equal(-1, items.IndexOf("xAge"));

        Assert.Equal(0, items.IndexOf("Id"));
        Assert.Equal(0, items.IndexOf("ID"));

        Assert.Equal(3, items.LastIndexOf("Id"));
        Assert.Equal(3, items.LastIndexOf("ID"));

        var list = items.IndexesOf("Id");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);

        list = items.IndexesOf("ID");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_IndexOf_Predicate()
    {
        IEngine engine = new FakeEngine();
        var items = new Chain(engine, [x007, xJames, xBond, x007]);

        Assert.Equal(-1, items.IndexOf(x => x.Name.Contains('z')));

        Assert.Equal(0, items.IndexOf(x => x.Name.Contains('d')));
        Assert.Equal(3, items.LastIndexOf(x => x.Name.Contains('d')));

        var list = items.IndexesOf(x => x.Name.Contains('d'));
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

        source = new Chain(engine, [x007, xJames, xBond, x007]);
        target = source.GetRange(0, 4);
        Assert.Same(source, target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xJames, target[0]);
        Assert.Same(xBond, target[1]);

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
    public static void Test_Replace_Same()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine, [x007, xJames, xBond, x007]);
        var target = source.Replace(1, xJames);
        Assert.Same(source, target);

        var item = x007.Clone();
        try { target = source.Replace(0, item); Assert.Fail(); }
        catch (DuplicateException) { }
    }
}
/*

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Replace()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree]);
        var target = source.Replace(0, xone);
        Assert.Same(source, target);

        target = source.Replace(0, new TItem("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("ONE", ((TItem)target[0]).Name);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        var xother = new TItem("other");
        try { source.Replace(-1, xother); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.Replace(3, xother); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Same()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Chain(engine, [xone, xtwo, xthree]);
        var target = source.Replace(0, xone);
        Assert.Same(source, target);

        try { source.Replace(1, xone); Assert.Fail(); } catch (DuplicateException) { }

        source = new Chain(engine) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xthree]);
        target = source.Replace(1, xtwo);
        Assert.Same(source, target);

        target = source.Replace(1, new TItem("TWO"));
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Empty_Nested()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree]);
        var target = source.Replace(0, new Chain(engine));
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Range_Nested()
    {
        var engine = new FakeEngine();
        var xalpha = new TItem("alpha");
        var xbeta = new TItem("beta");
        var source = new Chain(engine, [xone, xtwo, xthree]);

        var target = source.Replace(0, new Chain(engine, [xalpha, xbeta]));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xalpha, target[0]);
        Assert.Same(xbeta, target[1]);
        Assert.Same(xtwo, target[2]);
        Assert.Same(xthree, target[3]);

        target = source.Replace(1, new Chain(engine, [xalpha, xbeta]));
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xalpha, target[1]);
        Assert.Same(xbeta, target[2]);
        Assert.Same(xthree, target[3]);

        target = source.Replace(2, new Chain(engine, [xalpha, xbeta]));
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
        var engine = new FakeEngine();
        var source = new Chain(engine);
        var target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xone, target[0]);

        source = new Chain(engine, [xone]);
        target = source.Add(xtwo);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        try { source.Add(null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Add(xone); Assert.Fail(); } catch (DuplicateException) { }

        engine = new FakeEngine() { IgnoreCase = true };
        source = new Chain(engine, [xone, xtwo]);
        try { source.Add(new TItem("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        source.AcceptDuplicates = true;
        target = source.Add(xone);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xone, target[2]);

        target = source.Add(new TItem("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Equal("ONE", ((TItem)target[2]).Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Nested()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);
        var target = source.Add(new Chain(engine, [xthree, xfour]));

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
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);
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
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);
        var target = source.AddRange([xthree, new Chain(engine, [xfour, xfive])]);

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
        var engine = new FakeEngine();
        var source = new Chain(engine);
        var target = source.Insert(0, xone);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Same(xone, target[0]);

        source = (Chain)target;
        target = source.Insert(1, xtwo);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        source = (Chain)target;
        target = source.Insert(0, xthree);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xthree, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xtwo, target[2]);

        try { source.Insert(0, null!); Assert.Fail(); } catch (ArgumentNullException) { }
        try { source.Insert(0, xone); Assert.Fail(); } catch (DuplicateException) { }
        try { source.Insert(-1, xfive); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }
        try { source.Insert(4, xfive); Assert.Fail(); } catch (ArgumentOutOfRangeException) { }

        engine = new FakeEngine() { IgnoreCase = true };
        source = new Chain(engine, [xthree, xone, xtwo]);
        try { source.Insert(0, new TItem("ONE")); Assert.Fail(); } catch (DuplicateException) { }

        source = (Chain)target;
        source.AcceptDuplicates = true;
        target = source.Insert(3, xone);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xthree, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xtwo, target[2]);
        Assert.Same(xone, target[3]);

        source = (Chain)target;
        target = source.Insert(0, new TItem("ONE"));
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("ONE", ((TItem)target[0]).Name);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);
        Assert.Same(xtwo, target[3]);
        Assert.Same(xone, target[4]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Nested()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);
        var target = source.Insert(2, new Chain(engine, [xthree, xfour]));

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
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);
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
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo]);
        var target = source.InsertRange(1, [xthree, new Chain(engine, [xfour, xfive])]);

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
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree]);
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
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree]);
        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        var engine = new FakeEngine();
        var source = new Chain(engine, [xone, xtwo, xthree]);
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
    public static void Test_Remove_Key()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Chain(engine) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove("four");
        Assert.Same(source, target);

        target = source.Remove("one");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xthree, target[2]);

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
    public static void Test_Remove_Value_IgnoreCase()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Chain(engine) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.RemoveAll("ONE");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    //[Fact]
    //public static void Test_Remove_Value_Nested()
    // By default this capability is not supported: the remvoe methods take a 'key' argument, so
    // there is no default way of specifying a collection of elements to remove.

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var engine = new FakeEngine() { IgnoreCase = true };
        var source = new Chain(engine) { AcceptDuplicates = true };
        source = (Chain)source.AddRange([xone, xtwo, xone, xthree]);

        var target = source.Remove(x => x is TItem named && named.Name is null);
        Assert.Same(source, target);

        target = source.Remove(x => x is TItem named && named.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveLast(x => x is TItem named && named.Name.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll(x => x is TItem named && named.Name.Contains('n'));
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
        var engine = new FakeEngine();
        var source = new Chain(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new Chain(engine, [xone, xtwo, xthree]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
 */
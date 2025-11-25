using IChain = Yotei.ORM.IIdentifierChain;
using Chain = Yotei.ORM.Code.IdentifierChain;
using IItem = Yotei.ORM.IIdentifierUnit;
using Item = Yotei.ORM.Code.IdentifierUnit;

namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_IdentifierChain
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        IChain chain;
        IEngine engine = new FakeEngine();

        chain = new Chain(engine);
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        chain = new Chain(engine, (IEnumerable<IItem>)[]);
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        chain = new Chain(engine, (IEnumerable<string?>)[]);
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        chain = new Chain(engine, (string?)null);
        Assert.Empty(chain);
        Assert.Null(chain.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_From_String_Distinct_Terminators()
    {
        IChain chain;
        IEngine engine = new FakeEngine();

        chain = new Chain(engine, string.Empty);
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        chain = new Chain(engine, " ");
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        chain = new Chain(engine, "[]");
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        chain = new Chain(engine, " . [ ] . . [ ] . ");
        Assert.Empty(chain);
        Assert.Null(chain.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_From_String_Same_Terminators()
    {
        IChain chain;
        IEngine engine = new FakeEngine() { LeftTerminator = '#', RightTerminator = '#' };

        chain = new Chain(engine, string.Empty);
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        chain = new Chain(engine, " ");
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        chain = new Chain(engine, "##");
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        chain = new Chain(engine, " . # # . . # # . ");
        Assert.Empty(chain);
        Assert.Null(chain.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_Range_Parts()
    {
        IChain chain;
        IEngine engine = new FakeEngine();
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var xthree = new Item(engine, "three");

        chain = new Chain(engine, [xone, xtwo, xthree]);
        Assert.Equal(3, chain.Count);
        Assert.Equal("[one].[two].[three]", chain.Value);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        chain = new Chain(engine, [xone, xtwo, xone]); // With duplicates...
        Assert.Equal(3, chain.Count);
        Assert.Equal("[one].[two].[one]", chain.Value);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xone, chain[2]);

        chain = new Chain(engine, [xone, xtwo, new Item(engine, "one")]); // With duplicates...
        Assert.Equal(3, chain.Count);
        Assert.Equal("[one].[two].[one]", chain.Value);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Equal("one", chain[2].RawValue);

        try { _ = new Chain(engine, (IEnumerable<IItem>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Chain(engine, [xone, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single_String()
    {
        IChain chain;
        IEngine engine = new FakeEngine();

        chain = new Chain(engine, "one");
        Assert.Single(chain);
        Assert.Equal("[one]", chain.Value);
        Assert.Equal("one", chain[0].RawValue);

        chain = new Chain(engine, " [ one two ] ");
        Assert.Single(chain);
        Assert.Equal("[one two]", chain.Value);
        Assert.Equal("one two", chain[0].RawValue);

        chain = new Chain(engine, " [ one . two ] ");
        Assert.Single(chain);
        Assert.Equal("[one . two]", chain.Value);
        Assert.Equal("one . two", chain[0].RawValue);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range_String()
    {
        IChain chain;
        IEngine engine = new FakeEngine();

        chain = new Chain(engine, " one . two . three ");
        Assert.Equal(3, chain.Count);
        Assert.Equal("[one].[two].[three]", chain.Value);
        Assert.Equal("one", chain[0].RawValue);
        Assert.Equal("two", chain[1].RawValue);
        Assert.Equal("three", chain[2].RawValue);

        chain = new Chain(engine, " . two . three ");
        Assert.Equal(2, chain.Count);
        Assert.Equal("[two].[three]", chain.Value);
        Assert.Equal("two", chain[0].RawValue);
        Assert.Equal("three", chain[1].RawValue);

        chain = new Chain(engine, " . . three ");
        Assert.Single(chain);
        Assert.Equal("[three]", chain.Value);
        Assert.Equal("three", chain[0].RawValue);

        chain = new Chain(engine, " one . two . ");
        Assert.Equal(3, chain.Count);
        Assert.Equal("[one].[two].", chain.Value);
        Assert.Equal("one", chain[0].RawValue);
        Assert.Equal("two", chain[1].RawValue);
        Assert.Null(chain[2].RawValue);

        chain = new Chain(engine, " one . . ");
        Assert.Equal(3, chain.Count);
        Assert.Equal("[one]..", chain.Value);
        Assert.Equal("one", chain[0].RawValue);
        Assert.Null(chain[1].RawValue);
        Assert.Null(chain[2].RawValue);

        chain = new Chain(engine, " one . . three ");
        Assert.Equal(3, chain.Count);
        Assert.Equal("[one]..[three]", chain.Value);
        Assert.Equal("one", chain[0].RawValue);
        Assert.Null(chain[1].RawValue);
        Assert.Equal("three", chain[2].RawValue);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine);
        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Empty(target);
        Assert.Null(target.Value);

        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var xthree = new Item(engine, "three");

        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal("[one].[two].[three].[one]", target.Value);
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
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var xthree = new Item(engine, "three");
        var items = new Chain(engine, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf("xfive"));

        Assert.Equal(0, items.IndexOf("one"));
        Assert.Equal(0, items.IndexOf("ONE"));

        Assert.Equal(3, items.LastIndexOf("one"));
        Assert.Equal(3, items.LastIndexOf("ONE"));

        var list = items.AllIndexesOf("one");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);

        list = items.AllIndexesOf("ONE");
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find_Predicate()
    {
        IEngine engine = new FakeEngine();
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var xthree = new Item(engine, "three");
        var items = new Chain(engine, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf(x => ((Item)x).Value!.Contains('z')));

        Assert.Equal(0, items.IndexOf(x => ((Item)x).Value!.Contains('n')));
        Assert.Equal(3, items.LastIndexOf(x => ((Item)x).Value!.Contains('n')));

        var list = items.AllIndexesOf(x => ((Item)x).Value!.Contains('n'));
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

        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var xthree = new Item(engine, "three");
        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        target = source.GetRange(0, 4);
        Assert.Same(source, target);

        target = source.GetRange(1, 2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("[two].[three]", target.Value);
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
    public static void Test_Replace_Item()
    {
        IEngine engine = new FakeEngine();
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var xthree = new Item(engine, "three");
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);

        var target = source.Replace(1, xone);
        Assert.NotSame(source, target);
        Assert.Equal("[one].[one].[three].[one]", target.Value);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xone, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xone, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Empty_String()
    {
        IEngine engine = new FakeEngine();
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var xthree = new Item(engine, "three");

        var source = new Chain(engine, [xone, xtwo, xthree, xone]);
        var target = source.Replace(1, null);
        Assert.NotSame(source, target);
        Assert.Equal("[one]..[three].[one]", target.Value);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Null(target[1].RawValue);
        Assert.Same(xthree, target[2]);
        Assert.Same(xone, target[3]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Regular_String()
    {
        IEngine engine = new FakeEngine();
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var xthree = new Item(engine, "three");

        var source = new Chain(engine, [xone, xtwo, xthree, xone]);
        var target = source.Replace(1, "TWO");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[TWO].[three].[one]", target.Value);
        Assert.Same(xone, target[0]);
        Assert.Equal("TWO", target[1].RawValue);
        Assert.Same(xthree, target[2]);
        Assert.Same(xone, target[3]);

        target = source.Replace(1, "TWO.THREE");
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[one].[TWO].[THREE].[three].[one]", target.Value);
        Assert.Same(xone, target[0]);
        Assert.Equal("TWO", target[1].RawValue);
        Assert.Equal("THREE", target[2].RawValue);
        Assert.Same(xthree, target[3]);
        Assert.Same(xone, target[4]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add_Item()
    {
        IEngine engine = new FakeEngine();
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var xthree = new Item(engine, "three");

        var source = new Chain(engine, [xone, xtwo]);
        var target = source.Add(xthree);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("[one].[two].[three]", target.Value);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        try { _ = source.Add((IIdentifierUnit)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Null_String()
    {
        IEngine engine = new FakeEngine();
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");

        var source = new Chain(engine, [xone, xtwo]);
        var target = source.Add(null);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("[one].[two].", target.Value);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Null(target[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Populated_String()
    {
        IEngine engine = new FakeEngine();
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");

        var source = new Chain(engine, [xone, xtwo]);
        var target = source.Add(".");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[two]..", target.Value);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Null(target[2].Value);
        Assert.Null(target[3].Value);

        target = source.Add("THREE.FOUR");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[two].[THREE].[FOUR]", target.Value);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Equal("THREE", target[2].RawValue);
        Assert.Equal("FOUR", target[3].RawValue);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Items()
    {
        IEngine engine = new FakeEngine();
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var source = new Chain(engine, [xone, xtwo]);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        var xthree = new Item(engine, "three");
        target = source.AddRange([xthree, xone]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[two].[three].[one]", target.Value);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xone, target[3]);

        try { _ = source.AddRange(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.AddRange([xthree, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Values()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        target = source.AddRange(["", ".one"]);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("[one]", target.Value);

        source = new Chain(engine, "one.two");
        target = source.AddRange([""]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("[one].[two].", target.Value);

        target = source.AddRange(["", ".five"]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[one].[two]...[five]", target.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert_Item()
    {
        IEngine engine = new FakeEngine();
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var source = new Chain(engine, [xone, xtwo]);

        var xthree = new Item(engine);
        var target = source.Insert(0, xthree);
        Assert.Same(source, target);

        xthree = new Item(engine, "three");
        target = source.Insert(0, xthree);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("[three].[one].[two]", target.Value);
        Assert.Same(xthree, target[0]);
        Assert.Same(xone, target[1]);
        Assert.Same(xtwo, target[2]);

        try { _ = source.Add((IIdentifierUnit)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Null_String()
    {
        IEngine engine = new FakeEngine();
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var source = new Chain(engine, [xone, xtwo]);

        var target = source.Insert(0, null);
        Assert.Same(source, target);

        target = source.Insert(1, null);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("[one]..[two]", target.Value);
        Assert.Same(xone, target[0]);
        Assert.Null(target[1].Value);
        Assert.Same(xtwo, target[2]);

        target = source.Insert(2, null);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("[one].[two].", target.Value);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Null(target[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Populated_String()
    {
        IEngine engine = new FakeEngine();
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var source = new Chain(engine, [xone, xtwo]);

        var target = source.Insert(0, ".");
        Assert.Same(source, target);

        target = source.Insert(0, "ALPHA.");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[ALPHA]..[one].[two]", target.Value);
        Assert.Equal("ALPHA", target[0].RawValue);
        Assert.Null(target[1].RawValue);
        Assert.Same(xone, target[2]);
        Assert.Same(xtwo, target[3]);

        target = source.Insert(2, "THREE.");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[two].[THREE].", target.Value);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Equal("THREE", target[2].RawValue);
        Assert.Null(target[3].RawValue);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Items()
    {
        IEngine engine = new FakeEngine();
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var source = new Chain(engine, [xone, xtwo]);
        
        var target = source.InsertRange(0, []); Assert.Same(source, target);
        target = source.InsertRange(1, []); Assert.Same(source, target);

        var xitem = new Item(engine);
        target = source.InsertRange(0, [xitem, xitem]);
        Assert.Same(source, target);

        var xthree = new Item(engine, "three");
        target = source.InsertRange(1, [xthree, xone]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one].[three].[one].[two]", target.Value);
        Assert.Same(xone, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);
        Assert.Same(xtwo, target[3]);

        try { _ = source.AddRange(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = source.AddRange([xthree, null!]); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Values()
    {
        IEngine engine = new FakeEngine();
        var source = new Chain(engine);
        var target = source.InsertRange(0, []);
        Assert.Same(source, target);

        target = source.InsertRange(0, ["", ".one"]);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("[one]", target.Value);

        source = new Chain(engine, "one.two");
        target = source.InsertRange(0, [""]); Assert.Same(source, target);
        target = source.InsertRange(0, ["", ".."]); Assert.Same(source, target);

        target = source.InsertRange(0, ["..alpha", "..beta"]);
        Assert.NotSame(source, target);
        Assert.Equal("[alpha]...[beta].[one].[two]", target.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        IEngine engine = new FakeEngine();
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var xthree = new Item(engine, "three");
        var source = new Chain(engine, [xone, xtwo, xthree]);

        var target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("[two].[three]", target.Value);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        source = new Chain(engine, [xone, new Item(engine), xthree]);
        target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("[three]", target.Value);
        Assert.Same(xthree, target[0]);

        target = source.RemoveAt(2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("[one].", target.Value);
        Assert.Same(xone, target[0]);
        Assert.Null(target[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Range()
    {
        IEngine engine = new FakeEngine();
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var xthree = new Item(engine, "three");
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);

        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 4);
        Assert.Empty(target);

        target = source.RemoveRange(0, 1);
        Assert.Equal(3, target.Count);
        Assert.Equal("[two].[three].[one]", target.Value);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveRange(3, 1);
        Assert.Equal(3, target.Count);
        Assert.Equal("[one].[two].[three]", target.Value);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        try { _ = source.RemoveRange(0, -1); Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }

        try { _ = source.RemoveRange(-1, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { _ = source.RemoveRange(5, 0); Assert.Fail(); }
        catch (IndexOutOfRangeException) { }

        try { _ = source.RemoveRange(0, 5); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item()
    {
        IEngine engine = new FakeEngine();
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var xthree = new Item(engine, "three");
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);

        var target = source.Remove("four");
        Assert.Same(source, target);

        target = source.Remove("one");
        Assert.Equal(3, target.Count);
        Assert.Equal("[two].[three].[one]", target.Value);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.Remove("ONE");
        Assert.Equal(3, target.Count);
        Assert.Equal("[two].[three].[one]", target.Value);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        source = new Chain(engine, [xone, new Item(engine), xthree, xone]);
        target = source.Remove("one");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("[three].[one]", target.Value);
        Assert.Same(xthree, target[0]);
        Assert.Same(xone, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item_Last()
    {
        IEngine engine = new FakeEngine();
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var xthree = new Item(engine, "three");
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);

        var target = source.RemoveLast("one");
        Assert.Equal(3, target.Count);
        Assert.Equal("[one].[two].[three]", target.Value);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        source = new Chain(engine, [xone, xtwo, new Item(engine), xone]);
        target = source.RemoveLast("one");
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("[one].[two].", target.Value);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Null(target[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove_Item_All()
    {
        IEngine engine = new FakeEngine();
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var xthree = new Item(engine, "three");
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);

        var target = source.RemoveAll("one");
        Assert.Equal(2, target.Count);
        Assert.Equal("[two].[three]", target.Value);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        source = new Chain(engine, [xone, new Item(engine), xone]);
        target = source.RemoveAll("one");
        Assert.Empty(target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Remove_Predicate()
    {
        IEngine engine = new FakeEngine();
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var xthree = new Item(engine, "three");
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);

        var target = source.Remove(x => ((Item)x).Value!.Contains('z'));
        Assert.Same(source, target);

        target = source.Remove(x => ((Item)x).Value!.Contains('n'));
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
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var xthree = new Item(engine, "three");
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);

        var target = source.RemoveLast(x => ((Item)x).Value!.Contains('n'));
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
        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var xthree = new Item(engine, "three");
        var source = new Chain(engine, [xone, xtwo, xthree, xone]);

        var target = source.RemoveAll(x => ((Item)x).Value!.Contains('n'));
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

        var xone = new Item(engine, "one");
        var xtwo = new Item(engine, "two");
        var xthree = new Item(engine, "three");
        source = new Chain(engine, [xone, xtwo, xthree, xone]);
        target = source.Clear();
        Assert.Empty(target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Match_Empty()
    {
        var engine = new FakeEngine();
        var item = new IdentifierChain(engine);

        Assert.True(Identifier.Match(item, null));
        Assert.True(Identifier.Match(item, ""));

        Assert.False(Identifier.Match(item, "two"));
        Assert.False(Identifier.Match(item, "two."));
    }

    //[Enforced]
    [Fact]
    public static void Test_Match_Populated_Smaller()
    {
        var engine = new FakeEngine();
        var item = new IdentifierChain(engine, "one");

        Assert.True(Identifier.Match(item, null));
        Assert.True(Identifier.Match(item, ""));

        Assert.True(Identifier.Match(item, "one"));
        Assert.True(Identifier.Match(item, ".one"));

        Assert.False(Identifier.Match(item, "two"));
        Assert.False(Identifier.Match(item, "two."));
    }

    //[Enforced]
    [Fact]
    public static void Test_Match_Populated_Bigger()
    {
        var engine = new FakeEngine();
        var item = new IdentifierChain(engine, "two.one");

        Assert.True(Identifier.Match(item, null));
        Assert.True(Identifier.Match(item, ""));

        Assert.True(Identifier.Match(item, "one"));
        Assert.True(Identifier.Match(item, ".one"));
        Assert.True(Identifier.Match(item, "two.one"));
        Assert.True(Identifier.Match(item, "two."));

        Assert.False(Identifier.Match(item, "two"));
        Assert.False(Identifier.Match(item, "one."));
    }
}
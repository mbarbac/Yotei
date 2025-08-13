namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_IdentifierChain
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_Parts()
    {
        IIdentifierChain chain;
        IEngine engine = new FakeEngine();

        chain = new IdentifierChain(engine);
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        chain = new IdentifierChain(engine, (IEnumerable<IIdentifierPart>)[]);
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        try { _ = new IdentifierChain(engine, (IEnumerable<IIdentifierPart>)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_String()
    {
        IIdentifierChain chain;
        IEngine engine = new FakeEngine();

        chain = new IdentifierChain(engine, (string?)null);
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        chain = new IdentifierChain(engine, string.Empty);
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        chain = new IdentifierChain(engine, "[]");
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        chain = new IdentifierChain(engine, "[]..[]");
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        chain = new IdentifierChain(engine, (IEnumerable<string?>)[]);
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        try { _ = new IdentifierChain(engine, " "); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_Same_Terminators()
    {
        IIdentifierChain chain;
        char coma = '!';
        IEngine engine = new FakeEngine() { LeftTerminator = coma, RightTerminator = coma };

        chain = new IdentifierChain(engine, "!!.!!.!!");
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        try { _ = new IdentifierChain(engine, "!! !!"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Single_String()
    {
        IIdentifierChain chain;
        IEngine engine = new FakeEngine();

        chain = new IdentifierChain(engine, "one");
        Assert.Equal("[one]", chain.Value);
        Assert.Single(chain);
        Assert.Equal("one", chain[0].RawValue);

        chain = new IdentifierChain(engine, "[one two]");
        Assert.Equal("[one two]", chain.Value);
        Assert.Single(chain);
        Assert.Equal("one two", chain[0].RawValue);

        chain = new IdentifierChain(engine, "[one.two]");
        Assert.Equal("[one.two]", chain.Value);
        Assert.Single(chain);
        Assert.Equal("one.two", chain[0].RawValue);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range_Parts()
    {
        IIdentifierChain chain;
        IEngine engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");
        var xthree = new IdentifierPart(engine, "three");

        IEnumerable<IIdentifierPart> parts = [];
        chain = new IdentifierChain(engine, parts);
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        chain = new IdentifierChain(engine, [xone, xtwo, xthree]);
        Assert.Equal("[one].[two].[three]", chain.Value);
        Assert.Equal(3, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xtwo, chain[1]);
        Assert.Same(xthree, chain[2]);

        parts = null!;
        try { _ = new IdentifierChain(engine, parts); Assert.Fail(); }
        catch (ArgumentNullException) { }

        parts = [null!];
        try { _ = new IdentifierChain(engine, parts); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Range_String()
    {
        IIdentifierChain chain;
        IEngine engine = new FakeEngine();

        chain = new IdentifierChain(engine, "one.two.three");
        Assert.Equal("[one].[two].[three]", chain.Value);
        Assert.Equal(3, chain.Count);
        Assert.Equal("one", chain[0].RawValue);
        Assert.Equal("two", chain[1].RawValue);
        Assert.Equal("three", chain[2].RawValue);

        chain = new IdentifierChain(engine, ".two.three");
        Assert.Equal("[two].[three]", chain.Value);
        Assert.Equal(2, chain.Count);
        Assert.Equal("two", chain[0].RawValue);
        Assert.Equal("three", chain[1].RawValue);

        chain = new IdentifierChain(engine, "..three");
        Assert.Equal("[three]", chain.Value);
        Assert.Single(chain);
        Assert.Equal("three", chain[0].RawValue);

        chain = new IdentifierChain(engine, "one..three");
        Assert.Equal("[one]..[three]", chain.Value);
        Assert.Equal(3, chain.Count);
        Assert.Equal("one", chain[0].RawValue);
        Assert.Null(chain[1].RawValue);
        Assert.Equal("three", chain[2].RawValue);

        chain = new IdentifierChain(engine, "one..");
        Assert.Equal("[one]..", chain.Value);
        Assert.Equal(3, chain.Count);
        Assert.Equal("one", chain[0].RawValue);
        Assert.Null(chain[1].RawValue);
        Assert.Null(chain[2].RawValue);

        chain = new IdentifierChain(engine, "one.[two.three]");
        Assert.Equal("[one].[two.three]", chain.Value);
        Assert.Equal(2, chain.Count);
        Assert.Equal("one", chain[0].RawValue);
        Assert.Equal("two.three", chain[1].RawValue);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Duplicates()
    {
        IIdentifierChain chain;
        IEngine engine = new FakeEngine();

        chain = new IdentifierChain(engine, "one.one");
        Assert.Equal("[one].[one]", chain.Value);
        Assert.Equal(2, chain.Count);
        Assert.Equal("one", chain[0].RawValue);
        Assert.Equal("one", chain[1].RawValue);

        var xone = new IdentifierPart(engine, "one");
        chain = new IdentifierChain(engine, [xone, xone]);
        Assert.Equal(2, chain.Count);
        Assert.Same(xone, chain[0]);
        Assert.Same(xone, chain[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");
        var xthree = new IdentifierPart(engine, "three");

        var items = new IdentifierChain(engine, [xone, xtwo, xthree, xone]);
        var target = items.Clone();

        Assert.NotSame(items, target);
        Assert.Equal("[one].[two].[three].[one]", target.Value);
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
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");
        var xthree = new IdentifierPart(engine, "three");

        var items = new IdentifierChain(engine, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf("xfive"));

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
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");
        var xthree = new IdentifierPart(engine, "three");

        var items = new IdentifierChain(engine, [xone, xtwo, xthree, xone]);

        Assert.Equal(-1, items.IndexOf(x => ((IdentifierPart)x).Value!.Contains('z')));

        Assert.Equal(0, items.IndexOf(x => ((IdentifierPart)x).Value!.Contains('n')));
        Assert.Equal(3, items.LastIndexOf(x => ((IdentifierPart)x).Value!.Contains('n')));

        var list = items.IndexesOf(x => ((IdentifierPart)x).Value!.Contains('n'));
        Assert.Equal(2, list.Count);
        Assert.Equal(0, list[0]);
        Assert.Equal(3, list[1]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");
        var xthree = new IdentifierPart(engine, "three");
        var xfour = new IdentifierPart(engine, "four");

        var source = new IdentifierChain(engine, [xone, xtwo, xthree, xfour]);
        var target = source.GetRange(0, 0);
        Assert.Empty(target);

        target = source.GetRange(1, 2);
        Assert.Equal("[two].[three]", target.Value);
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
    public static void Test_Replace_Extended()
    {
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");
        var xthree = new IdentifierPart(engine, "three");
        var source = new IdentifierChain(engine, [xone, xtwo, xthree]);

        var target = source.Replace(1, "four.five");
        Assert.NotSame(source, target);
        Assert.Equal("[one].[four].[five].[three]", target.Value);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal("four", target[1].RawValue);
        Assert.Equal("five", target[2].RawValue);
        Assert.Same(xthree, target[3]);

        target = source.Replace(1, "");
        Assert.NotSame(source, target);
        Assert.Equal("[one]..[three]", target.Value);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Null(target[1].RawValue);
        Assert.Same(xthree, target[2]);

        target = source.Replace(0, "");
        Assert.NotSame(source, target);
        Assert.Equal("[two].[three]", target.Value);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.Replace(0, ".");
        Assert.NotSame(source, target);
        Assert.Equal("[two].[three]", target.Value);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);

        target = source.Replace(0, ".four.");
        Assert.NotSame(source, target);
        Assert.Equal("[four]..[two].[three]", target.Value);
        Assert.Equal(4, target.Count);
        Assert.Equal("four", target[0].RawValue);
        Assert.Null(target[1].RawValue);
        Assert.Same(xtwo, target[2]);
        Assert.Same(xthree, target[3]);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");
        var xthree = new IdentifierPart(engine, "three");

        var source = new IdentifierChain(engine, [xone, xtwo]);
        var target = source.Add(xthree);

        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_String()
    {
        var engine = new FakeEngine();
        var source = new IdentifierChain(engine);
        Assert.Empty(source);

        var target = source.Add(null);
        Assert.Same(source, target);

        target = source.Add(string.Empty);
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Change_Engine()
    {
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var source = new IdentifierChain(engine, [xone]);

        var other = engine.Clone();
        var xtwo = new IdentifierPart(other, "two");

        var target = source.Add(xtwo);
        Assert.NotSame(source, target);
        Assert.Equal("[one].[two]", target.ToString());
        Assert.Equal(2, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);

        other = new FakeEngine() { CaseSensitiveNames = true };
        xtwo = new IdentifierPart(other, "two");
        try { _ = source.Add(xtwo); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Add_Duplicates()
    {
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");

        var source = new IdentifierChain(engine, [xone, xtwo]);
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
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");
        var xthree = new IdentifierPart(engine, "three");

        var source = new IdentifierChain(engine, [xone, xtwo, xthree]);
        var target = source.Add("");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Null(target[3].RawValue);

        target = source.Add("four.five");
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Equal("four", target[3].RawValue);
        Assert.Equal("five", target[4].RawValue);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");

        var source = new IdentifierChain(engine, [xone, xtwo]);
        var target = source.AddRange([]);
        Assert.Same(source, target);

        var xthree = new IdentifierPart(engine, "three");
        var xfour = new IdentifierPart(engine, "four");
        target = source.AddRange([xthree, xfour]);

        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        try { _ = source.AddRange([xone, null!]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange_Extended()
    {
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");

        var source = new IdentifierChain(engine, [xone, xtwo]);
        var target = source.AddRange([""]);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Null(target[2].RawValue);

        target = source.AddRange(["three", "four.five"]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Equal(xtwo, target[1]);
        Assert.Equal("three", target[2].RawValue);
        Assert.Equal("four", target[3].RawValue);
        Assert.Equal("five", target[4].RawValue);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");
        var xthree = new IdentifierPart(engine, "three");

        var source = new IdentifierChain(engine, [xone, xtwo]);
        var target = source.Insert(2, xthree);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.Insert(0, null!);
        Assert.Same(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert_Extended()
    {
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");
        var xthree = new IdentifierPart(engine, "three");
        var xfour = new IdentifierPart(engine, "four");

        var source = new IdentifierChain(engine, [xone, xtwo, xthree]);
        var target = source.Insert(3, "");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Null(target[3].RawValue);

        target = source.Insert(3, "four.five");
        Assert.Equal(5, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Equal("four", target[3].RawValue);
        Assert.Equal("five", target[4].RawValue);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");
        var xthree = new IdentifierPart(engine, "three");
        var xfour = new IdentifierPart(engine, "four");

        var source = new IdentifierChain(engine, [xone, xtwo]);
        var target = source.InsertRange(2, []);
        Assert.Same(source, target);

        target = source.InsertRange(2, [xthree, xfour]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);
        Assert.Same(xfour, target[3]);

        try { _ = source.InsertRange(0, [xone, null!]); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange_Extended()
    {
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");

        // Target will actually insert an empty head element, later removed when reduced, but
        // it means that there are a number of changes, namely one, that make the target a
        // different instance...
        var source = new IdentifierChain(engine, [xone, xtwo]);
        var target = source.InsertRange(0, [""]);
        Assert.NotSame(source, target);
        Assert.Equal(source.Count, target.Count);
        Assert.Same(source[0], target[0]);
        Assert.Same(source[1], target[1]);

        var xthree = new IdentifierPart(engine, "three");
        var xfour = new IdentifierPart(engine, "four");
        target = source.InsertRange(2, [xthree, xfour]);
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
    public static void Test_RemoveAt()
    {
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");
        var xthree = new IdentifierPart(engine, "three");
        var source = new IdentifierChain(engine, [xone, xtwo, xthree, xone]);

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
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");
        var xthree = new IdentifierPart(engine, "three");
        var xfour = new IdentifierPart(engine, "four");

        var source = new IdentifierChain(engine, [xone, xtwo, xthree, xone]);
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
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");
        var xthree = new IdentifierPart(engine, "three");
        var xfour = new IdentifierPart(engine, "four");

        var source = new IdentifierChain(engine, [xone, xtwo, xthree, xone]);
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
    [Fact]
    public static void Test_Remove_Predicate()
    {
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");
        var xthree = new IdentifierPart(engine, "three");
        var xfour = new IdentifierPart(engine, "four");

        var source = new IdentifierChain(engine, [xone, xtwo, xthree, xone]);
        var target = source.Remove(x => x.Value != null && x.Value.Contains('z'));
        Assert.Same(source, target);

        target = source.Remove(x => x.Value != null && x.Value.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
        Assert.Same(xone, target[2]);

        target = source.RemoveLast(x => x.Value != null && x.Value.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Same(xone, target[0]);
        Assert.Same(xtwo, target[1]);
        Assert.Same(xthree, target[2]);

        target = source.RemoveAll(x => x.Value != null && x.Value.Contains('n'));
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Same(xtwo, target[0]);
        Assert.Same(xthree, target[1]);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        var engine = new FakeEngine();
        var xone = new IdentifierPart(engine, "one");
        var xtwo = new IdentifierPart(engine, "two");
        var xthree = new IdentifierPart(engine, "three");
        var xfour = new IdentifierPart(engine, "four");

        var source = new IdentifierChain(engine);
        var target = source.Clear();
        Assert.Same(source, target);

        source = new IdentifierChain(engine, [xone, xtwo, xthree, xone]);
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}
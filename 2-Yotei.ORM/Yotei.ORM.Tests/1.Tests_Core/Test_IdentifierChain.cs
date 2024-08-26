namespace Yotei.ORM.Tests;

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
        List<IIdentifierPart> parts = [];

        chain = new IdentifierChain(engine);
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        chain = new IdentifierChain(engine, parts);
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        parts.AddRange([new IdentifierPart(engine), new IdentifierPart(engine)]);
        chain = new IdentifierChain(engine, parts);
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        parts.Add(null!);
        try { chain = new IdentifierChain(engine, parts); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_String()
    {
        IIdentifierChain chain;
        IEngine engine = new FakeEngine();
        List<IIdentifierPart> parts = [];

        chain = new IdentifierChain(engine, (string?)null);
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        chain = new IdentifierChain(engine, string.Empty);
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        chain = new IdentifierChain(engine, "[].[].[]");
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        chain = new IdentifierChain(engine, "..");
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        engine = new FakeEngine() { LeftTerminator = '\'', RightTerminator = '\'', };
        chain = new IdentifierChain(engine, "''.''.''");
        Assert.Empty(chain);
        Assert.Null(chain.Value);

        try { chain = new IdentifierChain(engine, " "); Assert.Fail(); }
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
        Assert.Equal("[one]", chain.Value);

        chain = new IdentifierChain(engine, "[one.two]");
        Assert.Equal("[one.two]", chain.Value);
        Assert.Single(chain);
        Assert.Equal("[one.two]", chain.Value);

        engine = new FakeEngine() { LeftTerminator = '\'', RightTerminator = '\'', };
        chain = new IdentifierChain(engine, "'one.two'");
        Assert.Equal("'one.two'", chain.Value);
        Assert.Single(chain);
        Assert.Equal("'one.two'", chain[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Multi_String()
    {
        IIdentifierChain chain;
        IEngine engine = new FakeEngine();

        chain = new IdentifierChain(engine, "[one].two.[three]");
        Assert.Equal("[one].[two].[three]", chain.Value);
        Assert.Equal(3, chain.Count);
        Assert.Equal("[one]", chain[0].Value);
        Assert.Equal("[two]", chain[1].Value);
        Assert.Equal("[three]", chain[2].Value);

        chain = new IdentifierChain(engine, ".two.[three]");
        Assert.Equal("[two].[three]", chain.Value);
        Assert.Equal(2, chain.Count);
        Assert.Equal("[two]", chain[0].Value);
        Assert.Equal("[three]", chain[1].Value);

        chain = new IdentifierChain(engine, "..[three]");
        Assert.Equal("[three]", chain.Value);
        Assert.Single(chain);
        Assert.Equal("[three]", chain[0].Value);

        chain = new IdentifierChain(engine, "[one]..[three]");
        Assert.Equal("[one]..[three]", chain.Value);
        Assert.Equal(3, chain.Count);
        Assert.Equal("[one]", chain[0].Value);
        Assert.Null(chain[1].Value);
        Assert.Equal("[three]", chain[2].Value);

        chain = new IdentifierChain(engine, "[one]..");
        Assert.Equal("[one]..", chain.Value);
        Assert.Equal(3, chain.Count);
        Assert.Equal("[one]", chain[0].Value);
        Assert.Null(chain[1].Value);
        Assert.Null(chain[2].Value);

        engine = new FakeEngine() { LeftTerminator = '\'', RightTerminator = '\'', };
        chain = new IdentifierChain(engine, "'one'.two");
        Assert.Equal("'one'.'two'", chain.Value);
        Assert.Equal(2, chain.Count);
        Assert.Equal("'one'", chain[0].Value);
        Assert.Equal("'two'", chain[1].Value);
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
        Assert.Equal("[one]", chain[0].Value);
        Assert.Equal("[one]", chain[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        IEngine engine = new FakeEngine();
        var source = new IdentifierChain(engine, "one.two.one");

        var target = source.Clone();
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);
        Assert.Equal("[one]", target[2].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Find()
    {
        IEngine engine = new FakeEngine();
        var chain = new IdentifierChain(engine, "one.two.three.two");

        Assert.Equal(-1, chain.IndexOf("other"));

        Assert.Equal(1, chain.IndexOf("TWO"));
        Assert.Equal(3, chain.LastIndexOf("TWO"));
        var list = chain.IndexesOf("TWO");
        Assert.Equal(2, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(3, list[1]);

        try { chain.IndexOf("one.two"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_GetRange()
    {
        IEngine engine = new FakeEngine();
        var source = new IdentifierChain(engine, "one.two.one.four");

        var target = source.GetRange(0, 0);
        Assert.Empty(target);

        target = source.GetRange(0, 4);
        Assert.Same(source, target);

        target = source.GetRange(2, 2);
        Assert.Equal(2, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Equal("[four]", target[1].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Single_Null()
    {
        IEngine engine = new FakeEngine();
        var source = new IdentifierChain(engine, "one.two.one.four");

        var target = source.Replace(0, null);
        Assert.NotSame(source, target);
        Assert.Equal(3, target.Count);
        Assert.Equal("[two]", target[0].Value);
        Assert.Equal("[one]", target[1].Value);
        Assert.Equal("[four]", target[2].Value);

        target = source.Replace(1, null);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Null(target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Equal("[four]", target[3].Value);

        target = source.Replace(3, null);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Null(target[3].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Single()
    {
        IEngine engine = new FakeEngine();
        var source = new IdentifierChain(engine, "one.two.one.four");

        var target = source.Replace(0, "one");
        Assert.Same(source, target);

        target = source.Replace(0, "other");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[other]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Equal("[four]", target[3].Value);

        target = source.Replace(0, "ONE");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[ONE]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Equal("[four]", target[3].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Replace_Multiple()
    {
        IEngine engine = new FakeEngine();
        var source = new IdentifierChain(engine, "one.two.one.four");

        var target = source.Replace(0, "alpha.beta");
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[alpha]", target[0].Value);
        Assert.Equal("[beta]", target[1].Value);
        Assert.Equal("[two]", target[2].Value);
        Assert.Equal("[one]", target[3].Value);
        Assert.Equal("[four]", target[4].Value);

        target = source.Replace(0, "alpha.");
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[alpha]", target[0].Value);
        Assert.Null(target[1].Value);
        Assert.Equal("[two]", target[2].Value);
        Assert.Equal("[one]", target[3].Value);
        Assert.Equal("[four]", target[4].Value);

        target = source.Replace(0, ".beta");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[beta]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Equal("[four]", target[3].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Add()
    {
        IEngine engine = new FakeEngine();
        var source = new IdentifierChain(engine, "one.two.one");

        var target = source.Add("alpha");
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Equal("[alpha]", target[3].Value);

        target = source.Add(null);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Null(target[3].Value);

        target = source.Add("alpha.beta");
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Equal("[alpha]", target[3].Value);
        Assert.Equal("[beta]", target[4].Value);

        target = source.Add(".beta");
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Null(target[3].Value);
        Assert.Equal("[beta]", target[4].Value);

        target = source.Add(".");
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Null(target[3].Value);
        Assert.Null(target[4].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_AddRange()
    {
        IEngine engine = new FakeEngine();
        var source = new IdentifierChain(engine, "one.two.one");

        var target = source.AddRange(["alpha", "beta"]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Equal("[alpha]", target[3].Value);
        Assert.Equal("[beta]", target[4].Value);

        target = source.AddRange([null, "[alpha.beta]"]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Null(target[3].Value);
        Assert.Equal("[alpha.beta]", target[4].Value);

        target = source.AddRange(["alpha", "."]);
        Assert.NotSame(source, target);
        Assert.Equal(6, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Equal("[alpha]", target[3].Value);
        Assert.Null(target[4].Value);
        Assert.Null(target[5].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Insert()
    {
        IEngine engine = new FakeEngine();
        var source = new IdentifierChain(engine, "one.two.one");

        var target = source.Insert(0, null);
        Assert.Same(source, target);

        target = source.Insert(0, ".");
        Assert.Same(source, target);

        target = source.Insert(0, "alpha.");
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[alpha]", target[0].Value);
        Assert.Null(target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Equal("[two]", target[3].Value);
        Assert.Equal("[one]", target[4].Value);

        target = source.Insert(3, null);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Null(target[3].Value);

        target = source.Insert(3, ".");
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Null(target[3].Value);
        Assert.Null(target[4].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_InsertRange()
    {
        IEngine engine = new FakeEngine();
        var source = new IdentifierChain(engine, "one.two.one");

        var target = source.InsertRange(0, [null]);
        Assert.Same(source, target);

        target = source.InsertRange(0, [null, null]);
        Assert.Same(source, target);

        target = source.InsertRange(0, [".", null]);
        Assert.Same(source, target);

        target = source.InsertRange(0, ["alpha", "beta"]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[alpha]", target[0].Value);
        Assert.Equal("[beta]", target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Equal("[two]", target[3].Value);
        Assert.Equal("[one]", target[4].Value);

        target = source.InsertRange(0, [null, "beta"]);
        Assert.NotSame(source, target);
        Assert.Equal(4, target.Count);
        Assert.Equal("[beta]", target[0].Value);
        Assert.Equal("[one]", target[1].Value);
        Assert.Equal("[two]", target[2].Value);
        Assert.Equal("[one]", target[3].Value);

        target = source.InsertRange(0, ["alpha", null]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[alpha]", target[0].Value);
        Assert.Null(target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Equal("[two]", target[3].Value);
        Assert.Equal("[one]", target[4].Value);

        target = source.InsertRange(3, ["", ""]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Null(target[3].Value);
        Assert.Null(target[4].Value);

        target = source.InsertRange(3, ["", "[alpha.beta]"]);
        Assert.NotSame(source, target);
        Assert.Equal(5, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);
        Assert.Equal("[one]", target[2].Value);
        Assert.Null(target[3].Value);
        Assert.Equal("[alpha.beta]", target[4].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveAt()
    {
        IEngine engine = new FakeEngine();
        var source = new IdentifierChain(engine, "one.two.one");

        var target = source.RemoveAt(2);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);

        target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("[two]", target[0].Value);
        Assert.Equal("[one]", target[1].Value);

        source = new IdentifierChain(engine, "one..one");
        target = source.RemoveAt(1);
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Equal("[one]", target[1].Value);

        target = source.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("[one]", target[0].Value);

        target = target.RemoveAt(0);
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRange()
    {
        IEngine engine = new FakeEngine();
        var source = new IdentifierChain(engine, "one.two.one");

        var target = source.RemoveRange(0, 0);
        Assert.Same(source, target);

        target = source.RemoveRange(0, 3);
        Assert.Empty(target);

        target = source.RemoveRange(1, 2);
        Assert.Single(target);
        Assert.Equal("[one]", target[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Remove()
    {
        IEngine engine = new FakeEngine();
        var source = new IdentifierChain(engine, "one.two.one");

        var target = source.Remove("other");
        Assert.Same(source, target);

        target = source.Remove("ONE");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("[two]", target[0].Value);
        Assert.Equal("[one]", target[1].Value);

        target = source.RemoveLast("ONE");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);

        target = source.RemoveAll("ONE");
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("[two]", target[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemovePredicate()
    {
        IEngine engine = new FakeEngine();
        var source = new IdentifierChain(engine, "one.two.one");

        var target = source.Remove(x => x.Value == null);
        Assert.Same(source, target);

        target = source.Remove(x => x.Value == "[one]");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("[two]", target[0].Value);
        Assert.Equal("[one]", target[1].Value);

        target = source.RemoveLast(x => x.Value == "[one]");
        Assert.NotSame(source, target);
        Assert.Equal(2, target.Count);
        Assert.Equal("[one]", target[0].Value);
        Assert.Equal("[two]", target[1].Value);

        target = source.RemoveAll(x => x.Value == "[one]");
        Assert.NotSame(source, target);
        Assert.Single(target);
        Assert.Equal("[two]", target[0].Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Clear()
    {
        IEngine engine = new FakeEngine();
        var source = new IdentifierChain(engine);

        var target = source.Clear();
        Assert.Same(source, target);

        source = new IdentifierChain(engine, "one.two.one");
        target = source.Clear();
        Assert.NotSame(source, target);
        Assert.Empty(target);
    }
}
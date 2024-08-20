namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Identifier
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        IIdentifier item;
        IIdentifierPart part;
        IIdentifierChain chain;
        var engine = new FakeEngine();

        item = Identifier.Create(engine);
        part = Assert.IsType<IdentifierPart>(item);
        Assert.Null(part.Value);

        item = Identifier.Create(engine, "");
        part = Assert.IsType<IdentifierPart>(item);
        Assert.Null(part.Value);

        item = Identifier.Create(engine, "aa");
        part = Assert.IsType<IdentifierPart>(item);
        Assert.Equal("[aa]", part.Value);

        item = Identifier.Create(engine, "aa.bb");
        chain = Assert.IsType<IdentifierChain>(item);
        Assert.Equal("[aa]", chain[0].Value);
        Assert.Equal("[bb]", chain[1].Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Match_Empty()
    {
        var engine = new FakeEngine();
        var item = new IdentifierChain(engine);

        Assert.True(item.Match(null));
        Assert.True(item.Match(""));
        Assert.True(item.Match(" "));

        Assert.False(item.Match("two"));
        Assert.False(item.Match("two."));
    }

    //[Enforced]
    [Fact]
    public static void Test_Match_Populated_Smaller()
    {
        var engine = new FakeEngine();
        var item = new IdentifierChain(engine, "one");

        Assert.True(item.Match(null));
        Assert.True(item.Match(""));
        Assert.True(item.Match(" "));

        Assert.True(item.Match("one"));
        Assert.True(item.Match(".one"));

        Assert.False(item.Match("two"));
        Assert.False(item.Match("two."));
    }

    //[Enforced]
    [Fact]
    public static void Test_Match_Populated_Bigger()
    {
        var engine = new FakeEngine();
        var item = new IdentifierChain(engine, "two.one");

        Assert.True(item.Match(null));
        Assert.True(item.Match(""));
        Assert.True(item.Match(" "));

        Assert.True(item.Match("one"));
        Assert.True(item.Match(".one"));
        Assert.True(item.Match("two.one"));
        Assert.True(item.Match("two."));

        Assert.False(item.Match("two"));
        Assert.False(item.Match("one."));
    }
}
namespace Yotei.ORM.Core.Tests;

// ========================================================
//[Enforced]
public static class Test_IdentifierSinglePart
{
    //[Enforced]
    [Fact]
    public static void Test_Create_WithTerminators_Empty()
    {
        IIdentifierSinglePart item;
        var engine = new FakeEngine();

        item = new IdentifierSinglePart(engine);
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);

        item = new IdentifierSinglePart(engine, null);
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);

        item = new IdentifierSinglePart(engine, string.Empty);
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);

        item = new IdentifierSinglePart(engine, " ");
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);

        item = new IdentifierSinglePart(engine, " [ ] ");
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);

        item = new IdentifierSinglePart(engine, " [ [ ] ] ");
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_WithTerminators_Populated()
    {
        IIdentifierSinglePart item;
        var engine = new FakeEngine();

        item = new IdentifierSinglePart(engine, "one");
        Assert.Equal("[one]", item.Value);
        Assert.Equal("one", item.NonTerminatedValue);

        try { _ = new IdentifierSinglePart(engine, "one.two"); Assert.Fail(); }
        catch (ArgumentException) { }

        item = new IdentifierSinglePart(engine, "one two");
        Assert.Equal("[one two]", item.Value);
        Assert.Equal("one two", item.NonTerminatedValue);

        item = new IdentifierSinglePart(engine, " [ one ] ");
        Assert.Equal("[one]", item.Value);
        Assert.Equal("one", item.NonTerminatedValue);

        item = new IdentifierSinglePart(engine, " [ [ one ] ] ");
        Assert.Equal("[one]", item.Value);
        Assert.Equal("one", item.NonTerminatedValue);

        item = new IdentifierSinglePart(engine, " [ one.two ] ");
        Assert.Equal("[one.two]", item.Value);
        Assert.Equal("one.two", item.NonTerminatedValue);

        item = new IdentifierSinglePart(engine, " [ one two ] ");
        Assert.Equal("[one two]", item.Value);
        Assert.Equal("one two", item.NonTerminatedValue);

        item = new IdentifierSinglePart(engine, " [ one [ two.three ] ] ");
        Assert.Equal("[one [ two.three ]]", item.Value);
        Assert.Equal("one [ two.three ]", item.NonTerminatedValue);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_NoTerminators_Empty()
    {
        IIdentifierSinglePart item;
        var engine = new FakeEngine { UseTerminators = false };

        item = new IdentifierSinglePart(engine);
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);

        item = new IdentifierSinglePart(engine, null);
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);

        item = new IdentifierSinglePart(engine, string.Empty);
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);

        item = new IdentifierSinglePart(engine, " ");
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);

        try { _ = new IdentifierSinglePart(engine, " [ ] "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new IdentifierSinglePart(engine, " [ [ ] ] "); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_NoTerminators_Populated()
    {
        IIdentifierSinglePart item;
        var engine = new FakeEngine { UseTerminators = false };

        item = new IdentifierSinglePart(engine, "one");
        Assert.Equal("one", item.Value);
        Assert.Equal("one", item.NonTerminatedValue);

        try { _ = new IdentifierSinglePart(engine, "one.two"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new IdentifierSinglePart(engine, "one two"); Assert.Fail(); }
        catch (ArgumentException) { }

        item = new IdentifierSinglePart(engine, "[one]");
        Assert.Equal("[one]", item.Value);
        Assert.Equal("[one]", item.NonTerminatedValue);

        try { _ = new IdentifierSinglePart(engine, "[one two]"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new IdentifierSinglePart(engine, " [ [ one ] ] "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new IdentifierSinglePart(engine, " [ one.two ] "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new IdentifierSinglePart(engine, " [ one two ] "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new IdentifierSinglePart(engine, " [ one [ two.three ] ] "); Assert.Fail(); }
        catch (ArgumentException) { }
    }
}
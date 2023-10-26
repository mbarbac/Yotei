using TItem = Yotei.ORM.Code.IdentifierSinglePart;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_IdentifierSinglePart
{
    //[Enforced]
    [Fact]
    public static void Test_Create_WithTerminators_Empty()
    {
        var engine = new FakeEngine();

        TItem item = new TItem(engine);
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);

        item = new TItem(engine, null);
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);

        item = new TItem(engine, string.Empty);
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);

        item = new TItem(engine, " ");
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);

        item = new TItem(engine, " [ ] ");
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);

        item = new TItem(engine, " [ [ ] ] ");
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_WithTerminators_Populated()
    {
        var engine = new FakeEngine();

        TItem item = new TItem(engine, "one");
        Assert.Equal("[one]", item.Value);
        Assert.Equal("one", item.NonTerminatedValue);

        try { _ = new TItem(engine, "one.two"); Assert.Fail(); }
        catch (ArgumentException) { }

        item = new TItem(engine, "one two");
        Assert.Equal("[one two]", item.Value);
        Assert.Equal("one two", item.NonTerminatedValue);

        item = new TItem(engine, " [ one ] ");
        Assert.Equal("[one]", item.Value);
        Assert.Equal("one", item.NonTerminatedValue);

        item = new TItem(engine, " [ [ one ] ] ");
        Assert.Equal("[one]", item.Value);
        Assert.Equal("one", item.NonTerminatedValue);

        item = new TItem(engine, " [ one.two ] ");
        Assert.Equal("[one.two]", item.Value);
        Assert.Equal("one.two", item.NonTerminatedValue);

        item = new TItem(engine, " [ one two ] ");
        Assert.Equal("[one two]", item.Value);
        Assert.Equal("one two", item.NonTerminatedValue);

        item = new TItem(engine, " [ one [ two.three ] ] ");
        Assert.Equal("[one [ two.three ]]", item.Value);
        Assert.Equal("one [ two.three ]", item.NonTerminatedValue);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Create_NoTerminators_Empty()
    {
        var engine = new FakeEngine { UseTerminators = false };

        TItem item = new TItem(engine);
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);

        item = new TItem(engine, null);
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);

        item = new TItem(engine, string.Empty);
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);

        item = new TItem(engine, " ");
        Assert.Null(item.Value);
        Assert.Null(item.NonTerminatedValue);

        try { _ = new TItem(engine, " [ ] "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new TItem(engine, " [ [ ] ] "); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_NoTerminators_Populated()
    {
        var engine = new FakeEngine { UseTerminators = false };

        TItem item = new TItem(engine, "one");
        Assert.Equal("one", item.Value);
        Assert.Equal("one", item.NonTerminatedValue);

        try { _ = new TItem(engine, "one.two"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new TItem(engine, "one two"); Assert.Fail(); }
        catch (ArgumentException) { }

        item = new TItem(engine, "[one]");
        Assert.Equal("[one]", item.Value);
        Assert.Equal("[one]", item.NonTerminatedValue);

        try { _ = new TItem(engine, "[one two]"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new TItem(engine, " [ [ one ] ] "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new TItem(engine, " [ one.two ] "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new TItem(engine, " [ one two ] "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new TItem(engine, " [ one [ two.three ] ] "); Assert.Fail(); }
        catch (ArgumentException) { }
    }
}
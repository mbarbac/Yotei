namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_IdentifierEx
{
    //[Enforced]
    [Fact]
    public static void Test_GetParts_NoTerminators_Empty()
    {
        List<string?> items;
        var engine = new FakeEngine();

        items = Identifier.GetParts(engine, null, reduce: true); Assert.Empty(items);
        items = Identifier.GetParts(engine, "", reduce: true); Assert.Empty(items);
        items = Identifier.GetParts(engine, "  ", reduce: true); Assert.Empty(items);

        items = Identifier.GetParts(engine, "..", reduce: true); Assert.Empty(items);

        items = Identifier.GetParts(engine, "..");
        Assert.Equal(3, items.Count);
        Assert.Null(items[0]);
        Assert.Null(items[1]);
        Assert.Null(items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetParts_NoTerminators_Populated()
    {
        List<string?> items;
        var engine = new FakeEngine();

        items = Identifier.GetParts(engine, "one.two.three");
        Assert.Equal(3, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Equal("two", items[1]);
        Assert.Equal("three", items[2]);

        items = Identifier.GetParts(engine, " . . one . . three . . ", reduce: true);
        Assert.Equal(5, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Null(items[1]);
        Assert.Equal("three", items[2]);
        Assert.Null(items[3]);
        Assert.Null(items[4]);

        items = Identifier.GetParts(engine, " . . one . . three . . ");
        Assert.Equal(7, items.Count);
        Assert.Null(items[0]);
        Assert.Null(items[1]);
        Assert.Equal("one", items[2]);
        Assert.Null(items[3]);
        Assert.Equal("three", items[4]);
        Assert.Null(items[5]);
        Assert.Null(items[6]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetParts_SameTerminators_Empty()
    {
        List<string?> items;
        var engine = new FakeEngine() { LeftTerminator = '-', RightTerminator = '-' };

        items = Identifier.GetParts(engine, "--", reduce: true); Assert.Empty(items);
        items = Identifier.GetParts(engine, "----", reduce: true); Assert.Empty(items);

        items = Identifier.GetParts(engine, " . -- . - - ", reduce: true); Assert.Empty(items);

        items = Identifier.GetParts(engine, " . -- . - - ");
        Assert.Equal(3, items.Count);
        Assert.Null(items[0]);
        Assert.Null(items[1]);
        Assert.Null(items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetParts_SameTerminators_Populated()
    {
        List<string?> items;
        var engine = new FakeEngine() { LeftTerminator = '-', RightTerminator = '-' };

        items = Identifier.GetParts(engine, " - - one - - ");
        Assert.Single(items);
        Assert.Equal("one", items[0]);

        items = Identifier.GetParts(engine, " - one two - ");
        Assert.Single(items);
        Assert.Equal("one two", items[0]);

        items = Identifier.GetParts(engine, " - one . two - ");
        Assert.Single(items);
        Assert.Equal("one . two", items[0]);

        items = Identifier.GetParts(engine, " - one - . - two - . - three - ");
        Assert.Equal(3, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Equal("two", items[1]);
        Assert.Equal("three", items[2]);

        items = Identifier.GetParts(engine, " . . - one - . . - three - . . ", reduce: true);
        Assert.Equal(5, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Null(items[1]);
        Assert.Equal("three", items[2]);
        Assert.Null(items[3]);
        Assert.Null(items[4]);

        items = Identifier.GetParts(engine, " . . - one - . . - three - . . ");
        Assert.Equal(7, items.Count);
        Assert.Null(items[0]);
        Assert.Null(items[1]);
        Assert.Equal("one", items[2]);
        Assert.Null(items[3]);
        Assert.Equal("three", items[4]);
        Assert.Null(items[5]);
        Assert.Null(items[6]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetParts_DifferentTerminators_Empty()
    {
        List<string?> items;
        var engine = new FakeEngine();

        items = Identifier.GetParts(engine, "[]", reduce: true); Assert.Empty(items);
        items = Identifier.GetParts(engine, "[[]]", reduce: true); Assert.Empty(items);

        items = Identifier.GetParts(engine, " . [] . [ ] ", reduce: true); Assert.Empty(items);

        items = Identifier.GetParts(engine, " . [] . [ ] ");
        Assert.Equal(3, items.Count);
        Assert.Null(items[0]);
        Assert.Null(items[1]);
        Assert.Null(items[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetParts_DifferentTerminators_Populated()
    {
        List<string?> items;
        var engine = new FakeEngine();

        items = Identifier.GetParts(engine, " [ [ one ] ] ");
        Assert.Single(items);
        Assert.Equal("one", items[0]);

        items = Identifier.GetParts(engine, " [ one two ] ");
        Assert.Single(items);
        Assert.Equal("one two", items[0]);

        items = Identifier.GetParts(engine, " [ one . two ] ");
        Assert.Single(items);
        Assert.Equal("one . two", items[0]);

        items = Identifier.GetParts(engine, " [ one ] . [ two ] . [ three ] ");
        Assert.Equal(3, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Equal("two", items[1]);
        Assert.Equal("three", items[2]);

        items = Identifier.GetParts(engine, " . . [ one ] . . [ three ] . . ", reduce: true);
        Assert.Equal(5, items.Count);
        Assert.Equal("one", items[0]);
        Assert.Null(items[1]);
        Assert.Equal("three", items[2]);
        Assert.Null(items[3]);
        Assert.Null(items[4]);

        items = Identifier.GetParts(engine, " . . [ one ] . . [ three ] . . ");
        Assert.Equal(7, items.Count);
        Assert.Null(items[0]);
        Assert.Null(items[1]);
        Assert.Equal("one", items[2]);
        Assert.Null(items[3]);
        Assert.Equal("three", items[4]);
        Assert.Null(items[5]);
        Assert.Null(items[6]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetParts_DifferentTerminators_Complex()
    {
        List<string?> items;
        var engine = new FakeEngine();

        items = Identifier.GetParts(engine, "[one.[two]].[three]");
        Assert.Equal(2, items.Count);
        Assert.Equal("one.[two]", items[0]);
        Assert.Equal("three", items[1]);
    }
}
namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_IdentifierSinglePart
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var item = new IdentifierSinglePart();
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        item = new IdentifierSinglePart(new FakeEngine(), null);
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Terminators()
    {
        var engine = new FakeEngine();
        var item = IdentifierSinglePart.Empty;
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        item = new IdentifierSinglePart(engine, string.Empty);
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        item = new IdentifierSinglePart(engine, " ");
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        item = new IdentifierSinglePart(engine, " one ");
        Assert.Equal("one", item.UnwrappedValue);
        Assert.Equal("[one]", item.Value);

        item = new IdentifierSinglePart(engine, " [ ] ");
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        item = new IdentifierSinglePart(engine, " [ [ ] ] ");
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        item = new IdentifierSinglePart(engine, " [ one two ] ");
        Assert.Equal("one two", item.UnwrappedValue);
        Assert.Equal("[one two]", item.Value);

        item = new IdentifierSinglePart(engine, " [ one.two ] ");
        Assert.Equal("one.two", item.UnwrappedValue);
        Assert.Equal("[one.two]", item.Value);

        item = new IdentifierSinglePart(engine, " [ one [ two three ] ] ");
        Assert.Equal("one [ two three ]", item.UnwrappedValue);
        Assert.Equal("[one [ two three ]]", item.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Same_Terminators()
    {
        var engine = new FakeEngine() { LeftTerminator = '\'', RightTerminator = '\'' };

        var item = new IdentifierSinglePart(engine, "'one'");
        Assert.Equal("one", item.UnwrappedValue);
        Assert.Equal("'one'", item.Value);

        item = new IdentifierSinglePart(engine, "'one.two'");
        Assert.Equal("one.two", item.UnwrappedValue);
        Assert.Equal("'one.two'", item.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_No_Terminators()
    {
        var engine = new FakeEngine() { UseTerminators = false };
        var item = new IdentifierSinglePart(engine, " ");
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        item = new IdentifierSinglePart(engine, " one ");
        Assert.Equal("one", item.UnwrappedValue);
        Assert.Equal("one", item.Value);

        try { _ = new IdentifierSinglePart(engine, " [ ] "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new IdentifierSinglePart(engine, " one two "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new IdentifierSinglePart(engine, " [ one two ] "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new IdentifierSinglePart(engine, "one.two"); Assert.Fail(); }
        catch (ArgumentException) { }
    }
}
namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_IdentifierPart
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var engine = new FakeEngine();
        var item = new Code.IdentifierPart(engine);
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        item = new Code.IdentifierPart(engine, null);
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Terminators()
    {
        var engine = new FakeEngine();
        var item = new Code.IdentifierPart(engine, " ");
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        item = new Code.IdentifierPart(engine, " one ");
        Assert.Equal("one", item.UnwrappedValue);
        Assert.Equal("[one]", item.Value);

        item = new Code.IdentifierPart(engine, " [ ] ");
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        item = new Code.IdentifierPart(engine, " [ [ ] ] ");
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        item = new Code.IdentifierPart(engine, " [ one two ] ");
        Assert.Equal("one two", item.UnwrappedValue);
        Assert.Equal("[one two]", item.Value);

        item = new Code.IdentifierPart(engine, " [ one.two ] ");
        Assert.Equal("one.two", item.UnwrappedValue);
        Assert.Equal("[one.two]", item.Value);

        item = new Code.IdentifierPart(engine, " [ one [ two three ] ] ");
        Assert.Equal("one [ two three ]", item.UnwrappedValue);
        Assert.Equal("[one [ two three ]]", item.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Same_Terminators()
    {
        var engine = new FakeEngine() { LeftTerminator = '\'', RightTerminator = '\'' };

        var item = new Code.IdentifierPart(engine, "'one'");
        Assert.Equal("one", item.UnwrappedValue);
        Assert.Equal("'one'", item.Value);

        item = new Code.IdentifierPart(engine, "'one.two'");
        Assert.Equal("one.two", item.UnwrappedValue);
        Assert.Equal("'one.two'", item.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_No_Terminators()
    {
        var engine = new FakeEngine() { UseTerminators = false };
        var item = new Code.IdentifierPart(engine, " ");
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        item = new Code.IdentifierPart(engine, " one ");
        Assert.Equal("one", item.UnwrappedValue);
        Assert.Equal("one", item.Value);

        try { _ = new Code.IdentifierPart(engine, " [ ] "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Code.IdentifierPart(engine, " one two "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Code.IdentifierPart(engine, " [ one two ] "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Code.IdentifierPart(engine, "one.two"); Assert.Fail(); }
        catch (ArgumentException) { }
    }
}
namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_IdentifierPart
{
    //[Enforced]
    [Fact]
    public static void Test_Create_With_Terminators()
    {
        var engine = new FakeEngine();
        var item = new IdentifierPart(engine);
        Assert.Null(item.Value);
        Assert.Null(item.UnwrappedValue);

        item = new IdentifierPart(engine, null);
        Assert.Null(item.Value);
        Assert.Null(item.UnwrappedValue);

        item = new IdentifierPart(engine, " ");
        Assert.Null(item.Value);
        Assert.Null(item.UnwrappedValue);
        
        item = new IdentifierPart(engine, " [ ] ");
        Assert.Null(item.Value);
        Assert.Null(item.UnwrappedValue);

        item = new IdentifierPart(engine, " [ [ ] ] ");
        Assert.Null(item.Value);
        Assert.Null(item.UnwrappedValue);

        item = new IdentifierPart(engine, " one ");
        Assert.Equal("[one]", item.Value);
        Assert.Equal("one", item.UnwrappedValue);

        item = new IdentifierPart(engine, " [ one ] ");
        Assert.Equal("[one]", item.Value);
        Assert.Equal("one", item.UnwrappedValue);

        item = new IdentifierPart(engine, " [ one two ] ");
        Assert.Equal("[one two]", item.Value);
        Assert.Equal("one two", item.UnwrappedValue);

        item = new IdentifierPart(engine, " [ one.two ] ");
        Assert.Equal("[one.two]", item.Value);
        Assert.Equal("one.two", item.UnwrappedValue);

        item = new IdentifierPart(engine, " [ one [ two three ] ] ");
        Assert.Equal("[one [ two three ]]", item.Value);
        Assert.Equal("one [ two three ]", item.UnwrappedValue);

        try { _ = new IdentifierPart(engine, "one.two"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new IdentifierPart(engine, "one two"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_With_Same_Terminators()
    {
        var ch = '\'';
        var engine = new FakeEngine() { LeftTerminator = ch, RightTerminator = ch };
        var item = new IdentifierPart(engine);
        Assert.Null(item.Value);
        Assert.Null(item.UnwrappedValue);

        item = new IdentifierPart(engine, null);
        Assert.Null(item.Value);
        Assert.Null(item.UnwrappedValue);

        item = new IdentifierPart(engine, " ");
        Assert.Null(item.Value);
        Assert.Null(item.UnwrappedValue);

        item = new IdentifierPart(engine, " ' ' ");
        Assert.Null(item.Value);
        Assert.Null(item.UnwrappedValue);

        try { _ = new IdentifierPart(engine, " ' "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new IdentifierPart(engine, " ' ' ' "); Assert.Fail(); }
        catch (ArgumentException) { }

        item = new IdentifierPart(engine, " one ");
        Assert.Equal("'one'", item.Value);
        Assert.Equal("one", item.UnwrappedValue);

        item = new IdentifierPart(engine, " ' one ' ");
        Assert.Equal("'one'", item.Value);
        Assert.Equal("one", item.UnwrappedValue);

        item = new IdentifierPart(engine, " ' one two ' ");
        Assert.Equal("'one two'", item.Value);
        Assert.Equal("one two", item.UnwrappedValue);

        item = new IdentifierPart(engine, " ' one.two ' ");
        Assert.Equal("'one.two'", item.Value);
        Assert.Equal("one.two", item.UnwrappedValue);

        try { new IdentifierPart(engine, "one.two"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { new IdentifierPart(engine, "one two"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_No_Terminators()
    {
        var engine = new FakeEngine() { UseTerminators = false };
        var item = new IdentifierPart(engine);
        Assert.Null(item.Value);
        Assert.Null(item.UnwrappedValue);

        item = new IdentifierPart(engine, null);
        Assert.Null(item.Value);
        Assert.Null(item.UnwrappedValue);

        item = new IdentifierPart(engine, " ");
        Assert.Null(item.Value);
        Assert.Null(item.UnwrappedValue);

        item = new IdentifierPart(engine, " one ");
        Assert.Equal("one", item.Value);
        Assert.Equal("one", item.UnwrappedValue);

        try { _ = new IdentifierPart(engine, " [ ] "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new IdentifierPart(engine, " one two "); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new IdentifierPart(engine, " one.two "); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Equality()
    {
        var source = new IdentifierPart(new FakeEngine(), " one ");
        var target = new IdentifierPart(new FakeEngine(), " [ ONE ] ");
        Assert.Equal(source, target);
    }
}
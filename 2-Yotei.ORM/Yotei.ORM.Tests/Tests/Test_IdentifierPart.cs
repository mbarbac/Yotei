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
    public static void Test_Equals()
    {
        var source = new IdentifierPart(new FakeEngine());
        var target = new IdentifierPart(new FakeEngine());
        Assert.Equal(source, target);

        source = new IdentifierPart(new FakeEngine(), " one ");
        target = new IdentifierPart(new FakeEngine(), " [ ONE ] ");
        Assert.Equal(source, target);

        target = new IdentifierPart(new FakeEngine(), "two");
        Assert.NotEqual(source, target);

        var chain = new IdentifierChain(new FakeEngine(), "one");
        Assert.True(source.Equals(chain));

        chain = new IdentifierChain(new FakeEngine(), "..one");
        Assert.True(source.Equals(chain));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Match_Empty()
    {
        var engine = new FakeEngine();
        var source = new IdentifierPart(engine);

        Assert.True(source.Match(null));
        Assert.True(source.Match(""));
        Assert.True(source.Match(" "));

        Assert.False(source.Match("two"));
        Assert.False(source.Match("two."));
    }

    //[Enforced]
    [Fact]
    public static void Test_Match_Populated()
    {
        var engine = new FakeEngine();
        var source = new IdentifierPart(engine, "one");

        Assert.True(source.Match(null));
        Assert.True(source.Match(""));
        Assert.True(source.Match(" "));

        Assert.True(source.Match("one"));
        Assert.True(source.Match("[ONE]"));
        Assert.False(source.Match("two"));

        Assert.True(source.Match(".one"));
        Assert.False(source.Match("two.one"));
        Assert.False(source.Match("two."));
    }
}
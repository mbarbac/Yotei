namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_IdentifierUnit
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        IIdentifierUnit item;
        var engine = new FakeEngine();

        item = new IdentifierUnit(engine, null);
        Assert.Null(item.RawValue);
        Assert.Null(item.Value);

        item = new IdentifierUnit(engine, "");
        Assert.Null(item.RawValue);
        Assert.Null(item.Value);

        try { item = new IdentifierUnit(engine, " "); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Same_Terminators()
    {
        IIdentifierUnit item;
        var engine = new FakeEngine() { LeftTerminator = '\'', RightTerminator = '\'' };

        item = new IdentifierUnit(engine, "''");
        Assert.Null(item.RawValue);
        Assert.Null(item.Value);

        item = new IdentifierUnit(engine, ".");
        Assert.Null(item.RawValue);
        Assert.Null(item.Value);

        item = new IdentifierUnit(engine, "''.''"); // Empty contents in each part
        Assert.Null(item.RawValue);
        Assert.Null(item.Value);

        item = new IdentifierUnit(engine, "aa");
        Assert.Equal("aa", item.RawValue);
        Assert.Equal("'aa'", item.Value);

        item = new IdentifierUnit(engine, "'aa'");
        Assert.Equal("aa", item.RawValue);
        Assert.Equal("'aa'", item.Value);

        item = new IdentifierUnit(engine, "'a.a'"); // Embedded dots are protected
        Assert.Equal("a.a", item.RawValue);
        Assert.Equal("'a.a'", item.Value);

        item = new IdentifierUnit(engine, "'a a'"); // Embedded spaces are protected
        Assert.Equal("a a", item.RawValue);
        Assert.Equal("'a a'", item.Value);

        try { item = new IdentifierUnit(engine, "'"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { item = new IdentifierUnit(engine, "' '"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Different_Terminators()
    {
        IIdentifierUnit item;
        var engine = new FakeEngine();

        item = new IdentifierUnit(engine, "[]");
        Assert.Null(item.RawValue);
        Assert.Null(item.Value);

        item = new IdentifierUnit(engine, ".");
        Assert.Null(item.RawValue);
        Assert.Null(item.Value);

        item = new IdentifierUnit(engine, "[].[]"); // Empty contents in each part
        Assert.Null(item.RawValue);
        Assert.Null(item.Value);

        item = new IdentifierUnit(engine, "aa");
        Assert.Equal("aa", item.RawValue);
        Assert.Equal("[aa]", item.Value);

        item = new IdentifierUnit(engine, "[aa]");
        Assert.Equal("aa", item.RawValue);
        Assert.Equal("[aa]", item.Value);

        item = new IdentifierUnit(engine, "[[aa]]");
        Assert.Equal("aa", item.RawValue);
        Assert.Equal("[aa]", item.Value);

        item = new IdentifierUnit(engine, "[a.a]"); // Embedded dots are protected
        Assert.Equal("a.a", item.RawValue);
        Assert.Equal("[a.a]", item.Value);

        item = new IdentifierUnit(engine, "[a a]"); // Embedded spaces are protected
        Assert.Equal("a a", item.RawValue);
        Assert.Equal("[a a]", item.Value);

        try { item = new IdentifierUnit(engine, "["); Assert.Fail(); }
        catch (ArgumentException) { }

        try { item = new IdentifierUnit(engine, "]"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { item = new IdentifierUnit(engine, "[ ]"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Match_Empty()
    {
        var engine = new FakeEngine();
        var item = new IdentifierUnit(engine);

        Assert.False(item.Match("other"));
        Assert.True(item.Match(null));
    }

    //[Enforced]
    [Fact]
    public static void Test_Match_Populated()
    {
        var engine = new FakeEngine();
        var item = new IdentifierUnit(engine, "one");

        Assert.False(item.Match("other"));

        Assert.True(item.Match("one"));
        Assert.True(item.Match("ONE"));
        Assert.True(item.Match(null));
    }
}
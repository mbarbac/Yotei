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

        Assert.True(Identifier.Match(item, null));
        Assert.True(Identifier.Match(item, ""));
        Assert.True(Identifier.Match(item, " "));

        Assert.False(Identifier.Match(item, "two"));
        Assert.False(Identifier.Match(item, "two."));
    }

    //[Enforced]
    [Fact]
    public static void Test_Match_Populated_Smaller()
    {
        var engine = new FakeEngine();
        var item = new IdentifierChain(engine, "one");

        Assert.True(Identifier.Match(item, null));
        Assert.True(Identifier.Match(item, ""));
        Assert.True(Identifier.Match(item, " "));

        Assert.True(Identifier.Match(item, "one"));
        Assert.True(Identifier.Match(item, ".one"));

        Assert.False(Identifier.Match(item, "two"));
        Assert.False(Identifier.Match(item, "two."));
    }

    //[Enforced]
    [Fact]
    public static void Test_Match_Populated_Bigger()
    {
        var engine = new FakeEngine();
        var item = new IdentifierChain(engine, "two.one");

        Assert.True(Identifier.Match(item, null));
        Assert.True(Identifier.Match(item, ""));
        Assert.True(Identifier.Match(item, " "));

        Assert.True(Identifier.Match(item, "one"));
        Assert.True(Identifier.Match(item, ".one"));
        Assert.True(Identifier.Match(item, "two.one"));
        Assert.True(Identifier.Match(item, "two."));

        Assert.False(Identifier.Match(item, "two"));
        Assert.False(Identifier.Match(item, "one."));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetParts_NoTerminators()
    {
        string? value;
        List<string?> parts;
        var engine = new FakeEngine() { UseTerminators = false };

        value = null;
        parts = Identifier.GetParts(engine, value);
        Assert.Empty(parts);

        value = string.Empty;
        parts = Identifier.GetParts(engine, value);
        Assert.Empty(parts);

        value = ".";
        parts = Identifier.GetParts(engine, value);
        Assert.Empty(parts);

        value = ".two";
        parts = Identifier.GetParts(engine, value);
        Assert.Single(parts);
        Assert.Equal("two", parts[0]);

        value = "one..three.";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(4, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Null(parts[1]);
        Assert.Equal("three", parts[2]);
        Assert.Null(parts[3]);

        value = " ";
        try { parts = Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }

        value = "one. ";
        try { parts = Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }

        value = " .two";
        try { parts = Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_GetParts_WithTerminators_NotUsed()
    {
        string? value;
        List<string?> parts;
        var engine = new FakeEngine();

        value = null;
        parts = Identifier.GetParts(engine, value);
        Assert.Empty(parts);

        value = string.Empty;
        parts = Identifier.GetParts(engine, value);
        Assert.Empty(parts);

        value = ".";
        parts = Identifier.GetParts(engine, value);
        Assert.Empty(parts);

        value = ".two";
        parts = Identifier.GetParts(engine, value);
        Assert.Single(parts);
        Assert.Equal("two", parts[0]);

        value = "one..three.";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(4, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Null(parts[1]);
        Assert.Equal("three", parts[2]);
        Assert.Null(parts[3]);

        value = " ";
        try { parts = Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }

        value = "one. ";
        try { parts = Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }

        value = " .two";
        try { parts = Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_GetParts_WithTerminators_SingleWrapped()
    {
        string? value;
        List<string?> parts;
        var engine = new FakeEngine();

        value = "[]";
        parts = Identifier.GetParts(engine, value);
        Assert.Empty(parts);

        value = "[[]]";
        parts = Identifier.GetParts(engine, value);
        Assert.Empty(parts);

        value = "[one]";
        parts = Identifier.GetParts(engine, value);
        Assert.Single(parts);
        Assert.Equal("one", parts[0]);

        value = "[[one]]";
        parts = Identifier.GetParts(engine, value);
        Assert.Single(parts);
        Assert.Equal("one", parts[0]);

        value = "[one xtra]";
        parts = Identifier.GetParts(engine, value);
        Assert.Single(parts);
        Assert.Equal("one xtra", parts[0]);

        value = "[one.xtra]";
        parts = Identifier.GetParts(engine, value);
        Assert.Single(parts);
        Assert.Equal("one.xtra", parts[0]);

        value = "[ one ]";
        try { parts = Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }

        value = "[.one.]";
        try { parts = Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_GetParts_WithTerminators_Chained()
    {
        string? value;
        List<string?> parts;
        var engine = new FakeEngine();

        value = ".";
        parts = Identifier.GetParts(engine, value);
        Assert.Empty(parts);

        value = "[].[]";
        parts = Identifier.GetParts(engine, value);
        Assert.Empty(parts);

        value = ".[two]";
        parts = Identifier.GetParts(engine, value);
        Assert.Single(parts);
        Assert.Equal("two", parts[0]);

        value = "..[three]";
        parts = Identifier.GetParts(engine, value);
        Assert.Single(parts);
        Assert.Equal("three", parts[0]);

        value = "...[four]";
        parts = Identifier.GetParts(engine, value);
        Assert.Single(parts);
        Assert.Equal("four", parts[0]);

        value = "one.[two]";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(2, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Equal("two", parts[1]);

        value = "[one].[two]";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(2, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Equal("two", parts[1]);

        value = "[one].";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(2, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Null(parts[1]);

        value = "[one]....";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(5, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Null(parts[1]);
        Assert.Null(parts[2]);
        Assert.Null(parts[3]);
        Assert.Null(parts[4]);

        value = "[one]..three..";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(5, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Null(parts[1]);
        Assert.Equal("three", parts[2]);
        Assert.Null(parts[3]);
        Assert.Null(parts[4]);

        value = "one.two..[four]";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(4, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Equal("two", parts[1]);
        Assert.Null(parts[2]);
        Assert.Equal("four", parts[3]);

        value = "one.[two other]";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(2, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Equal("two other", parts[1]);

        value = "one.[two.other]";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(2, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Equal("two.other", parts[1]);

        value = "[][]";
        try { parts = Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }

        value = "[one.[two]]";
        try { parts = Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }
    }
}
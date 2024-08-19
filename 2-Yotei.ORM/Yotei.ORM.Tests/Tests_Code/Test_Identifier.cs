namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Identifier
{
    //[Enforced]
    [Fact]
    public static void Test_GetParts_NoTerminators()
    {
        string? value;
        List<string?> parts;
        var engine = new FakeEngine() { UseTerminators = false };

        value = null;
        parts = engine.GetParts(value);
        Assert.Empty(parts);

        value = string.Empty;
        parts = engine.GetParts(value);
        Assert.Empty(parts);

        value = ".";
        parts = engine.GetParts(value);
        Assert.Empty(parts);

        value = ".two";
        parts = engine.GetParts(value);
        Assert.Single(parts);
        Assert.Equal("two", parts[0]);

        value = "one..three.";
        parts = engine.GetParts(value);
        Assert.Equal(4, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Null(parts[1]);
        Assert.Equal("three", parts[2]);
        Assert.Null(parts[3]);

        value = " ";
        try { parts = engine.GetParts(value); Assert.Fail(); }
        catch (ArgumentException) { }

        value = "one. ";
        try { parts = engine.GetParts(value); Assert.Fail(); }
        catch (ArgumentException) { }

        value = " .two";
        try { parts = engine.GetParts(value); Assert.Fail(); }
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
        parts = engine.GetParts(value);
        Assert.Empty(parts);

        value = string.Empty;
        parts = engine.GetParts(value);
        Assert.Empty(parts);

        value = ".";
        parts = engine.GetParts(value);
        Assert.Empty(parts);

        value = ".two";
        parts = engine.GetParts(value);
        Assert.Single(parts);
        Assert.Equal("two", parts[0]);

        value = "one..three.";
        parts = engine.GetParts(value);
        Assert.Equal(4, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Null(parts[1]);
        Assert.Equal("three", parts[2]);
        Assert.Null(parts[3]);

        value = " ";
        try { parts = engine.GetParts(value); Assert.Fail(); }
        catch (ArgumentException) { }

        value = "one. ";
        try { parts = engine.GetParts(value); Assert.Fail(); }
        catch (ArgumentException) { }

        value = " .two";
        try { parts = engine.GetParts(value); Assert.Fail(); }
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
        parts = engine.GetParts(value);
        Assert.Empty(parts);

        value = "[[]]";
        parts = engine.GetParts(value);
        Assert.Empty(parts);

        value = "[one]";
        parts = engine.GetParts(value);
        Assert.Single(parts);
        Assert.Equal("one", parts[0]);

        value = "[[one]]";
        parts = engine.GetParts(value);
        Assert.Single(parts);
        Assert.Equal("one", parts[0]);

        value = "[one xtra]";
        parts = engine.GetParts(value);
        Assert.Single(parts);
        Assert.Equal("one xtra", parts[0]);

        value = "[one.xtra]";
        parts = engine.GetParts(value);
        Assert.Single(parts);
        Assert.Equal("one.xtra", parts[0]);

        value = "[ one ]";
        try { parts = engine.GetParts(value); Assert.Fail(); }
        catch (ArgumentException) { }

        value = "[.one.]";
        try { parts = engine.GetParts(value); Assert.Fail(); }
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
        parts = engine.GetParts(value);
        Assert.Empty(parts);

        value = "[].[]";
        parts = engine.GetParts(value);
        Assert.Empty(parts);

        value = ".[two]";
        parts = engine.GetParts(value);
        Assert.Single(parts);
        Assert.Equal("two", parts[0]);

        value = "..[three]";
        parts = engine.GetParts(value);
        Assert.Single(parts);
        Assert.Equal("three", parts[0]);

        value = "...[four]";
        parts = engine.GetParts(value);
        Assert.Single(parts);
        Assert.Equal("four", parts[0]);

        value = "one.[two]";
        parts = engine.GetParts(value);
        Assert.Equal(2, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Equal("two", parts[1]);

        value = "[one].[two]";
        parts = engine.GetParts(value);
        Assert.Equal(2, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Equal("two", parts[1]);

        value = "[one].";
        parts = engine.GetParts(value);
        Assert.Equal(2, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Null(parts[1]);

        value = "[one]....";
        parts = engine.GetParts(value);
        Assert.Equal(5, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Null(parts[1]);
        Assert.Null(parts[2]);
        Assert.Null(parts[3]);
        Assert.Null(parts[4]);

        value = "[one]..three..";
        parts = engine.GetParts(value);
        Assert.Equal(5, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Null(parts[1]);
        Assert.Equal("three", parts[2]);
        Assert.Null(parts[3]);
        Assert.Null(parts[4]);

        value = "one.two..[four]";
        parts = engine.GetParts(value);
        Assert.Equal(4, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Equal("two", parts[1]);
        Assert.Null(parts[2]);
        Assert.Equal("four", parts[3]);

        value = "one.[two other]";
        parts = engine.GetParts(value);
        Assert.Equal(2, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Equal("two other", parts[1]);

        value = "one.[two.other]";
        parts = engine.GetParts(value);
        Assert.Equal(2, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Equal("two.other", parts[1]);

        value = "[][]";
        try { parts = engine.GetParts(value); Assert.Fail(); }
        catch (ArgumentException) { }

        value = "[one.[two]]";
        try { parts = engine.GetParts(value); Assert.Fail(); }
        catch (ArgumentException) { }
    }
}
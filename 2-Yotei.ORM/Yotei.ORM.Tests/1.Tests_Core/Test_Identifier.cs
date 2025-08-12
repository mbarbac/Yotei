namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Identifier
{
    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetParts_NoTerminators_Empty()
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

        value = "..";
        parts = Identifier.GetParts(engine, value);
        Assert.Empty(parts);

        parts = Identifier.GetParts(engine, value, reduce: false); // Not-empty reduce=false
        Assert.Equal(3, parts.Count);
        Assert.Null(parts[0]);
        Assert.Null(parts[1]);
        Assert.Null(parts[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetParts_NoTerminators_Populated()
    {
        string? value;
        List<string?> parts;
        var engine = new FakeEngine() { UseTerminators = false };

        value = "one.two.three";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(3, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Equal("two", parts[1]);
        Assert.Equal("three", parts[2]);

        value = ".two.three";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(2, parts.Count);
        Assert.Equal("two", parts[0]);
        Assert.Equal("three", parts[1]);

        value = "..three";
        parts = Identifier.GetParts(engine, value);
        Assert.Single(parts);
        Assert.Equal("three", parts[0]);

        parts = Identifier.GetParts(engine, value, reduce: false);
        Assert.Equal(3, parts.Count);
        Assert.Null(parts[0]);
        Assert.Null(parts[1]);
        Assert.Equal("three", parts[2]);

        value = "one.two.";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(3, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Equal("two", parts[1]);
        Assert.Null(parts[2]);

        value = "one..";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(3, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Null(parts[1]);
        Assert.Null(parts[2]);

        value = "one..three";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(3, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Null(parts[1]);
        Assert.Equal("three", parts[2]);

        value = " ";
        try { Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }

        value = "one. ";
        try { Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }

        value = " .two";
        try { Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_GetParts_WithTerminators_Empty()
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

        value = "..";
        parts = Identifier.GetParts(engine, value);
        Assert.Empty(parts);

        parts = Identifier.GetParts(engine, value, reduce: false); // Not-empty because reduce=false
        Assert.Equal(3, parts.Count);
        Assert.Null(parts[0]);
        Assert.Null(parts[1]);
        Assert.Null(parts[2]);
    }

    //[Enforced]
    [Fact]
    public static void Test_GetParts_WithTerminators_Populated_NotUsed()
    {
        string? value;
        List<string?> parts;
        var engine = new FakeEngine();

        value = "one.two.three";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(3, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Equal("two", parts[1]);
        Assert.Equal("three", parts[2]);

        value = ".two.three";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(2, parts.Count);
        Assert.Equal("two", parts[0]);
        Assert.Equal("three", parts[1]);

        value = "..three";
        parts = Identifier.GetParts(engine, value);
        Assert.Single(parts);
        Assert.Equal("three", parts[0]);

        parts = Identifier.GetParts(engine, value, reduce: false);
        Assert.Equal(3, parts.Count);
        Assert.Null(parts[0]);
        Assert.Null(parts[1]);
        Assert.Equal("three", parts[2]);

        value = "one.two.";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(3, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Equal("two", parts[1]);
        Assert.Null(parts[2]);

        value = "one..";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(3, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Null(parts[1]);
        Assert.Null(parts[2]);

        value = "one..three";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(3, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Null(parts[1]);
        Assert.Equal("three", parts[2]);

        value = " ";
        try { Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }

        value = "one. ";
        try { Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }

        value = " .two";
        try { Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_GetParts_WithTerminators_Populated_Used_Single()
    {
        string? value;
        List<string?> parts;
        var engine = new FakeEngine();

        value = "[]";
        parts = Identifier.GetParts(engine, value);
        Assert.Empty(parts);

        parts = Identifier.GetParts(engine, value, reduce: false);
        Assert.Single(parts);
        Assert.Null(parts[0]);

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

        value = "[one two]";
        parts = Identifier.GetParts(engine, value);
        Assert.Single(parts);
        Assert.Equal("one two", parts[0]);

        value = "[one.two]";
        parts = Identifier.GetParts(engine, value);
        Assert.Single(parts);
        Assert.Equal("one.two", parts[0]);

        value = "[ one]";
        try { Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }

        value = "[one ]";
        try { Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }

        value = "[.one]";
        try { Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }

        value = "[one.]";
        try { Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_GetParts_WithTerminators_Populated_Used_Chained()
    {
        string? value;
        List<string?> parts;
        var engine = new FakeEngine();

        value = "[].[]";
        parts = Identifier.GetParts(engine, value);
        Assert.Empty(parts);

        value = "[one].[two].[three]";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(3, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Equal("two", parts[1]);
        Assert.Equal("three", parts[2]);

        value = ".[two].[three]";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(2, parts.Count);
        Assert.Equal("two", parts[0]);
        Assert.Equal("three", parts[1]);

        value = "..[three]";
        parts = Identifier.GetParts(engine, value);
        Assert.Single(parts);
        Assert.Equal("three", parts[0]);

        parts = Identifier.GetParts(engine, value, reduce: false);
        Assert.Equal(3, parts.Count);
        Assert.Null(parts[0]);
        Assert.Null(parts[1]);
        Assert.Equal("three", parts[2]);

        value = "[one].[two].";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(3, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Equal("two", parts[1]);
        Assert.Null(parts[2]);

        value = "[one]..";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(3, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Null(parts[1]);
        Assert.Null(parts[2]);

        value = "[one]..[three]";
        parts = Identifier.GetParts(engine, value);
        Assert.Equal(3, parts.Count);
        Assert.Equal("one", parts[0]);
        Assert.Null(parts[1]);
        Assert.Equal("three", parts[2]);

        value = "[][]";
        try { Identifier.GetParts(engine, value); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_GetParts_WithTerminators_Populated_Used_Embedded()
    {
        string? value;
        List<string?> parts;
        var engine = new FakeEngine();

        value = "[one.[two]]";
        parts = Identifier.GetParts(engine, value);
        Assert.Single(parts);
        Assert.Equal("one.[two]", parts[0]);
    }
}
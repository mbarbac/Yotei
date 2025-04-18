#pragma warning disable IDE0079
#pragma warning disable CA1859

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public static class Test_IdentifierPart
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        IIdentifierPart item;
        var engine = new FakeEngine();

        item = new IdentifierPart(engine, null);
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        item = new IdentifierPart(engine, "");
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        try { item = new IdentifierPart(engine, " "); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Same_Terminators()
    {
        IIdentifierPart item;
        var engine = new FakeEngine() { LeftTerminator = '\'', RightTerminator = '\'' };

        item = new IdentifierPart(engine, "''");
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        item = new IdentifierPart(engine, ".");
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        item = new IdentifierPart(engine, "''.''");
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        item = new IdentifierPart(engine, "aa");
        Assert.Equal("aa", item.UnwrappedValue);
        Assert.Equal("'aa'", item.Value);

        item = new IdentifierPart(engine, "'aa'");
        Assert.Equal("aa", item.UnwrappedValue);
        Assert.Equal("'aa'", item.Value);

        item = new IdentifierPart(engine, "'a.a'"); // Embedded dots are protected
        Assert.Equal("a.a", item.UnwrappedValue);
        Assert.Equal("'a.a'", item.Value);

        item = new IdentifierPart(engine, "'a a'"); // Embedded spaces are protected
        Assert.Equal("a a", item.UnwrappedValue);
        Assert.Equal("'a a'", item.Value);

        try { item = new IdentifierPart(engine, "'"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { item = new IdentifierPart(engine, "' '"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Different_Terminators()
    {
        IIdentifierPart item;
        var engine = new FakeEngine();

        item = new IdentifierPart(engine, "[]");
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        item = new IdentifierPart(engine, ".");
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        item = new IdentifierPart(engine, "[].[]");
        Assert.Null(item.UnwrappedValue);
        Assert.Null(item.Value);

        item = new IdentifierPart(engine, "aa");
        Assert.Equal("aa", item.UnwrappedValue);
        Assert.Equal("[aa]", item.Value);

        item = new IdentifierPart(engine, "[aa]");
        Assert.Equal("aa", item.UnwrappedValue);
        Assert.Equal("[aa]", item.Value);

        item = new IdentifierPart(engine, "[[aa]]");
        Assert.Equal("aa", item.UnwrappedValue);
        Assert.Equal("[aa]", item.Value);

        item = new IdentifierPart(engine, "[a.a]"); // Embedded dots are protected
        Assert.Equal("a.a", item.UnwrappedValue);
        Assert.Equal("[a.a]", item.Value);

        item = new IdentifierPart(engine, "[a a]"); // Embedded spaces are protected
        Assert.Equal("a a", item.UnwrappedValue);
        Assert.Equal("[a a]", item.Value);

        try { item = new IdentifierPart(engine, "["); Assert.Fail(); }
        catch (ArgumentException) { }

        try { item = new IdentifierPart(engine, "]"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { item = new IdentifierPart(engine, "[ ]"); Assert.Fail(); }
        catch (ArgumentException) { }
    }
}
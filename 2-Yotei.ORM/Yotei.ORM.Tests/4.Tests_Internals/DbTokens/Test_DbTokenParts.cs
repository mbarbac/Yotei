using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_DbTokenParts
{
    //[Enforced]
    [Fact]
    public static void Test_ExtractParts_NoHead_NoTail()
    {
        var engine = new FakeEngine();
        IDbToken source;
        DbTokenParts parts;

        source = DbLambdaParser.Parse(engine, x => x);
        parts = source.ExtractParts();
        Assert.Null(parts.Head);
        Assert.Same(source, parts.Body);
        Assert.Null(parts.Tail);

        source = DbLambdaParser.Parse(engine, x => x.b);
        parts = source.ExtractParts();
        Assert.Null(parts.Head);
        Assert.Same(source, parts.Body);
        Assert.Null(parts.Tail);

        source = DbLambdaParser.Parse(engine, x => x.b1.b2);
        parts = source.ExtractParts();
        Assert.Null(parts.Head);
        Assert.Same(source, parts.Body);
        Assert.Null(parts.Tail);

        source = DbLambdaParser.Parse(engine, x => x.b1.x("any").b2);
        parts = source.ExtractParts();
        Assert.Null(parts.Head);
        Assert.Same(source, parts.Body);
        Assert.Null(parts.Tail);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractParts_All_Invokes()
    {
        var engine = new FakeEngine();
        IDbToken source;
        DbTokenParts parts;

        source = DbLambdaParser.Parse(engine, x => x()()());
        parts = source.ExtractParts();
        Assert.IsType<DbTokenInvoke>(parts.Head); Assert.Equal("x()", parts.Head.ToString());
        Assert.IsType<DbTokenArgument>(parts.Body); Assert.Equal("x", parts.Body.ToString());
        Assert.Null(parts.Tail);

        source = DbLambdaParser.Parse(engine, x => x("h1")("h2"));
        parts = source.ExtractParts();
        Assert.IsType<DbTokenInvoke>(parts.Head); Assert.Equal("x(h1, h2)", parts.Head.ToString());
        Assert.IsType<DbTokenArgument>(parts.Body); Assert.Equal("x", parts.Body.ToString());
        Assert.Null(parts.Tail);

        source = DbLambdaParser.Parse(engine, x => x.x("h").x().x("t"));
        parts = source.ExtractParts();
        Assert.IsType<DbTokenInvoke>(parts.Head); Assert.Equal("x(h, t)", parts.Head.ToString());
        Assert.IsType<DbTokenArgument>(parts.Body); Assert.Equal("x", parts.Body.ToString());
        Assert.Null(parts.Tail);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractParts_With_Head()
    {
        var engine = new FakeEngine();
        IDbToken source;
        DbTokenParts parts;

        source = DbLambdaParser.Parse(engine, x => x("h")); // Head takes precedence...
        parts = source.ExtractParts();
        Assert.IsType<DbTokenInvoke>(parts.Head); Assert.Equal("x(h)", parts.Head.ToString());
        Assert.IsType<DbTokenArgument>(parts.Body); Assert.Equal("x", parts.Body.ToString());
        Assert.Null(parts.Tail);

        source = DbLambdaParser.Parse(engine, x => x("h").b);
        parts = source.ExtractParts();
        Assert.IsType<DbTokenInvoke>(parts.Head); Assert.Equal("x(h)", parts.Head.ToString());
        Assert.IsType<DbTokenIdentifier>(parts.Body); Assert.Equal("x.[b]", parts.Body.ToString());
        Assert.Null(parts.Tail);

        source = DbLambdaParser.Parse(engine, x => x("h1").x("h2").b1.b2);
        parts = source.ExtractParts();
        Assert.IsType<DbTokenInvoke>(parts.Head); Assert.Equal("x(h1, h2)", parts.Head.ToString());
        Assert.IsType<DbTokenIdentifier>(parts.Body); Assert.Equal("x.[b1].[b2]", parts.Body.ToString());
        Assert.Null(parts.Tail);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractParts_With_Tail()
    {
        var engine = new FakeEngine();
        IDbToken source;
        DbTokenParts parts;

        source = DbLambdaParser.Parse(engine, x => x.b.x("t1")("t2"));
        parts = source.ExtractParts();
        Assert.Null(parts.Head);
        Assert.IsType<DbTokenIdentifier>(parts.Body); Assert.Equal("x.[b]", parts.Body.ToString());
        Assert.IsType<DbTokenInvoke>(parts.Tail); Assert.Equal("x(t1, t2)", parts.Tail.ToString());

        source = DbLambdaParser.Parse(engine, x => x.b1.b2.x("t1").x("t2"));
        parts = source.ExtractParts();
        Assert.Null(parts.Head);
        Assert.IsType<DbTokenIdentifier>(parts.Body); Assert.Equal("x.[b1].[b2]", parts.Body.ToString());
        Assert.IsType<DbTokenInvoke>(parts.Tail); Assert.Equal("x(t1, t2)", parts.Tail.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractParts_With_Head_And_Tail()
    {
        var engine = new FakeEngine();
        IDbToken source;
        DbTokenParts parts;

        source = DbLambdaParser.Parse(engine, x => x("h").b.x("t"));
        parts = source.ExtractParts();
        Assert.IsType<DbTokenInvoke>(parts.Head); Assert.Equal("x(h)", parts.Head.ToString());
        Assert.IsType<DbTokenIdentifier>(parts.Body); Assert.Equal("x.[b]", parts.Body.ToString());
        Assert.IsType<DbTokenInvoke>(parts.Tail); Assert.Equal("x(t)", parts.Tail.ToString());

        source = DbLambdaParser.Parse(engine, x => x("h1")("h2").b1.x("any").b2.x("t1")("t2"));
        parts = source.ExtractParts();
        Assert.IsType<DbTokenInvoke>(parts.Head); Assert.Equal("x(h1, h2)", parts.Head.ToString());
        Assert.IsType<DbTokenIdentifier>(parts.Body); Assert.Equal("x.[b1](any).[b2]", parts.Body.ToString());
        Assert.IsType<DbTokenInvoke>(parts.Tail); Assert.Equal("x(t1, t2)", parts.Tail.ToString());
    }
}
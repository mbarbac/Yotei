#pragma warning disable IDE0018

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_DbTokenExtract
{
    //[Enforced]
    [Fact]
    public static void Test_ExtractHead_NotFound()
    {
        var engine = new FakeEngine();
        IDbToken source, body;
        DbTokenInvoke? head;
        bool found;

        source = DbLambdaParser.Parse(engine, x => x);
        found = source.ExtractHeadInvokes(out body, out head);
        Assert.False(found);

        source = DbLambdaParser.Parse(engine, x => x.b);
        found = source.ExtractHeadInvokes(out body, out head);
        Assert.False(found);

        source = DbLambdaParser.Parse(engine, x => x.b.x("t"));
        found = source.ExtractHeadInvokes(out body, out head);
        Assert.False(found);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractHead_Standard()
    {
        var engine = new FakeEngine();
        IDbToken source, body;
        DbTokenInvoke? head;
        bool found;

        source = DbLambdaParser.Parse(engine, x => x("h1")("h2"));
        found = source.ExtractHeadInvokes(out body, out head);
        Assert.True(found);
        Assert.IsType<DbTokenArgument>(body); Assert.Equal("x", body.ToString());
        Assert.NotNull(head); Assert.Equal("x(h1, h2)", head.ToString());

        source = DbLambdaParser.Parse(engine, x => x.x("h1").x("h2"));
        found = source.ExtractHeadInvokes(out body, out head);
        Assert.True(found);
        Assert.IsType<DbTokenArgument>(body); Assert.Equal("x", body.ToString());
        Assert.NotNull(head); Assert.Equal("x(h1, h2)", head.ToString());

        source = DbLambdaParser.Parse(engine, x => x("h1")("h2").b1.b2);
        found = source.ExtractHeadInvokes(out body, out head);
        Assert.True(found);
        Assert.IsType<DbTokenIdentifier>(body); Assert.Equal("x.[b1].[b2]", body.ToString());
        Assert.NotNull(head); Assert.Equal("x(h1, h2)", head.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractHead_NotRecurrent()
    {
        var engine = new FakeEngine();
        IDbToken source, body;
        DbTokenInvoke? head;
        bool found;

        source = DbLambdaParser.Parse(engine, x => x("h1")("h2"));
        found = source.ExtractHeadInvokes(out body, out head, false);
        Assert.True(found);
        Assert.IsType<DbTokenInvoke>(body); Assert.Equal("x(h2)", body.ToString());
        Assert.NotNull(head); Assert.Equal("x(h1)", head.ToString());

        source = DbLambdaParser.Parse(engine, x => x.x("h1").x("h2"));
        found = source.ExtractHeadInvokes(out body, out head, false);
        Assert.True(found);
        Assert.IsType<DbTokenInvoke>(body); Assert.Equal("x(h2)", body.ToString());
        Assert.NotNull(head); Assert.Equal("x(h1)", head.ToString());

        source = DbLambdaParser.Parse(engine, x => x("h1")("h2").b1.b2);
        found = source.ExtractHeadInvokes(out body, out head, false);
        Assert.True(found);
        Assert.IsType<DbTokenIdentifier>(body); Assert.Equal("x(h2).[b1].[b2]", body.ToString());
        Assert.NotNull(head); Assert.Equal("x(h1)", head.ToString());

        source = DbLambdaParser.Parse(engine, x => x("h1").x("h2").b1.b2);
        found = source.ExtractHeadInvokes(out body, out head, false);
        Assert.True(found);
        Assert.IsType<DbTokenIdentifier>(body); Assert.Equal("x(h2).[b1].[b2]", body.ToString());
        Assert.NotNull(head); Assert.Equal("x(h1)", head.ToString());
    }

    // ----------------------------------------------------
    //[Enforced]
    [Fact]
    public static void Test_ExtractTail_NotFound()
    {
        var engine = new FakeEngine();
        IDbToken source, body;
        DbTokenInvoke? head;
        bool found;

        source = DbLambdaParser.Parse(engine, x => x);
        found = source.ExtractTailInvokes(out body, out head);
        Assert.False(found);

        source = DbLambdaParser.Parse(engine, x => x.b);
        found = source.ExtractTailInvokes(out body, out head);
        Assert.False(found);

        source = DbLambdaParser.Parse(engine, x => x("h").b);
        found = source.ExtractTailInvokes(out body, out head);
        Assert.False(found);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractTail_Standard()
    {
        var engine = new FakeEngine();
        IDbToken source, body;
        DbTokenInvoke? head;
        bool found;

        source = DbLambdaParser.Parse(engine, x => x("t1")("t2"));
        found = source.ExtractTailInvokes(out body, out head);
        Assert.True(found);
        Assert.IsType<DbTokenArgument>(body); Assert.Equal("x", body.ToString());
        Assert.NotNull(head); Assert.Equal("x(t1, t2)", head.ToString());

        source = DbLambdaParser.Parse(engine, x => x.x("t1").x("t2"));
        found = source.ExtractTailInvokes(out body, out head);
        Assert.True(found);
        Assert.IsType<DbTokenArgument>(body); Assert.Equal("x", body.ToString());
        Assert.NotNull(head); Assert.Equal("x(t1, t2)", head.ToString());

        source = DbLambdaParser.Parse(engine, x => x.b1.b2.x("t1")("t2"));
        found = source.ExtractTailInvokes(out body, out head);
        Assert.True(found);
        Assert.IsType<DbTokenIdentifier>(body); Assert.Equal("x.[b1].[b2]", body.ToString());
        Assert.NotNull(head); Assert.Equal("x(t1, t2)", head.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractTail_Not_Recurrent()
    {
        var engine = new FakeEngine();
        IDbToken source, body;
        DbTokenInvoke? head;
        bool found;

        source = DbLambdaParser.Parse(engine, x => x("t1")("t2"));
        found = source.ExtractTailInvokes(out body, out head, false);
        Assert.True(found);
        Assert.IsType<DbTokenInvoke>(body); Assert.Equal("x(t1)", body.ToString());
        Assert.NotNull(head); Assert.Equal("x(t2)", head.ToString());

        source = DbLambdaParser.Parse(engine, x => x.x("t1").x("t2"));
        found = source.ExtractTailInvokes(out body, out head, false);
        Assert.True(found);
        Assert.IsType<DbTokenInvoke>(body); Assert.Equal("x(t1)", body.ToString());
        Assert.NotNull(head); Assert.Equal("x(t2)", head.ToString());

        source = DbLambdaParser.Parse(engine, x => x.b1.b2.x("t1")("t2"));
        found = source.ExtractTailInvokes(out body, out head, false);
        Assert.True(found);
        Assert.IsType<DbTokenInvoke>(body); Assert.Equal("x.[b1].[b2](t1)", body.ToString());
        Assert.NotNull(head); Assert.Equal("x(t2)", head.ToString());

        source = DbLambdaParser.Parse(engine, x => x.b1.b2.x("t1").x("t2"));
        found = source.ExtractTailInvokes(out body, out head, false);
        Assert.True(found);
        Assert.IsType<DbTokenInvoke>(body); Assert.Equal("x.[b1].[b2](t1)", body.ToString());
        Assert.NotNull(head); Assert.Equal("x(t2)", head.ToString());
    }
}
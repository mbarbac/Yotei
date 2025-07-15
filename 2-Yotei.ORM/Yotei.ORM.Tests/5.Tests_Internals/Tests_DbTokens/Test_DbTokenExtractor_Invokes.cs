#pragma warning disable IDE0018

namespace Yotei.ORM.Tests.Internals.DbTokens;

// ========================================================
//[Enforced]
public static class Test_DbTokenExtractor_Invokes
{
    //[Enforced]
    [Fact]
    public static void Test_ExtractHead_NotFound()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        DbTokenInvoke? removed;

        source = DbLambdaParser.Parse(engine, x => x);
        result = source.ExtractHeadInvokes(out removed);
        Assert.Same(source, result);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        result = source.ExtractHeadInvokes(out removed);
        Assert.Same(source, result);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => x.One.x("Two").Three);
        result = source.ExtractHeadInvokes(out removed);
        Assert.Same(source, result);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractHead_Found_Isolated()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        DbTokenInvoke? removed;

        source = DbLambdaParser.Parse(engine, x => x("h1"));
        result = source.ExtractHeadInvokes(out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenArgument>(result); Assert.Equal("x", result.ToString());
        Assert.IsType<DbTokenInvoke>(removed); Assert.Equal("x(h1)", removed.ToString());

        source = DbLambdaParser.Parse(engine, x => x("h1")("h2"));
        result = source.ExtractHeadInvokes(out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenArgument>(result); Assert.Equal("x", result.ToString());
        Assert.IsType<DbTokenInvoke>(removed); Assert.Equal("x(h1, 'h2')", removed.ToString());

        source = DbLambdaParser.Parse(engine, x => x("h1").x(x("h2")));
        result = source.ExtractHeadInvokes(out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenArgument>(result); Assert.Equal("x", result.ToString());
        Assert.IsType<DbTokenInvoke>(removed); Assert.Equal("x(h1, x(h2))", removed.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractHead_Found_Isolated_NotRecurrent()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        DbTokenInvoke? removed;

        source = DbLambdaParser.Parse(engine, x => x("h1")("h2"));
        result = source.ExtractHeadInvokes(out removed, recurrent: false);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenInvoke>(result); Assert.Equal("x('h2')", result.ToString());
        Assert.IsType<DbTokenInvoke>(removed); Assert.Equal("x(h1)", removed.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractHead_Found_Complex()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        DbTokenInvoke? removed;

        source = DbLambdaParser.Parse(engine, x => x("h1").b1);
        result = source.ExtractHeadInvokes(out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenIdentifier>(result); Assert.Equal("x.[b1]", result.ToString());
        Assert.IsType<DbTokenInvoke>(removed); Assert.Equal("x(h1)", removed.ToString());

        source = DbLambdaParser.Parse(engine, x => x("h1").b1.x("t1"));
        result = source.ExtractHeadInvokes(out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenInvoke>(result); Assert.Equal("x.[b1]('t1')", result.ToString());
        Assert.IsType<DbTokenInvoke>(removed); Assert.Equal("x(h1)", removed.ToString());

        source = DbLambdaParser.Parse(engine, x => x("h1")("h2").b1.b2.x("t1"));
        result = source.ExtractHeadInvokes(out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenInvoke>(result); Assert.Equal("x.[b1].[b2]('t1')", result.ToString());
        Assert.IsType<DbTokenInvoke>(removed); Assert.Equal("x(h1, 'h2')", removed.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractHead_Found_Complex_NotRecurrent()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        DbTokenInvoke? removed;

        source = DbLambdaParser.Parse(engine, x => x("h1")("h2").b1.b2.x("t1"));
        result = source.ExtractHeadInvokes(out removed, recurrent: false);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenInvoke>(result); Assert.Equal("x('h2').[b1].[b2]('t1')", result.ToString());
        Assert.IsType<DbTokenInvoke>(removed); Assert.Equal("x(h1)", removed.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractTail_NotFound()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        DbTokenInvoke? removed;

        source = DbLambdaParser.Parse(engine, x => x);
        result = source.ExtractTailInvokes(out removed);
        Assert.Same(source, result);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        result = source.ExtractTailInvokes(out removed);
        Assert.Same(source, result);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => x.One.x("Two").Three);
        result = source.ExtractTailInvokes(out removed);
        Assert.Same(source, result);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractTail_Found_Isolated()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        DbTokenInvoke? removed;

        source = DbLambdaParser.Parse(engine, x => x("t1"));
        result = source.ExtractTailInvokes(out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenArgument>(result); Assert.Equal("x", result.ToString());
        Assert.IsType<DbTokenInvoke>(removed); Assert.Equal("x(t1)", removed.ToString());

        source = DbLambdaParser.Parse(engine, x => x("t1")("t2"));
        result = source.ExtractTailInvokes(out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenArgument>(result); Assert.Equal("x", result.ToString());
        Assert.IsType<DbTokenInvoke>(removed); Assert.Equal("x(t1, 't2')", removed.ToString());

        source = DbLambdaParser.Parse(engine, x => x("t1").x("t2"));
        result = source.ExtractTailInvokes(out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenArgument>(result); Assert.Equal("x", result.ToString());
        Assert.IsType<DbTokenInvoke>(removed); Assert.Equal("x(t1, 't2')", removed.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractTail_Found_Isolated_NotRecurrent()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        DbTokenInvoke? removed;

        source = DbLambdaParser.Parse(engine, x => x("t1")("t2"));
        result = source.ExtractTailInvokes(out removed, recurrent: false);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenInvoke>(result); Assert.Equal("x(t1)", result.ToString());
        Assert.IsType<DbTokenInvoke>(removed); Assert.Equal("x('t2')", removed.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractTail_Found_Complex()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        DbTokenInvoke? removed;

        source = DbLambdaParser.Parse(engine, x => x.b1.x("t1"));
        result = source.ExtractTailInvokes(out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenIdentifier>(result); Assert.Equal("x.[b1]", result.ToString());
        Assert.IsType<DbTokenInvoke>(removed); Assert.Equal("x('t1')", removed.ToString());

        source = DbLambdaParser.Parse(engine, x => x("h1").b1.x("t1"));
        result = source.ExtractTailInvokes(out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenIdentifier>(result); Assert.Equal("x(h1).[b1]", result.ToString());
        Assert.IsType<DbTokenInvoke>(removed); Assert.Equal("x('t1')", removed.ToString());

        source = DbLambdaParser.Parse(engine, x => x("h1").b1.b2.x("t1").x("t2"));
        result = source.ExtractTailInvokes(out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenIdentifier>(result); Assert.Equal("x(h1).[b1].[b2]", result.ToString());
        Assert.IsType<DbTokenInvoke>(removed); Assert.Equal("x('t1', 't2')", removed.ToString());
    }


    //[Enforced]
    [Fact]
    public static void Test_ExtractTail_Found_Complex_NotRecurrent()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        DbTokenInvoke? removed;

        source = DbLambdaParser.Parse(engine, x => x("h1").b1.b2.x("t1").x("t2"));
        result = source.ExtractTailInvokes(out removed, recurrent: false);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenInvoke>(result); Assert.Equal("x(h1).[b1].[b2]('t1')", result.ToString());
        Assert.IsType<DbTokenInvoke>(removed); Assert.Equal("x('t2')", removed.ToString());
    }

}
#pragma warning disable IDE0018

namespace Yotei.ORM.Tests.Internals.DbTokens;

// ========================================================
//[Enforced]
public static class Test_DbTokenExtractor_Tree
{
    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_Argument()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        result = source.ExtractFirst(x => x is DbTokenArgument, out removed);
        Assert.Same(source, result);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_NotFound()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        result = source.ExtractFirst(x => x is DbTokenIdentifier id && id.Value == "[Four]", out removed);
        Assert.Same(source, result);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_Found()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        result = source.ExtractFirst(x => x is DbTokenIdentifier id && id.Value!.Contains('e'), out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenIdentifier>(result); Assert.Equal("x.[Two].[Three]", result.ToString());
        Assert.NotNull(removed);
        Assert.IsType<DbTokenIdentifier>(removed); Assert.Equal("x.[One]", removed.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_NotHosted()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => (x.One = x.Two).Three.Four);
        result = source.ExtractFirst(x => x is DbTokenSetter, out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenIdentifier>(result); Assert.Equal("x.[Three].[Four]", result.ToString());
        Assert.NotNull(removed);
        Assert.IsType<DbTokenSetter>(removed); Assert.Equal("(x.[One] = x.[Two])", removed.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_Special()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => x.One.Two == "any");
        result = source.ExtractFirst(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenBinary>(result); Assert.Equal("(x.[Two] Equal 'any')", result.ToString());
        Assert.NotNull(removed);
        Assert.IsType<DbTokenIdentifier>(removed); Assert.Equal("x.[One]", removed.ToString());

        source = DbLambdaParser.Parse(engine, x => (string)x.One.Two);
        result = source.ExtractFirst(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenConvert.ToType>(result); Assert.Equal("((String) x.[Two])", result.ToString());
        Assert.NotNull(removed);
        Assert.IsType<DbTokenIdentifier>(removed); Assert.Equal("x.[One]", removed.ToString());

        source = DbLambdaParser.Parse(engine, x => x.One.Two = "any");
        result = source.ExtractFirst(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenSetter>(result); Assert.Equal("(x.[Two] = 'any')", result.ToString());
        Assert.NotNull(removed);
        Assert.IsType<DbTokenIdentifier>(removed); Assert.Equal("x.[One]", removed.ToString());

        source = DbLambdaParser.Parse(engine, x => !x.One.Two);
        result = source.ExtractFirst(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenUnary>(result); Assert.Equal("(Not x.[Two])", result.ToString());
        Assert.NotNull(removed);
        Assert.IsType<DbTokenIdentifier>(removed); Assert.Equal("x.[One]", removed.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_Argument()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        result = source.ExtractLast(x => x is DbTokenArgument, out removed);
        Assert.Same(source, result);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_NotFound()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        result = source.ExtractLast(x => x is DbTokenIdentifier id && id.Value == "[Four]", out removed);
        Assert.Same(source, result);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_Found()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        result = source.ExtractLast(x => x is DbTokenIdentifier id && id.Value!.Contains('e'), out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenIdentifier>(result); Assert.Equal("x.[One].[Two]", result.ToString());
        Assert.NotNull(removed);
        Assert.IsType<DbTokenIdentifier>(removed); Assert.Equal("x.[Three]", removed.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_NotHosted()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => (x.One = x.Two).Three.Four);
        result = source.ExtractLast(x => x is DbTokenSetter, out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenIdentifier>(result); Assert.Equal("x.[Three].[Four]", result.ToString());
        Assert.NotNull(removed);
        Assert.IsType<DbTokenSetter>(removed); Assert.Equal("(x.[One] = x.[Two])", removed.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_Special()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => x.One.Two == "any");
        result = source.ExtractLast(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenBinary>(result); Assert.Equal("(x.[Two] Equal 'any')", result.ToString());
        Assert.NotNull(removed);
        Assert.IsType<DbTokenIdentifier>(removed); Assert.Equal("x.[One]", removed.ToString());

        source = DbLambdaParser.Parse(engine, x => (string)x.One.Two);
        result = source.ExtractLast(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenConvert.ToType>(result); Assert.Equal("((String) x.[Two])", result.ToString());
        Assert.NotNull(removed);
        Assert.IsType<DbTokenIdentifier>(removed); Assert.Equal("x.[One]", removed.ToString());

        source = DbLambdaParser.Parse(engine, x => x.One.Two = "any");
        result = source.ExtractLast(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenSetter>(result); Assert.Equal("(x.[Two] = 'any')", result.ToString());
        Assert.NotNull(removed);
        Assert.IsType<DbTokenIdentifier>(removed); Assert.Equal("x.[One]", removed.ToString());

        source = DbLambdaParser.Parse(engine, x => !x.One.Two);
        result = source.ExtractLast(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenUnary>(result); Assert.Equal("(Not x.[Two])", result.ToString());
        Assert.NotNull(removed);
        Assert.IsType<DbTokenIdentifier>(removed); Assert.Equal("x.[One]", removed.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractAll_Argument()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        List<IDbToken> removed;

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        result = source.ExtractAll(x => x is DbTokenArgument, out removed);
        Assert.Same(source, result);
        Assert.Empty(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractAll_NotFound()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        List<IDbToken> removed;

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        result = source.ExtractAll(x => x is DbTokenIdentifier id && id.Value == "[Four]", out removed);
        Assert.Same(source, result);
        Assert.Empty(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractAll_Found()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        List<IDbToken> removed;

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        result = source.ExtractAll(x => x is DbTokenIdentifier id && id.Value!.Contains('e'), out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenIdentifier>(result); Assert.Equal("x.[Two]", result.ToString());
        Assert.Equal(2, removed.Count);
        Assert.IsType<DbTokenIdentifier>(removed[0]); Assert.Equal("x.[One]", removed[0].ToString());
        Assert.IsType<DbTokenIdentifier>(removed[1]); Assert.Equal("x.[Three]", removed[1].ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractAll_NotHosted()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        List<IDbToken> removed;

        source = DbLambdaParser.Parse(engine, x => (x.One = x.Two).Three.Four);
        result = source.ExtractAll(x => x is DbTokenSetter, out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenIdentifier>(result); Assert.Equal("x.[Three].[Four]", result.ToString());
        Assert.Single(removed);
        Assert.IsType<DbTokenSetter>(removed[0]); Assert.Equal("(x.[One] = x.[Two])", removed[0].ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractAll_Special()
    {
        var engine = new FakeEngine();
        IDbToken source, result; 
        List<IDbToken> removed;

        source = DbLambdaParser.Parse(engine, x => x.One.Two == "any");
        result = source.ExtractAll(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenBinary>(result); Assert.Equal("(x.[Two] Equal 'any')", result.ToString());
        Assert.Single(removed);
        Assert.IsType<DbTokenIdentifier>(removed[0]); Assert.Equal("x.[One]", removed[0].ToString());

        source = DbLambdaParser.Parse(engine, x => (string)x.One.Two);
        result = source.ExtractAll(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenConvert.ToType>(result); Assert.Equal("((String) x.[Two])", result.ToString());
        Assert.Single(removed);
        Assert.IsType<DbTokenIdentifier>(removed[0]); Assert.Equal("x.[One]", removed[0].ToString());

        source = DbLambdaParser.Parse(engine, x => x.One.Two = "any");
        result = source.ExtractAll(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenSetter>(result); Assert.Equal("(x.[Two] = 'any')", result.ToString());
        Assert.Single(removed);
        Assert.IsType<DbTokenIdentifier>(removed[0]); Assert.Equal("x.[One]", removed[0].ToString());

        source = DbLambdaParser.Parse(engine, x => !x.One.Two);
        result = source.ExtractAll(x => x is DbTokenIdentifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenUnary>(result); Assert.Equal("(Not x.[Two])", result.ToString());
        Assert.Single(removed);
        Assert.IsType<DbTokenIdentifier>(removed[0]); Assert.Equal("x.[One]", removed[0].ToString());
    }
}
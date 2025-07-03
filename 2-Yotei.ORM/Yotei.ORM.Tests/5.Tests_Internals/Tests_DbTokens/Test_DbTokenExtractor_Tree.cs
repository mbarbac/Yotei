#pragma warning disable IDE0018

using Argument = Yotei.ORM.Internals.DbTokenArgument;
using Literal = Yotei.ORM.Internals.DbTokenLiteral;
using Chain = Yotei.ORM.Internals.DbTokenChain;
using Identifier = Yotei.ORM.Internals.DbTokenIdentifier;

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
        IDbToken source, item;
        IDbToken? removed = null;

        // Arguments ARE NOT extracted from trees...
        source = DbLambdaParser.Parse(engine, x => x);
        item = source.ExtractFirst(x => x is Argument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.ExtractFirst(x => x is Argument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_NotHosted()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => x.Coalesce(x.One, x.Two));
        item = source.ExtractFirst(x => x is Argument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => x.One == x.Two);
        item = source.ExtractFirst(x => x is Argument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => x.One == 7);
        item = source.ExtractFirst(x => x is Argument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => !x.One);
        item = source.ExtractFirst(x => x is Argument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_NotFound()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.ExtractFirst(x => x is Identifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_Found()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;
        Argument argument;
        Identifier id;

        source = DbLambdaParser.Parse(engine, x => x.One);
        item = source.ExtractFirst(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        argument = Assert.IsType<Argument>(item); Assert.Equal("x", argument.Name);
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.ExtractFirst(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("[Two]", id.Value);
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.ExtractFirst(x => x is Identifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("[One]", id.Value);
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[Two]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        item = source.ExtractFirst(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("x.[Two].[Three]", id.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        item = source.ExtractFirst(x => x is Identifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("x.[One].[Three]", id.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[Two]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        item = source.ExtractFirst(x => x is Identifier id && id.Value == "[Three]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("x.[One].[Two]", id.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[Three]", id.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_Duplicates()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;
        Identifier id;

        source = DbLambdaParser.Parse(engine, x => x.One.Two.One);
        item = source.ExtractFirst(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("x.[Two].[One]", id.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[One]", id.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_Special_Binary()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;
        Identifier id;
        DbTokenBinary binary;

        source = DbLambdaParser.Parse(engine, x => x.One.Two == "any");
        item = source.ExtractFirst(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        binary = Assert.IsType<DbTokenBinary>(item); Assert.Equal("(x.[Two] Equal 'any')", item.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two == "any");
        item = source.ExtractFirst(x => x is Identifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        binary = Assert.IsType<DbTokenBinary>(item); Assert.Equal("(x.[One] Equal 'any')", item.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[Two]", id.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_Special_Convert()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;
        Identifier id;
        DbTokenConvert convert;

        source = DbLambdaParser.Parse(engine, x => (string)x.One.Two);
        item = source.ExtractFirst(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        convert = Assert.IsType<DbTokenConvert.ToType>(item); Assert.Equal("((String) x.[Two])", item.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => (string)x.One.Two);
        item = source.ExtractFirst(x => x is Identifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        convert = Assert.IsType<DbTokenConvert.ToType>(item); Assert.Equal("((String) x.[One])", item.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[Two]", id.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_Special_Setter()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;
        Identifier id;
        DbTokenSetter setter;

        source = DbLambdaParser.Parse(engine, x => x.One.Two = "any");
        item = source.ExtractFirst(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        setter = Assert.IsType<DbTokenSetter>(item); Assert.Equal("(x.[Two] = 'any')", item.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two = "any");
        item = source.ExtractFirst(x => x is Identifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        setter = Assert.IsType<DbTokenSetter>(item); Assert.Equal("(x.[One] = 'any')", item.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[Two]", id.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_Special_Unary()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;
        Identifier id;
        DbTokenUnary unary;

        source = DbLambdaParser.Parse(engine, x => !x.One.Two);
        item = source.ExtractFirst(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        unary = Assert.IsType<DbTokenUnary>(item); Assert.Equal("(Not x.[Two])", item.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => !x.One.Two);
        item = source.ExtractFirst(x => x is Identifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        unary = Assert.IsType<DbTokenUnary>(item); Assert.Equal("(Not x.[One])", item.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[Two]", id.Value);
    }

    // ----------------------------------------------------
    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_Argument()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;

        // Arguments ARE NOT extracted from trees...
        source = DbLambdaParser.Parse(engine, x => x);
        item = source.ExtractLast(x => x is Argument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.ExtractLast(x => x is Argument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_NotHosted()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => x.Coalesce(x.One, x.Two));
        item = source.ExtractLast(x => x is Argument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => x.One == x.Two);
        item = source.ExtractLast(x => x is Argument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => x.One == 7);
        item = source.ExtractLast(x => x is Argument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);

        source = DbLambdaParser.Parse(engine, x => !x.One);
        item = source.ExtractLast(x => x is Argument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_NotFound()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.ExtractLast(x => x is Identifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_Found()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;
        Argument argument;
        Identifier id;

        source = DbLambdaParser.Parse(engine, x => x.One);
        item = source.ExtractLast(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        argument = Assert.IsType<Argument>(item); Assert.Equal("x", argument.Name);
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.ExtractLast(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("[Two]", id.Value);
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.ExtractLast(x => x is Identifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("[One]", id.Value);
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[Two]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        item = source.ExtractLast(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("x.[Two].[Three]", id.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        item = source.ExtractLast(x => x is Identifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("x.[One].[Three]", id.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[Two]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        item = source.ExtractLast(x => x is Identifier id && id.Value == "[Three]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("x.[One].[Two]", id.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[Three]", id.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_Duplicates()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;
        Identifier id;

        source = DbLambdaParser.Parse(engine, x => x.One.Two.One);
        item = source.ExtractLast(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("x.[One].[Two]", id.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[One]", id.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_Special_Binary()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;
        Identifier id;
        DbTokenBinary binary;

        source = DbLambdaParser.Parse(engine, x => x.One.Two == "any");
        item = source.ExtractLast(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        binary = Assert.IsType<DbTokenBinary>(item); Assert.Equal("(x.[Two] Equal 'any')", item.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two == "any");
        item = source.ExtractLast(x => x is Identifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        binary = Assert.IsType<DbTokenBinary>(item); Assert.Equal("(x.[One] Equal 'any')", item.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[Two]", id.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_Special_Convert()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;
        Identifier id;
        DbTokenConvert convert;

        source = DbLambdaParser.Parse(engine, x => (string)x.One.Two);
        item = source.ExtractLast(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        convert = Assert.IsType<DbTokenConvert.ToType>(item); Assert.Equal("((String) x.[Two])", item.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => (string)x.One.Two);
        item = source.ExtractLast(x => x is Identifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        convert = Assert.IsType<DbTokenConvert.ToType>(item); Assert.Equal("((String) x.[One])", item.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[Two]", id.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_Special_Setter()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;
        Identifier id;
        DbTokenSetter setter;

        source = DbLambdaParser.Parse(engine, x => x.One.Two = "any");
        item = source.ExtractLast(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        setter = Assert.IsType<DbTokenSetter>(item); Assert.Equal("(x.[Two] = 'any')", item.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two = "any");
        item = source.ExtractLast(x => x is Identifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        setter = Assert.IsType<DbTokenSetter>(item); Assert.Equal("(x.[One] = 'any')", item.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[Two]", id.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_Special_Unary()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;
        Identifier id;
        DbTokenUnary unary;

        source = DbLambdaParser.Parse(engine, x => !x.One.Two);
        item = source.ExtractLast(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        unary = Assert.IsType<DbTokenUnary>(item); Assert.Equal("(Not x.[Two])", item.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => !x.One.Two);
        item = source.ExtractLast(x => x is Identifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        unary = Assert.IsType<DbTokenUnary>(item); Assert.Equal("(Not x.[One])", item.ToString());
        id = Assert.IsType<Identifier>(removed); Assert.Equal("[Two]", id.Value);
    }


    // ----------------------------------------------------
    //[Enforced]
    [Fact]
    public static void Test_ExtractAll_Argument()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        List<IDbToken> removed;

        // Arguments ARE NOT extracted from trees...
        source = DbLambdaParser.Parse(engine, x => x);
        item = source.ExtractAll(x => x is Argument, out removed);
        Assert.Same(source, item);
        Assert.Empty(removed);

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.ExtractAll(x => x is Argument, out removed);
        Assert.Same(source, item);
        Assert.Empty(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractAll_NotHosted()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        List<IDbToken> removed;

        source = DbLambdaParser.Parse(engine, x => x.Coalesce(x.One, x.Two));
        item = source.ExtractAll(x => x is Argument, out removed);
        Assert.Same(source, item);
        Assert.Empty(removed);

        source = DbLambdaParser.Parse(engine, x => x.One == x.Two);
        item = source.ExtractAll(x => x is Argument, out removed);
        Assert.Same(source, item);
        Assert.Empty(removed);

        source = DbLambdaParser.Parse(engine, x => x.One == 7);
        item = source.ExtractAll(x => x is Argument, out removed);
        Assert.Same(source, item);
        Assert.Empty(removed);

        source = DbLambdaParser.Parse(engine, x => !x.One);
        item = source.ExtractAll(x => x is Argument, out removed);
        Assert.Same(source, item);
        Assert.Empty(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractAll_NotFound()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        List<IDbToken> removed;

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.ExtractAll(x => x is Identifier id && id.Value == "[Three]", out removed);
        Assert.Same(source, item);
        Assert.Empty(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractAll_Found()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        List<IDbToken> removed;
        Argument argument;
        Identifier id;

        source = DbLambdaParser.Parse(engine, x => x.One);
        item = source.ExtractAll(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        argument = Assert.IsType<Argument>(item); Assert.Equal("x", argument.Name);
        Assert.Single(removed);
        id = Assert.IsType<Identifier>(removed[0]); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.ExtractAll(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("[Two]", id.Value);
        Assert.Single(removed);
        id = Assert.IsType<Identifier>(removed[0]); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.ExtractAll(x => x is Identifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("[One]", id.Value);
        Assert.Single(removed);
        id = Assert.IsType<Identifier>(removed[0]); Assert.Equal("[Two]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        item = source.ExtractAll(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("x.[Two].[Three]", id.ToString());
        Assert.Single(removed);
        id = Assert.IsType<Identifier>(removed[0]); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        item = source.ExtractAll(x => x is Identifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("x.[One].[Three]", id.ToString());
        Assert.Single(removed);
        id = Assert.IsType<Identifier>(removed[0]); Assert.Equal("[Two]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two.Three);
        item = source.ExtractAll(x => x is Identifier id && id.Value == "[Three]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("x.[One].[Two]", id.ToString());
        Assert.Single(removed);
        id = Assert.IsType<Identifier>(removed[0]); Assert.Equal("[Three]", id.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractAll_Duplicates()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        List<IDbToken> removed;
        Identifier id;

        source = DbLambdaParser.Parse(engine, x => x.One.Two.One);
        item = source.ExtractAll(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("x.[Two]", id.ToString());
        Assert.Equal(2, removed.Count);
        id = Assert.IsType<Identifier>(removed[0]); Assert.Equal("[One]", id.Value);
        id = Assert.IsType<Identifier>(removed[1]); Assert.Equal("[One]", id.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractAll_Special_Binary()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        List<IDbToken> removed;
        Identifier id;
        DbTokenBinary binary;

        source = DbLambdaParser.Parse(engine, x => x.One.Two == "any");
        item = source.ExtractAll(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        binary = Assert.IsType<DbTokenBinary>(item); Assert.Equal("(x.[Two] Equal 'any')", item.ToString());
        Assert.Single(removed);
        id = Assert.IsType<Identifier>(removed[0]); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two == "any");
        item = source.ExtractAll(x => x is Identifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        binary = Assert.IsType<DbTokenBinary>(item); Assert.Equal("(x.[One] Equal 'any')", item.ToString());
        Assert.Single(removed);
        id = Assert.IsType<Identifier>(removed[0]); Assert.Equal("[Two]", id.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractAll_Special_Convert()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        List<IDbToken> removed;
        Identifier id;
        DbTokenConvert convert;

        source = DbLambdaParser.Parse(engine, x => (string)x.One.Two);
        item = source.ExtractAll(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        convert = Assert.IsType<DbTokenConvert.ToType>(item); Assert.Equal("((String) x.[Two])", item.ToString());
        Assert.Single(removed);
        id = Assert.IsType<Identifier>(removed[0]); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => (string)x.One.Two);
        item = source.ExtractAll(x => x is Identifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        convert = Assert.IsType<DbTokenConvert.ToType>(item); Assert.Equal("((String) x.[One])", item.ToString());
        Assert.Single(removed);
        id = Assert.IsType<Identifier>(removed[0]); Assert.Equal("[Two]", id.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractAll_Special_Setter()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        List<IDbToken> removed;
        Identifier id;
        DbTokenSetter setter;

        source = DbLambdaParser.Parse(engine, x => x.One.Two = "any");
        item = source.ExtractAll(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        setter = Assert.IsType<DbTokenSetter>(item); Assert.Equal("(x.[Two] = 'any')", item.ToString());
        Assert.Single(removed);
        id = Assert.IsType<Identifier>(removed[0]); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => x.One.Two = "any");
        item = source.ExtractAll(x => x is Identifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        setter = Assert.IsType<DbTokenSetter>(item); Assert.Equal("(x.[One] = 'any')", item.ToString());
        Assert.Single(removed);
        id = Assert.IsType<Identifier>(removed[0]); Assert.Equal("[Two]", id.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractAll_Special_Unary()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        List<IDbToken> removed;
        Identifier id;
        DbTokenUnary unary;

        source = DbLambdaParser.Parse(engine, x => !x.One.Two);
        item = source.ExtractAll(x => x is Identifier id && id.Value == "[One]", out removed);
        Assert.NotSame(source, item);
        unary = Assert.IsType<DbTokenUnary>(item); Assert.Equal("(Not x.[Two])", item.ToString());
        Assert.Single(removed);
        id = Assert.IsType<Identifier>(removed[0]); Assert.Equal("[One]", id.Value);

        source = DbLambdaParser.Parse(engine, x => !x.One.Two);
        item = source.ExtractAll(x => x is Identifier id && id.Value == "[Two]", out removed);
        Assert.NotSame(source, item);
        unary = Assert.IsType<DbTokenUnary>(item); Assert.Equal("(Not x.[One])", item.ToString());
        Assert.Single(removed);
        id = Assert.IsType<Identifier>(removed[0]); Assert.Equal("[Two]", id.Value);
    }
}
#pragma warning disable IDE0018

using Argument = Yotei.ORM.Internals.DbTokenArgument;
using Literal = Yotei.ORM.Internals.DbTokenLiteral;
using Chain = Yotei.ORM.Internals.DbTokenChain;
using Identifier = Yotei.ORM.Internals.DbTokenIdentifier;
using Setter = Yotei.ORM.Internals.DbTokenSetter;

namespace Yotei.ORM.Tests.Internals.DbTokens;

// ========================================================
//[Enforced]
public static class Test_Tree_DbTokenExtractor
{
    //[Enforced]
    [Fact]
    public static void Test_Tree_ExtractFirst_Argument()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;

        // Arguments ARE NOT extracted from trees...
        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.ExtractFirst(x => x is Argument, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Tree_ExtractFirst_Not_Found()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;

        source = DbLambdaParser.Parse(engine, x => x.One.Two);
        item = source.ExtractFirst(x => x is Literal, out removed);
        Assert.Same(source, item);
        Assert.Null(removed);
    }

    //[Enforced]
    [Fact]
    public static void Test_Tree_ExtractFirst_Found_Not_Hosted_Header()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;
        Identifier id;
        Setter setter;

        source = DbLambdaParser.Parse(engine, x => (x.One = x.Two).Three.Four);
        item = source.ExtractFirst(x => x is Setter, out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("x.[Three].[Four]", id.ToString());
        Assert.NotNull(removed);
        setter = Assert.IsType<Setter>(removed); Assert.Equal("(x.[One] = x.[Two])", setter.ToString());

        item = source.ExtractFirst(x => x is Identifier id && id.Value == "[Three]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("(x.[One] = x.[Two]).[Four]", id.ToString());
        Assert.NotNull(removed);
        id = Assert.IsType<Identifier>(removed); Assert.Equal("x.[Three]", id.ToString());

        item = source.ExtractFirst(x => x is Identifier id && id.Value == "[Four]", out removed);
        Assert.NotSame(source, item);
        id = Assert.IsType<Identifier>(item); Assert.Equal("(x.[One] = x.[Two]).[Three]", id.ToString());
        Assert.NotNull(removed);
        id = Assert.IsType<Identifier>(removed); Assert.Equal("x.[Four]", id.ToString());
    }

    // ----------------------------------------------------


}
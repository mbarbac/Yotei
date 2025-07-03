#pragma warning disable IDE0018

using Argument = Yotei.ORM.Internals.DbTokenArgument;
using Literal = Yotei.ORM.Internals.DbTokenLiteral;
using Chain = Yotei.ORM.Internals.DbTokenChain;
using Identifier = Yotei.ORM.Internals.DbTokenIdentifier;

namespace Yotei.ORM.Tests.Internals.DbTokens;

// ========================================================
//[Enforced]
public static class Test_DbTokenExtractor
{
    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_Argument()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;
        Argument argument;
        Literal literal;
        Chain chain;

        // But arguments ARE extracted from chains...
        source = new Chain([new Literal("one"), new Argument("x"), new Literal("two")]);
        item = source.ExtractFirst(x => x is Argument, out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Equal(2, chain.Count);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("one", literal.Value);
        literal = Assert.IsType<Literal>(chain[1]); Assert.Equal("two", literal.Value);
        argument = Assert.IsType<Argument>(removed); Assert.Equal("x", argument.Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_NotFound()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;

        source = new Chain([new Literal("one"), new Argument("x"), new Literal("two")]);
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
        Chain chain;
        Literal literal;

        source = new Chain([new Literal("one")]);
        item = source.ExtractFirst(x => x is Literal id && id.Value == "one", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Empty(chain);
        literal = Assert.IsType<Literal>(removed); Assert.Equal("one", literal.Value);

        source = new Chain([new Literal("one"), new Literal("two")]);
        item = source.ExtractFirst(x => x is Literal id && id.Value == "one", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Single(chain);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("two", literal.Value);
        literal = Assert.IsType<Literal>(removed); Assert.Equal("one", literal.Value);

        source = new Chain([new Literal("one"), new Literal("two")]);
        item = source.ExtractFirst(x => x is Literal id && id.Value == "two", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Single(chain);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("one", literal.Value);
        literal = Assert.IsType<Literal>(removed); Assert.Equal("two", literal.Value);

        source = new Chain([new Literal("one"), new Literal("two"), new Literal("three")]);
        item = source.ExtractFirst(x => x is Literal id && id.Value == "one", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Equal(2, chain.Count);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("two", literal.Value);
        literal = Assert.IsType<Literal>(chain[1]); Assert.Equal("three", literal.Value);
        literal = Assert.IsType<Literal>(removed); Assert.Equal("one", literal.Value);

        source = new Chain([new Literal("one"), new Literal("two"), new Literal("three")]);
        item = source.ExtractFirst(x => x is Literal id && id.Value == "two", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Equal(2, chain.Count);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("one", literal.Value);
        literal = Assert.IsType<Literal>(chain[1]); Assert.Equal("three", literal.Value);
        literal = Assert.IsType<Literal>(removed); Assert.Equal("two", literal.Value);

        source = new Chain([new Literal("one"), new Literal("two"), new Literal("three")]);
        item = source.ExtractFirst(x => x is Literal id && id.Value == "three", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Equal(2, chain.Count);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("one", literal.Value);
        literal = Assert.IsType<Literal>(chain[1]); Assert.Equal("two", literal.Value);
        literal = Assert.IsType<Literal>(removed); Assert.Equal("three", literal.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_Duplicates()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;
        Chain chain;
        Literal literal;

        source = new Chain([new Literal("one"), new Literal("two"), new Literal("one")]);
        item = source.ExtractFirst(x => x is Literal id && id.Value == "one", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Equal(2, chain.Count);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("two", literal.Value);
        literal = Assert.IsType<Literal>(chain[1]); Assert.Equal("one", literal.Value);
        literal = Assert.IsType<Literal>(removed); Assert.Equal("one", literal.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_Argument()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;
        Argument argument;
        Literal literal;
        Chain chain;

        // But arguments ARE extracted from chains...
        source = new Chain([new Literal("one"), new Argument("x"), new Literal("two")]);
        item = source.ExtractLast(x => x is Argument, out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Equal(2, chain.Count);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("one", literal.Value);
        literal = Assert.IsType<Literal>(chain[1]); Assert.Equal("two", literal.Value);
        argument = Assert.IsType<Argument>(removed); Assert.Equal("x", argument.Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_NotFound()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;

        source = new Chain([new Literal("one"), new Argument("x"), new Literal("two")]);
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
        Chain chain;
        Literal literal;

        source = new Chain([new Literal("one")]);
        item = source.ExtractLast(x => x is Literal id && id.Value == "one", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Empty(chain);
        literal = Assert.IsType<Literal>(removed); Assert.Equal("one", literal.Value);

        source = new Chain([new Literal("one"), new Literal("two")]);
        item = source.ExtractLast(x => x is Literal id && id.Value == "one", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Single(chain);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("two", literal.Value);
        literal = Assert.IsType<Literal>(removed); Assert.Equal("one", literal.Value);

        source = new Chain([new Literal("one"), new Literal("two")]);
        item = source.ExtractLast(x => x is Literal id && id.Value == "two", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Single(chain);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("one", literal.Value);
        literal = Assert.IsType<Literal>(removed); Assert.Equal("two", literal.Value);

        source = new Chain([new Literal("one"), new Literal("two"), new Literal("three")]);
        item = source.ExtractLast(x => x is Literal id && id.Value == "one", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Equal(2, chain.Count);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("two", literal.Value);
        literal = Assert.IsType<Literal>(chain[1]); Assert.Equal("three", literal.Value);
        literal = Assert.IsType<Literal>(removed); Assert.Equal("one", literal.Value);

        source = new Chain([new Literal("one"), new Literal("two"), new Literal("three")]);
        item = source.ExtractLast(x => x is Literal id && id.Value == "two", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Equal(2, chain.Count);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("one", literal.Value);
        literal = Assert.IsType<Literal>(chain[1]); Assert.Equal("three", literal.Value);
        literal = Assert.IsType<Literal>(removed); Assert.Equal("two", literal.Value);

        source = new Chain([new Literal("one"), new Literal("two"), new Literal("three")]);
        item = source.ExtractLast(x => x is Literal id && id.Value == "three", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Equal(2, chain.Count);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("one", literal.Value);
        literal = Assert.IsType<Literal>(chain[1]); Assert.Equal("two", literal.Value);
        literal = Assert.IsType<Literal>(removed); Assert.Equal("three", literal.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_Duplicates()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        IDbToken? removed = null;
        Chain chain;
        Literal literal;

        source = new Chain([new Literal("one"), new Literal("two"), new Literal("one")]);
        item = source.ExtractLast(x => x is Literal id && id.Value == "one", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Equal(2, chain.Count);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("one", literal.Value);
        literal = Assert.IsType<Literal>(chain[1]); Assert.Equal("two", literal.Value);
        literal = Assert.IsType<Literal>(removed); Assert.Equal("one", literal.Value);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractAll_Argument()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        List<IDbToken> removed;
        Argument argument;
        Literal literal;
        Chain chain;

        // But arguments ARE extracted from chains...
        source = new Chain([new Literal("one"), new Argument("x"), new Literal("two")]);
        item = source.ExtractAll(x => x is Argument, out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Equal(2, chain.Count);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("one", literal.Value);
        literal = Assert.IsType<Literal>(chain[1]); Assert.Equal("two", literal.Value);
        Assert.Single(removed);
        argument = Assert.IsType<Argument>(removed[0]); Assert.Equal("x", argument.Name);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractAll_NotFound()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        List<IDbToken> removed;

        source = new Chain([new Literal("one"), new Argument("x"), new Literal("two")]);
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
        Chain chain;
        Literal literal;

        source = new Chain([new Literal("one")]);
        item = source.ExtractAll(x => x is Literal id && id.Value == "one", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Empty(chain);
        Assert.Single(removed);
        literal = Assert.IsType<Literal>(removed[0]); Assert.Equal("one", literal.Value);

        source = new Chain([new Literal("one"), new Literal("two")]);
        item = source.ExtractAll(x => x is Literal id && id.Value == "one", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Single(chain);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("two", literal.Value);
        Assert.Single(removed);
        literal = Assert.IsType<Literal>(removed[0]); Assert.Equal("one", literal.Value);

        source = new Chain([new Literal("one"), new Literal("two")]);
        item = source.ExtractAll(x => x is Literal id && id.Value == "two", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Single(chain);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("one", literal.Value);
        Assert.Single(removed);
        literal = Assert.IsType<Literal>(removed[0]); Assert.Equal("two", literal.Value);

        source = new Chain([new Literal("one"), new Literal("two"), new Literal("three")]);
        item = source.ExtractAll(x => x is Literal id && id.Value == "one", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Equal(2, chain.Count);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("two", literal.Value);
        literal = Assert.IsType<Literal>(chain[1]); Assert.Equal("three", literal.Value);
        Assert.Single(removed);
        literal = Assert.IsType<Literal>(removed[0]); Assert.Equal("one", literal.Value);

        source = new Chain([new Literal("one"), new Literal("two"), new Literal("three")]);
        item = source.ExtractAll(x => x is Literal id && id.Value == "two", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Equal(2, chain.Count);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("one", literal.Value);
        literal = Assert.IsType<Literal>(chain[1]); Assert.Equal("three", literal.Value);
        Assert.Single(removed);
        literal = Assert.IsType<Literal>(removed[0]); Assert.Equal("two", literal.Value);

        source = new Chain([new Literal("one"), new Literal("two"), new Literal("three")]);
        item = source.ExtractAll(x => x is Literal id && id.Value == "three", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Equal(2, chain.Count);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("one", literal.Value);
        literal = Assert.IsType<Literal>(chain[1]); Assert.Equal("two", literal.Value);
        Assert.Single(removed);
        literal = Assert.IsType<Literal>(removed[0]); Assert.Equal("three", literal.Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractAll_Duplicates()
    {
        var engine = new FakeEngine();
        IDbToken source, item;
        List<IDbToken> removed;
        Chain chain;
        Literal literal;

        source = new Chain([new Literal("one"), new Literal("two"), new Literal("one")]);
        item = source.ExtractAll(x => x is Literal id && id.Value == "one", out removed);
        Assert.NotSame(source, item);
        chain = Assert.IsType<Chain>(item);
        Assert.Single(chain);
        literal = Assert.IsType<Literal>(chain[0]); Assert.Equal("two", literal.Value);
        Assert.Equal(2, removed.Count);
        literal = Assert.IsType<Literal>(removed[0]); Assert.Equal("one", literal.Value);
        literal = Assert.IsType<Literal>(removed[1]); Assert.Equal("one", literal.Value);
    }
}
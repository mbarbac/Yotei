#pragma warning disable IDE0018

namespace Yotei.ORM.Tests.Internals.DbTokens;

// ========================================================
//[Enforced]
public static class Test_DbTokenExtractor_Chain
{
    static DbTokenChain Create(params string[] names)
    {
        var chain = new DbTokenChain.Builder();
        foreach (var name in names) chain.Add(new DbTokenLiteral(name));
        return chain.CreateInstance();
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_Argument()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        IDbToken? removed = null;

        source = Create("one", "two", "three");
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

        source = Create("one", "two", "three");
        result = source.ExtractFirst(x => x is DbTokenIdentifier, out removed);
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

        source = Create("one", "two", "three");
        result = source.ExtractFirst(x => x is DbTokenLiteral y && y.Value.Contains('e'), out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenChain>(result); Assert.Equal("[two, three]", result.ToString());
        Assert.NotNull(removed); Assert.Equal("one", removed.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_Argument()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        IDbToken? removed = null;

        source = Create("one", "two", "three");
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

        source = Create("one", "two", "three");
        result = source.ExtractLast(x => x is DbTokenIdentifier, out removed);
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

        source = Create("one", "two", "three");
        result = source.ExtractLast(x => x is DbTokenLiteral y && y.Value.Contains('e'), out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenChain>(result); Assert.Equal("[one, two]", result.ToString());
        Assert.NotNull(removed); Assert.Equal("three", removed.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractAll_Argument()
    {
        var engine = new FakeEngine();
        IDbToken source, result;
        List<IDbToken> removed;

        source = Create("one", "two", "three");
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

        source = Create("one", "two", "three");
        result = source.ExtractAll(x => x is DbTokenIdentifier, out removed);
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

        source = Create("one", "two", "three");
        result = source.ExtractAll(x => x is DbTokenLiteral y && y.Value.Contains('e'), out removed);
        Assert.NotSame(source, result);
        Assert.IsType<DbTokenChain>(result); Assert.Equal("[two]", result.ToString());
        Assert.Equal(2, removed.Count);
        Assert.Equal("one", removed[0].ToString());
        Assert.Equal("three", removed[1].ToString());
    }
}
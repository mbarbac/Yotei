#pragma warning disable IDE0042

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_StrExtract
{
    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_Literal_Not_Found()
    {
        var separator = "==";
        var source = "aa";

        var parts = source.ExtractFirst(separator, false, out var found);
        Assert.False(found);
        Assert.Equal("aa", parts.Left);
        Assert.Null(parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_Literal_Found()
    {
        var separator = "==";
        var source = "aa==bb==cc";

        var parts = source.ExtractFirst(separator, false, out var found);
        Assert.True(found);
        Assert.Equal("aa", parts.Left);
        Assert.Equal("bb==cc", parts.Right);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_Literal_Not_Found()
    {
        var separator = "==";
        var source = "aa";

        var parts = source.ExtractLast(separator, false, out var found);
        Assert.False(found);
        Assert.Equal("aa", parts.Left);
        Assert.Null(parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_Literal_Found()
    {
        var separator = "==";
        var source = "aa==bb==cc";

        var parts = source.ExtractLast(separator, false, out var found);
        Assert.True(found);
        Assert.Equal("aa==bb", parts.Left);
        Assert.Equal("cc", parts.Right);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_Chain_Not_Found()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrTokenizerWrapped('[', ']') { Comparison = comparison };
        IStrTokenText text;
        IStrTokenChain chain;
        var separator = "==";

        var master = "aa";
        var token = tokenizer.Tokenize(master);
        var item = token.ExtractFirst(separator, false, out var found);
        Assert.False(found);
        text = Assert.IsType<StrTokenText>(item);
        Assert.Same(token, text);

        master = "[aa==bb]cc";
        token = tokenizer.Tokenize(master);
        item = token.ExtractFirst(separator, false, out found);
        Assert.False(found);
        chain = Assert.IsType<StrTokenChain>(item);
        Assert.Same(token, chain);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_Chain_Found()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrTokenizerWrapped('[', ']') { Comparison = comparison };
        IStrTokenChain chain;
        var separator = "==";

        var master = "[aa==bb]==cc==[dd==ee]";
        var token = tokenizer.Tokenize(master);
        var item = token.ExtractFirst(separator, false, out var found);
        Assert.True(found);
        chain = Assert.IsType<StrTokenChain>(item);
        Assert.Equal(3, chain.Count);
        Assert.Equal("[aa==bb]", chain[0].ToString());
        Assert.Equal("cc==", chain[1].ToString());
        Assert.Equal("[dd==ee]", chain[2].ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_Chain_Not_Found()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrTokenizerWrapped('[', ']') { Comparison = comparison };
        IStrTokenText text;
        IStrTokenChain chain;
        var separator = "==";

        var master = "aa";
        var token = tokenizer.Tokenize(master);
        var item = token.ExtractLast(separator, false, out var found);
        Assert.False(found);
        text = Assert.IsType<StrTokenText>(item);
        Assert.Same(token, text);

        master = "[aa==bb]cc";
        token = tokenizer.Tokenize(master);
        item = token.ExtractLast(separator, false, out found);
        Assert.False(found);
        chain = Assert.IsType<StrTokenChain>(item);
        Assert.Same(token, chain);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_Chain_Found()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrTokenizerWrapped('[', ']') { Comparison = comparison };
        IStrTokenChain chain;
        var separator = "==";

        var master = "[aa==bb]==cc==[dd==ee]";
        var token = tokenizer.Tokenize(master);
        var item = token.ExtractLast(separator, false, out var found);
        Assert.True(found);
        chain = Assert.IsType<StrTokenChain>(item);
        Assert.Equal(3, chain.Count);
        Assert.Equal("[aa==bb]", chain[0].ToString());
        Assert.Equal("==cc", chain[1].ToString());
        Assert.Equal("[dd==ee]", chain[2].ToString());
    }
}
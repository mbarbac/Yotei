namespace Yotei.ORM.Tests.Internal;

// ========================================================
//[Enforced]
public static class Test_StrKeywordTokenizer
{
    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrKeywordTokenizer("XX") { Comparison = comparison };
        StrTokenText text;

        source = null!;
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Empty(text.Payload);

        source = "";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Empty(text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Isolated()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrKeywordTokenizer("XX") { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithPreventSourceKeyword(true).WithKeepEscape(true);
        StrTokenText text;
        StrTokenFixed key;

        source = "xx";
        target = tokenizer.Tokenize(source);
        key = Assert.IsType<StrTokenFixed>(target); Assert.Equal("xx", key.Payload);

        target = xtokenizer.Tokenize(source);
        key = Assert.IsType<StrTokenFixed>(target); Assert.Equal("XX", key.Payload);

        source = ".xx";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("xx", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(".xx", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Start()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrKeywordTokenizer("XX") { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithPreventSourceKeyword(true).WithKeepEscape(true);
        StrTokenText text;
        StrTokenFixed key;
        StrTokenChain chain;

        source = "xx (xx other";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(4, chain.Count);
        key = Assert.IsType<StrTokenFixed>(chain[0]); Assert.Equal("xx", key.Payload);
        text = Assert.IsType<StrTokenText>(chain[1]); Assert.Equal(" (", text.Payload);
        key = Assert.IsType<StrTokenFixed>(chain[2]); Assert.Equal("xx", key.Payload);
        text = Assert.IsType<StrTokenText>(chain[3]); Assert.Equal(" other", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_End()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrKeywordTokenizer("XX") { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithPreventSourceKeyword(true).WithKeepEscape(true);
        StrTokenText text;
        StrTokenFixed key;
        StrTokenChain chain;

        source = "other xx) xx";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(4, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("other ", text.Payload);
        key = Assert.IsType<StrTokenFixed>(chain[1]); Assert.Equal("xx", key.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal(") ", text.Payload);
        key = Assert.IsType<StrTokenFixed>(chain[3]); Assert.Equal("xx", key.Payload);
    }
}
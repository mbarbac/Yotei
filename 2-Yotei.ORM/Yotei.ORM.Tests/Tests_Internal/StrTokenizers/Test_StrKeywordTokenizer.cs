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
        StrTokenChain chain;

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
}
/*
    
    {
        

        
    }

    //[Enforced]
    [Fact]
    public static void Test_String_Source()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrKeywordTokenizer("XX") { Escape = ".", Comparison = comparison };
        StrTokenText text;
        StrTokenFixed fix;
        StrTokenChain chain;

        source = " ";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(" ", text.Payload);

        source = "aaaxx.xxbbb";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aaa", text.Payload);
        fix = Assert.IsType<StrTokenFixed>(chain[1]); Assert.Equal("xx", fix.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal("xxbbb", text.Payload);

        tokenizer = tokenizer.WithPreventSourceValue(true).WithKeepEscape(true);
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aaa", text.Payload);
        fix = Assert.IsType<StrTokenFixed>(chain[1]); Assert.Equal("XX", fix.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal(".xxbbb", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Chain_Source()
    {
        StrTokenChain source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrKeywordTokenizer("XX") { Escape = ".", Comparison = comparison };
        StrTokenText text;
        StrTokenFixed fix;
        StrTokenChain chain;

        source = new([
            new StrTokenText("aaa"),
            StrTokenText.Empty,
            new StrTokenText("xx"),
            new StrTokenText("."),
            new StrTokenText("xx"),
            new StrTokenText("bbb"),
            ]);

        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aaa", text.Payload);
        fix = Assert.IsType<StrTokenFixed>(chain[1]); Assert.Equal("xx", fix.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal("xxbbb", text.Payload);

        var xtokenizer = tokenizer.WithPreventSourceValue(true).WithKeepEscape(true);
        target = xtokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aaa", text.Payload);
        fix = Assert.IsType<StrTokenFixed>(chain[1]); Assert.Equal("XX", fix.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal(".xxbbb", text.Payload);
    }
}*/
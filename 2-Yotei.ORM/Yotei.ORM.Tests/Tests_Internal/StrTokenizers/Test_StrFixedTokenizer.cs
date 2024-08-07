namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_StrFixedTokenizer
{
    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrFixedTokenizer("--") { Comparison = comparison };
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
    public static void Test_String_Source()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrFixedTokenizer("xx") { Escape = ".", Comparison = comparison };
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
    }

    //[Enforced]
    [Fact]
    public static void Test_String_Source_ForceValue_KeepEscape()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrFixedTokenizer("XX")
        {
            Escape = ".",
            Comparison = comparison,
            ForceValue = true,
            KeepEscape = true
        };
        StrTokenText text;
        StrTokenFixed fix;
        StrTokenChain chain;

        source = "aaaxx.xxbbb";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aaa", text.Payload);
        fix = Assert.IsType<StrTokenFixed>(chain[1]); Assert.Equal("XX", fix.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal(".XXbbb", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Chain_Source()
    {
        StrTokenChain source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrFixedTokenizer("xx") { Escape = ".", Comparison = comparison };
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
    }
}
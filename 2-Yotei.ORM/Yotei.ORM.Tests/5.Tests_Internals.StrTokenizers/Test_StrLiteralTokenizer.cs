namespace Yotei.ORM.Tests.Internals.StrTokenizers;

// ========================================================
//[Enforced]
public static class Test_StrLiteralTokenizer
{
    //[Enforced]
    [Fact]
    public static void Test_NotFound()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrLiteralTokenizer("XX") { Escape = ".", Comparison = comparison };
        StrTokenText text;

        source = string.Empty;
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Empty(text.Payload);

        source = " ";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(" ", text.Payload);

        source = "hello";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("hello", text.Payload);

        try { _ = tokenizer.Tokenize((string)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_String_Source()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrLiteralTokenizer("XX") { Escape = ".", Comparison = comparison };
        StrTokenText text;
        StrTokenLiteral item;
        StrTokenChain chain;

        source = "aaxx.xxbb";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aa", text.Payload);
        item = Assert.IsType<StrTokenLiteral>(chain[1]); Assert.Equal("XX", item.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal(".xxbb", text.Payload);

        var xtokenizer = tokenizer.WithValueFromSource(true).WithRemoveEscape(true);
        target = xtokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aa", text.Payload);
        item = Assert.IsType<StrTokenLiteral>(chain[1]); Assert.Equal("xx", item.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal("xxbb", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Chained_Source()
    {
        StrTokenChain source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrLiteralTokenizer("XX") { Escape = ".", Comparison = comparison };
        StrTokenText text;
        StrTokenLiteral item;
        StrTokenChain chain;

        source = new([
            new StrTokenText("aa"),
            StrTokenText.Empty,
            new StrTokenText("xx"),
            new StrTokenText("."),
            new StrTokenText("xx"),
            new StrTokenText("bb"),
            ]);

        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aa", text.Payload);
        item = Assert.IsType<StrTokenLiteral>(chain[1]); Assert.Equal("XX", item.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal(".xxbb", text.Payload);

        var xtokenizer = tokenizer.WithValueFromSource(true).WithRemoveEscape(true);
        target = xtokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aa", text.Payload);
        item = Assert.IsType<StrTokenLiteral>(chain[1]); Assert.Equal("xx", item.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal("xxbb", text.Payload);
    }
}
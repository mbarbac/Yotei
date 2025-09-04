namespace Yotei.ORM.Tests.Internals.StrTokenizers;

// ========================================================
//[Enforced]
public static class Test_StrLiteralTokenizer
{
    //[Enforced]
    [Fact]
    public static void Test_NotFound()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrLiteralTokenizer("XX") { Escape = ".", Comparison = comparison };
        string source;
        IStrToken target;
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
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrLiteralTokenizer("XX") { Escape = ".", Comparison = comparison };
        string source;
        IStrToken target;
        StrTokenText text;
        StrTokenChain chain;
        StrTokenLiteral literal;

        source = "aaxxbb.xxcc";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aa", text.Payload);
        literal = Assert.IsType<StrTokenLiteral>(chain[1]); Assert.Equal("xx", literal.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal("bbxxcc", text.Payload);

        tokenizer = tokenizer with { KeepTarget = true, KeepEscape = true };
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aa", text.Payload);
        literal = Assert.IsType<StrTokenLiteral>(chain[1]); Assert.Equal("XX", literal.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal("bb.xxcc", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Chained_Source()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrLiteralTokenizer("XX") { Escape = ".", Comparison = comparison };
        StrTokenChain source;
        IStrToken target;
        StrTokenText text;
        StrTokenChain chain;
        StrTokenLiteral literal;

        source = new([
            new StrTokenText("aa"),
            new StrTokenText("xx"),
            new StrTokenText("bb"),
            new StrTokenText("."),
            new StrTokenText("xx"),
            new StrTokenText("cc")]);

        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aa", text.Payload);
        literal = Assert.IsType<StrTokenLiteral>(chain[1]); Assert.Equal("xx", literal.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal("bbxxcc", text.Payload);

        tokenizer = tokenizer with { KeepTarget = true, KeepEscape = true };
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aa", text.Payload);
        literal = Assert.IsType<StrTokenLiteral>(chain[1]); Assert.Equal("XX", literal.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal("bb.xxcc", text.Payload);
    }
}
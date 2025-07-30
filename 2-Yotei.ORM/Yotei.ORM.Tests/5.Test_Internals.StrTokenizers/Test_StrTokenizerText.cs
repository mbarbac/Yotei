namespace Yotei.ORM.Tests.Internals.StrTokenizers;

// Note that StrTokenText is somehow a special case, because it is reduced by default. Hence
// why in the following tests we use 'reduce: false' to validate the results that, otherwise,
// would have been reduced into combined text ones.

// ========================================================
//[Enforced]
public static class Test_TokenizerText
{
    //[Enforced]
    [Fact]
    public static void Test_NotFound()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrTokenizerText("XX") { Escape = ".", Comparison = comparison };
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
        var tokenizer = new StrTokenizerText("XX") { Escape = ".", Comparison = comparison };
        string source;
        IStrToken target;
        StrTokenText text;
        StrTokenChain chain;

        source = "aaxxbb.xxcc";
        target = tokenizer.Tokenize(source, reduce: false);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aa", text.Payload);
        text = Assert.IsType<StrTokenText>(chain[1]); Assert.Equal("xx", text.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal("bbxxcc", text.Payload);

        tokenizer = tokenizer with { KeepValue = true, KeepEscape = true };
        target = tokenizer.Tokenize(source, reduce: false);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aa", text.Payload);
        text = Assert.IsType<StrTokenText>(chain[1]); Assert.Equal("XX", text.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal("bb.xxcc", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Chained_Source()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrTokenizerText("XX") { Escape = ".", Comparison = comparison };
        StrTokenChain source;
        IStrToken target;
        StrTokenText text;
        StrTokenChain chain;

        source = new([
            new StrTokenText("aa"),
            new StrTokenText("xx"),
            new StrTokenText("bb"),
            new StrTokenText("."),
            new StrTokenText("xx"),
            new StrTokenText("cc")]);

        target = tokenizer.Tokenize(source, reduce: false);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aa", text.Payload);
        text = Assert.IsType<StrTokenText>(chain[1]); Assert.Equal("xx", text.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal("bbxxcc", text.Payload);

        tokenizer = tokenizer with { KeepValue = true, KeepEscape = true };
        target = tokenizer.Tokenize(source, reduce: false);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aa", text.Payload);
        text = Assert.IsType<StrTokenText>(chain[1]); Assert.Equal("XX", text.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal("bb.xxcc", text.Payload);
    }
}
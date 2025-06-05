using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_StrKeywordTokenizer
{
    //[Enforced]
    [Fact]
    public static void Test_Not_Found()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrKeywordTokenizer("XX") { Escape = ".", Comparison = comparison };
        StrTokenText text;

        try { target = tokenizer.Tokenize((string)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        source = string.Empty;
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Empty(text.Payload);

        source = " ";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(" ", text.Payload);

        source = "whatever";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("whatever", target.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Isolated()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrKeywordTokenizer("XX") { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithKeywordFromSource(true).WithRemoveEscape(true);
        StrTokenText text;
        StrTokenKeyword word;

        source = "xx";
        target = tokenizer.Tokenize(source);
        word = Assert.IsType<StrTokenKeyword>(target); Assert.Equal("XX", word.Payload);

        target = xtokenizer.Tokenize(source);
        word = Assert.IsType<StrTokenKeyword>(target); Assert.Equal("xx", word.Payload);

        source = ".xx";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(".xx", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("xx", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Heading()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrKeywordTokenizer("XX") { Escape = ".", Comparison = comparison };
        StrTokenText text;
        StrTokenKeyword word;
        StrTokenChain chain;

        source = "xxother";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("xxother", text.Payload);

        source = " xxother";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(" xxother", text.Payload);

        source = "xx other";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(2, chain.Count);
        word = Assert.IsType<StrTokenKeyword>(chain[0]); Assert.Equal("XX", word.Payload);
        text = Assert.IsType<StrTokenText>(chain[1]); Assert.Equal(" other", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Middle()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrKeywordTokenizer("XX") { Escape = ".", Comparison = comparison };
        StrTokenText text;
        StrTokenKeyword word;
        StrTokenChain chain;

        //source = "any(xx)other";
        //target = tokenizer.Tokenize(source);
        //chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        //text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("any(", text.Payload);
        //word = Assert.IsType<StrTokenKeyword>(chain[1]); Assert.Equal("XX", word.Payload);
        //text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal(")other", text.Payload);

        tokenizer = new StrKeywordTokenizer("=") { Escape = ".", Comparison = comparison };
        source = "x=50";
        target = tokenizer.Tokenize(source);
    }

    //[Enforced]
    [Fact]
    public static void Test_Tailing()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrKeywordTokenizer("XX") { Escape = ".", Comparison = comparison };
        StrTokenText text;
        StrTokenKeyword word;
        StrTokenChain chain;

        source = "otherxx";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("otherxx", text.Payload);

        source = "otherxx ";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("otherxx ", text.Payload);

        source = "other xx";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(2, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("other ", text.Payload);
        word = Assert.IsType<StrTokenKeyword>(chain[1]); Assert.Equal("XX", word.Payload);
    }
}
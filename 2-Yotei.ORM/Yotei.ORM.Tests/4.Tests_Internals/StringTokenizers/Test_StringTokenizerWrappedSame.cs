using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_StringTokenizerWrappedSame
{
    //[Enforced]
    [Fact]
    public static void Test_No_Separators()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrTokenizerWrapped("X") { Escape = ".", Comparison = comparison };
        StrTokenText text;

        try { target = tokenizer.Tokenize((string)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        source = string.Empty;
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Empty(text.Payload);

        source = " ";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(" ", text.Payload);

        source = "other";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("other", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Isolated_Separator()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrTokenizerWrapped("X") { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithWrappersFromSource(true).WithRemoveEscape(true);
        StrTokenText text;

        source = "x";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("x", text.Payload);

        source = ".x";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(".x", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("x", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Unpaired_Separator()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrTokenizerWrapped("X") { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithWrappersFromSource(true).WithRemoveEscape(true);
        StrTokenText text;

        source = "aax";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("aax", text.Payload);

        source = "xbb";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("xbb", text.Payload);

        source = "aaxbb";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("aaxbb", text.Payload);

        source = "axb.xc";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("axb.xc", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("axbxc", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Empty_Payload()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrTokenizerWrapped("X") { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithWrappersFromSource(true).WithRemoveEscape(true);
        StrTokenText text;
        StrTokenChain chain;
        StrTokenWrapped wrapped;

        source = "xx";
        target = tokenizer.Tokenize(source);
        wrapped = Assert.IsType<StrTokenWrapped>(target);
        Assert.Equal("X", wrapped.Head);
        Assert.Equal("X", wrapped.Tail);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Empty(text.Payload);

        target = xtokenizer.Tokenize(source);
        wrapped = Assert.IsType<StrTokenWrapped>(target);
        Assert.Equal("x", wrapped.Head);
        Assert.Equal("x", wrapped.Tail);

        source = "xxxx";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(2, chain.Count);
        Assert.IsType<StrTokenWrapped>(chain[0]);
        Assert.IsType<StrTokenWrapped>(chain[1]);

        source = "xxx";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(2, chain.Count);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[0]);
        Assert.Equal("X", wrapped.Head);
        Assert.Equal("X", wrapped.Tail);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Empty(text.Payload);
        text = Assert.IsType<StrTokenText>(chain[1]); Assert.Equal("x", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Populated_Payload()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrTokenizerWrapped("X") { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithWrappersFromSource(true).WithRemoveEscape(true);
        StrTokenText text;
        StrTokenChain chain;
        StrTokenWrapped wrapped;

        source = "xaax";
        target = tokenizer.Tokenize(source);
        wrapped = Assert.IsType<StrTokenWrapped>(target);
        Assert.Equal("X", wrapped.Head);
        Assert.Equal("X", wrapped.Tail);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Equal("aa", text.Payload);

        source = "aaxbbxcc";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aa", text.Payload);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[1]);
        Assert.Equal("X", wrapped.Head);
        Assert.Equal("X", wrapped.Tail);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Equal("bb", text.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal("cc", text.Payload);

        source = "xaa.x";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("xaa.x", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("xaax", text.Payload);
    }
}
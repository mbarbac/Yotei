#pragma warning disable IDE0059

namespace Yotei.ORM.Tests.Internals.StrTokenizers;

// ========================================================
//[Enforced]
public static class Test_WrapSameTokenizer
{
    //[Enforced]
    [Fact]
    public static void Test_No_Terminators()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrapTokenizer('X') { Escape = ".", Comparison = comparison };
        string source;
        IStrToken target;
        StrTokenText text;

        source = string.Empty;
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(source, text.Payload);

        source = " ";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(source, text.Payload);

        source = "hello";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(source, text.Payload);

        try { _ = tokenizer.Tokenize((string)null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Isolated_Terminators()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrapTokenizer('X') { Escape = ".", Comparison = comparison };
        var keeper = tokenizer with { KeepEscape = true };
        string source;
        IStrToken target;
        StrTokenText text;

        source = "x";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(source, text.Payload);

        source = ".x";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("x", text.Payload);
        target = keeper.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(source, text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Unpaired_Terminators()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrapTokenizer('X') { Escape = ".", Comparison = comparison };
        var keeper = tokenizer with { KeepEscape = true };
        string source;
        IStrToken target;
        StrTokenText text;

        source = "aax";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(source, text.Payload);

        source = "xbb";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(source, text.Payload);

        source = "aaxbb";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(source, text.Payload);

        source = "aa.xbbxcc";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("aaxbbxcc", text.Payload);
        target = keeper.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(source, text.Payload);

        source = "aaxbb.xcc";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("aaxbbxcc", text.Payload);
        target = keeper.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(source, text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Empty_Payload()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrapTokenizer('X') { Escape = ".", Comparison = comparison };
        var keeper = tokenizer with { KeepTerminators = true, KeepEscape = true };
        string source;
        IStrToken target;
        StrTokenText text;
        StrTokenChain chain;
        StrTokenWrapped wrapped;

        source = "xx";
        target = tokenizer.Tokenize(source);
        wrapped = Assert.IsType<StrTokenWrapped>(target);
        Assert.Equal("x", wrapped.Head);
        Assert.Equal("x", wrapped.Tail);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Empty(text.Payload);

        target = keeper.Tokenize(source);
        wrapped = Assert.IsType<StrTokenWrapped>(target);
        Assert.Equal("X", wrapped.Head);
        Assert.Equal("X", wrapped.Tail);

        source = "xxxx";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(2, chain.Count);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[0]);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Empty(text.Payload);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[1]);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Empty(text.Payload);

        source = "xxx";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(2, chain.Count);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[0]);
        Assert.Equal("x", wrapped.Head);
        Assert.Equal("x", wrapped.Tail);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Empty(text.Payload);
        text = Assert.IsType<StrTokenText>(chain[1]); Assert.Equal("x", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Populated_Payload()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrapTokenizer('X') { Escape = ".", Comparison = comparison };
        var keeper = tokenizer with { KeepEscape = true };
        string source;
        IStrToken target;
        StrTokenText text;
        StrTokenChain chain;
        StrTokenWrapped wrapped;

        source = "xaax";
        target = tokenizer.Tokenize(source);
        wrapped = Assert.IsType<StrTokenWrapped>(target);
        Assert.Equal("x", wrapped.Head);
        Assert.Equal("x", wrapped.Tail);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Equal("aa", text.Payload);

        source = "aaxbbxcc";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aa", text.Payload);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[1]);
        Assert.Equal("x", wrapped.Head);
        Assert.Equal("x", wrapped.Tail);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Equal("bb", text.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal("cc", text.Payload);

        source = "xaa.x";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("xaax", text.Payload);
        target = keeper.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(source, text.Payload);
    }
}
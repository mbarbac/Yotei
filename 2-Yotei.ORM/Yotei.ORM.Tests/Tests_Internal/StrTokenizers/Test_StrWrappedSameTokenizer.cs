using Text = Yotei.ORM.Internal.StrTokenText;
using Chain = Yotei.ORM.Internal.StrTokenChain;
using Wrapped = Yotei.ORM.Internal.StrTokenWrapped;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_StrWrappedSameTokenizer
{
    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer("X") { Comparison = comparison };
        Text text;

        source = null!;
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Empty(text.Payload);

        source = "";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Empty(text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_No_Separators()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer("X") { Comparison = comparison };
        Text text;

        source = "aa";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("aa", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Isolated_Separator()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer("X") { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithUseTokenizerWrappers(true).WithKeepEscape(true);
        Text text;

        source = "x";

        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("x", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("x", text.Payload);

        source = ".x";

        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("x", text.Payload);
        
        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal(".x", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Unpaired_Separator()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer("X") { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithUseTokenizerWrappers(true).WithKeepEscape(true);
        Text text;

        source = "aax";

        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("aax", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("aax", text.Payload);

        source = "xbb";

        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("xbb", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("xbb", text.Payload);

        source = "aaxbb";

        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("aaxbb", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("aaxbb", text.Payload);

        source = "aaxbb.x";

        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("aaxbbx", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("aaxbb.x", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Empty_Payload()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer("X") { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithUseTokenizerWrappers(true).WithKeepEscape(true);
        Text text;
        Chain chain;
        Wrapped wrapped;

        source = "xx";

        target = tokenizer.Tokenize(source);
        wrapped = Assert.IsType<Wrapped>(target);
        Assert.Equal("x", wrapped.Head);
        Assert.Equal("x", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Empty(text.Payload);

        target = xtokenizer.Tokenize(source);
        wrapped = Assert.IsType<Wrapped>(target);
        Assert.Equal("X", wrapped.Head);
        Assert.Equal("X", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Empty(text.Payload);

        source = ".xx";

        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("xx", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal(".xx", text.Payload);

        source = "xxx";

        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<Chain>(target); Assert.Equal(2, chain.Count);
        wrapped = Assert.IsType<Wrapped>(chain[0]);
        Assert.Equal("x", wrapped.Head);
        Assert.Equal("x", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Empty(text.Payload);
        text = Assert.IsType<Text>(chain[1]); Assert.Equal("x", text.Payload);

        target = xtokenizer.Tokenize(source);
        chain = Assert.IsType<Chain>(target); Assert.Equal(2, chain.Count);
        wrapped = Assert.IsType<Wrapped>(chain[0]);
        Assert.Equal("X", wrapped.Head);
        Assert.Equal("X", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Empty(text.Payload);
        text = Assert.IsType<Text>(chain[1]); Assert.Equal("x", text.Payload);

        source = "x.x";

        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("xx", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("x.x", text.Payload);

        source = "xxxx";

        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<Chain>(target); Assert.Equal(2, chain.Count);
        wrapped = Assert.IsType<Wrapped>(chain[0]);
        Assert.Equal("x", wrapped.Head);
        Assert.Equal("x", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Empty(text.Payload);
        wrapped = Assert.IsType<Wrapped>(chain[1]);
        Assert.Equal("x", wrapped.Head);
        Assert.Equal("x", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Empty(text.Payload);

        target = xtokenizer.Tokenize(source);
        chain = Assert.IsType<Chain>(target); Assert.Equal(2, chain.Count);
        wrapped = Assert.IsType<Wrapped>(chain[0]);
        Assert.Equal("X", wrapped.Head);
        Assert.Equal("X", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Empty(text.Payload);
        wrapped = Assert.IsType<Wrapped>(chain[1]);
        Assert.Equal("X", wrapped.Head);
        Assert.Equal("X", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Empty(text.Payload);

        source = "x.xxx";

        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<Chain>(target); Assert.Equal(2, chain.Count);
        wrapped = Assert.IsType<Wrapped>(chain[0]);
        Assert.Equal("x", wrapped.Head);
        Assert.Equal("x", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Equal("x", text.Payload);
        text = Assert.IsType<Text>(chain[1]); Assert.Equal("x", text.Payload);

        target = xtokenizer.Tokenize(source);
        chain = Assert.IsType<Chain>(target); Assert.Equal(2, chain.Count);
        wrapped = Assert.IsType<Wrapped>(chain[0]);
        Assert.Equal("X", wrapped.Head);
        Assert.Equal("X", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Equal(".x", text.Payload);
        text = Assert.IsType<Text>(chain[1]); Assert.Equal("x", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Payload()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer("X") { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithUseTokenizerWrappers(true).WithKeepEscape(true);
        Text text;
        Chain chain;
        Wrapped wrapped;

        source = "xaax";

        target = tokenizer.Tokenize(source);
        wrapped = Assert.IsType<Wrapped>(target);
        Assert.Equal("x", wrapped.Head);
        Assert.Equal("x", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Equal("aa", text.Payload);

        target = xtokenizer.Tokenize(source);
        wrapped = Assert.IsType<Wrapped>(target);
        Assert.Equal("X", wrapped.Head);
        Assert.Equal("X", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Equal("aa", text.Payload);

        source = "xaa.x";

        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("xaax", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("xaa.x", text.Payload);

        source = "aaxbbx";

        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<Chain>(target); Assert.Equal(2, chain.Count);
        text = Assert.IsType<Text>(chain[0]); Assert.Equal("aa", text.Payload);
        wrapped = Assert.IsType<Wrapped>(chain[1]);
        Assert.Equal("x", wrapped.Head);
        Assert.Equal("x", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Equal("bb", text.Payload);

        target = xtokenizer.Tokenize(source);
        chain = Assert.IsType<Chain>(target); Assert.Equal(2, chain.Count);
        text = Assert.IsType<Text>(chain[0]); Assert.Equal("aa", text.Payload);
        wrapped = Assert.IsType<Wrapped>(chain[1]);
        Assert.Equal("X", wrapped.Head);
        Assert.Equal("X", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Equal("bb", text.Payload);

        source = "aa.xbbx";

        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("aaxbbx", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("aa.xbbx", text.Payload);

        source = "aaxbbx" + "xccxdd";

        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<Chain>(target); Assert.Equal(4, chain.Count);
        text = Assert.IsType<Text>(chain[0]); Assert.Equal("aa", text.Payload);
        wrapped = Assert.IsType<Wrapped>(chain[1]);
        Assert.Equal("x", wrapped.Head);
        Assert.Equal("x", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Equal("bb", text.Payload);
        wrapped = Assert.IsType<Wrapped>(chain[2]);
        Assert.Equal("x", wrapped.Head);
        Assert.Equal("x", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Equal("cc", text.Payload);
        text = Assert.IsType<Text>(chain[3]); Assert.Equal("dd", text.Payload);

        target = xtokenizer.Tokenize(source);
        chain = Assert.IsType<Chain>(target); Assert.Equal(4, chain.Count);
        text = Assert.IsType<Text>(chain[0]); Assert.Equal("aa", text.Payload);
        wrapped = Assert.IsType<Wrapped>(chain[1]);
        Assert.Equal("X", wrapped.Head);
        Assert.Equal("X", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Equal("bb", text.Payload);
        wrapped = Assert.IsType<Wrapped>(chain[2]);
        Assert.Equal("X", wrapped.Head);
        Assert.Equal("X", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Equal("cc", text.Payload);
        text = Assert.IsType<Text>(chain[3]); Assert.Equal("dd", text.Payload);

        source = "aaxbbx" + ".xccxdd";

        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<Chain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<Text>(chain[0]); Assert.Equal("aa", text.Payload);
        wrapped = Assert.IsType<Wrapped>(chain[1]);
        Assert.Equal("x", wrapped.Head);
        Assert.Equal("x", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Equal("bb", text.Payload);
        text = Assert.IsType<Text>(chain[2]); Assert.Equal("xccxdd", text.Payload);

        target = xtokenizer.Tokenize(source);
        chain = Assert.IsType<Chain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<Text>(chain[0]); Assert.Equal("aa", text.Payload);
        wrapped = Assert.IsType<Wrapped>(chain[1]);
        Assert.Equal("X", wrapped.Head);
        Assert.Equal("X", wrapped.Tail);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Equal("bb", text.Payload);
        text = Assert.IsType<Text>(chain[2]); Assert.Equal(".xccxdd", text.Payload);
    }
}
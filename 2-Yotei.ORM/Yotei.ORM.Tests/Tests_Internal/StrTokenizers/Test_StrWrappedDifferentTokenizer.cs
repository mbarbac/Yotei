using Text = Yotei.ORM.Internal.StrTokenText;
using Chain = Yotei.ORM.Internal.StrTokenChain;
using Wrapped = Yotei.ORM.Internal.StrTokenWrapped;
using Xunit.Sdk;

namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_StrWrappedDifferentTokenizer
{
    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('(', ')') { Comparison = comparison };
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
        var tokenizer = new StrWrappedTokenizer('(', ')') { Comparison = comparison };
        Text text;

        source = "aa";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("aa", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Isolated_Separators()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('(', ')') { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithUseTokenizerWrappers(true).WithKeepEscape(true);
        Text text;

        source = "(";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("(", text.Payload);

        source = ")";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal(")", text.Payload);

        source = ".(";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("(", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal(".(", text.Payload);

        source = ".)";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal(")", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal(".)", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Unpaired_Separator()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('(', ')') { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithUseTokenizerWrappers(true).WithKeepEscape(true);
        Text text;

        source = ")(";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal(")(", text.Payload);

        source = "aa)bb(cc";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("aa)bb(cc", text.Payload);
    }


    //[Enforced]
    [Fact]
    public static void Test_Empty_Payload()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('(', ')') { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithUseTokenizerWrappers(true).WithKeepEscape(true);
        Text text;
        Chain chain;
        Wrapped wrapped;

        source = "()";
        target = tokenizer.Tokenize(source);
        wrapped = Assert.IsType<Wrapped>(target);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Empty(text.Payload);

        source = ".()";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("()", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal(".()", text.Payload);

        source = "(.)";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("()", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("(.)", text.Payload);

        source = "()()";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<Chain>(target); Assert.Equal(2, chain.Count);
        wrapped = Assert.IsType<Wrapped>(chain[0]);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Empty(text.Payload);
        wrapped = Assert.IsType<Wrapped>(chain[1]);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Empty(text.Payload);

        source = "().()";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<Chain>(target); Assert.Equal(2, chain.Count);
        wrapped = Assert.IsType<Wrapped>(chain[0]);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Empty(text.Payload);
        text = Assert.IsType<Text>(chain[1]); Assert.Equal("()", text.Payload);

        target = xtokenizer.Tokenize(source);
        chain = Assert.IsType<Chain>(target); Assert.Equal(2, chain.Count);
        wrapped = Assert.IsType<Wrapped>(chain[0]);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Empty(text.Payload);
        text = Assert.IsType<Text>(chain[1]); Assert.Equal(".()", text.Payload);

        source = "(()";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<Chain>(target); Assert.Equal(2, chain.Count);
        text = Assert.IsType<Text>(chain[0]); Assert.Equal("(", text.Payload);
        wrapped = Assert.IsType<Wrapped>(chain[1]);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Empty(text.Payload);

        source = "())";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<Chain>(target); Assert.Equal(2, chain.Count);
        wrapped = Assert.IsType<Wrapped>(chain[0]);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Empty(text.Payload);
        text = Assert.IsType<Text>(chain[1]); Assert.Equal(")", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Payload()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('(', ')') { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithUseTokenizerWrappers(true).WithKeepEscape(true);
        Text text;
        Chain chain;
        Wrapped wrapped;

        source = "(bb)";
        target = tokenizer.Tokenize(source);
        wrapped = Assert.IsType<Wrapped>(target);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Equal("bb", text.Payload);

        source = "aa(bb)cc";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<Chain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<Text>(chain[0]); Assert.Equal("aa", text.Payload);
        wrapped = Assert.IsType<Wrapped>(chain[1]);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Equal("bb", text.Payload);
        text = Assert.IsType<Text>(chain[2]); Assert.Equal("cc", text.Payload);

        source = "aa.(bb)cc";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("aa(bb)cc", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("aa.(bb)cc", text.Payload);

        source = "aa(bb.)cc";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("aa(bb)cc", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<Text>(target); Assert.Equal("aa(bb.)cc", text.Payload);

        source = "(aa)(bb)";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<Chain>(target); Assert.Equal(2, chain.Count);
        wrapped = Assert.IsType<Wrapped>(chain[0]);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Equal("aa", text.Payload);
        wrapped = Assert.IsType<Wrapped>(chain[1]);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Equal("bb", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Embedded()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('(', ')') { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithUseTokenizerWrappers(true).WithKeepEscape(true);
        Text text;
        Chain chain;
        Wrapped wrapped;

        source = "((aa))";
        target = tokenizer.Tokenize(source);
        wrapped = Assert.IsType<Wrapped>(target);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Equal("aa", text.Payload);

        source = "((aa)(bb))";
        target = tokenizer.Tokenize(source);
        wrapped = Assert.IsType<Wrapped>(target);
        chain = Assert.IsType<Chain>(wrapped.Payload); Assert.Equal(2, chain.Count);
        wrapped = Assert.IsType<Wrapped>(chain[0]);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Equal("aa", text.Payload);
        wrapped = Assert.IsType<Wrapped>(chain[1]);
        text = Assert.IsType<Text>(wrapped.Payload); Assert.Equal("bb", text.Payload);
    }
}
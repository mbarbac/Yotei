using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_Diff_StrWrappedTokenizer
{
    //[Enforced]
    [Fact]
    public static void Test_No_Separators()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('(', ')') { Escape = ".", Comparison = comparison };
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
        var tokenizer = new StrWrappedTokenizer('(', ')') { Escape = ".", Comparison = comparison };
        StrTokenText text;

        source = "(";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("(", text.Payload);

        source = ")";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(")", text.Payload);

        source = "(.)";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("(.)", text.Payload);

        var xtokenizer = tokenizer.WithWrappersFromSource(true).WithRemoveEscape(true);
        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("()", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Unpaired_Separator()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('(', ')') { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithWrappersFromSource(true).WithRemoveEscape(true);
        StrTokenText text;

        source = ")(";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(")(", text.Payload);

        source = "aa)bb(cc";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("aa)bb(cc", text.Payload);

        source = ".(aa)";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(".(aa)", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("(aa)", text.Payload);

        source = "(aa.)";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("(aa.)", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("(aa)", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Empty_Payload()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('(', ')') { Escape = ".", Comparison = comparison };
        StrTokenText text;
        StrTokenChain chain;
        StrTokenWrapped wrapped;

        source = "()";
        target = tokenizer.Tokenize(source);
        wrapped = Assert.IsType<StrTokenWrapped>(target);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Empty(text.Payload);

        source = "()()";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(2, chain.Count);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[0]);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Empty(text.Payload);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[1]);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Empty(text.Payload);

        source = "(()";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(2, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("(", text.Payload);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[1]);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Empty(text.Payload);

        source = "())";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(2, chain.Count);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[0]);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Empty(text.Payload);
        text = Assert.IsType<StrTokenText>(chain[1]); Assert.Equal(")", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Populated_Payload()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('(', ')') { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithWrappersFromSource(true).WithRemoveEscape(true);
        StrTokenText text;
        StrTokenChain chain;
        StrTokenWrapped wrapped;

        source = "(aa)";
        target = tokenizer.Tokenize(source);
        wrapped = Assert.IsType<StrTokenWrapped>(target);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Equal("aa", text.Payload);

        source = ".(aa)";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(".(aa)", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("(aa)", text.Payload);

        source = "aa(bb)cc";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aa", text.Payload);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[1]);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Equal("bb", text.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal("cc", text.Payload);

        source = "aa.(bb)cc";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("aa.(bb)cc", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("aa(bb)cc", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Chained_Payload()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('(', ')') { Escape = ".", Comparison = comparison };

        StrTokenText text;
        StrTokenChain chain;
        StrTokenWrapped wrapped;

        source = "(aa)(bb)";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(2, chain.Count);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[0]);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Equal("aa", text.Payload);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[1]);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Equal("bb", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Embedded_Payload()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('(', ')') { Escape = ".", Comparison = comparison };
        StrTokenText text;
        StrTokenChain chain;
        StrTokenWrapped wrapped;

        source = "((aa))";
        target = tokenizer.Tokenize(source);
        wrapped = Assert.IsType<StrTokenWrapped>(target);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Equal("aa", text.Payload);

        target = tokenizer.Tokenize(source, reduce: false);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Single(chain);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[0]);
        chain = Assert.IsType<StrTokenChain>(wrapped.Payload); Assert.Single(chain);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[0]);
        chain = Assert.IsType<StrTokenChain>(wrapped.Payload);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aa", text.Payload);

        source = "((aa)(bb))";
        target = tokenizer.Tokenize(source);
        wrapped = Assert.IsType<StrTokenWrapped>(target);
        chain = Assert.IsType<StrTokenChain>(wrapped.Payload); Assert.Equal(2, chain.Count);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[0]);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Equal("aa", text.Payload);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[1]);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Equal("bb", text.Payload);
    }
}
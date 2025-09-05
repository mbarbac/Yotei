namespace Yotei.ORM.Tests.Internals.StrTokenizers;

// ========================================================
//[Enforced]
public static class Test_StrWrappedDistinctTokenizer
{
    //[Enforced]
    [Fact]
    public static void Test_No_Terminators()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('(', ')') { Escape = ".", Comparison = comparison };
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
        var tokenizer = new StrWrappedTokenizer('(', ')') { Escape = ".", Comparison = comparison };
        var keeper = tokenizer with { KeepEscape = true };
        string source;
        IStrToken target;
        StrTokenText text;

        source = "(";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(source, text.Payload);

        source = ")";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(source, text.Payload);

        source = "(.)";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("()", text.Payload);
        target = keeper.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(source, text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Unpaired_Terminators()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('(', ')') { Escape = ".", Comparison = comparison };
        var keeper = tokenizer with { KeepEscape = true };
        string source;
        IStrToken target;
        StrTokenText text;

        source = ")(";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(source, text.Payload);

        source = "aa)bb(cc";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(source, text.Payload);

        source = ".(aa)";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("(aa)", text.Payload);
        target = keeper.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(source, text.Payload);

        source = "(aa.)";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("(aa)", text.Payload);
        target = keeper.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(source, text.Payload);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Empty_Payload()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('(', ')') { Escape = ".", Comparison = comparison };
        string source;
        IStrToken target;
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
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('(', ')') { Escape = ".", Comparison = comparison };
        var keeper = tokenizer with { KeepEscape = true };
        string source;
        IStrToken target;
        StrTokenText text;
        StrTokenChain chain;
        StrTokenWrapped wrapped;

        source = "(aa)";
        target = tokenizer.Tokenize(source);
        wrapped = Assert.IsType<StrTokenWrapped>(target);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Equal("aa", text.Payload);

        source = "(.))";
        target = tokenizer.Tokenize(source);
        wrapped = Assert.IsType<StrTokenWrapped>(target);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Equal(")", text.Payload);

        source = "aa(bb)cc";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(3, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aa", text.Payload);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[1]);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Equal("bb", text.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal("cc", text.Payload);

        source = "aa.(bb)cc";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("aa(bb)cc", text.Payload);
        target = keeper.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal(source, text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Chained_Payload()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('(', ')') { Escape = ".", Comparison = comparison };
        string source;
        IStrToken target;
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

        source = "aa(bb)cc(dd)ee";
        target = tokenizer.Tokenize(source);
        chain = Assert.IsType<StrTokenChain>(target); Assert.Equal(5, chain.Count);
        text = Assert.IsType<StrTokenText>(chain[0]); Assert.Equal("aa", text.Payload);
        text = Assert.IsType<StrTokenText>(chain[2]); Assert.Equal("cc", text.Payload);
        text = Assert.IsType<StrTokenText>(chain[4]); Assert.Equal("ee", text.Payload);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[1]);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Equal("bb", text.Payload);
        wrapped = Assert.IsType<StrTokenWrapped>(chain[3]);
        text = Assert.IsType<StrTokenText>(wrapped.Payload); Assert.Equal("dd", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Embedded_Payload()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('(', ')') { Escape = ".", Comparison = comparison };
        string source;
        IStrToken target;
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
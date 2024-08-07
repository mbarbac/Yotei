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
        StrTokenText text;

        source = null!;
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Empty(text.Payload);

        source = "";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Empty(text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Source_No_Separators()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer("X") { Comparison = comparison };
        StrTokenText text;

        source = "aa";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("aa", text.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Source_Isolated_Separator()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer("X") { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithUseSourceWrappers(true).WithRemoveEscape(true);
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
    public static void Test_Source_Unpaired_Separator()
    {
        string source;
        IStrToken target;
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer("X") { Escape = ".", Comparison = comparison };
        var xtokenizer = tokenizer.WithUseSourceWrappers(true).WithRemoveEscape(true);
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

        source = "aaxbb.x";
        target = tokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("aaxbb.x", text.Payload);

        target = xtokenizer.Tokenize(source);
        text = Assert.IsType<StrTokenText>(target); Assert.Equal("aaxbbx", text.Payload);
    }
}
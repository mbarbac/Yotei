#pragma warning disable IDE0042

namespace Yotei.ORM.Tests.Internals.Miscelanea;

// ========================================================
//[Enforced]
public static class Test_StrExtractor
{
    //[Enforced]
    [Fact]
    public static void Test_Literal_ExtractFirst_NotFound()
    {
        var source = "aa";
        var separator = "==";

        var parts = source.ExtractFirst(separator, false, out var found);
        Assert.False(found);
        Assert.Same(source, parts.Left);
        Assert.Null(parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_ExtractFirst_Found()
    {
        var source = "aa==bb==cc";
        var separator = "==";

        var parts = source.ExtractFirst(separator, false, out var found);
        Assert.True(found);
        Assert.Equal("aa", parts.Left);
        Assert.Equal("bb==cc", parts.Right);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Chain_ExtractFirst_NotFound()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('[', ']') { Comparison = comparison };
        var source = tokenizer.Tokenize("aa");
        var separator = "==";

        var parts = source.ExtractFirst(separator, false, out var found);
        Assert.False(found);
        Assert.Same(source, parts.Left);
        Assert.Null(parts.Right);

        source = tokenizer.Tokenize("[aa]bb");
        parts = source.ExtractFirst(separator, false, out found);
        Assert.False(found);
        Assert.Same(source, parts.Left);
        Assert.Null(parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_Chain_ExtractFirst_Found()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('[', ']') { Comparison = comparison };
        var source = tokenizer.Tokenize("aa==bb==cc");
        var separator = "==";

        var parts = source.ExtractFirst(separator, false, out var found);
        Assert.True(found);
        Assert.Equal("aa", parts.Left.ToString());
        Assert.Equal("bb==cc", parts.Right!.ToString());

        source = tokenizer.Tokenize("[aa==]==bb==[==cc]");
        parts = source.ExtractFirst(separator, false, out found);
        Assert.True(found);
        Assert.Equal("[aa==]", parts.Left.ToString());
        Assert.Equal("bb==[==cc]", parts.Right!.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Literal_ExtractLast_NotFound()
    {
        var source = "aa";
        var separator = "==";

        var parts = source.ExtractLast(separator, false, out var found);
        Assert.False(found);
        Assert.Same(source, parts.Left);
        Assert.Null(parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_Literal_ExtractLast_Found()
    {
        var source = "aa==bb==cc";
        var separator = "==";

        var parts = source.ExtractLast(separator, false, out var found);
        Assert.True(found);
        Assert.Equal("aa==bb", parts.Left);
        Assert.Equal("cc", parts.Right);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Chain_ExtractLast_NotFound()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('[', ']') { Comparison = comparison };
        var source = tokenizer.Tokenize("aa");
        var separator = "==";

        var parts = source.ExtractLast(separator, false, out var found);
        Assert.False(found);
        Assert.Same(source, parts.Left);
        Assert.Null(parts.Right);

        source = tokenizer.Tokenize("[aa]bb");
        parts = source.ExtractLast(separator, false, out found);
        Assert.False(found);
        Assert.Same(source, parts.Left);
        Assert.Null(parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_Chain_ExtractLast_Found()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var tokenizer = new StrWrappedTokenizer('[', ']') { Comparison = comparison };
        var source = tokenizer.Tokenize("aa==bb==cc");
        var separator = "==";

        var parts = source.ExtractLast(separator, false, out var found);
        Assert.True(found);
        Assert.Equal("aa==bb", parts.Left.ToString());
        Assert.Equal("cc", parts.Right!.ToString());

        source = tokenizer.Tokenize("[aa==]==bb==[==cc]");
        parts = source.ExtractLast(separator, false, out found);
        Assert.True(found);
        Assert.Equal("[aa==]==bb", parts.Left.ToString());
        Assert.Equal("[==cc]", parts.Right!.ToString());
    }
}
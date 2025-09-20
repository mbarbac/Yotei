namespace Yotei.ORM.Tests.Internals.Miscelanea;
/*
// ========================================================
//[Enforced]
public static class Test_StrExtractor
{
    //[Enforced]
    [Fact]
    public static void Test_RemoveBrackets_Not_Found()
    {
        var left = "";
        var right = "";
        Assert.False(StrExtractor.RemoveBrackets(ref left, ref right));

        left = "aa";
        right = "bb";
        Assert.False(StrExtractor.RemoveBrackets(ref left, ref right));
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveBrackets_Not_Match()
    {
        var left = "aa)";
        var right = "(bb";
        Assert.False(StrExtractor.RemoveBrackets(ref left, ref right));

        left = "(aa)";
        right = "x";
        Assert.False(StrExtractor.RemoveBrackets(ref left, ref right));

        left = "x";
        right = "(aa)";
        Assert.False(StrExtractor.RemoveBrackets(ref left, ref right));

        left = "(aa)";
        right = "(bb)";
        Assert.False(StrExtractor.RemoveBrackets(ref left, ref right));
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveBrackets_Match()
    {
        var left = " ( aa ";
        var right = " bb ) ";
        Assert.True(StrExtractor.RemoveBrackets(ref left, ref right));
        Assert.Equal("aa ", left);
        Assert.Equal(" bb", right);

        left = " ( aa ";
        right = " bb ) ";
        Assert.True(StrExtractor.RemoveBrackets(ref left, ref right, removeoutside: false));
        Assert.Equal("  aa ", left);
        Assert.Equal(" bb  ", right);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveBrackets_Match_Embedded()
    {
        var left = " ( (aa) ";
        var right = " (bb) ) ";
        Assert.True(StrExtractor.RemoveBrackets(ref left, ref right));
        Assert.Equal("aa ", left);
        Assert.Equal(" bb", right);

        left = " ( (aa) ";
        right = " (bb) ) ";
        Assert.True(StrExtractor.RemoveBrackets(ref left, ref right, removeoutside: false));
        Assert.Equal("  aa ", left);
        Assert.Equal(" bb  ", right);
    }
}*/
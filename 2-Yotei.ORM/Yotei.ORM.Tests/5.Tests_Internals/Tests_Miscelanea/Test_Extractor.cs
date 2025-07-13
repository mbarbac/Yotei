namespace Yotei.ORM.Tests.Internals.Miscelanea;

// ========================================================
//[Enforced]
public static class Test_Extractor
{
    //[Enforced]
    [Fact]
    public static void Test_RemoveRoundedBrackets_NotFound()
    {
        var left = "ab";
        var right = "cd";
        var done = Extractor.RemoveRoundedBrackets(ref left, ref right);
        Assert.False(done);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRoundedBrackets_NotMatching()
    {
        var left = "(ab";
        var right = "cd";
        var done = Extractor.RemoveRoundedBrackets(ref left, ref right);
        Assert.False(done);

        left = "ab";
        right = "cd)";
        done = Extractor.RemoveRoundedBrackets(ref left, ref right);
        Assert.False(done);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRoundedBrackets_Matching()
    {
        var left = "(ab";
        var right = "cd)";
        var done = Extractor.RemoveRoundedBrackets(ref left, ref right);
        Assert.True(done);
        Assert.Equal("ab", left);
        Assert.Equal("cd", right);

        left = "( ab";
        right = "cd )";
        done = Extractor.RemoveRoundedBrackets(ref left, ref right);
        Assert.True(done);
        Assert.Equal(" ab", left);
        Assert.Equal("cd ", right);

        left = " ( ab";
        right = "cd ) ";
        done = Extractor.RemoveRoundedBrackets(ref left, ref right);
        Assert.True(done);
        Assert.Equal(" ab", left);
        Assert.Equal("cd ", right);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractHead_NotFound()
    {
        bool found;

        string source = "abc";
        var parts = Extractor.ExtractHead(source, true, isolated: false, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
        parts = Extractor.ExtractHead(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractHead_StandAlone()
    {
        bool found;

        var source = "xy";
        var parts = Extractor.ExtractHead(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Empty(parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractHead(source, true, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Empty(parts.Main);
        Assert.Equal("xy", parts.Spec);

        source = " xy ";
        parts = Extractor.ExtractHead(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractHead(source, true, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Equal("  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractHead_Regular()
    {
        bool found;

        var source = "xyabc";
        var parts = Extractor.ExtractHead(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("abc", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractHead(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = " xyabc ";
        parts = Extractor.ExtractHead(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal(" abc ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractHead(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = " xy abc ";
        parts = Extractor.ExtractHead(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("  abc ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractHead(source, true, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Equal("  abc ", parts.Main);
        Assert.Equal("xy", parts.Spec);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractTail_NotFound()
    {
        bool found;

        string source = "abc";
        var parts = Extractor.ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
        parts = Extractor.ExtractTail(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractTail_StandAlone()
    {
        bool found;

        var source = "xy";
        var parts = Extractor.ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Empty(parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractTail(source, true, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Empty(parts.Main);
        Assert.Equal("xy", parts.Spec);

        source = " xy ";
        parts = Extractor.ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractTail(source, true, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Equal("  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractTail_Regular()
    {
        bool found;

        var source = "abcxy";
        var parts = Extractor.ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("abc", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractTail(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = " abcxy ";
        parts = Extractor.ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal(" abc ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractTail(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = " abc xy ";
        parts = Extractor.ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal(" abc  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractTail(source, true, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Equal(" abc  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirstSeparator_NotFound()
    {
        bool found;

        string source = "abc";
        var parts = Extractor.ExtractFirstSeparator(source, false, isolated: false, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);
        parts = Extractor.ExtractFirstSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirstSeparator_StandAlone()
    {
        bool found;

        string source = "=";
        var parts = Extractor.ExtractFirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);
        parts = Extractor.ExtractFirstSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);

        source = " =";
        parts = Extractor.ExtractFirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);
        parts = Extractor.ExtractFirstSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);

        source = "= ";
        parts = Extractor.ExtractFirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);
        parts = Extractor.ExtractFirstSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);

        source = " = ";
        parts = Extractor.ExtractFirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);
        parts = Extractor.ExtractFirstSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirstSeparator_Not_Isolated()
    {
        bool found;

        var source = "ab=cd";
        var parts = Extractor.ExtractFirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal("cd", parts.Right);

        source = "ab =cd";
        parts = Extractor.ExtractFirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal("cd", parts.Right);

        source = "ab= cd";
        parts = Extractor.ExtractFirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd", parts.Right);

        source = "ab = cd";
        parts = Extractor.ExtractFirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd", parts.Right);

        source = " ab = cd ";
        parts = Extractor.ExtractFirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd ", parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirstSeparator_Isolated()
    {
        bool found;

        var source = "ab=cd";
        var parts = Extractor.ExtractFirstSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = "ab =cd";
        parts = Extractor.ExtractFirstSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = "ab= cd";
        parts = Extractor.ExtractFirstSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = "ab = cd";
        parts = Extractor.ExtractFirstSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal("ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd", parts.Right);

        source = " ab = cd ";
        parts = Extractor.ExtractFirstSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd ", parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirstSeparator_Several()
    {
        bool found;

        var source = "ab=cd=ef";
        var parts = Extractor.ExtractFirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal("cd=ef", parts.Right);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractLastSeparator_NotFound()
    {
        bool found;

        string source = "abc";
        var parts = Extractor.ExtractLastSeparator(source, false, isolated: false, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);
        parts = Extractor.ExtractLastSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLastSeparator_StandAlone()
    {
        bool found;

        string source = "=";
        var parts = Extractor.ExtractLastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);
        parts = Extractor.ExtractLastSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);

        source = " =";
        parts = Extractor.ExtractLastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);
        parts = Extractor.ExtractLastSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);

        source = "= ";
        parts = Extractor.ExtractLastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);
        parts = Extractor.ExtractLastSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);

        source = " = ";
        parts = Extractor.ExtractLastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);
        parts = Extractor.ExtractLastSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLastSeparator_Not_Isolated()
    {
        bool found;

        var source = "ab=cd";
        var parts = Extractor.ExtractLastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal("cd", parts.Right);

        source = "ab =cd";
        parts = Extractor.ExtractLastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal("cd", parts.Right);

        source = "ab= cd";
        parts = Extractor.ExtractLastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd", parts.Right);

        source = "ab = cd";
        parts = Extractor.ExtractLastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd", parts.Right);

        source = " ab = cd ";
        parts = Extractor.ExtractLastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd ", parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLastSeparator_Isolated()
    {
        bool found;

        var source = "ab=cd";
        var parts = Extractor.ExtractLastSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = "ab =cd";
        parts = Extractor.ExtractLastSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = "ab= cd";
        parts = Extractor.ExtractLastSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = "ab = cd";
        parts = Extractor.ExtractLastSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal("ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd", parts.Right);

        source = " ab = cd ";
        parts = Extractor.ExtractLastSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd ", parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLastSeparator_Several()
    {
        bool found;

        var source = "ab=cd=ef";
        var parts = Extractor.ExtractLastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab=cd", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal("ef", parts.Right);
    }
}
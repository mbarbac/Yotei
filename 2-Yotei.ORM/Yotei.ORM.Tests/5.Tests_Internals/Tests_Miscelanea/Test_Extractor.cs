#pragma warning disable IDE0018
#pragma warning disable IDE0042

namespace Yotei.ORM.Tests.Internals.Miscelanea;

// ========================================================
//[Enforced]
public static class Test_Extractor
{
    //[Enforced]
    [Fact]
    public static void Test_RoundedBrackets_NotFound()
    {
        var left = "ab";
        var right = "cd";
        var done = Extractor.RoundedBrackets(ref left, ref right);
        Assert.False(done);
    }

    //[Enforced]
    [Fact]
    public static void Test_RoundedBrackets_NotMatching()
    {
        var left = "(ab";
        var right = "cd";
        var done = Extractor.RoundedBrackets(ref left, ref right);
        Assert.False(done);

        left = "ab";
        right = "cd)";
        done = Extractor.RoundedBrackets(ref left, ref right);
        Assert.False(done);
    }

    //[Enforced]
    [Fact]
    public static void Test_RoundedBrackets_Matching()
    {
        var left = "(ab";
        var right = "cd)";
        var done = Extractor.RoundedBrackets(ref left, ref right);
        Assert.True(done);
        Assert.Equal("ab", left);
        Assert.Equal("cd", right);

        left = "( ab";
        right = "cd )";
        done = Extractor.RoundedBrackets(ref left, ref right);
        Assert.True(done);
        Assert.Equal(" ab", left);
        Assert.Equal("cd ", right);

        left = " ( ab";
        right = "cd ) ";
        done = Extractor.RoundedBrackets(ref left, ref right);
        Assert.True(done);
        Assert.Equal(" ab", left);
        Assert.Equal("cd ", right);
    }

    //[Enforced]
    [Fact]
    public static void Test_RoundedBrackets_Embedded()
    {
        var left = "(ab)";
        var right = "(cd)";
        var done = Extractor.RoundedBrackets(ref left, ref right);
        Assert.False(done);
        Assert.Equal("(ab)", left);
        Assert.Equal("(cd)", right);

        left = "((ab)";
        right = "(cd))";
        done = Extractor.RoundedBrackets(ref left, ref right);
        Assert.True(done);
        Assert.Equal("(ab)", left);
        Assert.Equal("(cd)", right);

        left = " ( (ab) ";
        right = " (cd) ) ";
        done = Extractor.RoundedBrackets(ref left, ref right);
        Assert.True(done);
        Assert.Equal(" (ab) ", left);
        Assert.Equal(" (cd) ", right);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Head_NotFound()
    {
        bool found;

        string source = "abc";
        var parts = Extractor.Head(source, true, isolated: false, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
        parts = Extractor.Head(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
    }

    //[Enforced]
    [Fact]
    public static void Test_Head_StandAlone()
    {
        bool found;

        var source = "xy";
        var parts = Extractor.Head(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Empty(parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.Head(source, true, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Empty(parts.Main);
        Assert.Equal("xy", parts.Spec);

        source = " xy ";
        parts = Extractor.Head(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.Head(source, true, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Equal("  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
    }

    //[Enforced]
    [Fact]
    public static void Test_Head_Regular()
    {
        bool found;

        var source = "xyabc";
        var parts = Extractor.Head(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("abc", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.Head(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = " xyabc ";
        parts = Extractor.Head(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal(" abc ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.Head(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = " xy abc ";
        parts = Extractor.Head(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("  abc ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.Head(source, true, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Equal("  abc ", parts.Main);
        Assert.Equal("xy", parts.Spec);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Tail_NotFound()
    {
        bool found;

        string source = "abc";
        var parts = Extractor.Tail(source, true, isolated: false, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
        parts = Extractor.Tail(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
    }

    //[Enforced]
    [Fact]
    public static void Test_Tail_StandAlone()
    {
        bool found;

        var source = "xy";
        var parts = Extractor.Tail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Empty(parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.Tail(source, true, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Empty(parts.Main);
        Assert.Equal("xy", parts.Spec);

        source = " xy ";
        parts = Extractor.Tail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.Tail(source, true, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Equal("  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
    }

    //[Enforced]
    [Fact]
    public static void Test_Tail_Regular()
    {
        bool found;

        var source = "abcxy";
        var parts = Extractor.Tail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("abc", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.Tail(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = " abcxy ";
        parts = Extractor.Tail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal(" abc ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.Tail(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = " abc xy ";
        parts = Extractor.Tail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal(" abc  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.Tail(source, true, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Equal(" abc  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_FirstSeparator_NotFound()
    {
        bool found;

        string source = "abc";
        var parts = Extractor.FirstSeparator(source, false, isolated: false, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);
        parts = Extractor.FirstSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_FirstSeparator_StandAlone()
    {
        bool found;

        string source = "=";
        var parts = Extractor.FirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);
        parts = Extractor.FirstSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);

        source = " =";
        parts = Extractor.FirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);
        parts = Extractor.FirstSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);

        source = "= ";
        parts = Extractor.FirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);
        parts = Extractor.FirstSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);

        source = " = ";
        parts = Extractor.FirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);
        parts = Extractor.FirstSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_FirstSeparator_Not_Isolated()
    {
        bool found;

        var source = "ab=cd";
        var parts = Extractor.FirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal("cd", parts.Right);

        source = "ab =cd";
        parts = Extractor.FirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal("cd", parts.Right);

        source = "ab= cd";
        parts = Extractor.FirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd", parts.Right);

        source = "ab = cd";
        parts = Extractor.FirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd", parts.Right);

        source = " ab = cd ";
        parts = Extractor.FirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd ", parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_FirstSeparator_Isolated()
    {
        bool found;

        var source = "ab=cd";
        var parts = Extractor.FirstSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = "ab =cd";
        parts = Extractor.FirstSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = "ab= cd";
        parts = Extractor.FirstSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = "ab = cd";
        parts = Extractor.FirstSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal("ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd", parts.Right);

        source = " ab = cd ";
        parts = Extractor.FirstSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd ", parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_FirstSeparator_Several()
    {
        bool found;

        var source = "ab=cd=ef";
        var parts = Extractor.FirstSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal("cd=ef", parts.Right);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_LastSeparator_NotFound()
    {
        bool found;

        string source = "abc";
        var parts = Extractor.LastSeparator(source, false, isolated: false, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);
        parts = Extractor.LastSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_LastSeparator_StandAlone()
    {
        bool found;

        string source = "=";
        var parts = Extractor.LastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);
        parts = Extractor.LastSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);

        source = " =";
        parts = Extractor.LastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);
        parts = Extractor.LastSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);

        source = "= ";
        parts = Extractor.LastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);
        parts = Extractor.LastSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);

        source = " = ";
        parts = Extractor.LastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);
        parts = Extractor.LastSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_LastSeparator_Not_Isolated()
    {
        bool found;

        var source = "ab=cd";
        var parts = Extractor.LastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal("cd", parts.Right);

        source = "ab =cd";
        parts = Extractor.LastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal("cd", parts.Right);

        source = "ab= cd";
        parts = Extractor.LastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd", parts.Right);

        source = "ab = cd";
        parts = Extractor.LastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd", parts.Right);

        source = " ab = cd ";
        parts = Extractor.LastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd ", parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_LastSeparator_Isolated()
    {
        bool found;

        var source = "ab=cd";
        var parts = Extractor.LastSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = "ab =cd";
        parts = Extractor.LastSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = "ab= cd";
        parts = Extractor.LastSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = "ab = cd";
        parts = Extractor.LastSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal("ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd", parts.Right);

        source = " ab = cd ";
        parts = Extractor.LastSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd ", parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_LastSeparator_Several()
    {
        bool found;

        var source = "ab=cd=ef";
        var parts = Extractor.LastSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab=cd", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal("ef", parts.Right);
    }
}
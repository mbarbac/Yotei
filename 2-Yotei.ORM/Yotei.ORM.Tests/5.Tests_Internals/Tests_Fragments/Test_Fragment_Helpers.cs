#pragma warning disable IDE0042

namespace Yotei.ORM.Tests.Internals.Fragments;

// ========================================================
//[Enforced]
public static class Test_Fragment_Helpers
{
    //[Enforced]
    [Fact]
    public static void Test_RemoveRoundedBrackets_NotFound()
    {
        var left = "ab";
        var right = "cd";
        var done = Fragment.RemoveRoundedBrackets(ref left, ref right);
        Assert.False(done);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRoundedBrackets_NotMatching()
    {
        var left = "(ab";
        var right = "cd";
        var done = Fragment.RemoveRoundedBrackets(ref left, ref right);
        Assert.False(done);

        left = "ab";
        right = "cd)";
        done = Fragment.RemoveRoundedBrackets(ref left, ref right);
        Assert.False(done);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRoundedBrackets_Matching()
    {
        var left = "(ab";
        var right = "cd)";
        var done = Fragment.RemoveRoundedBrackets(ref left, ref right);
        Assert.True(done);
        Assert.Equal("ab", left);
        Assert.Equal("cd", right);

        left = "( ab";
        right = "cd )";
        done = Fragment.RemoveRoundedBrackets(ref left, ref right);
        Assert.True(done);
        Assert.Equal(" ab", left);
        Assert.Equal("cd ", right);

        left = " ( ab";
        right = "cd ) ";
        done = Fragment.RemoveRoundedBrackets(ref left, ref right);
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
        var parts = Fragment.ExtractHead(source, true, isolated: false, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
        
        parts = Fragment.ExtractHead(source, true, isolated: true, out found, "xy");
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
        var parts = Fragment.ExtractHead(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Empty(parts.Main);
        Assert.Equal("xy", parts.Spec);
        
        parts = Fragment.ExtractHead(source, true, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Empty(parts.Main);
        Assert.Equal("xy", parts.Spec);

        source = " xy ";
        parts = Fragment.ExtractHead(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        
        parts = Fragment.ExtractHead(source, true, isolated: true, out found, "xy");
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
        var parts = Fragment.ExtractHead(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("abc", parts.Main);
        Assert.Equal("xy", parts.Spec);
        
        parts = Fragment.ExtractHead(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = " xyabc ";
        parts = Fragment.ExtractHead(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal(" abc ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        
        parts = Fragment.ExtractHead(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = " xy abc ";
        parts = Fragment.ExtractHead(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("  abc ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        
        parts = Fragment.ExtractHead(source, true, isolated: true, out found, "xy");
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
        var parts = Fragment.ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
        
        parts = Fragment.ExtractTail(source, true, isolated: true, out found, "xy");
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
        var parts = Fragment.ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Empty(parts.Main);
        Assert.Equal("xy", parts.Spec);
        
        parts = Fragment.ExtractTail(source, true, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Empty(parts.Main);
        Assert.Equal("xy", parts.Spec);

        source = " xy ";
        parts = Fragment.ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        
        parts = Fragment.ExtractTail(source, true, isolated: true, out found, "xy");
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
        var parts = Fragment.ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("abc", parts.Main);
        Assert.Equal("xy", parts.Spec);
        
        parts = Fragment.ExtractTail(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = " abcxy ";
        parts = Fragment.ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal(" abc ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        
        parts = Fragment.ExtractTail(source, true, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = " abc xy ";
        parts = Fragment.ExtractTail(source, true, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal(" abc  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        
        parts = Fragment.ExtractTail(source, true, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Equal(" abc  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractSeparator_NotFound()
    {
        bool found;

        string source = "abc";
        var parts = Fragment.ExtractSeparator(source, false, isolated: false, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        parts = Fragment.ExtractSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractSeparator_StandAlone()
    {
        bool found;

        string source = "=";
        var parts = Fragment.ExtractSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);

        parts = Fragment.ExtractSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);

        source = " =";
        parts = Fragment.ExtractSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);

        parts = Fragment.ExtractSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);

        source = "= ";
        parts = Fragment.ExtractSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);

        parts = Fragment.ExtractSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);

        source = " = ";
        parts = Fragment.ExtractSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);

        parts = Fragment.ExtractSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractSeparator_Not_Isolated()
    {
        bool found;

        var source = "ab=cd";
        var parts = Fragment.ExtractSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal("cd", parts.Right);

        source = "ab =cd";
        parts = Fragment.ExtractSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal("cd", parts.Right);

        source = "ab= cd";
        parts = Fragment.ExtractSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd", parts.Right);

        source = "ab = cd";
        parts = Fragment.ExtractSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd", parts.Right);

        source = " ab = cd ";
        parts = Fragment.ExtractSeparator(source, false, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd ", parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractSeparator_Isolated()
    {
        bool found;

        var source = "ab=cd";
        var parts = Fragment.ExtractSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = "ab =cd";
        parts = Fragment.ExtractSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = "ab= cd";
        parts = Fragment.ExtractSeparator(source, false, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = "ab = cd";
        parts = Fragment.ExtractSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal("ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd", parts.Right);

        source = " ab = cd ";
        parts = Fragment.ExtractSeparator(source, false, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd ", parts.Right);
    }
}
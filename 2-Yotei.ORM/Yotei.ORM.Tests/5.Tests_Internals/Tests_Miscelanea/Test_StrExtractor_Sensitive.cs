#pragma warning disable IDE0018
#pragma warning disable IDE0059

namespace Yotei.ORM.Tests.Internals.Miscelanea;

// ========================================================
//[Enforced]
public static class Test_StrExtractor_Sensitive
{
    //[Enforced]
    [Fact]
    public static void Test_RemoveRoundedBrackets_Not_Found()
    {
        bool done;
        string head, spec, tail;

        "aa=bb".ExtractFirst(false, isolated: false, out head, out spec, out tail, "=");
        done = StrExtractor.RemoveBrackets(ref head, ref tail);
        Assert.False(done);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRoundedBrackets_Not_Matching()
    {
        bool done;
        string head, spec, tail;

        "(aa=bb".ExtractFirst(false, isolated: false, out head, out spec, out tail, "=");
        done = StrExtractor.RemoveBrackets(ref head, ref tail);
        Assert.False(done);

        "aa=bb)".ExtractFirst(false, isolated: false, out head, out spec, out tail, "=");
        done = StrExtractor.RemoveBrackets(ref head, ref tail);
        Assert.False(done);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRoundedBrackets_Matching()
    {
        bool done;
        string head, spec, tail;

        "(aa=bb)".ExtractFirst(false, isolated: false, out head, out spec, out tail, "=");
        done = StrExtractor.RemoveBrackets(ref head, ref tail);
        Assert.True(done);
        Assert.Equal("aa", head);
        Assert.Equal("bb", tail);

        " ( aa = bb ) ".ExtractFirst(false, isolated: false, out head, out spec, out tail, "=");
        done = StrExtractor.RemoveBrackets(ref head, ref tail);
        Assert.True(done);
        Assert.Equal(" aa ", head);
        Assert.Equal(" bb ", tail);
    }

    //[Enforced]
    [Fact]
    public static void Test_RemoveRoundedBrackets_Embedded()
    {
        bool done;
        string head, spec, tail;

        "((aa)=(bb))".ExtractFirst(false, isolated: false, out head, out spec, out tail, "=");
        done = StrExtractor.RemoveBrackets(ref head, ref tail);
        Assert.True(done);
        Assert.Equal("(aa)", head);
        Assert.Equal("(bb)", tail);

        " ( (aa) = (bb) ) ".ExtractFirst(false, isolated: false, out head, out spec, out tail, "=");
        done = StrExtractor.RemoveBrackets(ref head, ref tail);
        Assert.True(done);
        Assert.Equal(" (aa) ", head);
        Assert.Equal(" (bb) ", tail);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractHead_Not_Found()
    {
        string main, spec;
        var source = "abc";
        var done = source.ExtractHead(false, isolated: false, out main, out spec, "XY");
        Assert.False(done);
        done = source.ExtractHead(false, isolated: true, out main, out spec, "XY");
        Assert.False(done);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractHead_StandAlone()
    {
        string main, spec;
        var source = "xy";
        var done = source.ExtractHead(false, isolated: false, out main, out spec, "XY");
        Assert.True(done);
        Assert.Empty(main);
        Assert.Equal("xy", spec);
        done = source.ExtractHead(false, isolated: true, out main, out spec, "XY");
        Assert.True(done);
        Assert.Empty(main);
        Assert.Equal("xy", spec);

        source = " xy ";
        done = source.ExtractHead(false, isolated: false, out main, out spec, "XY");
        Assert.True(done);
        Assert.Equal("  ", main);
        Assert.Equal("xy", spec);
        done = source.ExtractHead(false, isolated: true, out main, out spec, "XY");
        Assert.True(done);
        Assert.Equal("  ", main);
        Assert.Equal("xy", spec);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractHead_Other()
    {
        string main, spec;
        var source = "xyabc";
        var done = source.ExtractHead(false, isolated: false, out main, out spec, "XY");
        Assert.True(done);
        Assert.Equal("abc", main);
        Assert.Equal("xy", spec);
        done = source.ExtractHead(false, isolated: true, out main, out spec, "XY");
        Assert.False(done);

        source = " xyabc ";
        done = source.ExtractHead(false, isolated: false, out main, out spec, "XY");
        Assert.True(done);
        Assert.Equal(" abc ", main);
        Assert.Equal("xy", spec);
        done = source.ExtractHead(false, isolated: true, out main, out spec, "XY");
        Assert.False(done);

        source = " xy abc ";
        done = source.ExtractHead(false, isolated: false, out main, out spec, "XY");
        Assert.True(done);
        Assert.Equal("  abc ", main);
        Assert.Equal("xy", spec);
        done = source.ExtractHead(false, isolated: true, out main, out spec, "XY");
        Assert.True(done);
        Assert.Equal("  abc ", main);
        Assert.Equal("xy", spec);

        source = " xyabc xy ";
        done = source.ExtractHead(false, isolated: false, out main, out spec, "XY");
        Assert.True(done);
        Assert.Equal(" abc xy ", main);
        Assert.Equal("xy", spec);
        done = source.ExtractHead(false, isolated: true, out main, out spec, "XY");
        Assert.False(done);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_Not_Found()
    {
        string head, spec, tail;
        var source = "abc";
        var done = source.ExtractFirst(false, isolated: false, out head, out spec, out tail, "=");
        Assert.False(done);

        done = source.ExtractFirst(false, isolated: true, out head, out spec, out tail, "=");
        Assert.False(done);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_StandAlone()
    {
        string head, spec, tail;
        var source = "=";
        var done = source.ExtractFirst(false, isolated: false, out head, out spec, out tail, "=");
        Assert.True(done);
        Assert.Empty(head);
        Assert.Equal("=", spec);
        Assert.Empty(tail);

        done = source.ExtractFirst(false, isolated: true, out head, out spec, out tail, "=");
        Assert.True(done);
        Assert.Empty(head);
        Assert.Equal("=", spec);
        Assert.Empty(tail);

        source = " = ";
        done = source.ExtractFirst(false, isolated: false, out head, out spec, out tail, "=");
        Assert.True(done);
        Assert.Equal(" ", head);
        Assert.Equal("=", spec);
        Assert.Equal(" ", tail);

        done = source.ExtractFirst(false, isolated: true, out head, out spec, out tail, "=");
        Assert.True(done);
        Assert.Equal(" ", head);
        Assert.Equal("=", spec);
        Assert.Equal(" ", tail);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirst_Other()
    {
        string head, spec, tail;
        var source = "aa=bb=cc";
        var done = source.ExtractFirst(false, isolated: false, out head, out spec, out tail, "=");
        Assert.True(done);
        Assert.Equal("aa", head);
        Assert.Equal("=", spec);
        Assert.Equal("bb=cc", tail);

        done = source.ExtractFirst(false, isolated: true, out head, out spec, out tail, "=");
        Assert.False(done);

        source = " aa = bb = cc ";
        done = source.ExtractFirst(false, isolated: false, out head, out spec, out tail, "=");
        Assert.True(done);
        Assert.Equal(" aa ", head);
        Assert.Equal("=", spec);
        Assert.Equal(" bb = cc ", tail);

        done = source.ExtractFirst(false, isolated: true, out head, out spec, out tail, "=");
        Assert.True(done);
        Assert.Equal(" aa ", head);
        Assert.Equal("=", spec);
        Assert.Equal(" bb = cc ", tail);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractTail_Not_Found()
    {
        string main, spec;
        var source = "abc";
        var done = source.ExtractTail(false, isolated: false, out main, out spec, "XY");
        Assert.False(done);
        done = source.ExtractTail(false, isolated: true, out main, out spec, "XY");
        Assert.False(done);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractTail_StandAlone()
    {
        string main, spec;
        var source = "xy";
        var done = source.ExtractTail(false, isolated: false, out main, out spec, "XY");
        Assert.True(done);
        Assert.Empty(main);
        Assert.Equal("xy", spec);
        done = source.ExtractTail(false, isolated: true, out main, out spec, "XY");
        Assert.True(done);
        Assert.Empty(main);
        Assert.Equal("xy", spec);

        source = " xy ";
        done = source.ExtractTail(false, isolated: false, out main, out spec, "XY");
        Assert.True(done);
        Assert.Equal("  ", main);
        Assert.Equal("xy", spec);
        done = source.ExtractTail(false, isolated: true, out main, out spec, "XY");
        Assert.True(done);
        Assert.Equal("  ", main);
        Assert.Equal("xy", spec);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractTail_Other()
    {
        string main, spec;
        var source = "abcxy";
        var done = source.ExtractTail(false, isolated: false, out main, out spec, "XY");
        Assert.True(done);
        Assert.Equal("abc", main);
        Assert.Equal("xy", spec);
        done = source.ExtractTail(false, isolated: true, out main, out spec, "XY");
        Assert.False(done);

        source = " abcxy ";
        done = source.ExtractTail(false, isolated: false, out main, out spec, "XY");
        Assert.True(done);
        Assert.Equal(" abc ", main);
        Assert.Equal("xy", spec);
        done = source.ExtractTail(false, isolated: true, out main, out spec, "XY");
        Assert.False(done);

        source = " abc xy ";
        done = source.ExtractTail(false, isolated: false, out main, out spec, "XY");
        Assert.True(done);
        Assert.Equal(" abc  ", main);
        Assert.Equal("xy", spec);
        done = source.ExtractTail(false, isolated: true, out main, out spec, "XY");
        Assert.True(done);
        Assert.Equal(" abc  ", main);
        Assert.Equal("xy", spec);

        source = " xy abcxy ";
        done = source.ExtractTail(false, isolated: false, out main, out spec, "XY");
        Assert.True(done);
        Assert.Equal(" xy abc ", main);
        Assert.Equal("xy", spec);
        done = source.ExtractTail(false, isolated: true, out main, out spec, "XY");
        Assert.False(done);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_Not_Found()
    {
        string head, spec, tail;
        var source = "abc";
        var done = source.ExtractLast(false, isolated: false, out head, out spec, out tail, "=");
        Assert.False(done);

        done = source.ExtractLast(false, isolated: true, out head, out spec, out tail, "=");
        Assert.False(done);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_StandAlone()
    {
        string head, spec, tail;
        var source = "=";
        var done = source.ExtractLast(false, isolated: false, out head, out spec, out tail, "=");
        Assert.True(done);
        Assert.Empty(head);
        Assert.Equal("=", spec);
        Assert.Empty(tail);

        done = source.ExtractLast(false, isolated: true, out head, out spec, out tail, "=");
        Assert.True(done);
        Assert.Empty(head);
        Assert.Equal("=", spec);
        Assert.Empty(tail);

        source = " = ";
        done = source.ExtractLast(false, isolated: false, out head, out spec, out tail, "=");
        Assert.True(done);
        Assert.Equal(" ", head);
        Assert.Equal("=", spec);
        Assert.Equal(" ", tail);

        done = source.ExtractLast(false, isolated: true, out head, out spec, out tail, "=");
        Assert.True(done);
        Assert.Equal(" ", head);
        Assert.Equal("=", spec);
        Assert.Equal(" ", tail);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLast_Other()
    {
        string head, spec, tail;
        var source = "aa=bb=cc";
        var done = source.ExtractLast(false, isolated: false, out head, out spec, out tail, "=");
        Assert.True(done);
        Assert.Equal("aa=bb", head);
        Assert.Equal("=", spec);
        Assert.Equal("cc", tail);

        done = source.ExtractLast(false, isolated: true, out head, out spec, out tail, "=");
        Assert.False(done);

        source = " aa = bb = cc ";
        done = source.ExtractLast(false, isolated: false, out head, out spec, out tail, "=");
        Assert.True(done);
        Assert.Equal(" aa = bb ", head);
        Assert.Equal("=", spec);
        Assert.Equal(" cc ", tail);

        done = source.ExtractLast(false, isolated: true, out head, out spec, out tail, "=");
        Assert.True(done);
        Assert.Equal(" aa = bb ", head);
        Assert.Equal("=", spec);
        Assert.Equal(" cc ", tail);
    }
}
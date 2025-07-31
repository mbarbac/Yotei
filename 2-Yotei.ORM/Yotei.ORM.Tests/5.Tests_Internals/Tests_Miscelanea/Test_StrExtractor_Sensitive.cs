#pragma warning disable IDE0018
#pragma warning disable IDE0059

namespace Yotei.ORM.Tests.Internals.Miscelanea;

// ========================================================
//[Enforced]
public static class Test_StrExtractor_Sensitive
{
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
    public static void Test_ExtractHead_Stand_Alone()
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
    public static void Test_ExtractTail_Stand_Alone()
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
        Assert.Equal("  abcxy ", main);
        Assert.Equal("xy", spec);
        done = source.ExtractTail(false, isolated: true, out main, out spec, "XY");
        Assert.False(done);
    }
}
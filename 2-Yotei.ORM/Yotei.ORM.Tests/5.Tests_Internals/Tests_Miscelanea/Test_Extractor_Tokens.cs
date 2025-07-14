#pragma warning disable IDE0018
#pragma warning disable IDE0042

namespace Yotei.ORM.Tests.Internals.Miscelanea;

// ========================================================
//[Enforced]
public static class Test_Extractor_Tokens
{
    //[Enforced]
    [Fact]
    public static void Test_ExtractHead_NotFound()
    {
        var engine = new FakeEngine();
        bool found;

        string source = "abc";
        var parts = Extractor.ExtractHead(source, engine, isolated: false, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
        parts = Extractor.ExtractHead(source, engine, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = "[xy]";
        parts = Extractor.ExtractHead(source, engine, isolated: false, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
        parts = Extractor.ExtractHead(source, engine, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = "[xy]abc";
        parts = Extractor.ExtractHead(source, engine, isolated: false, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
        parts = Extractor.ExtractHead(source, engine, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = "ab[xy]cd";
        parts = Extractor.ExtractHead(source, engine, isolated: false, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
        parts = Extractor.ExtractHead(source, engine, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractHead_StandAlone()
    {
        var engine = new FakeEngine();
        bool found;

        var source = "xy";
        var parts = Extractor.ExtractHead(source, engine, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Empty(parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractHead(source, engine, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Empty(parts.Main);
        Assert.Equal("xy", parts.Spec);

        source = " xy ";
        parts = Extractor.ExtractHead(source, engine, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractHead(source, engine, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Equal("  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractHead_Regular()
    {
        var engine = new FakeEngine();
        bool found;

        var source = "xyabc";
        var parts = Extractor.ExtractHead(source, engine, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("abc", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractHead(source, engine, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = "xy[ab]cd";
        parts = Extractor.ExtractHead(source, engine, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("[ab]cd", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractHead(source, engine, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Equal("[ab]cd", parts.Main); // '[' acts as an isolate separator...
        Assert.Equal("xy", parts.Spec);

        source = "xyab[cd]ef";
        parts = Extractor.ExtractHead(source, engine, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("ab[cd]ef", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractHead(source, engine, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = " xy [ab] cd ";
        parts = Extractor.ExtractHead(source, engine, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("  [ab] cd ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractHead(source, engine, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Equal("  [ab] cd ", parts.Main);
        Assert.Equal("xy", parts.Spec);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractTail_NotFound()
    {
        var engine = new FakeEngine();
        bool found;

        string source = "abc";
        var parts = Extractor.ExtractTail(source, engine, isolated: false, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
        parts = Extractor.ExtractTail(source, engine, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = "[xy]";
        parts = Extractor.ExtractTail(source, engine, isolated: false, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
        parts = Extractor.ExtractTail(source, engine, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = "abc[xy]";
        parts = Extractor.ExtractTail(source, engine, isolated: false, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
        parts = Extractor.ExtractTail(source, engine, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = "ab[xy]cd";
        parts = Extractor.ExtractTail(source, engine, isolated: false, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
        parts = Extractor.ExtractTail(source, engine, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractTail_StandAlone()
    {
        var engine = new FakeEngine();
        bool found;

        var source = "xy";
        var parts = Extractor.ExtractTail(source, engine, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Empty(parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractTail(source, engine, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Empty(parts.Main);
        Assert.Equal("xy", parts.Spec);

        source = " xy ";
        parts = Extractor.ExtractTail(source, engine, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractTail(source, engine, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Equal("  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractTail_Regular()
    {
        var engine = new FakeEngine();
        bool found;

        var source = "abcxy";
        var parts = Extractor.ExtractTail(source, engine, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("abc", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractTail(source, engine, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = "ab[cd]xy";
        parts = Extractor.ExtractTail(source, engine, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("ab[cd]", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractTail(source, engine, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Equal("ab[cd]", parts.Main); // ']' acts as an isolate separator...
        Assert.Equal("xy", parts.Spec);

        source = "ab[cd]efxy";
        parts = Extractor.ExtractTail(source, engine, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal("ab[cd]ef", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractTail(source, engine, isolated: true, out found, "xy");
        Assert.False(found);
        Assert.Empty(parts.Main);
        Assert.Empty(parts.Spec);

        source = " ab [cd] xy ";
        parts = Extractor.ExtractTail(source, engine, isolated: false, out found, "xy");
        Assert.True(found);
        Assert.Equal(" ab [cd]  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
        parts = Extractor.ExtractTail(source, engine, isolated: true, out found, "xy");
        Assert.True(found);
        Assert.Equal(" ab [cd]  ", parts.Main);
        Assert.Equal("xy", parts.Spec);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirstSeparator_NotFound()
    {
        var engine = new FakeEngine();
        bool found;

        string source = "abc";
        var parts = Extractor.ExtractFirstSeparator(source, engine, isolated: false, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);
        parts = Extractor.ExtractFirstSeparator(source, engine, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = "ab[=]cd";
        parts = Extractor.ExtractFirstSeparator(source, engine, isolated: false, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);
        parts = Extractor.ExtractFirstSeparator(source, engine, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirstSeparator_StandAlone()
    {
        var engine = new FakeEngine();
        bool found;

        string source = "=";
        var parts = Extractor.ExtractFirstSeparator(source, engine, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);
        parts = Extractor.ExtractFirstSeparator(source, engine, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);

        source = " = ";
        parts = Extractor.ExtractFirstSeparator(source, engine, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);
        parts = Extractor.ExtractFirstSeparator(source, engine, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractFirstSeparator_Regular()
    {
        var engine = new FakeEngine();
        bool found;

        string source = "ab=cd=ef";
        var parts = Extractor.ExtractFirstSeparator(source, engine, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal("cd=ef", parts.Right);
        parts = Extractor.ExtractFirstSeparator(source, engine, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = " ab = cd = ef ";
        parts = Extractor.ExtractFirstSeparator(source, engine, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd = ef ", parts.Right);
        parts = Extractor.ExtractFirstSeparator(source, engine, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" cd = ef ", parts.Right);

        source = "[ab=cd]=ef";
        parts = Extractor.ExtractFirstSeparator(source, engine, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("[ab=cd]", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal("ef", parts.Right);
        parts = Extractor.ExtractFirstSeparator(source, engine, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = " [ab=cd] = ef ";
        parts = Extractor.ExtractFirstSeparator(source, engine, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" [ab=cd] ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ef ", parts.Right);
        parts = Extractor.ExtractFirstSeparator(source, engine, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" [ab=cd] ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ef ", parts.Right);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractLastSeparator_NotFound()
    {
        var engine = new FakeEngine();
        bool found;

        string source = "abc";
        var parts = Extractor.ExtractLastSeparator(source, engine, isolated: false, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);
        parts = Extractor.ExtractLastSeparator(source, engine, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = "ab[=]cd";
        parts = Extractor.ExtractLastSeparator(source, engine, isolated: false, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);
        parts = Extractor.ExtractLastSeparator(source, engine, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLastSeparator_StandAlone()
    {
        var engine = new FakeEngine();
        bool found;

        string source = "=";
        var parts = Extractor.ExtractLastSeparator(source, engine, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);
        parts = Extractor.ExtractLastSeparator(source, engine, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Empty(parts.Right);

        source = " = ";
        parts = Extractor.ExtractLastSeparator(source, engine, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);
        parts = Extractor.ExtractLastSeparator(source, engine, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ", parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLastSeparator_Regular()
    {
        var engine = new FakeEngine();
        bool found;

        string source = "ab=cd=ef";
        var parts = Extractor.ExtractLastSeparator(source, engine, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab=cd", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal("ef", parts.Right);
        parts = Extractor.ExtractLastSeparator(source, engine, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = " ab = cd = ef ";
        parts = Extractor.ExtractLastSeparator(source, engine, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ab = cd ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ef ", parts.Right);
        parts = Extractor.ExtractLastSeparator(source, engine, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ab = cd ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" ef ", parts.Right);

        source = "ab=[cd=ef]";
        parts = Extractor.ExtractLastSeparator(source, engine, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal("ab", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal("[cd=ef]", parts.Right);
        parts = Extractor.ExtractLastSeparator(source, engine, isolated: true, out found, "=");
        Assert.False(found);
        Assert.Empty(parts.Left);
        Assert.Empty(parts.Spec);
        Assert.Empty(parts.Right);

        source = " ab = [cd=ef] ";
        parts = Extractor.ExtractLastSeparator(source, engine, isolated: false, out found, "=");
        Assert.True(found);
        Assert.Equal(" ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" [cd=ef] ", parts.Right);
        parts = Extractor.ExtractLastSeparator(source, engine, isolated: true, out found, "=");
        Assert.True(found);
        Assert.Equal(" ab ", parts.Left);
        Assert.Equal("=", parts.Spec);
        Assert.Equal(" [cd=ef] ", parts.Right);
    }
}
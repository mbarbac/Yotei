#pragma warning disable IDE0042

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_StringExtensions2
{
    //[Enforced]
    [Fact]
    public static void Test_ExtractLeftRight_Sensitive_NotFound()
    {
        var source = "any";
        var separator = "=";
        var parts = source.ExtractLeftRight(separator, false, out var found);
        Assert.Same(source, parts.Left);
        Assert.Null(parts.Right);
        Assert.False(found);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLeftRigh_Sensitive_FoundMany()
    {
        var source = "any==other";
        var separator = "=";

        try { source.ExtractLeftRight(separator, false); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLeftRight_Sensitive_Found()
    {
        var source = "alpha==beta";
        var separator = "==";
        var parts = source.ExtractLeftRight(separator, false);
        Assert.Equal("alpha", parts.Left);
        Assert.Equal("beta", parts.Right);

        source = "alpha==";
        parts = source.ExtractLeftRight(separator, false);
        Assert.Equal("alpha", parts.Left);
        Assert.Empty(parts.Right!);

        source = "==beta";
        parts = source.ExtractLeftRight(separator, false);
        Assert.Empty(parts.Left);
        Assert.Equal("beta", parts.Right);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractLeftRight_Engine_NotFound()
    {
        var engine = new FakeEngine();
        var source = "any";
        var separator = "=";
        var parts = source.ExtractLeftRight(separator, engine, out var found);
        Assert.False(found);
        Assert.Same(source, parts.Left);
        Assert.Null(parts.Right);

        source = "[alpha=beta]";
        parts = source.ExtractLeftRight(separator, engine, out found);
        Assert.False(found);
        Assert.Same(source, parts.Left);
        Assert.Null(parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLeftRigh_Engine_FoundMany()
    {
        var source = "any==other";
        var separator = "=";

        try { source.ExtractLeftRight(separator, false); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLeftRight_Engine_Found()
    {
        var engine = new FakeEngine();
        var source = "alpha==beta";
        var separator = "==";
        var parts = source.ExtractLeftRight(separator, engine, out var found);
        Assert.True(found);
        Assert.Equal("alpha", parts.Left);
        Assert.Equal("beta", parts.Right);

        source = "alpha==";
        parts = source.ExtractLeftRight(separator, engine, out found);
        Assert.True(found);
        Assert.Equal("alpha", parts.Left);
        Assert.Empty(parts.Right!);

        source = "==beta";
        parts = source.ExtractLeftRight(separator, engine, out found);
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("beta", parts.Right);
    }
}
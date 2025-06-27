#pragma warning disable IDE0042

using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_StringExtract
{
    //[Enforced]
    [Fact]
    public static void Test_ExtractLeftRight_Sensitive_Not_Found()
    {
        var source = "aa";
        var separator = "==";

        var parts = source.ExtractLeftRight(separator, false, out var found);
        Assert.False(found);
        Assert.Same(source, parts.Left);
        Assert.Null(parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLeftRight_Sensitive_Found()
    {
        var source = "aa==bb";
        var separator = "==";
        var parts = source.ExtractLeftRight(separator, false, out var found);
        Assert.True(found);
        Assert.Equal("aa", parts.Left);
        Assert.Equal("bb", parts.Right);

        source = "aa==";
        parts = source.ExtractLeftRight(separator, false, out found);
        Assert.True(found);
        Assert.Equal("aa", parts.Left);
        Assert.Empty(parts.Right!);

        source = "==bb";
        parts = source.ExtractLeftRight(separator, false, out found);
        Assert.True(found);
        Assert.Empty(parts.Left);
        Assert.Equal("bb", parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLeftRight_Sensitive_Many()
    {
        var source = "aa==bb==cc";
        var separator = "==";

        try { source.ExtractLeftRight(separator, false, out _); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractLeftRight_Engine_Not_Found()
    {
        var engine = new FakeEngine();
        var source = "aa";
        var separator = "==";
        var parts = source.ExtractLeftRight(separator, engine, out var found);
        Assert.False(found);
        Assert.Same(source, parts.Left);
        Assert.Null(parts.Right);

        source = "[aa==bb]";
        parts = source.ExtractLeftRight(separator, engine, out found);
        Assert.False(found);
        Assert.Same(source, parts.Left);
        Assert.Null(parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLeftRight_Engine_Found()
    {
        var engine = new FakeEngine();
        var source = "aa==bb";
        var separator = "==";
        var parts = source.ExtractLeftRight(separator, engine, out var found);
        Assert.True(found);
        Assert.Equal("aa", parts.Left);
        Assert.Equal("bb", parts.Right);

        source = "[aa==]==bb";
        parts = source.ExtractLeftRight(separator, engine, out found);
        Assert.True(found);
        Assert.Equal("[aa==]", parts.Left);
        Assert.Equal("bb", parts.Right);

        source = "aa==[==bb]";
        parts = source.ExtractLeftRight(separator, engine, out found);
        Assert.True(found);
        Assert.Equal("aa", parts.Left);
        Assert.Equal("[==bb]", parts.Right);

        source = "[aa==]bb==cc[==dd]";
        parts = source.ExtractLeftRight(separator, engine, out found);
        Assert.True(found);
        Assert.Equal("[aa==]bb", parts.Left);
        Assert.Equal("cc[==dd]", parts.Right);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractLeftRight_Engine_Many()
    {
        var engine = new FakeEngine();
        var separator = "==";

        try { "aa==bb==cc".ExtractLeftRight(separator, engine, out _); Assert.Fail(); }
        catch (DuplicateException) { }

        try { "[aa]==bb==[cc]".ExtractLeftRight(separator, engine, out _); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractMainAlias_Sensitive_Not_Found()
    {
        var source = "aa";
        var parts = source.ExtractMainAlias(false, out var found);
        Assert.False(found);
        Assert.Same(source, parts.Main);
        Assert.Null(parts.Alias);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractMainAlias_Sensitive_Found()
    {
        var source = "aa as bb";
        var parts = source.ExtractMainAlias(false, out var found);
        Assert.True(found);
        Assert.Equal("aa", parts.Main);
        Assert.Equal("bb", parts.Alias);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractMainAlias_Sensitive_Errors()
    {
        try { "as".ExtractMainAlias(false, out _); Assert.Fail(); }
        catch (ArgumentException) { }

        try { " as ".ExtractMainAlias(false, out _); Assert.Fail(); }
        catch (EmptyException) { }

        try { "aa as ".ExtractMainAlias(false, out _); Assert.Fail(); }
        catch (EmptyException) { }

        try { " as bb".ExtractMainAlias(false, out _); Assert.Fail(); }
        catch (EmptyException) { }

        try { "aa as bb as cc".ExtractMainAlias(false, out _); Assert.Fail(); }
        catch (DuplicateException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_ExtractMainAlias_Engine_Not_Found()
    {
        var engine = new FakeEngine();
        var source = "aa";
        var parts = source.ExtractMainAlias(engine, out var found);
        Assert.False(found);
        Assert.Same(source, parts.Main);
        Assert.Null(parts.Alias);

        source = "[aa AS bb]";
        parts = source.ExtractMainAlias(engine, out found);
        Assert.False(found);
        Assert.Equal("[aa AS bb]", parts.Main);
        Assert.Null(parts.Alias);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractMainAlias_Engine_Found()
    {
        var engine = new FakeEngine();
        var source = "aa as bb";
        var parts = source.ExtractMainAlias(engine, out var found);
        Assert.True(found);
        Assert.Equal("aa", parts.Main);
        Assert.Equal("bb", parts.Alias);

        source = "[aa as] as bb";
        parts = source.ExtractMainAlias(engine, out found);
        Assert.True(found);
        Assert.Equal("[aa as]", parts.Main);
        Assert.Equal("bb", parts.Alias);

        source = "aa as [as bb]";
        parts = source.ExtractMainAlias(engine, out found);
        Assert.True(found);
        Assert.Equal("aa", parts.Main);
        Assert.Equal("[as bb]", parts.Alias);

        source = "aa[bb as] as [as cc]dd";
        parts = source.ExtractMainAlias(engine, out found);
        Assert.True(found);
        Assert.Equal("aa[bb as]", parts.Main);
        Assert.Equal("[as cc]dd", parts.Alias);
    }

    //[Enforced]
    [Fact]
    public static void Test_ExtractMainAlias_Engine_Errors()
    {
        var engine = new FakeEngine();

        try { "as".ExtractMainAlias(engine, out _); Assert.Fail(); }
        catch (ArgumentException) { }

        try { " as ".ExtractMainAlias(engine, out _); Assert.Fail(); }
        catch (EmptyException) { }

        try { "aa as ".ExtractMainAlias(engine, out _); Assert.Fail(); }
        catch (EmptyException) { }

        try { " as bb".ExtractMainAlias(engine, out _); Assert.Fail(); }
        catch (EmptyException) { }

        try { "aa as bb as cc".ExtractMainAlias(engine, out _); Assert.Fail(); }
        catch (DuplicateException) { }
    }
}
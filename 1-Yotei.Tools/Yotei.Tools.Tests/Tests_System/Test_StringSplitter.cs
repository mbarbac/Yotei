using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_StringSplitter
{
    // [Enforced]
    [Fact]
    public static void Test_Null()
    {
        string source = null!;

        try { _ = source.Splitter('.'); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        string source = "";

        string[] items = source.Split("..");
        Assert.Single(items);
        Assert.Equal("", items[0]);

        var iter = source.Splitter("..");
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Single(items);
        Assert.Equal("", items[0]);

        iter = iter with { OmitEmptyEntries = true };
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Empty(items);
    }

    // [Enforced]
    [Fact]
    public static void Test_NotFound()
    {
        string source = "ab";

        string[] items = source.Split("..");
        Assert.Single(items);
        Assert.Equal("ab", items[0]);

        var iter = source.Splitter("..");
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Single(items);
        Assert.Equal("ab", items[0]);
    }

    // [Enforced]
    [Fact]
    public static void Test_FoundUnique()
    {
        string source = "..";

        string[] items = source.Split("..");
        Assert.Equal(2, items.Length);
        Assert.Equal("", items[0]);
        Assert.Equal("", items[1]);

        var iter = source.Splitter("..");
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Equal(3, items.Length);
        Assert.Equal("", items[0]);
        Assert.Equal("..", items[1]);
        Assert.Equal("", items[2]);

        iter = iter with { OmitEmptyEntries = true };
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Single(items);
        Assert.Equal("..", items[0]);

        iter = iter with { OmitSeparators = true };
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Empty(items);
    }

    // [Enforced]
    [Fact]
    public static void Test_FoundFirst()
    {
        string source = "..ab";

        string[] items = source.Split("..");
        Assert.Equal(2, items.Length);
        Assert.Equal("", items[0]);
        Assert.Equal("ab", items[1]);

        var iter = source.Splitter("..");
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Equal(3, items.Length);
        Assert.Equal("", items[0]);
        Assert.Equal("..", items[1]);
        Assert.Equal("ab", items[2]);

        iter = iter with { OmitEmptyEntries = true };
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Equal(2, items.Length);
        Assert.Equal("..", items[0]);
        Assert.Equal("ab", items[1]);

        iter = iter with { OmitSeparators = true };
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Single(items);
        Assert.Equal("ab", items[0]);
    }

    // [Enforced]
    [Fact]
    public static void Test_FoundLast()
    {
        string source = "ab..";

        string[] items = source.Split("..");
        Assert.Equal(2, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("", items[1]);

        var iter = source.Splitter("..");
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Equal(3, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("..", items[1]);
        Assert.Equal("", items[2]);

        iter = iter with { OmitEmptyEntries = true };
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Equal(2, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("..", items[1]);

        iter = iter with { OmitSeparators = true };
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Single(items);
        Assert.Equal("ab", items[0]);
    }

    // [Enforced]
    [Fact]
    public static void Test_FoundMiddle()
    {
        string source = "ab....ef";

        string[] items = source.Split("..");
        Assert.Equal(3, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("", items[1]);
        Assert.Equal("ef", items[2]);

        var iter = source.Splitter("..");
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Equal(5, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("..", items[1]);
        Assert.Equal("", items[2]);
        Assert.Equal("..", items[3]);
        Assert.Equal("ef", items[4]);

        iter = iter with { OmitEmptyEntries = true };
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Equal(4, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("..", items[1]);
        Assert.Equal("..", items[2]);
        Assert.Equal("ef", items[3]);

        iter = iter with { OmitSeparators = true };
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Equal(2, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("ef", items[1]);
    }
}
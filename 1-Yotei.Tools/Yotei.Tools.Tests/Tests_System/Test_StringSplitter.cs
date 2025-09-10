using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_StringSplitter
{
    // [Enforced]
    [Fact]
    public static void Test_Empty()
    {
        string source = "";

        // Standard behavior...
        string[] items = source.Split('.');
        Assert.Single(items);
        Assert.Equal("", items[0]);

        items = source.Split(".", StringSplitOptions.RemoveEmptyEntries);
        Assert.Empty(items);

        // Splitter...
        var iter = source.Splitter('.');
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Single(items);
        Assert.Equal("", items[0]);

        iter = iter with { RemoveEmptyEntries = true };
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Empty(items);
    }

    // [Enforced]
    [Fact]
    public static void Test_Not_Found()
    {
        string source = "ab";

        // Standard behavior...
        string[] items = source.Split('.');
        Assert.Single(items);
        Assert.Equal("ab", items[0]);

        // Splitter...
        var iter = source.Splitter('.');
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Single(items);
        Assert.Equal("ab", items[0]);
    }

    // [Enforced]
    [Fact]
    public static void Test_Found_First()
    {
        string source = ".ab";

        // Standard behavior...
        string[] items = source.Split('.');
        Assert.Equal(2, items.Length);
        Assert.Empty(items[0]);
        Assert.Equal("ab", items[1]);

        items = source.Split(".", StringSplitOptions.RemoveEmptyEntries);
        Assert.Single(items);
        Assert.Equal("ab", items[0]);

        // Splitter...
        var iter = source.Splitter('.');
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Equal(3, items.Length);
        Assert.Empty(items[0]);
        Assert.Equal(".", items[1]);
        Assert.Equal("ab", items[2]);

        var temp = iter with { OmitSeparators = true };
        items = temp.Select(x => x.ToString()).ToArray();
        Assert.Equal(2, items.Length);
        Assert.Empty(items[0]);
        Assert.Equal("ab", items[1]);

        temp = iter with { RemoveEmptyEntries = true };
        items = temp.Select(x => x.ToString()).ToArray();
        Assert.Equal(2, items.Length);
        Assert.Equal(".", items[0]);
        Assert.Equal("ab", items[1]);
    }

    // [Enforced]
    [Fact]
    public static void Test_Found_Last()
    {
        string source = "ab.";

        // Standard behavior...
        string[] items = source.Split('.');
        Assert.Equal(2, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Empty(items[1]);

        items = source.Split(".", StringSplitOptions.RemoveEmptyEntries);
        Assert.Single(items);
        Assert.Equal("ab", items[0]);

        // Splitter...
        var iter = source.Splitter('.');
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Equal(3, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal(".", items[1]);
        Assert.Empty(items[2]);

        var temp = iter with { OmitSeparators = true };
        items = temp.Select(x => x.ToString()).ToArray();
        Assert.Equal(2, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Empty(items[1]);

        temp = iter with { RemoveEmptyEntries = true };
        items = temp.Select(x => x.ToString()).ToArray();
        Assert.Equal(2, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal(".", items[1]);

        temp = iter with { OmitSeparators = true, RemoveEmptyEntries = true };
        items = temp.Select(x => x.ToString()).ToArray();
        Assert.Single(items);
        Assert.Equal("ab", items[0]);
    }

    // [Enforced]
    [Fact]
    public static void Test_Found_Many()
    {
        string source = ".ab.cd. .ef.";

        // Standard behavior...
        string[] items = source.Split('.');
        Assert.Equal(6, items.Length);
        Assert.Empty(items[0]);
        Assert.Equal("ab", items[1]);
        Assert.Equal("cd", items[2]);
        Assert.Equal(" ", items[3]);
        Assert.Equal("ef", items[4]);
        Assert.Empty(items[5]);

        items = source.Split(".", StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(4, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("cd", items[1]);
        Assert.Equal(" ", items[2]);
        Assert.Equal("ef", items[3]);

        items = source.Split(".", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(3, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("cd", items[1]);
        Assert.Equal("ef", items[2]);

        //// Splitter...
        var iter = source.Splitter('.');
        items = iter.Select(x => x.ToString()).ToArray();
        Assert.Equal(11, items.Length);
        Assert.Empty(items[0]);
        Assert.Equal(".", items[1]);
        Assert.Equal("ab", items[2]);
        Assert.Equal(".", items[3]);
        Assert.Equal("cd", items[4]);
        Assert.Equal(".", items[5]);
        Assert.Equal(" ", items[6]);
        Assert.Equal(".", items[7]);
        Assert.Equal("ef", items[8]);
        Assert.Equal(".", items[9]);
        Assert.Empty(items[10]);

        var temp = iter with { OmitSeparators = true };
        items = temp.Select(x => x.ToString()).ToArray();
        Assert.Equal(6, items.Length);
        Assert.Empty(items[0]);
        Assert.Equal("ab", items[1]);
        Assert.Equal("cd", items[2]);
        Assert.Equal(" ", items[3]);
        Assert.Equal("ef", items[4]);
        Assert.Empty(items[5]);

        temp = iter with { RemoveEmptyEntries = true };
        items = temp.Select(x => x.ToString()).ToArray();
        Assert.Equal(9, items.Length);
        Assert.Equal(".", items[0]);
        Assert.Equal("ab", items[1]);
        Assert.Equal(".", items[2]);
        Assert.Equal("cd", items[3]);
        Assert.Equal(".", items[4]);
        Assert.Equal(" ", items[5]);
        Assert.Equal(".", items[6]);
        Assert.Equal("ef", items[7]);
        Assert.Equal(".", items[8]);

        temp = iter with { TrimEntries = true, RemoveEmptyEntries = true };
        items = temp.Select(x => x.ToString()).ToArray();
        Assert.Equal(8, items.Length);
        Assert.Equal(".", items[0]);
        Assert.Equal("ab", items[1]);
        Assert.Equal(".", items[2]);
        Assert.Equal("cd", items[3]);
        Assert.Equal(".", items[4]);
        Assert.Equal(".", items[5]);
        Assert.Equal("ef", items[6]);
        Assert.Equal(".", items[7]);

        temp = iter with { OmitSeparators = true, TrimEntries = true, RemoveEmptyEntries = true };
        items = temp.Select(x => x.ToString()).ToArray();
        Assert.Equal(3, items.Length);
        Assert.Equal("ab", items[0]);
        Assert.Equal("cd", items[1]);
        Assert.Equal("ef", items[2]);
    }
}
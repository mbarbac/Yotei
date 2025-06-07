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
        StringSplitter iter;
        List<string> items;

        iter = new StringSplitter(string.Empty, Environment.NewLine);
        items = iter.Select(x => x.ToString()).ToList();
        Assert.Empty(items);

        iter = iter with { OmitEmptyEntries = true };
        items = iter.Select(x => x.ToString()).ToList();
        Assert.Empty(items);
    }

    // [Enforced]
    [Fact]
    public static void Test_NotFound()
    {
        StringSplitter iter;
        List<string> items;

        iter = new StringSplitter(" ", Environment.NewLine);
        items = iter.Select(x => x.ToString()).ToList();
        Assert.Single(items);
        Assert.Equal(" ", items[0]);

        iter = iter with { OmitEmptyEntries = true };
        items = iter.Select(x => x.ToString()).ToList();
        Assert.Empty(items);
    }

    //[Enforced]
    [Fact]
    public static void Test_EmptySource_WithNL()
    {
        StringSplitter iter;
        List<string> items;

        string source = Environment.NewLine;
        iter = new StringSplitter(source, Environment.NewLine);
        items = iter.Select(x => x.ToString()).ToList();
        Assert.Single(items);
        Assert.Equal(Environment.NewLine, items[0]);

        // This is equivalent to source = Environment.NewLine
        source = """


            """;
        items = iter.Select(x => x.ToString()).ToList();
        Assert.Single(items);
        Assert.Equal(Environment.NewLine, items[0]);

        iter = iter with { OmitSeparators = true };
        items = iter.Select(x => x.ToString()).ToList();
        Assert.Empty(items);
    }
    //{



    //    Assert.Equal(3, items.Count);
    //    Assert.Equal(string.Empty, items[0]);
    //    Assert.Equal(Environment.NewLine, items[1]);
    //    Assert.Equal(string.Empty, items[2]);

    //    iter = iter with { OmitEmptyEntries = true };
    //    items = iter.Select(x => x.ToString()).ToList();
    //    Assert.Single(items);
    //    Assert.Equal(Environment.NewLine, items[0]);
    //}
}
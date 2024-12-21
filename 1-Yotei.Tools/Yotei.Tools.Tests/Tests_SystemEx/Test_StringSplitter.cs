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
        var iter = new StringSplitter(null, Environment.NewLine);
        var items = iter.Select(x => x.ToString());

        Assert.Empty(items);
    }

    // [Enforced]
    [Fact]
    public static void Test_Empty()
    {
        var iter = new StringSplitter(string.Empty, Environment.NewLine);
        var items = iter.Select(x => x.ToString()).ToList();

        Assert.Single(items);
        Assert.Empty(items[0]);
    }

    // [Enforced]
    [Fact]
    public static void Test_Empty_PlusNL()
    {
        var source = """


            """;

        var iter = new StringSplitter(source, Environment.NewLine);
        var items = iter.Select(x => x.ToString()).ToList();

        Assert.Equal(3, items.Count);
        Assert.Equal(string.Empty, items[0]);
        Assert.Equal(Environment.NewLine, items[1]);
        Assert.Equal(string.Empty, items[2]);
    }

    // [Enforced]
    [Fact]
    public static void Test_Empty_OneElement()
    {
        var iter = new StringSplitter("Hi", Environment.NewLine);
        var items = iter.Select(x => x.ToString()).ToList();

        Assert.Single(items);
        Assert.Equal("Hi", items[0]);
    }

    // [Enforced]
    [Fact]
    public static void Test_Empty_OneElement_PlusNL()
    {
        var source = """
            Hi

            """;

        var iter = new StringSplitter(source, Environment.NewLine);
        var items = iter.Select(x => x.ToString()).ToList();

        Assert.Equal(3, items.Count);
        Assert.Equal("Hi", items[0]);
        Assert.Equal(Environment.NewLine, items[1]);
        Assert.Empty(items[2]);
    }

    // [Enforced]
    [Fact]
    public static void Test_Empty_OneElement_PreNL()
    {
        var source = """

            Hi
            """;

        var iter = new StringSplitter(source, Environment.NewLine);
        var items = iter.Select(x => x.ToString()).ToList();

        Assert.Equal(3, items.Count);
        Assert.Empty(items[0]);
        Assert.Equal(Environment.NewLine, items[1]);
        Assert.Equal("Hi", items[2]);
    }

    // [Enforced]
    [Fact]
    public static void Test_Empty_TwoElements()
    {
        var source = """
            Hi
            Dear
            """;

        var iter = new StringSplitter(source, Environment.NewLine);
        var items = iter.Select(x => x.ToString()).ToList();

        Assert.Equal(3, items.Count);
        Assert.Equal("Hi", items[0]);
        Assert.Equal(Environment.NewLine, items[1]);
        Assert.Equal("Dear", items[2]);
    }

    // [Enforced]
    [Fact]
    public static void Test_Empty_TwoElements_PreNL_PostNL()
    {
        var source = """

            Hi
            Dear

            """;

        var iter = new StringSplitter(source, Environment.NewLine);
        var items = iter.Select(x => x.ToString()).ToList();

        Assert.Equal(7, items.Count);
        Assert.Empty(items[0]);
        Assert.Equal(Environment.NewLine, items[1]);
        Assert.Equal("Hi", items[2]);
        Assert.Equal(Environment.NewLine, items[3]);
        Assert.Equal("Dear", items[4]);
        Assert.Equal(Environment.NewLine, items[5]);
        Assert.Empty(items[6]);
    }

    // ----------------------------------------------------

    // [Enforced]
    [Fact]
    public static void Test_Where_Predicate()
    {
        var source = """

            Hi
            Dear

            """;

        var iter = new StringSplitter(source, Environment.NewLine);
        var items = iter.Where(x => x.Length > 0).Select(x => x.ToString()).ToList();

        Assert.Equal(5, items.Count);
        Assert.Equal(Environment.NewLine, items[0]);
        Assert.Equal("Hi", items[1]);
        Assert.Equal(Environment.NewLine, items[2]);
        Assert.Equal("Dear", items[3]);
        Assert.Equal(Environment.NewLine, items[4]);
    }

    // [Enforced]
    [Fact]
    public static void Test_Where_Predicate_With_BoolAsCurrentIsSeparator()
    {
        var source = """

            Hi
            Dear

            """;

        var iter = new StringSplitter(source, Environment.NewLine);
        var items = iter.Where((x, b) => x.Length > 0 && !b).Select(x => x.ToString()).ToList();

        Assert.Equal(2, items.Count);
        Assert.Equal("Hi", items[0]);
        Assert.Equal("Dear", items[1]);

    }
}
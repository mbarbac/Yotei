namespace Yotei.Tools.Tests;

// ========================================================
public class Test_SpanCharSplitter
{
    // [Enforced]
    [Fact]
    public void Test_Null()
    {
        var source = (string?)null;
        var iter = new SpanCharSplitter(source, Environment.NewLine);
        var items = iter.Select(x => x.ToString());

        Assert.Single(items);
        Assert.Empty(items[0]);
    }

    // [Enforced]
    [Fact]
    public void Test_Empty()
    {
        var source = string.Empty;
        var iter = new SpanCharSplitter(source, Environment.NewLine);
        var items = iter.Select(x => x.ToString());

        Assert.Single(items);
        Assert.Empty(items[0]);
    }

    // [Enforced]
    [Fact]
    public void Test_Empty_PlusNL()
    {
        var source = """
            

            """;
        var iter = new SpanCharSplitter(source, Environment.NewLine);
        var items = iter.Select(x => x.ToString());

        Assert.Equal(3, items.Count);
        Assert.Equal(string.Empty, items[0]);
        Assert.Equal(Environment.NewLine, items[1]);
        Assert.Equal(string.Empty, items[2]);
    }

    // [Enforced]
    [Fact]
    public void Test_OneElement()
    {
        var source = """
            Hi
            """;
        var iter = new SpanCharSplitter(source, Environment.NewLine);
        var items = iter.Select(x => x.ToString());

        Assert.Single(items);
        Assert.Equal("Hi", items[0]);
    }

    // [Enforced]
    [Fact]
    public void Test_OneElement_PlusNL()
    {
        var source = """
            Hi

            """;
        var iter = new SpanCharSplitter(source, Environment.NewLine);
        var items = iter.Select(x => x.ToString());

        Assert.Equal(3, items.Count);
        Assert.Equal("Hi", items[0]);
        Assert.Equal(Environment.NewLine, items[1]);
        Assert.Equal(string.Empty, items[2]);
    }

    // [Enforced]
    [Fact]
    public void Test_TwoElements()
    {
        var source = """
            Hi
            Dear
            """;
        var iter = new SpanCharSplitter(source, Environment.NewLine);
        var items = iter.Select(x => x.ToString());

        Assert.Equal(3, items.Count);
        Assert.Equal("Hi", items[0]);
        Assert.Equal(Environment.NewLine, items[1]);
        Assert.Equal("Dear", items[2]);
    }

    // [Enforced]
    [Fact]
    public void Test_TwoElements_NLFirst_NLEnd()
    {
        var source = """

            Hi
            Dear

            """;
        var iter = new SpanCharSplitter(source, Environment.NewLine);
        var items = iter.Select(x => x.ToString());

        Assert.Equal(7, items.Count);
        Assert.Equal(string.Empty, items[0]);
        Assert.Equal(Environment.NewLine, items[1]);
        Assert.Equal("Hi", items[2]);
        Assert.Equal(Environment.NewLine, items[3]);
        Assert.Equal("Dear", items[4]);
        Assert.Equal(Environment.NewLine, items[5]);
        Assert.Equal(string.Empty, items[6]);
    }

    // ----------------------------------------------------

    // [Enforced]
    [Fact]
    public void Test_TwoElements_NLFirst_NLEnd_When()
    {
        var source = """

            Hi
            Dear

            """;
        var iter = new SpanCharSplitter(source, Environment.NewLine);
        var items = iter.SelectWhen(
            x => x.ToString(),
            (x, nl) => !nl);

        Assert.Equal(4, items.Count);
        Assert.Equal(string.Empty, items[0]);
        Assert.Equal("Hi", items[1]);
        Assert.Equal("Dear", items[2]);
        Assert.Equal(string.Empty, items[3]);
    }
}
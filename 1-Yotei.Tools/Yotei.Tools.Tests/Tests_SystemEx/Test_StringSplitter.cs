namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_StringSplitter
{
    //[Enforced]
    [Fact]
    public static void Test_Empty()
    {
        StringSplitter iter;
        string source = "";

        // No options...
        string[] items = source.Split('.');
        Assert.Single(items);
        Assert.Equal("", items[0]);

        iter = new StringSplitter(source.AsSpan(), ".", "");
        Assert.Single(items);
        Assert.Equal("", items[0]);
        items = iter.Select(x => x.ToString()).ToArray();
    }
}

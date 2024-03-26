namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Parameter
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var item = new Parameter("#one", null);
        Assert.Equal("#one", item.Name);
        Assert.Null(item.Value);

        item = new Parameter("#one", 25);
        Assert.Equal("#one", item.Name);
        Assert.Equal(25, item.Value);

        try { _ = new Parameter(null!, "whatever"); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Parameter(string.Empty, "whatever"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Parameter(" ", "whatever"); Assert.Fail(); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Equals()
    {
        var source = new Parameter("one", 50);
        var target = new Parameter("ONE", 50);
        Assert.True(source.Equals(target, caseSensitiveNames: false));
        Assert.False(source.Equals(target, caseSensitiveNames: true));
    }
}
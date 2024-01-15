namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Parameter
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var item = new Code.Parameter("#one", null);
        Assert.Equal("#one", item.Name);
        Assert.Null(item.Value);

        item = new Code.Parameter("#one", 25);
        Assert.Equal("#one", item.Name);
        Assert.Equal(25, item.Value);

        try { _ = new Code.Parameter(null!, "whatever"); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new Code.Parameter(string.Empty, "whatever"); Assert.Fail(); }
        catch (ArgumentException) { }

        try { _ = new Code.Parameter(" ", "whatever"); Assert.Fail(); }
        catch (ArgumentException) { }
    }
}
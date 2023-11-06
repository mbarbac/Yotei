namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Parameter
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Populated()
    {
        var item = new Parameter("#one", null);
        Assert.Equal("#one", item.Name);
        Assert.Null(item.Value);

        item = new Parameter("#one", 25);
        Assert.Equal("#one", item.Name);
        Assert.Equal(25, item.Value);

        try { _ = new Parameter(null!, null); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_WithMethods()
    {
        var source = new Parameter("name", 50);

        var target = source.WithName("other");
        Assert.NotSame(source, target);
        Assert.Equal("other", target.Name);
        Assert.Equal(50, target.Value);

        target = source.WithValue(60);
        Assert.NotSame(source, target);
        Assert.Equal(source.Name, target.Name);
        Assert.Equal(60, target.Value);
    }
}
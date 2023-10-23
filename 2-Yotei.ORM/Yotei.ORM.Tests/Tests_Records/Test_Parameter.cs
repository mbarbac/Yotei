namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_Parameter
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var item = new Parameter();
        Assert.Null(item.Name);
        Assert.Null(item.Value);
    }

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
    public static void Test_ToString()
    {
        var item = new Parameter("#name", "value");
        var str = item.ToString();
        Assert.Equal("#name='value'", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_With_Methods()
    {
        var source = new Parameter("#one", "any");
        var target = source.WithName("other");
        Assert.NotSame(source, target);
        Assert.Equal("other", target.Name);
        Assert.Equal(source.Value, target.Value);

        target = source.WithValue("other");
        Assert.NotSame(source, target);
        Assert.Equal(source.Name, target.Name);
        Assert.Equal("other", target.Value);
    }
}
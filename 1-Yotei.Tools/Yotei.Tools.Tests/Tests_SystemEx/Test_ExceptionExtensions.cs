namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_ExceptionExtensions
{
    //[Enforced]
    [Fact]
    public static void Test_WithData()
    {
        var name = "any";
        var ex = new ArgumentException().WithData(name);

        Assert.True(ex.Data.Contains("name"));
        Assert.Equal("any", ex.Data["name"]);
    }
}
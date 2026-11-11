namespace Yotei.Tools.Tests;

// ========================================================
public static class Test_ExceptionExtensions
{
    [Fact]
    public static void Test_WithData()
    {
        var name = "any";
        var ex = new ArgumentException().WithData(name);

        Assert.True(ex.Data.Contains("name"));
        Assert.Equal("any", ex.Data["name"]);
    }
}
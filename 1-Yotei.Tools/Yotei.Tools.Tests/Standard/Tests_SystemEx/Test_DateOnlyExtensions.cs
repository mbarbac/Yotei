namespace Yotei.Tools.Tests;

// ========================================================
public static class Test_DateOnlyExtensions
{
    [Fact]
    public static void Test_ToIsoString()
    {
        var value = new DateOnly(2000, 12, 31);
        var str = value.ToIsoString();
        Assert.Equal("2000/12/31", str);
    }
}
namespace Yotei.Tools.Tests;

// ========================================================
public static class Test_TimeOnlyExtensions
{
    [Fact]
    public static void Test_ToIsoString()
    {
        var value = new TimeOnly(23, 59, 58, 999, 888);
        var str = value.ToIsoString();
        Assert.Equal("23:59", str);

        str = value.ToIsoString(':', '.', true, true, true, true);
        Assert.Equal("23:59:58.999.888.000", str);

        var ticks = value.Ticks + 1;
        value = new(ticks);
        str = value.ToIsoString(':', '.', true, true, true, true);
        Assert.Equal("23:59:58.999.888.100", str);
    }
}
namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_ClockTime
{
    //[Enforced]
    [Fact]
    public static void Error_Invalid_Constructors()
    {
        try { _ = new ClockTime(-1, 0, 0, 0); Assert.True(false); }
        catch (ArgumentException) { }

        try { _ = new ClockTime(0, -1, 0, 0); Assert.True(false); }
        catch (ArgumentException) { }

        try { _ = new ClockTime(0, 0, -1, 0); Assert.True(false); }
        catch (ArgumentException) { }

        try { _ = new ClockTime(0, 0, 0, 1000); Assert.True(false); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Equality_Scenarios()
    {
        var source = new ClockTime(1, 1);
        var target = new ClockTime(1, 1);
        Assert.True(source.CompareTo(target) == 0);

        source = new ClockTime(1, 1);
        target = new ClockTime(1, 1, 1);
        Assert.True(source.CompareTo(target) != 0);
    }

    //[Enforced]
    [Fact]
    public static void Test_Comparison_Scenarios()
    {
        var source = new ClockTime(1, 1, 1);
        var target = new ClockTime(1, 1, 1, 1);
        Assert.True(source.CompareTo(target) < 0);
        Assert.True(target.CompareTo(source) > 0);
    }

    //[Enforced]
    [Fact]
    public static void Test_Additions()
    {
        var obj = new ClockTime(0, 23, 59);
        var temp = obj.Add(24, 0);
        Assert.Equal(new ClockTime(0, 23, 59), temp);
    }

    //[Enforced]
    [Fact]
    public static void Test_ToString()
    {
        var item = new ClockTime(23, 59, 59, 999);
        var str = item.ToString();

        Assert.Equal("23:59:59.999", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_ToString_Culture_Info()
    {
        var item = new ClockTime(23, 59, 59, 999);
        string str;

        str = item.ToString(CultureInfo.InvariantCulture);
        Assert.Equal("23:59:59.999", str);

        str = item.ToString(CultureInfo.GetCultureInfo("ES-es"));
        Assert.Equal("23:59:59.999", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_ToString_Format()
    {
        var item = new ClockTime(23, 59, 59, 999);
        string str;

        str = item.ToString("HH"); Assert.Equal("23", str);
        str = item.ToString("-MM-"); Assert.Equal("-59-", str);
        str = item.ToString("SS:NNN"); Assert.Equal("59:999", str);

        item = new ClockTime(0, 0);
        str = item.ToString("HH-SS"); Assert.Equal("00-00", str);
    }
}
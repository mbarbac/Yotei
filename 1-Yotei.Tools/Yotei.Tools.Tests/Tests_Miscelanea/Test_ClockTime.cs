namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_ClockTime
{
    //[Enforced]
    [Fact]
    public static void Test_Constructors()
    {
        var item = new ClockTime();
        Assert.Equal(0, item.Hour);
        Assert.Equal(0, item.Minute);
        Assert.Equal(0, item.Second);
        Assert.Equal(0, item.Millisecond);

        item = new ClockTime(23, 59, 59, 999);
        Assert.Equal(23, item.Hour);
        Assert.Equal(59, item.Minute);
        Assert.Equal(59, item.Second);
        Assert.Equal(999, item.Millisecond);

        try { _ = new ClockTime(-1, 0, 0, 0); } catch (ArgumentException) { }
        try { _ = new ClockTime(0, -1, 0, 0); } catch (ArgumentException) { }
        try { _ = new ClockTime(0, 0, -1, 0); } catch (ArgumentException) { }
        try { _ = new ClockTime(0, 0, 0, -1); } catch (ArgumentException) { }

        try { _ = new ClockTime(24, 0, 0, 0); } catch (ArgumentException) { }
        try { _ = new ClockTime(0, 60, 0, 0); } catch (ArgumentException) { }
        try { _ = new ClockTime(0, 0, 60, 0); } catch (ArgumentException) { }
        try { _ = new ClockTime(0, 0, 0, 1000); } catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new ClockTime(23, 59, 59, 999);
        var target = source with { };

        Assert.NotSame(source, target);

        Assert.Equal(source.Hour, target.Hour);
        Assert.Equal(source.Minute, target.Minute);
        Assert.Equal(source.Second, target.Second);
        Assert.Equal(source.Millisecond, target.Millisecond);
    }

    //[Enforced]
    [Fact]
    public static void Test_Equality()
    {
        var source = new ClockTime(12, 30, 30);
        var target = new ClockTime(12, 30, 30);

        Assert.Equal(0, source.CompareTo(target));
        Assert.True(source == target);

        target = new ClockTime(12, 59);

        Assert.NotEqual(0, source.CompareTo(target));
        Assert.True(source != target);
        Assert.False(source == target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Comparison()
    {
        var source = new ClockTime(12, 30);
        var target = new ClockTime(12, 59);

        Assert.True(source.CompareTo(target) < 0);
        Assert.True(source < target);

        Assert.True(target.CompareTo(source) > 0);
        Assert.True(target > source);
    }

    //[Enforced]
    [Fact]
    public static void Test_Additions()
    {
        var source = new ClockTime();
        var target = source.Add(24);

        Assert.NotSame(source, target);
        Assert.Equal(0, target.Hour);
        Assert.Equal(0, target.Minute);
        Assert.Equal(0, target.Second);
        Assert.Equal(0, target.Millisecond);

        source = new ClockTime(23, 59, 59, 999);
        target = source.Add(0, 0, 0, 1);

        Assert.NotSame(source, target);
        Assert.Equal(0, target.Hour);
        Assert.Equal(0, target.Minute);
        Assert.Equal(0, target.Second);
        Assert.Equal(0, target.Millisecond);
    }

    //[Enforced]
    [Fact]
    public static void Test_ToString()
    {
        var item = new ClockTime(23, 59);
        var str = item.ToString();
        Assert.Equal("23:59", str);

        item = new ClockTime(23, 59, 59);
        str = item.ToString();
        Assert.Equal("23:59:59", str);

        item = new ClockTime(23, 59, 0, 999);
        str = item.ToString();
        Assert.Equal("23:59:00.999", str);

        item = new ClockTime(23, 59, 59, 999);
        str = item.ToString();
        Assert.Equal("23:59:59.999", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_ToString_Culture_Info()
    {
        var item = new ClockTime(23, 59, 59, 999);
        string str;

        str = item.ToString(CultureInfo.InvariantCulture);
        Assert.Equal("23:59", str);

        str = item.ToString(CultureInfo.GetCultureInfo("ES-es"));
        Assert.Equal("23:59", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_ToString_Format()
    {
        var item = new ClockTime(23, 59, 59, 999);
        string str;

        str = item.ToString("HH"); Assert.Equal("23", str);
        str = item.ToString("-mm-"); Assert.Equal("-59-", str);
        str = item.ToString("ss:fff"); Assert.Equal("59:999", str);

        item = new ClockTime(0, 0);
        str = item.ToString("HH-ss"); Assert.Equal("00-00", str);
    }
}
namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_DayDate
{
    //[Enforced]
    [Fact]
    public static void Test_Constructors()
    {
        var day = new DayDate(2000, 12, 31);
        Assert.Equal(2000, day.Year);
        Assert.Equal(12, day.Month);
        Assert.Equal(31, day.Day);

        try { _ = new DayDate(0, 1, 1); } catch (ArgumentException) { }
        try { _ = new DayDate(1, 0, 1); } catch (ArgumentException) { }
        try { _ = new DayDate(1, 1, 0); } catch (ArgumentException) { }

        Assert.True(DayDate.IsLeapYear(2000));
        _ = new DayDate(2000, 2, 29);
        try { _ = new DayDate(2000, 2, 30); Assert.Fail(); } catch (ArgumentException) { }

        Assert.False(DayDate.IsLeapYear(2001));
        try { _ = new DayDate(2001, 2, 29); Assert.Fail(); } catch (ArgumentException) { }
        try { _ = new DayDate(2001, 2, 30); Assert.Fail(); } catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Clone()
    {
        var source = new DayDate(2000, 12, 31);
        var target = source with { };

        Assert.NotSame(source, target);

        Assert.Equal(source.Year, target.Year);
        Assert.Equal(source.Month, target.Month);
        Assert.Equal(source.Day, target.Day);
    }

    //[Enforced]
    [Fact]
    public static void Test_Equality()
    {
        var source = new DayDate(1900, 1, 1);
        var target = new DayDate(1900, 1, 1);

        Assert.Equal(0, source.CompareTo(target));
        Assert.True(source == target);

        target = new DayDate(1901, 1, 1);

        Assert.NotEqual(0, source.CompareTo(target));
        Assert.True(source != target);
        Assert.False(source == target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Comparison()
    {
        var source = new DayDate(1900, 1, 1);
        var target = new DayDate(1900, 1, 2);

        Assert.True(source.CompareTo(target) < 0);
        Assert.True(source < target);

        Assert.True(target.CompareTo(source) > 0);
        Assert.True(target > source);
    }

    //[Enforced]
    [Fact]
    public static void Test_Additions()
    {
        var source = new DayDate(1900, 1, 31);
        var target = source.Add(1);
        Assert.NotSame(source, target);
        Assert.Equal(new DayDate(1900, 2, 1), target);

        source = new DayDate(1900, 12, 31);
        target = source.Add(1);
        Assert.NotSame(source, target);
        Assert.Equal(new DayDate(1901, 1, 1), target);
    }

    //[Enforced]
    [Fact]
    public static void Test_ToString()
    {
        var item = new DayDate(1999, 12, 31);
        var str = item.ToString();

        Assert.Equal("1999-12-31", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_ToString_Culture_Info()
    {
        var item = new DayDate(1999, 12, 31);
        string str;

        str = item.ToString(CultureInfo.InvariantCulture);
        Assert.Equal("12/31/1999", str);

        str = item.ToString(CultureInfo.GetCultureInfo("ES-es"));
        Assert.Equal("31/12/1999", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_ToString_Format()
    {
        var item = new DayDate(1999, 12, 31);
        string str;

        str = item.ToString("yyyy"); Assert.Equal("1999", str);
        str = item.ToString("-yy-"); Assert.Equal("-99-", str);
        str = item.ToString("d/M/yy"); Assert.Equal("31/12/99", str);

        item = new DayDate(1999, 3, 1);
        str = item.ToString("d/M/yy"); Assert.Equal("1/3/99", str);
    }
}
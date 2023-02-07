namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_CalendarDate
{
    //[Enforced]
    [Fact]
    public static void Error_Invalid_Constructors()
    {
        try { _ = new CalendarDate(0, 1, 1); Assert.True(false); }
        catch (ArgumentException) { }

        try { _ = new CalendarDate(1, 0, 1); Assert.True(false); }
        catch (ArgumentException) { }

        try { _ = new CalendarDate(1, 1, 0); Assert.True(false); }
        catch (ArgumentException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Equality_Scenarios()
    {
        var source = new CalendarDate(1900, 1, 1);
        var target = new CalendarDate(1900, 1, 1);
        Assert.True(source.CompareTo(target) == 0);

        source = new CalendarDate(1900, 1, 1);
        target = new CalendarDate(1900, 1, 2);
        Assert.True(source.CompareTo(target) != 0);
    }

    //[Enforced]
    [Fact]
    public static void Test_Comparison_Scenarios()
    {
        var source = new CalendarDate(1900, 1, 1);
        var target = new CalendarDate(1900, 1, 2);
        Assert.True(source.CompareTo(target) < 0);
        Assert.True(target.CompareTo(source) > 0);
    }

    //[Enforced]
    [Fact]
    public static void Test_Additions()
    {
        var obj = new CalendarDate(1900, 1, 31);
        var temp = obj.Add(1);
        Assert.Equal(new CalendarDate(1900, 2, 1), temp);
    }

    //[Enforced]
    [Fact]
    public static void Test_ToString()
    {
        var item = new CalendarDate(1999, 12, 31);
        var str = item.ToString();

        Assert.Equal("1999-12-31", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_ToString_Culture_Info()
    {
        var item = new CalendarDate(1999, 12, 31);
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
        var item = new CalendarDate(1999, 12, 31);
        string str;

        str = item.ToString("YYYY"); Assert.Equal("1999", str);
        str = item.ToString("-YY-"); Assert.Equal("-99-", str);
        str = item.ToString("D/M/YY"); Assert.Equal("31/12/99", str);

        item = new CalendarDate(1999, 3, 1);
        str = item.ToString("D/M/YY"); Assert.Equal("1/3/99", str);
    }
}
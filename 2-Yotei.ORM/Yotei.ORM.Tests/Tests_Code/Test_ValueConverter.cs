namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_ValueConverter
{
    //[Enforced]
    [Fact]
    public static void Test_DateTime_To_String()
    {
        var conv = new ValueConverter<DateTime, string>((x, loc) => x.ToString(loc.CultureInfo));

        var locale = new Locale(CultureInfo.GetCultureInfo("es-ES"));
        var source = new DateTime(2001, 12, 31, 23, 55, 59, 800, 900);
        var target = conv.Convert(source, locale);
        Assert.Equal("31/12/2001 23:55:59", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_DateOnly_To_DateTime()
    {
        var conv = new ValueConverter<DateOnly, DateTime>((x, loc) => x.ToDateTime(new()));

        var locale = new Locale(CultureInfo.GetCultureInfo("es-ES"));
        var source = new DateOnly(2001, 12, 31);
        var target = conv.Convert(source, locale);
        Assert.Equal(2001, target.Year);
        Assert.Equal(12, target.Month);
        Assert.Equal(31, target.Day);
        Assert.Equal(0, target.Hour);
        Assert.Equal(0, target.Minute);
        Assert.Equal(0, target.Second);
        Assert.Equal(0, target.Millisecond);
        Assert.Equal(0, target.Microsecond);
    }

    //[Enforced]
    [Fact]
    public static void Test_TimeOnly_To_TimeSpan()
    {
        var conv = new ValueConverter<TimeOnly, TimeSpan>((x, loc) => x.ToTimeSpan());

        var locale = new Locale(CultureInfo.GetCultureInfo("es-ES"));
        var source = new TimeOnly(23, 55, 59, 800, 900);
        var target = conv.Convert(source, locale);
        Assert.Equal(23, target.Hours);
        Assert.Equal(55, target.Minutes);
        Assert.Equal(59, target.Seconds);
        Assert.Equal(800, target.Milliseconds);
        Assert.Equal(900, target.Microseconds);
    }
}
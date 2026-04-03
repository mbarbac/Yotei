namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_ConvertToExtensions
{
    //[Enforced]
    [Fact]
    public static void Test_Conversion_From_Null()
    {
        object? target;

        target = ((object?)null).ConvertTo<object>(); Assert.Null(target);

        target = ((int?)null).ConvertTo<int?>(); Assert.Null(target);
        target = ((int?)null).ConvertTo<double?>(); Assert.Null(target);

        target = ((string?)null).ConvertTo<int?>(); Assert.Null(target);
        target = ((int?)null).ConvertTo<string?>(); Assert.Null(target);
    }

    //[Enforced]
    [Fact]
    public static void Error_Invalid_Conversion_From_Null()
    {
        try { ((int?)null).ConvertTo<int>(); Assert.Fail(); } catch (InvalidCastException) { }
        try { ((string?)null).ConvertTo<int>(); Assert.Fail(); } catch (InvalidCastException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Int_To_Int()
    {
        int source = 7;
        int target;

        target = source.ConvertTo<int, int>();
        Assert.Equal(7, target);

        target = source.ConvertTo<int>();
        Assert.Equal(7, target);

        target = (int)source.ConvertTo(typeof(int))!;
        Assert.Equal(7, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Int_To_Double()
    {
        int source = 7;
        double target;

        target = source.ConvertTo<int, double>();
        Assert.Equal(7.0, target);

        target = source.ConvertTo<double>();
        Assert.Equal(7.0, target);

        target = (double)source.ConvertTo(typeof(double))!;
        Assert.Equal(7.0, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Double_To_Int()
    {
        double source = 7.9;
        int target;

        target = source.ConvertTo<double, int>();
        Assert.Equal(7, target);

        target = source.ConvertTo<int>();
        Assert.Equal(7, target);

        target = (int)source.ConvertTo(typeof(int))!;
        Assert.Equal(7, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Int_To_String()
    {
        int source = 7;
        var target = source.ConvertTo<string>();
        Assert.Equal("7", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_String_To_Int()
    {
        string source = "7";
        var target = source.ConvertTo<int>();
        Assert.Equal(7, target);

        try { _ = "7.9".ConvertTo<int>(); Assert.Fail(); }
        catch (InvalidCastException) { }
    }


    // ----------------------------------------------------

    public record DayDate(int Year, int Month, int Day)
    {
        public static implicit operator DateTime(DayDate x) => new(x.Year, x.Month, x.Day);
        public static implicit operator DayDate(DateTime x) => new(x.Year, x.Month, x.Day);
    }

    //[Enforced]
    [Fact]
    public static void Test_Implicit_DayDate_To_DateTime()
    {
        var source = new DayDate(2001, 12, 31);
        var target = source.ConvertTo<DayDate, DateTime>();
        Assert.Equal(source.Year, target.Year);
        Assert.Equal(source.Month, target.Month);
        Assert.Equal(source.Day, target.Day);

        target = source.ConvertTo<DateTime>();
        Assert.Equal(source.Year, target.Year);
        Assert.Equal(source.Month, target.Month);
        Assert.Equal(source.Day, target.Day);

        target = (DateTime)source.ConvertTo(typeof(DateTime))!;
        Assert.Equal(source.Year, target.Year);
        Assert.Equal(source.Month, target.Month);
        Assert.Equal(source.Day, target.Day);
    }

    //[Enforced]
    [Fact]
    public static void Test_Implicit_DateTime_To_DayDate()
    {
        var source = new DateTime(2001, 12, 31);
        var target = source.ConvertTo<DateTime, DayDate>()!;
        Assert.Equal(source.Year, target.Year);
        Assert.Equal(source.Month, target.Month);
        Assert.Equal(source.Day, target.Day);

        target = source.ConvertTo<DayDate>()!;
        Assert.Equal(source.Year, target.Year);
        Assert.Equal(source.Month, target.Month);
        Assert.Equal(source.Day, target.Day);

        target = (DayDate)source.ConvertTo(typeof(DayDate))!;
        Assert.Equal(source.Year, target.Year);
        Assert.Equal(source.Month, target.Month);
        Assert.Equal(source.Day, target.Day);
    }

    //[Enforced]
    [Fact]
    public static void Test_DateTime_To_String_Locale()
    {
        var locale = CultureInfo.GetCultureInfo("es-ES");
        var source = new DateTime(2001, 12, 31);
        var target = source.ConvertTo<DateTime, string>(locale)!;
        Assert.Equal("31/12/2001 0:00:00", target);

        target = source.ConvertTo<string>(locale)!;
        Assert.Equal("31/12/2001 0:00:00", target);

        target = (string)source.ConvertTo(typeof(string), locale)!;
        Assert.Equal("31/12/2001 0:00:00", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_String_To_DateTime_Locale()
    {
        var locale = CultureInfo.GetCultureInfo("es-ES");
        var source = "31/12/2001";
        var target = source.ConvertTo<string, DateTime>(locale)!;
        Assert.Equal(2001, target.Year);
        Assert.Equal(12, target.Month);
        Assert.Equal(31, target.Day);

        target = source.ConvertTo<DateTime>(locale)!;
        Assert.Equal(2001, target.Year);
        Assert.Equal(12, target.Month);
        Assert.Equal(31, target.Day);

        target = (DateTime)source.ConvertTo(typeof(DateTime), locale)!;
        Assert.Equal(2001, target.Year);
        Assert.Equal(12, target.Month);
        Assert.Equal(31, target.Day);
    }

    // ----------------------------------------------------

    class TA { public string? FirstName { get; set; } = null; }
    class TB : TA { public string? LastName { get; set; } = null; }

    //[Enforced]
    [Fact]
    public static void Test_Covariant()
    {
        var source = new TB { FirstName = "James", LastName = "Bond" };
        var target = source.ConvertTo<TA>();
        Assert.Equal("James", target!.FirstName);
    }

    //[Enforced]
    [Fact]
    public static void Error_Contravariant_Fails()
    {
        var source = new TA { FirstName = "James" };

        try { source.ConvertTo<TB>(); Assert.Fail(); }
        catch (InvalidCastException) { }
    }

    // ----------------------------------------------------

    interface IE { }
    class TE : IE { }

    //[Enforced]
    [Fact]
    public static void Test_Class_To_Interface()
    {
        TE source = new();
        IE target = source.ConvertTo<IE>()!;

        Assert.True(typeof(IE).IsAssignableFrom(target.GetType()));
    }

    //[Enforced]
    [Fact]
    public static void Test_Interface_To_Class()
    {
        IE source = new TE();
        TE target = source.ConvertTo<TE>()!;

        Assert.True(typeof(TE).IsAssignableFrom(target.GetType()));
    }
}
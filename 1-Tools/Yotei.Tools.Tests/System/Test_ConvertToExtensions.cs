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
        try { ((int?)null).ConvertTo<int>(); Assert.True(false); }
        catch (InvalidCastException) { }

        try { ((string?)null).ConvertTo<int>(); Assert.True(false); }
        catch (InvalidCastException) { }
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
    public static void Test_Int_To_String_Fails()
    {
        Assert.False(7.TryConvertTo<int, string>(out var _));
        Assert.False(7.TryConvertTo<string>(out var _));
        Assert.False(7.TryConvertTo(typeof(string), out var _));
    }

    //[Enforced]
    [Fact]
    public static void Test_String_To_Int_Fails()
    {
        Assert.False("7".TryConvertTo<string, int>(out var _));
        Assert.False("7".TryConvertTo<int>(out var _));
        Assert.False("7".TryConvertTo(typeof(int), out var _));
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_DayDate_To_DateTime()
    {
        var source = new DayDate(2001, 12, 31);
        DateTime target;

        target = source.ConvertTo<DayDate, DateTime>();
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
    public static void Test_DateTime_To_DayDate()
    {
        var source = new DateTime(2001, 12, 31);
        DayDate target;

        target = source.ConvertTo<DateTime, DayDate>()!;
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

        try { var target = source.ConvertTo<TB>(); Assert.True(false); }
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
namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static class Test_ValueConverter
{
    //[Enforced]
    [Fact]
    public static void Test_Conversion()
    {
        var converter = new ValueConverter<DateOnly, string>();
        var value = new DateOnly(2000, 12, 31);

        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        var item = converter.Convert(value);
        Assert.Equal("12/31/2000", item);

        var locale = new Locale(new CultureInfo("es-ES"));
        item = converter.Convert(value, locale);
        Assert.Equal("31/12/2000", item);
    }
}
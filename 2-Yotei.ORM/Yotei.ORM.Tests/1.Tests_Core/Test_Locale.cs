namespace Yotei.ORM.Tests.Core;

// ========================================================
//[Enforced]
public class Test_Locale
{
    //[Enforced]
    [Fact]
    public void Test_Create()
    {
        var locale = new Locale();
        Assert.Equal(CultureInfo.InvariantCulture, locale.CultureInfo);
        Assert.Equal(CompareOptions.None, locale.CompareOptions);
        Assert.False(locale.CompareOptions.HasFlag(CompareOptions.OrdinalIgnoreCase));

        var culture = CultureInfo.GetCultureInfo("es-ES");
        var options = CompareOptions.OrdinalIgnoreCase;
        locale = new(culture, options);
        Assert.Equal("es-ES", locale.CultureInfo.Name);
        Assert.True(locale.CompareOptions.HasFlag(CompareOptions.OrdinalIgnoreCase));
    }

    //[Enforced]
    [Fact]
    public void Test_With()
    {
        var locale = new Locale();
        Assert.Equal(CultureInfo.InvariantCulture, locale.CultureInfo);
        Assert.Equal(CompareOptions.None, locale.CompareOptions);
        Assert.False(locale.CompareOptions.HasFlag(CompareOptions.OrdinalIgnoreCase));

        locale = locale with { CompareOptions = CompareOptions.OrdinalIgnoreCase };
        Assert.True(locale.CompareOptions.HasFlag(CompareOptions.OrdinalIgnoreCase));
    }
}
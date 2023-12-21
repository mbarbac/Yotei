namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_StringExtensions
{
    //[Enforced]
    [Fact]
    public static void Test_NullWhenEmpty()
    {
        string? source = null;
        string? target = source.NullWhenEmpty();
        Assert.Null(target);

        source = "  ";
        target = source.NullWhenEmpty();
        Assert.Null(target);

        source = "  ";
        target = source.NullWhenEmpty(trim: false);
        Assert.Null(target);
    }

    //[Enforced]
    [Fact]
    public static void Test_NotNullNotEmpty()
    {
        string? source = " any ";
        string? target = source.NotNullNotEmpty();
        Assert.Equal("any", target);

        source = " any ";
        target = source.NotNullNotEmpty(trim: false);
        Assert.Equal(" any ", target);

        try { source = null; target = source.NotNullNotEmpty(); Assert.Fail(); }
        catch (ArgumentNullException ex) { Assert.Equal(nameof(source), ex.ParamName); }

        try { source = " "; target = source.NotNullNotEmpty(); Assert.Fail(); }
        catch (ArgumentNullException ex) { Assert.Equal(nameof(source), ex.ParamName); }
    }

    // --------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_RemoveDiacritics()
    {
        string value, temp;

        value = "é"; temp = value.RemoveDiacritics(); Assert.Equal("e", temp);
        value = "É"; temp = value.RemoveDiacritics(); Assert.Equal("E", temp);
        value = "ñ"; temp = value.RemoveDiacritics(); Assert.Equal("n", temp);
        value = "Ñ"; temp = value.RemoveDiacritics(); Assert.Equal("N", temp);
        value = "ç"; temp = value.RemoveDiacritics(); Assert.Equal("c", temp);
        value = "Ç"; temp = value.RemoveDiacritics(); Assert.Equal("C", temp);
    }
}
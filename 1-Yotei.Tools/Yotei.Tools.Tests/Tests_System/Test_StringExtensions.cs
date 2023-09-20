namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_StringExtensions
{
    //[Enforced]
    [Fact]
    public static void Test_NullWhenEmpty()
    {
        string? source;
        string? target;

        source = null;
        target = source.NullWhenEmpty();
        Assert.Null(target);

        source = "  ";
        target = source.NullWhenEmpty();
        Assert.Null(target);

        target = source.NullWhenEmpty(trim: false);
        Assert.NotNull(target);
        Assert.Equal(source, target);
    }

    //[Enforced]
    [Fact]
    public static void Test_NotNullNotEmpty()
    {
        string? source;
        string? target;

        source = " any ";
        target = source.NotNullNotEmpty(trim: false);
        Assert.Equal(source, target);

        source = " any ";
        target = source.NotNullNotEmpty();
        Assert.Equal("any", target);

        try { source = null; target = source.NotNullNotEmpty(); Assert.Fail(); }
        catch (ArgumentNullException) { }
        
        try { source = "  "; target = source.NotNullNotEmpty(); Assert.Fail(); }
        catch (EmptyException) { }
    }
}
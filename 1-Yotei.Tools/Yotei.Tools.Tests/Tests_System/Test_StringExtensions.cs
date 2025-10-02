namespace Yotei.Tools.Tests;

// =============================================================
//[Enforced]
public static class Test_StringExtensions
{
    // [Enforced]
    [Fact]
    public static void Test_NullWhenEmpty_With_Trim()
    {
        string? source = null;
        string? result = source.NullWhenEmpty(true);
        Assert.Null(result);

        source = "";
        result = source.NullWhenEmpty(true);
        Assert.Null(result);

        source = " ";
        result = source.NullWhenEmpty(true);
        Assert.Null(result);
    }

    // [Enforced]
    [Fact]
    public static void Test_NullWhenEmpty_No_Trim()
    {
        string? source = null;
        string? result = source.NullWhenEmpty(false);
        Assert.Null(result);

        source = "";
        result = source.NullWhenEmpty(false);
        Assert.Null(result);

        source = " ";
        result = source.NullWhenEmpty(false); // spaces not removed!
        Assert.NotNull(result);
        Assert.Equal(" ", result);
    }

    // ----------------------------------------------------

    // [Enforced]
    [Fact]
    public static void Test_NotNullNotEmpty_With_Trim()
    {
        string? source = null;
        try { source.NotNullNotEmpty(true); Assert.Fail(); }
        catch (ArgumentNullException) { }

        source = string.Empty;
        try { source.NotNullNotEmpty(true); Assert.Fail(); }
        catch (EmptyException) { }

        source = " ";
        try { source.NotNullNotEmpty(true); Assert.Fail(); }
        catch (EmptyException) { }
    }

    // [Enforced]
    [Fact]
    public static void Test_NotNullNotEmpty_No_Trim()
    {
        string? source = null;
        try { source.NotNullNotEmpty(false); Assert.Fail(); }
        catch (ArgumentNullException) { }

        source = string.Empty;
        try { source.NotNullNotEmpty(false); Assert.Fail(); }
        catch (EmptyException) { }

        source = " ";
        Assert.Equal(" ", source.NotNullNotEmpty(false));
    }
}
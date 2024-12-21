using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_StringExtensions
{
    // [Enforced]
    [Fact]
    public static void Test_NullWhenEmpty_With_Trim()
    {
        string? source = null;
        string? result = source.NullWhenEmpty();
        Assert.Null(result);

        source = "";
        result = source.NullWhenEmpty();
        Assert.Null(result);

        source = " ";
        result = source.NullWhenEmpty();
        Assert.Null(result);
    }

    // [Enforced]
    [Fact]
    public static void Test_NullWhenEmpty_No_Trim()
    {
        string? source = null;
        string? result = source.NullWhenEmpty(trim: false);
        Assert.Null(result);

        source = "";
        result = source.NullWhenEmpty(trim: false);
        Assert.Null(result);

        source = " ";
        result = source.NullWhenEmpty(trim: false); // spaces not removed!
        Assert.NotNull(result);
        Assert.Equal(" ", result);
    }

    // ----------------------------------------------------

    // [Enforced]
    [Fact]
    public static void Test_NotNullNotEmpty_With_Trim()
    {
        string? source = null;
        Assert.Throws<ArgumentNullException>(() => source.NotNullNotEmpty());

        source = string.Empty;
        Assert.Throws<EmptyException>(() => source.NotNullNotEmpty());

        source = " ";
        Assert.Throws<EmptyException>(() => source.NotNullNotEmpty());
    }

    // [Enforced]
    [Fact]
    public static void Test_NotNullNotEmpty_No_Trim()
    {
        string? source = null;
        Assert.Throws<ArgumentNullException>(() => source.NotNullNotEmpty(trim: false));

        source = string.Empty;
        Assert.Throws<EmptyException>(() => source.NotNullNotEmpty(trim: false));

        source = " ";
        Assert.Equal(" ", source.NotNullNotEmpty(trim: false));
    }
}
using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_TimeSpanExtensions
{
    //[Enforced]
    [Fact]
    public static void Test_ValidateTimeout_Cero()
    {
        var span = TimeSpan.FromMilliseconds(0);
        var ms = span.ValidateTimeout();
        Assert.Equal(0, ms);
    }

    // [Enforced]
    [Fact]
    public static void Test_ValidateTimeout_Positive()
    {
        var span = TimeSpan.FromMilliseconds(5);
        var ms = span.ValidateTimeout();
        Assert.Equal(5, ms);
    }

    // [Enforced]
    [Fact]
    public static void Test_ValidateTimeout_Infinite()
    {
        var span = TimeSpan.FromMilliseconds(-1);
        var ms = span.ValidateTimeout();
        Assert.Equal(-1, ms);

        span = TimeSpan.FromMilliseconds(Timeout.Infinite);
        ms = span.ValidateTimeout();
        Assert.Equal(-1, ms);
    }

    // [Enforced]
    [Fact]
    public static void Test_ValidateTimeout_Negative_Must_Fail()
    {
        var span = TimeSpan.FromMilliseconds(-2);
        Assert.Throws<ArgumentOutOfRangeException>(() => span.ValidateTimeout());
    }
}
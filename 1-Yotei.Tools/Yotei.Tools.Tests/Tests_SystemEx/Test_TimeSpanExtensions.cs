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
        var ms = span.ValidatedTimeout;
        Assert.Equal(0, ms);
    }

    // [Enforced]
    [Fact]
    public static void Test_ValidateTimeout_Positive()
    {
        var span = TimeSpan.FromMilliseconds(5);
        var ms = span.ValidatedTimeout;
        Assert.Equal(5, ms);
    }

    // [Enforced]
    [Fact]
    public static void Test_ValidateTimeout_Infinite()
    {
        var span = TimeSpan.FromMilliseconds(-1);
        var ms = span.ValidatedTimeout;
        Assert.Equal(-1, ms);

        span = TimeSpan.FromMilliseconds(Timeout.Infinite);
        ms = span.ValidatedTimeout;
        Assert.Equal(-1, ms);
    }

    // [Enforced]
    [Fact]
    public static void Test_ValidateTimeout_Negative_Must_Fail()
    {
        var span = TimeSpan.FromMilliseconds(-2);
        try { _ = span.ValidatedTimeout; Assert.Fail(); }
        catch (ArgumentOutOfRangeException) { }
    }
}
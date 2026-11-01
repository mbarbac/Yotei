#if NETSTANDARD2_0
namespace Yotei.Tools.Std20.Tests;
#else
namespace Yotei.Tools;
#endif

// ========================================================
public static class Test_ArgumentException
{
    [Fact]
    public static void Test_ThrowIfNullOrEmpty()
    {
        string? value = null;
        try { ArgumentException.ThrowIfNullOrEmpty(value); Assert.Fail(""); }
        catch (ArgumentNullException) { }

        value = "";
        try { ArgumentException.ThrowIfNullOrEmpty(value); Assert.Fail(""); }
        catch (EmptyException) { }

        value = " ";
        ArgumentException.ThrowIfNullOrEmpty(value);
    }

    [Fact]
    public static void Test_ThrowIfNullOrWhiteSpace()
    {
        string? value = null;
        try { ArgumentException.ThrowIfNullOrWhiteSpace(value); Assert.Fail(""); }
        catch (ArgumentNullException) { }

        value = "";
        try { ArgumentException.ThrowIfNullOrWhiteSpace(value); Assert.Fail(""); }
        catch (EmptyException) { }

        value = " ";
        try { ArgumentException.ThrowIfNullOrWhiteSpace(value); Assert.Fail(""); }
        catch (ArgumentException) { }
    }
}
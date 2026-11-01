namespace Yotei.Tools.Tests;

// ========================================================
public static class Test_ArgumentException
{
    public class Example { }

    // ----------------------------------------------------

    [Fact]
    public static void Test_ThrowIfNull()
    {
        Example? value = null;
        try { ArgumentNullException.ThrowIfNull(value); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }
}
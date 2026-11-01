namespace Yotei.Tools.Tests;

// ========================================================
public static class Test_ArgumentNullException
{
    [Fact]
    public static void Test_ThrowIfNull()
    {
        string? value = null;
        try { ArgumentNullException.ThrowIfNull(value); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }
}
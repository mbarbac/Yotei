namespace Yotei.Tools.Tests;

// ========================================================
public static class Test_StringComparison
{
    [Fact]
    public static void Test_GetComparer()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var comparer = StringComparer.FromComparison(comparison);
    }
}
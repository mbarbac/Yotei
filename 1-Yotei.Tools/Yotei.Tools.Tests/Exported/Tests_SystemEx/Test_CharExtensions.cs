namespace Yotei.Tools.Tests;

// ========================================================
public static class Test_CharExtensions
{
    [Fact]
    public static void Test_EqualsEx()
    {
        var source = 'a';
        var target = 'A';

        Assert.False(source.Equals(target));

        Assert.False(source.Equals(target, ignoreCase: false));
        Assert.True(source.Equals(target, ignoreCase: true));

        Assert.False(source.Equals(target, StringComparer.Ordinal));
        Assert.True(source.Equals(target, StringComparer.OrdinalIgnoreCase));

        Assert.False(source.Equals(target, StringComparison.Ordinal));
        Assert.True(source.Equals(target, StringComparison.OrdinalIgnoreCase));
    }
}
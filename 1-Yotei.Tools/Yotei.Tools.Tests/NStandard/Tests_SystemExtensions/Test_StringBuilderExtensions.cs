namespace Yotei.Tools.Tests;

// ========================================================
public static partial class Test_StringBuilderExtensions
{
    [Fact]
    public static void Test_Equals()
    {
        var source = new StringBuilder();
        var target = new StringBuilder();        
        Assert.True(source.Equals(target));
        Assert.True(source.Equals(target, ignoreCase: true));
        Assert.True(source.Equals(target, char.CharComparer()));
        Assert.True(source.Equals(target, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.Equals(target, StringComparison.OrdinalIgnoreCase));

        source = new("abc");
        target = new("ABC");
        Assert.False(source.Equals(target));
        Assert.True(source.Equals(target, ignoreCase: true));
        Assert.False(source.Equals(target, char.CharComparer()));
        Assert.True(source.Equals(target, StringComparer.OrdinalIgnoreCase));
        Assert.True(source.Equals(target, StringComparison.OrdinalIgnoreCase));
    }
}
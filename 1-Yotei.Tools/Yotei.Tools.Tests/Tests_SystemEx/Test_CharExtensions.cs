namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_CharExtensions
{
    readonly struct CharComparer(bool IgnoreCase) : IEqualityComparer<char>
    {
        public bool Equals(char x, char y)
            => IgnoreCase ? char.ToLower(x) == char.ToLower(y) : x == y;

        public int GetHashCode([DisallowNull] char obj) => throw new NotImplementedException();
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Equals()
    {
        var source = 'a';
        var target = 'A';

        Assert.False(source.Equals(target));

        Assert.False(source.Equals(target, ignoreCase: false));
        Assert.True(source.Equals(target, ignoreCase: true));

        Assert.False(source.Equals(target, new CharComparer(false)));
        Assert.True(source.Equals(target, new CharComparer(true)));

        Assert.False(source.Equals(target, StringComparer.Ordinal));
        Assert.True(source.Equals(target, StringComparer.OrdinalIgnoreCase));

        Assert.False(source.Equals(target, StringComparison.Ordinal));
        Assert.True(source.Equals(target, StringComparison.OrdinalIgnoreCase));
    }
}
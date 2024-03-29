namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_CharExtensions
{
    //[Enforced]
    [Fact]
    public static void Test_Remove_Diacritics()
    {
        char value, temp;

        value = '�'; temp = value.RemoveDiacritics(); Assert.Equal('e', temp);
        value = '�'; temp = value.RemoveDiacritics(); Assert.Equal('E', temp);
        value = '�'; temp = value.RemoveDiacritics(); Assert.Equal('n', temp);
        value = '�'; temp = value.RemoveDiacritics(); Assert.Equal('N', temp);
        value = '�'; temp = value.RemoveDiacritics(); Assert.Equal('c', temp);
        value = '�'; temp = value.RemoveDiacritics(); Assert.Equal('C', temp);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Equals_CaseSensitive()
    {
        var source = 'a';
        var target = 'A';
        Assert.True(source.Equals(target, caseSensitive: false));
        Assert.False(source.Equals(target, caseSensitive: true));
    }

    //[Enforced]
    [Fact]
    public static void Test_Equals_Comparison()
    {
        var source = 'a';
        var target = 'A';
        Assert.True(source.Equals(target, StringComparison.OrdinalIgnoreCase));
        Assert.False(source.Equals(target, StringComparison.Ordinal));
    }

    //[Enforced]
    [Fact]
    public static void Test_Equals_Comparer()
    {
        var source = 'a';
        var target = 'A';
        Assert.True(source.Equals(target, StringComparer.OrdinalIgnoreCase));
        Assert.False(source.Equals(target, StringComparer.Ordinal));
    }
}
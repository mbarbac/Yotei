using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

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

        value = 'é'; temp = value.RemoveDiacritics(); Assert.Equal('e', temp);
        value = 'É'; temp = value.RemoveDiacritics(); Assert.Equal('E', temp);
        value = 'ñ'; temp = value.RemoveDiacritics(); Assert.Equal('n', temp);
        value = 'Ñ'; temp = value.RemoveDiacritics(); Assert.Equal('N', temp);
        value = 'ç'; temp = value.RemoveDiacritics(); Assert.Equal('c', temp);
        value = 'Ç'; temp = value.RemoveDiacritics(); Assert.Equal('C', temp);
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
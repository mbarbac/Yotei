namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_CharExtensions
{
    //[Enforced]
    [Fact]
    public static void Remove_Diacritics()
    {
        char value, temp;

        value = 'é'; temp = value.RemoveDiacritics(); Assert.Equal('e', temp);
        value = 'É'; temp = value.RemoveDiacritics(); Assert.Equal('E', temp);
        value = 'ñ'; temp = value.RemoveDiacritics(); Assert.Equal('n', temp);
        value = 'Ñ'; temp = value.RemoveDiacritics(); Assert.Equal('N', temp);
        value = 'ç'; temp = value.RemoveDiacritics(); Assert.Equal('c', temp);
        value = 'Ç'; temp = value.RemoveDiacritics(); Assert.Equal('C', temp);
    }
}
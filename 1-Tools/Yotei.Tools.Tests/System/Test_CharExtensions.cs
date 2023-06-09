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

        value = '�'; temp = value.RemoveDiacritics(); Assert.Equal('e', temp);
        value = '�'; temp = value.RemoveDiacritics(); Assert.Equal('E', temp);
        value = '�'; temp = value.RemoveDiacritics(); Assert.Equal('n', temp);
        value = '�'; temp = value.RemoveDiacritics(); Assert.Equal('N', temp);
        value = '�'; temp = value.RemoveDiacritics(); Assert.Equal('c', temp);
        value = '�'; temp = value.RemoveDiacritics(); Assert.Equal('C', temp);
    }
}
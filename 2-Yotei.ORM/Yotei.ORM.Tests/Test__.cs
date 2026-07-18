namespace Yotei.ORM.Tests;

// ========================================================
//[Enforced]
public static partial class Test_
{
    //[Enforced]
    [Fact]
    public static void Test()
    {
        FindNextOrdinalBracket("abc{25a}{100}xyz", 0, out _, out _);
    }

    static int FindNextOrdinalBracket(string text, int ini, out string? bracket, out int value)
    {
        bracket = null;
        value = 0;

        int pos = ini;
        while ((pos = FindNextBracket(text, pos, out bracket)) >= 0)
        {
            var span = bracket.AsSpan(1, bracket!.Length - 2);
            var valid = true;
            foreach (var c in span) if (!char.IsAsciiDigit(c)) { valid = false; break; }

            if (valid && int.TryParse(span, out value)) return pos;
            pos += bracket.Length;
        }

        return -1;
    }

    static int FindNextBracket(string text, int ini, out string? bracket)
    {
        bracket = null;

        var pos = text.IndexOf('{', ini); if (pos < 0) return -1;
        var end = text.IndexOf('}', pos); if (end < 0) return -1;

        bracket = text.Substring(pos, end - pos + 1);
        return pos;
    }
}
namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_StringBuilderPool
{
    //[Enforced]
    [Fact]
    public static void Test_Rent_And_Return()
    {
        var max = 100_000;
        for (int i = 0; i < max; i++)
        {
            var str = GetString();
            var sb = StringBuilder.Pool.Rent();
            Assert.Equal(0, sb.Length);
            sb.Append(str);

            var ret = StringBuilder.Pool.Return(sb);
            StringBuilder.Pool.Return(sb);
            StringBuilder.Pool.Return(sb);
            Assert.Equal(str, ret);
        }
    }

    readonly static Random Random = new();

    static string GetString()
    {
        var len = Random.Next(10, 100);
        var arr = new char[len];
        for (int i = 0; i < len; i++) arr[i] = CHARS[Random.Next(CHARS.Length)];
        return new string(arr); 
    }

    static readonly char[] CHARS =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"
        .ToCharArray();
}
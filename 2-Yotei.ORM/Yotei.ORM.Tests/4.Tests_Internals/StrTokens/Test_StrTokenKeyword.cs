using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_StrTokenKeyword
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var token = new StrTokenKeyword("XX");
        Assert.Equal("XX", token.Payload);

        try { _ = new StrTokenKeyword(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new StrTokenKeyword(" "); Assert.Fail(); }
        catch (EmptyException) { }
    }
}
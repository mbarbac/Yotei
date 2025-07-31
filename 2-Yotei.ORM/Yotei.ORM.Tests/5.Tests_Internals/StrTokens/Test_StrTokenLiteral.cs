namespace Yotei.ORM.Tests.Internals.StrTokens;

// ========================================================
//[Enforced]
public static class Test_StrTokenLiteral
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var token = new StrTokenLiteral(" ");
        Assert.Equal(" ", token.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty_Fails()
    {
        try { _ = new StrTokenLiteral(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }

        try { _ = new StrTokenLiteral(string.Empty); Assert.Fail(); }
        catch (EmptyException) { }
    }
}
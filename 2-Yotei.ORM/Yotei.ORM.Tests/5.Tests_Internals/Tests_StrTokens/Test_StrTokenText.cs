namespace Yotei.ORM.Tests.Internals.StrTokens;

// ========================================================
//[Enforced]
public static class Test_StrTokenText
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var token = new StrTokenText(" ");
        Assert.Equal(" ", token.Payload);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Empty()
    {
        var token = new StrTokenText(string.Empty);
        Assert.Equal(string.Empty, token.Payload);

        try { _ = new StrTokenText(null!); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Equals_Empty()
    {
        var source = new StrTokenText();
        var target = StrTokenText.Empty;

        var eq = source.Equals(target); Assert.True(eq);
        eq = source == target; Assert.True(eq);
    }
}
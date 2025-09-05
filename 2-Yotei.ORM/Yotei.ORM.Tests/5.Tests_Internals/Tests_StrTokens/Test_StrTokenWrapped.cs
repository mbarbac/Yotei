namespace Yotei.ORM.Tests.Internals.StrTokens;

// ========================================================
//[Enforced]
public static class Test_StrTokenWrapped
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var token = new StrTokenWrapped('[', new StrTokenText("xx"), ']');
        Assert.Equal("[xx]", token.ToString());

        try { _ = new StrTokenWrapped('[', null!, ']'); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Reduce_Nested()
    {
        var comparison = StringComparison.OrdinalIgnoreCase;
        var one = new StrTokenWrapped('[', new StrTokenText("xx"), ']');
        var two = new StrTokenWrapped('[', one, ']');

        var token = two.Reduce(comparison);
        Assert.Same(token, one);

        var three = new StrTokenWrapped('[', two, ']');
        token = three.Reduce(comparison);
        Assert.Same(token, one);
    }
}
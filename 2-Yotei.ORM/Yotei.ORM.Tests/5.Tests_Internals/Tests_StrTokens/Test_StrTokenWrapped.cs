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

    //[Enforced]
    [Fact]
    public static void Test_Create_Reduce()
    {
        var inner = new StrTokenWrapped('[', new StrTokenText("xx"), ']');
        var outer = new StrTokenWrapped('[', inner, ']');

        var comparison = StringComparison.OrdinalIgnoreCase;
        var token = outer.Reduce(comparison);
        Assert.Same(token, inner);

        outer = new StrTokenWrapped('{', inner, '}'); // Not reducing as wrappers not same...
        token = outer.Reduce(comparison);
        Assert.Same(token, outer);
    }
}
using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

using Text = Yotei.ORM.Internal.StrTokenText;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_StrTokenWrapped
{
    //[Enforced]
    [Fact]
    public static void Test_Create()
    {
        var token = new StrTokenWrapped('[', new Text("xx"), ']');
        Assert.Equal("[xx]", token.ToString());

        try { _ = new StrTokenWrapped('[', null!, ']'); Assert.Fail(); }
        catch (ArgumentNullException) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Reduce()
    {
        var inner = new StrTokenWrapped('[', new Text("xx"), ']');
        var outer = new StrTokenWrapped('[', inner, ']');

        var comparison = StringComparison.OrdinalIgnoreCase;
        var token = outer.Reduce(comparison);
        Assert.Same(token, inner);

        outer = new StrTokenWrapped('{', inner, '}'); // Not reducing as wrappers not same...
        token = outer.Reduce(comparison);
        Assert.Same(token, outer);
    }
}
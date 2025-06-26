using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.ORM.Tests.Internals;

// ========================================================
//[Enforced]
public static class Test_DbTokenMethod
{
    //[Enforced]
    [Fact]
    public static void Test_Create_Parameterless_NoGenerics()
    {
        var arg = new DbTokenArgument("x");
        var method = new DbTokenMethod(arg, "Any");

        Assert.Equal("Any", method.Name);
        Assert.Empty(method.TypeArguments);
        Assert.Empty(method.Arguments);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_ParameterFull_NoGenerics()
    {
        var arg = new DbTokenArgument("x");
        var method = new DbTokenMethod(arg, "Any", [
            new DbTokenValue(1),
            new DbTokenValue(2)]);

        Assert.Equal("Any", method.Name);
        Assert.Empty(method.TypeArguments);
        Assert.Equal(2, method.Arguments.Count);
        Assert.Equal(1, ((DbTokenValue)method.Arguments[0]).Value);
        Assert.Equal(2, ((DbTokenValue)method.Arguments[1]).Value);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_Parameterless_WithGenerics()
    {
        var arg = new DbTokenArgument("x");
        var method = new DbTokenMethod(arg, "Any", [typeof(string)]);

        Assert.Equal("Any", method.Name);
        Assert.Single(method.TypeArguments);
        Assert.Equal(typeof(string), method.TypeArguments[0]);
        Assert.Empty(method.Arguments);
    }

    //[Enforced]
    [Fact]
    public static void Test_Create_ParameterFull_WithGenerics()
    {
        var arg = new DbTokenArgument("x");
        var method = new DbTokenMethod(arg, "Any",
            [typeof(string)],
            [new DbTokenValue(1), new DbTokenValue(2)]);

        Assert.Equal("Any", method.Name);
        Assert.Single(method.TypeArguments);
        Assert.Equal(typeof(string), method.TypeArguments[0]);
        Assert.Equal(2, method.Arguments.Count);
        Assert.Equal(1, ((DbTokenValue)method.Arguments[0]).Value);
        Assert.Equal(2, ((DbTokenValue)method.Arguments[1]).Value);
    }
}
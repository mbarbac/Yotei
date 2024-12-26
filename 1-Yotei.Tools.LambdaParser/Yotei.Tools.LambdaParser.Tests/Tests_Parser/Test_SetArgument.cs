using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_SetArgument
{
    //[Enforced]
    [Fact]
    public static void Parse_ToConstant_Resolves_In_Constant()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeValue item;

        WriteLine();
        func = x => x = 7;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        Assert.Equal("'7'", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_ToArgument_Resolves_In_Argument()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeArgument item;

        WriteLine();
#pragma warning disable CS1717 // Assignment made to same variable
        func = x => x = x;
#pragma warning restore CS1717 // Assignment made to same variable
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeArgument>(node);
        Assert.Equal("x", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_ToDynamic()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeMember item;

        WriteLine();
        func = x => x = x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Alpha", node.ToString());

        WriteLine();
        func = x => x = x.Alpha.Beta;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Alpha.Beta", node.ToString());
    }
}
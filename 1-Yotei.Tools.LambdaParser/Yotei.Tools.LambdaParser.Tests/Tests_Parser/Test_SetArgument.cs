using static Yotei.Tools.Diagnostics.ConsoleWrapper;
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
        LambdaNodeConstant item;

        WriteLine(true);
        func = x => x = 7;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeConstant>(node);
        Assert.Equal("'7'", node.ToString());
    }

#pragma warning disable CS1717
    //[Enforced]
    [Fact]
    public static void Parse_ToArgument_Resolves_In_Argument()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeArgument item;

        WriteLine(true);
        func = x => x = x;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeArgument>(node);
        Assert.Equal("x", node.ToString());
    }
#pragma warning restore

    //[Enforced]
    [Fact]
    public static void Parse_ToDynamic()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeMember item;

        WriteLine(true);
        func = x => x = x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Alpha", node.ToString());

        WriteLine(true);
        func = x => x = x.Alpha.Beta;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Alpha.Beta", node.ToString());
    }
}
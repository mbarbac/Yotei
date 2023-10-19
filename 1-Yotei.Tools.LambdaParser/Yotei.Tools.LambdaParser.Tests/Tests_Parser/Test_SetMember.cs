using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_SetMember
{
    //[Enforced]
    [Fact]
    public static void Parse_ToConstant()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine(true);
        func = x => x.Alpha = null!;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha = 'NULL')", node.ToString());

        WriteLine(true);
        func = x => x.Alpha = 7;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha = '7')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Chained_ToConstant()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine(true);
        func = x => x.Alpha.Beta = null!;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha.Beta = 'NULL')", node.ToString());

        WriteLine(true);
        func = x => x.Alpha.Beta = 7;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha.Beta = '7')", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_ToDynamic()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine(true);
        func = x => x.Alpha = x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha = x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Chained_ToDynamic()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine(true);
        func = x => x.Alpha.Beta = x.Beta.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha.Beta = x.Beta.Alpha)", node.ToString());
    }
}
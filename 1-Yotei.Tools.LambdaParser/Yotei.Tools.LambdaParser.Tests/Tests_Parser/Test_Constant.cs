using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Constant
{
    //[Enforced]
    [Fact]
    public static void Parse_Null()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeConstant item;

        WriteLine(true);
        func = x => null!;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeConstant>(node);
        Assert.Equal("'NULL'", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Arbitrary()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeConstant item;

        WriteLine(true);
        func = x => 7;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeConstant>(node);
        Assert.Equal("'7'", node.ToString());

        WriteLine(true);
        func = x => "any";
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeConstant>(node);
        Assert.Equal("'any'", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_With_Cast()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeConstant item;

        WriteLine(true);
        func = x => (string?)null!;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeConstant>(node);
        Assert.Equal("'NULL'", node.ToString());

        WriteLine(true);
        func = x => (int)10.5!;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeConstant>(node);
        Assert.Equal("'10'", node.ToString());
    }
}
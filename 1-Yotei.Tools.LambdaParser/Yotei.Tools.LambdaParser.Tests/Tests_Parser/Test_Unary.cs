using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Unary
{
    //[Enforced]
    [Fact]
    public static void Parse_Not_Constant()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeConstant item;

        WriteLine(true);
        var value = true;
        func = x => !value;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeConstant>(node);
        Assert.Equal("'False'", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Not_Argument()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeUnary item;

        WriteLine(true);
        func = x => !x;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeUnary>(node);
        Assert.Equal("(Not x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Not_Dynamic()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeUnary item;

        WriteLine(true);
        func = x => !x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeUnary>(node);
        Assert.Equal("(Not x.Alpha)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Negate_Constant()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeConstant item;

        WriteLine(true);
        var value = 7;
        func = x => -value;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeConstant>(node);
        Assert.Equal("'-7'", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Negate_Argument()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeUnary item;

        WriteLine(true);
        func = x => -x;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeUnary>(node);
        Assert.Equal("(Negate x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Negate_Dynamic()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeUnary item;

        WriteLine(true);
        func = x => -x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeUnary>(node);
        Assert.Equal("(Negate x.Alpha)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_PreIncrement_Argument()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeUnary item;

        WriteLine(true);
        func = x => ++x;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeUnary>(node);
        Assert.Equal("(Increment x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_PreIncrement_Dynamic_Resolves_To_Setter()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        // A pre-increment is logically an increment followed by a setter, which exactly is
        // what the parser renders. So we consider this test as passed.

        WriteLine(true);
        func = x => ++x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha = (Increment x.Alpha))", node.ToString());
    }
}
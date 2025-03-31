using static Yotei.Tools.Diagnostics.DebugEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Constant
{
    //[Enforced]
    [Fact]
    public static void Parse_Null() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeValue item;

        WriteLine();
        func = x => null!; // Returns the value without invoking the dynamic argument...
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        Assert.Equal("'NULL'", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_Arbitrary() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeValue item;

        WriteLine();
        func = x => 7; // Returns the value without invoking the dynamic argument...
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        Assert.Equal("'7'", node.ToString());

        WriteLine();
        func = x => "any"; // Returns the value without invoking the dynamic argument...
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        Assert.Equal("'any'", node.ToString());
    });

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_With_Cast_Constant() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeValue item;

        WriteLine();
        func = x => (string?)null!;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        Assert.Equal("'NULL'", node.ToString());

        WriteLine();
        func = x => (int)10.5!;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        Assert.Equal("'10'", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_With_Cast_Dynamic() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeConvert item;

        WriteLine();
        func = x => (string?)x!;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeConvert>(node);
        Assert.Equal("((String) x)", node.ToString());

        WriteLine();
        func = x => (int)x.Delta.Beta.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeConvert>(node);
        Assert.Equal("((Int32) x.Delta.Beta.Alpha)", node.ToString());
    });
}
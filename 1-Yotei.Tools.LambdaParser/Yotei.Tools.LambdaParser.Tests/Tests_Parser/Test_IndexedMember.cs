using static Yotei.Tools.Diagnostics.DebugEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_IndexedMember
{
    //[Enforced]
    [Fact]
    public static void Parse_IxConstant() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeIndexed item;

        WriteLine();
        func = x => x.Alpha[7];
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha['7']", node.ToString());

        WriteLine();
        func = x => x.Alpha[7, 9];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha['7', '9']", node.ToString());

        WriteLine();
        func = x => x.Alpha[7][9];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha['7']['9']", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_IxArgument() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeIndexed item;

        WriteLine();
        func = x => x.Alpha[x];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[x]", node.ToString());

        WriteLine();
        func = x => x.Alpha[x][x];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[x][x]", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_IxDynamic() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeIndexed item;

        WriteLine();
        func = x => x.Alpha[x.Alpha];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[x.Alpha]", node.ToString());

        WriteLine();
        func = x => x.Alpha[x.Alpha, x.Beta];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[x.Alpha, x.Beta]", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_IxDynamic_Assign() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeIndexed item;

        WriteLine();
        func = x => x.Alpha[x.Alpha = 7];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[(x.Alpha = '7')]", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_IxDynamic_Chained() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeIndexed item;

        WriteLine();
        func = x => x.Alpha[x.Alpha][x.Beta];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[x.Alpha][x.Beta]", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_IxArbitrary() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeIndexed item;

        WriteLine();
        func = x => x.Alpha[x.Alpha, null, 7];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[x.Alpha, 'NULL', '7']", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_IxArbitrary_Chained() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeIndexed item;

        WriteLine();
        func = x => x.Alpha[x.Beta = 9][x[x.Alpha], null, 7];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[(x.Beta = '9')][x[x.Alpha], 'NULL', '7']", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_Nested() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeIndexed item;

        WriteLine();
        func = x => x.Alpha[x[x]];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[x[x]]", node.ToString());

        WriteLine();
        func = x => x.Alpha[x.Beta[x.Alpha]];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[x.Beta[x.Alpha]]", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_Nested_Assign() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeIndexed item;

        WriteLine();
        func = x => x.Alpha[x[x = 7]]; // Index reduced to the value becuase assigned to argument...
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[x['7']]", node.ToString());

        WriteLine();
        func = x => x.Alpha[x[x.Alpha = 7]];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[x[(x.Alpha = '7')]]", node.ToString());
    });
}
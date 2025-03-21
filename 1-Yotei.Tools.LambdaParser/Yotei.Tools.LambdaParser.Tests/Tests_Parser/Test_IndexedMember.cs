﻿using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_IndexedMember
{
    //[Enforced]
    [Fact]
    public static void Parse_IxConstant()
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
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxArgument()
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
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxDynamic()
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
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxDynamic_Chained()
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
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxArbitrary()
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
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxArbitrary_Chained()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeIndexed item;

        WriteLine();
        func = x => x.Alpha[x.Beta][x[x.Alpha], null, 7];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[x.Beta][x[x.Alpha], 'NULL', '7']", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Nested()
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
    }
}
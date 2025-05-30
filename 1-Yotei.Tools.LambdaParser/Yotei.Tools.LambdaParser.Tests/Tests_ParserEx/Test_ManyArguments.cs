﻿using static Yotei.Tools.Diagnostics.DebugEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_ManyArguments
{
    //[Enforced]
    [Fact]
    public static void Parse_Only_Concretes() => Repeater.Repeat(() =>
    {
        Func<int, int> func = (x) => x + 1;
        LambdaNode node;
        LambdaNodeValue item;

        WriteLine();
        node = LambdaParser.Parse(func, 1).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        Assert.Equal(2, item.LambdaValue);

        try { node = LambdaParser.Parse(func).Result; Assert.Fail(); }
        catch (NotFoundException) { }

        try { node = LambdaParser.Parse(func, 1, 1).Result; Assert.Fail(); }
        catch (InvalidOperationException) { }
    });

    //[Enforced]
    [Fact]
    public static void Parse_Several_Arguments_Equality() => Repeater.Repeat(() =>
    {
        Func<dynamic, dynamic, object> func = (x, y) => x == y;
        LambdaNode node;
        LambdaNodeBinary item;

        WriteLine();
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x Equal y)", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_Several_Arguments_Indexes() => Repeater.Repeat(() =>
    {
        Func<dynamic, dynamic, object> func = (x, y) => x.Alpha[y.Apha] = y.Alpha[x.Alpha];
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine();
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[y.Apha] = y.Alpha[x.Alpha])", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_Several_Arguments_Indexes_And_Convert() => Repeater.Repeat(() =>
    {
        Func<dynamic, dynamic, object>
            func = (x, y) => x.Alpha[(int)y.Apha] = (int)y.Alpha[(DateTime)x.Alpha];

        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine();
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal(
            "(x.Alpha[((Int32) y.Alpha[((DateTime) x.Alpha)])] = ((Int32) y.Alpha[((DateTime) x.Alpha)]))",
            node.ToString());
    });
}
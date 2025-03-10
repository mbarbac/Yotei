﻿using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Ternary_Caveats
{
    //[Enforced]
    [Fact]
    public static void Standard_Ternary_Does_Not_Work()
    {
        Func<dynamic, object> func;
        LambdaNode node;

        WriteLine();
        func = x => x.Alpha ? x.Beta : x.Delta;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");

        Assert.NotEqual("x.Alpha ? x.Beta : x.Delta", node.ToString());
        Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Delta", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Ternary_Method()
    {
        Func<dynamic, object> func;
        LambdaNode node;

        WriteLine();
        func = x => x.Ternary(x.Alpha, x.Beta, x.Delta);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");

        Assert.IsType<LambdaNodeTernary>(node);
        Assert.Equal("(x.Alpha ? x.Beta : x.Delta)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Ternary_As_Index()
    {
        Func<dynamic, object> func;
        LambdaNode node;

        WriteLine();
        func = x => x.Alpha[x.Ternary(x.Alpha, x.Beta, x.Delta)];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");

        Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[(x.Alpha ? x.Beta : x.Delta)]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Ternary_As_Host()
    {
        Func<dynamic, object> func;
        LambdaNode node;

        WriteLine();
        func = x => x.Ternary(x.Alpha, x.Beta, x.Delta).Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");

        Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("(x.Alpha ? x.Beta : x.Delta).Alpha", node.ToString());
    }
}
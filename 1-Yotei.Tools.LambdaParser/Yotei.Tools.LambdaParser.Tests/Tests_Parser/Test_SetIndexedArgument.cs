﻿namespace Yotei.Tools.LambdaParser.Tests;

// ========================================================
//[Enforced]
public class Test_SetIndexedArgument
{
    //[Enforced]
    [Fact]
    public static void Parse_IxArgument_ToConstant()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x[x] = null!;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x[x] = 'NULL')", node.ToString());

        Debug.WriteLine("");
        func = x => x[x] = 7;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x[x] = '7')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxArgument_ToArgument()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x[x] = x;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x[x] = x)", node.ToString());

        Debug.WriteLine("");
        func = x => x[x][x] = x;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x[x][x] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxArgument_ToDynamic()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x[x] = x.Alpha;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x[x] = x.Alpha)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_IxDynamic_ToConstant()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x[x.Alpha] = null!;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x[x.Alpha] = 'NULL')", node.ToString());

        Debug.WriteLine("");
        func = x => x[x.Beta] = 7;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x[x.Beta] = '7')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxDynamic_ToArgument()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x[x.Alpha] = x;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x[x.Alpha] = x)", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha[x.Beta][x.Delta] = x;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Beta][x.Delta] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxDynamic_ToDynamic()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x[x.Alpha] = x.Alpha;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x[x.Alpha] = x.Alpha)", node.ToString());

        Debug.WriteLine("");
        func = x => x[x.Beta] = x.Beta.Alpha;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x[x.Beta] = x.Beta.Alpha)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Nested()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x[x[x]] = x;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x[x[x]] = x)", node.ToString());

        Debug.WriteLine("");
        func = x => x[x[x]] = x[x];
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x[x[x]] = x[x])", node.ToString());
    }
}
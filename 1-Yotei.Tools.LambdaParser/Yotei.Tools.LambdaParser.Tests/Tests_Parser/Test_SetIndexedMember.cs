﻿namespace Yotei.Tools.LambdaParser.Tests;

// ========================================================
//[Enforced]
public class Test_SetIndexedMember
{
    //[Enforced]
    [Fact]
    public static void Parse_IxArgument_ToConstant()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x.Alpha[x] = null!;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x] = 'NULL')", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha[x] = 7;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x] = '7')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxArgument_ToArgument()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x.Alpha[x] = x;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x] = x)", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha[x][x] = x;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x][x] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxArgument_ToDynamic()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x.Alpha[x] = x.Alpha;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x] = x.Alpha)", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha.Beta[x] = x.Beta;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha.Beta[x] = x.Beta)", node.ToString());
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
        func = x => x.Alpha[x.Alpha] = null!;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Alpha] = 'NULL')", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha[x.Beta] = 7;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Beta] = '7')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxDynamic_ToArgument()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x.Alpha[x.Alpha] = x;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Alpha] = x)", node.ToString());

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
        func = x => x.Alpha[x.Alpha] = x.Alpha;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Alpha] = x.Alpha)", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha[x.Beta] = x.Beta.Alpha;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Beta] = x.Beta.Alpha)", node.ToString());
    }

    // ---------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Nested()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x.Alpha[x.Beta[x.Delta]] = x;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Beta[x.Delta]] = x)", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha[x.Beta[x.Delta]] = x.Delta[x.Beta[x.Alpha]];
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Beta[x.Delta]] = x.Delta[x.Beta[x.Alpha]])", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha[x.Alpha[x.Alpha]] = x.Alpha[x.Alpha[x.Alpha]];
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Alpha[x.Alpha]] = x.Alpha[x.Alpha[x.Alpha]])", node.ToString());
    }
}
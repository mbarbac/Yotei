﻿namespace Yotei.Tools.LambdaParser.Tests;

// ========================================================
//[Enforced]
public class Test_Unary
{
    //[Enforced]
    [Fact]
    public static void Parse_Not_On_Boolean()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeValue item;

        Debug.WriteLine("");
        var value = true;
        func = x => !value;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        Assert.Equal("'False'", node.ToString());

        Debug.WriteLine("");
        value = false;
        func = x => !value;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        Assert.Equal("'True'", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Not_On_Argument()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeUnary item;

        Debug.WriteLine("");
        func = x => !x;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeUnary>(node);
        Assert.Equal("(Not x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Not_On_Dynamic()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeUnary item;

        Debug.WriteLine("");
        func = x => !x.Alpha;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
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
        LambdaNodeValue item;

        Debug.WriteLine("");
        var value = 7;
        func = x => -value;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        Assert.Equal("'-7'", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Negate_Argument()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeUnary item;

        Debug.WriteLine("");
        func = x => -x;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
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

        Debug.WriteLine("");
        func = x => -x.Alpha;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeUnary>(node);
        Assert.Equal("(Negate x.Alpha)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Pre_Increment_Argument()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeUnary item;

        Debug.WriteLine("");
        func = x => ++x;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeUnary>(node);
        Assert.Equal("(Increment x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Pre_Increment_Dynamic_Resolves_To_Setter()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        // A pre-increment is logically an increment followed by a setter, which exactly is
        // what the parser renders. So we consider this test as passed.

        Debug.WriteLine("");
        func = x => ++x.Alpha;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha = (Increment x.Alpha))", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Post_Decrement_Argument()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeUnary item;

        Debug.WriteLine("");
        func = x => x--;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeUnary>(node);
        Assert.Equal("(Decrement x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Pre_Decrement_Dynamic_Resolves_To_Setter()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        // A pre-decrement is logically an increment followed by a setter, which exactly is
        // what the parser renders. So we consider this test as passed.

        Debug.WriteLine("");
        func = x => x.Alpha--;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha = (Decrement x.Alpha))", node.ToString());
    }
}
namespace Yotei.Tools.DynamicLambda.Tests;

// ========================================================
//[Enforced]
public static class Test_SetIndexedMember
{
    //[Enforced]
    [Fact]
    public static void Parse_IxArgument_ToConstant()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x.Alpha[x] = null!;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x] = 'NULL')", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha[x] = 7;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x] = '7')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxArgument_ToArgument()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x.Alpha[x] = x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x] = x)", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha[x][x] = x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x][x] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxArgument_ToDynamic()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x.Alpha[x] = x.Alpha;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x] = x.Alpha)", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha.Beta[x] = x.Beta;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha.Beta[x] = x.Beta)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_IxDynamic_ToConstant()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x.Alpha[x.Alpha] = null!;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Alpha] = 'NULL')", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha[x.Beta] = 7;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Beta] = '7')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxDynamic_ToArgument()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x.Alpha[x.Alpha] = x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Alpha] = x)", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha[x.Beta][x.Delta] = x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Beta][x.Delta] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxDynamic_ToDynamic()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x.Alpha[x.Alpha] = x.Alpha;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Alpha] = x.Alpha)", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha[x.Beta] = x.Beta.Alpha;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Beta] = x.Beta.Alpha)", node.ToString());
    }

    // ---------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Nested()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x.Alpha[x.Beta[x.Delta]] = x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Beta[x.Delta]] = x)", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha[x.Beta[x.Delta]] = x.Delta[x.Beta[x.Alpha]];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Beta[x.Delta]] = x.Delta[x.Beta[x.Alpha]])", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha[x.Alpha[x.Alpha]] = x.Alpha[x.Alpha[x.Alpha]];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Alpha[x.Alpha]] = x.Alpha[x.Alpha[x.Alpha]])", node.ToString());
    }
}
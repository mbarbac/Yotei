namespace Yotei.Tools.DynamicLambda.Tests;

// ========================================================
//[Enforced]
public static class Test_SetIndexedArgument
{
    //[Enforced]
    [Fact]
    public static void Parse_IxArgument_ToConstant()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x[x] = null!;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x[x] = 'NULL')", node.ToString());

        Debug.WriteLine("");
        func = x => x[x] = 7;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x[x] = '7')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxArgument_ToArgument()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x[x] = x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x[x] = x)", node.ToString());

        Debug.WriteLine("");
        func = x => x[x][x] = x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x[x][x] = x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxArgument_ToDynamic()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x[x] = x.Alpha;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x[x] = x.Alpha)", node.ToString());
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
        func = x => x[x.Alpha] = null!;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x[x.Alpha] = 'NULL')", node.ToString());

        Debug.WriteLine("");
        func = x => x[x.Beta] = 7;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x[x.Beta] = '7')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxDynamic_ToArgument()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x[x.Alpha] = x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x[x.Alpha] = x)", node.ToString());

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
        func = x => x[x.Alpha] = x.Alpha;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x[x.Alpha] = x.Alpha)", node.ToString());

        Debug.WriteLine("");
        func = x => x[x.Beta] = x.Beta.Alpha;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x[x.Beta] = x.Beta.Alpha)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Nested()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x[x[x]] = x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x[x[x]] = x)", node.ToString());

        Debug.WriteLine("");
        func = x => x[x[x]] = x[x];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x[x[x]] = x[x])", node.ToString());
    }
}
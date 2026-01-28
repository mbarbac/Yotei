namespace Yotei.Tools.DynamicLambda.Tests;

// ========================================================
//[Enforced]
public static class Test_IndexedArgument
{
    //[Enforced]
    [Fact]
    public static void Parse_IxConstant()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeIndexed item;

        Debug.WriteLine("");
        func = x => x[7];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeIndexed>(node);
        Assert.Equal("x['7']", node.ToString());

        Debug.WriteLine("");
        func = x => x[7, 9];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeIndexed>(node);
        Assert.Equal("x['7', '9']", node.ToString());

        Debug.WriteLine("");
        func = x => x[7][9];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeIndexed>(node);
        Assert.Equal("x['7']['9']", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxArgument()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeIndexed item;

        Debug.WriteLine("");
        func = x => x[x];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeIndexed>(node);
        Assert.Equal("x[x]", node.ToString());

        Debug.WriteLine("");
        func = x => x[x][x];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeIndexed>(node);
        Assert.Equal("x[x][x]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxDynamic()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeIndexed item;

        Debug.WriteLine("");
        func = x => x[x.Alpha];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeIndexed>(node);
        Assert.Equal("x[x.Alpha]", node.ToString());

        Debug.WriteLine("");
        func = x => x[x.Alpha, x.Beta];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeIndexed>(node);
        Assert.Equal("x[x.Alpha, x.Beta]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxDynamic_Chained()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeIndexed item;

        Debug.WriteLine("");
        func = x => x[x.Alpha][x.Beta];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeIndexed>(node);
        Assert.Equal("x[x.Alpha][x.Beta]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxArbitrary()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeIndexed item;

        Debug.WriteLine("");
        func = x => x[x.Alpha, null, 7];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeIndexed>(node);
        Assert.Equal("x[x.Alpha, 'NULL', '7']", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Nested()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeIndexed item;

        Debug.WriteLine("");
        func = x => x[x[x]];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeIndexed>(node);
        Assert.Equal("x[x[x]]", node.ToString());

        Debug.WriteLine("");
        func = x => x[x.Alpha[x.Alpha]];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeIndexed>(node);
        Assert.Equal("x[x.Alpha[x.Alpha]]", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_ArgumentIndexed_IxSetter()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeIndexed item;

        Debug.WriteLine("");
        func = x => x[x().x = x];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeIndexed>(node);
        Assert.Equal("x[(x().x = x)]", node.ToString());

        Debug.WriteLine("");
        func = x => x[x().Alpha = x.Alpha];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeIndexed>(node);
        Assert.Equal("x[(x().Alpha = x.Alpha)]", node.ToString());
    }
}
namespace Yotei.Tools.DynamicLambda.Tests;

// ========================================================
//[Enforced]
public static class Test_Invoke
{
    //[Enforced]
    [Fact]
    public static void Parse_IxEmpty()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeInvoke item;

        Debug.WriteLine("");
        func = x => x();
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeInvoke>(node);
        Assert.Equal("x()", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxArbitrary()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeInvoke item;

        Debug.WriteLine("");
        func = x => x(x.Alpha, null, 7);
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeInvoke>(node);
        Assert.Equal("x(x.Alpha, 'NULL', '7')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Chained()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeInvoke item;

        Debug.WriteLine("");
        func = x => x(x.Alpha)(x.Beta);
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeInvoke>(node);
        Assert.Equal("x(x.Alpha)(x.Beta)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Nested()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeInvoke item;

        Debug.WriteLine("");
        func = x => x(x());
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeInvoke>(node);
        Assert.Equal("x(x())", node.ToString());

        Debug.WriteLine("");
        func = x => x(x(x(x.Alpha)));
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeInvoke>(node);
        Assert.Equal("x(x(x(x.Alpha)))", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Nested_String_Value()
    {
        Func<dynamic, object> func;
        DLambdaNode node;

        Debug.WriteLine("");
        func = x => x(x("007"));
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        var invoke = Assert.IsType<DLambdaNodeInvoke>(node);
        Assert.Single(invoke.DLambdaArguments);
        invoke = Assert.IsType<DLambdaNodeInvoke>(invoke.DLambdaArguments[0]);
        Assert.Single(invoke.DLambdaArguments);
        var value = Assert.IsType<DLambdaNodeValue>(invoke.DLambdaArguments[0]);
        Assert.Equal("007", value.DLambdaValue);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_IxSetter()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeInvoke item;

        Debug.WriteLine("");
        func = x => x(x = x.Alpha);
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeInvoke>(node);
        Assert.Equal("x(x.Alpha)", node.ToString());

        Debug.WriteLine("");
        func = x => x(x.Alpha = x.Alpha);
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeInvoke>(node);
        Assert.Equal("x((x.Alpha = x.Alpha))", node.ToString());

        Debug.WriteLine("");
        func = x => x(x.Alpha = x.Beta)(x.Beta = x.Alpha);
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeInvoke>(node);
        Assert.Equal("x((x.Alpha = x.Beta))((x.Beta = x.Alpha))", node.ToString());

        Debug.WriteLine("");
        func = x => x(x[x] = x);
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeInvoke>(node);
        Assert.Equal("x((x[x] = x))", node.ToString());
    }
}
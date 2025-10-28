namespace Yotei.Tools.LambdaParser.Tests;

// ========================================================
//[Enforced]
public class Test_Invoke
{
    //[Enforced]
    [Fact]
    public static void Parse_IxEmpty()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeInvoke item;

        Debug.WriteLine("");
        func = x => x();
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x()", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_IxArbitrary()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeInvoke item;

        Debug.WriteLine("");
        func = x => x(x.Alpha, null, 7);
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x(x.Alpha, 'NULL', '7')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Chained()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeInvoke item;

        Debug.WriteLine("");
        func = x => x(x.Alpha)(x.Beta);
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x(x.Alpha)(x.Beta)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Nested()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeInvoke item;

        Debug.WriteLine("");
        func = x => x(x());
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x(x())", node.ToString());

        Debug.WriteLine("");
        func = x => x(x(x(x.Alpha)));
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x(x(x(x.Alpha)))", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Nested_String_Value()
    {
        Func<dynamic, object> func;
        LambdaNode node;

        Debug.WriteLine("");
        func = x => x(x("007"));
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        var invoke = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Single(invoke.LambdaArguments);
        invoke = Assert.IsType<LambdaNodeInvoke>(invoke.LambdaArguments[0]);
        Assert.Single(invoke.LambdaArguments);
        var value = Assert.IsType<LambdaNodeValue>(invoke.LambdaArguments[0]);
        Assert.Equal("007", value.LambdaValue);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_IxSetter()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeInvoke item;

        Debug.WriteLine("");
        func = x => x(x = x.Alpha);
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x(x.Alpha)", node.ToString());

        Debug.WriteLine("");
        func = x => x(x.Alpha = x.Alpha);
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x((x.Alpha = x.Alpha))", node.ToString());

        Debug.WriteLine("");
        func = x => x(x.Alpha = x.Beta)(x.Beta = x.Alpha);
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x((x.Alpha = x.Beta))((x.Beta = x.Alpha))", node.ToString());

        Debug.WriteLine("");
        func = x => x(x[x] = x);
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x((x[x] = x))", node.ToString());
    }
}
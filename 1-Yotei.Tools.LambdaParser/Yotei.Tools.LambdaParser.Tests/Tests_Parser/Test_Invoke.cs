using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Invoke
{
    //[Enforced]
    [Fact]
    public static void Parse_IxEmpty()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeInvoke item;

        WriteLine(true);
        func = x => x();
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
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

        WriteLine(true);
        func = x => x(x.Alpha, null, 7);
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
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

        WriteLine(true);
        func = x => x(x.Alpha)(x.Beta);
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
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

        WriteLine(true);
        func = x => x(x());
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x(x())", node.ToString());

        WriteLine(true);
        func = x => x(x(x(x.Alpha)));
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x(x(x(x.Alpha)))", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_IxSetter()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeInvoke item;

        WriteLine(true);
        func = x => x(x = x.Alpha);
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x(x.Alpha)", node.ToString());

        WriteLine(true);
        func = x => x(x.Alpha = x.Alpha);
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x((x.Alpha = x.Alpha))", node.ToString());

        WriteLine(true);
        func = x => x(x.Alpha = x.Beta)(x.Beta = x.Alpha);
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x((x.Alpha = x.Beta))((x.Beta = x.Alpha))", node.ToString());

        WriteLine(true);
        func = x => x(x[x] = x);
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x((x[x] = x))", node.ToString());
    }
}
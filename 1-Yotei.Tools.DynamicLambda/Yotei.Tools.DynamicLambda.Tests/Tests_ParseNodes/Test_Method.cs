namespace Yotei.Tools.DynamicLambda.Tests;

// ========================================================
//[Enforced]
public static class Test_Method
{
    //[Enforced]
    [Fact]
    public static void Parse_No_Arguments()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeMethod item;

        Debug.WriteLine("");
        func = x => x.Alpha();
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeMethod>(node);
        Assert.Equal("x.Alpha()", node.ToString());

        Debug.WriteLine("");
        func = x => x.x();
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeMethod>(node);
        Assert.Equal("x.x()", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Chained_No_Arguments()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeMethod item;

        Debug.WriteLine("");
        func = x => x.Alpha.Beta();
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeMethod>(node);
        Assert.Equal("x.Alpha.Beta()", node.ToString());

        Debug.WriteLine("");
        func = x => x.x.x();
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeMethod>(node);
        Assert.Equal("x.x.x()", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_With_Arguments()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeMethod item;

        Debug.WriteLine("");
        func = x => x.Alpha(x.Beta, null, 7);
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeMethod>(node);
        Assert.Equal("x.Alpha(x.Beta, 'NULL', '7')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Nested()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeMethod item;

        Debug.WriteLine("");
        func = x => x.Alpha(x.Alpha());
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeMethod>(node);
        Assert.Equal("x.Alpha(x.Alpha())", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha(x.Beta());
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeMethod>(node);
        Assert.Equal("x.Alpha(x.Beta())", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Typed()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeMethod item;

        Debug.WriteLine("");
        func = x => x.Alpha<int>(x.Beta);
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeMethod>(node);
        Assert.Equal("x.Alpha<Int32>(x.Beta)", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha<int, string>(x.Beta);
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeMethod>(node);
        Assert.Equal("x.Alpha<Int32, String>(x.Beta)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Method_IxSetter()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeMethod item;

        Debug.WriteLine("");
        func = x => x.Alpha(x.Alpha = x.Alpha()[x]);
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeMethod>(node);
        Assert.Equal("x.Alpha((x.Alpha = x.Alpha()[x]))", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha(x.Alpha = x.Alpha()[x.Alpha = x.Beta]);
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeMethod>(node);
        Assert.Equal("x.Alpha((x.Alpha = x.Alpha()[(x.Alpha = x.Beta)]))", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha(x.Alpha = x.Alpha(x.Beta = x.Alpha)[x.Alpha = x.Beta]);
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeMethod>(node);
        Assert.Equal("x.Alpha((x.Alpha = x.Alpha((x.Beta = x.Alpha))[(x.Alpha = x.Beta)]))", node.ToString());
    }
}
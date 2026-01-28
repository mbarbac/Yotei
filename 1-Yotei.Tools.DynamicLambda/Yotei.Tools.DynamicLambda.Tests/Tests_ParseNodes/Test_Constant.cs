namespace Yotei.Tools.DynamicLambda.Tests;

// ========================================================
//[Enforced]
public static class Test_Constant
{
    //[Enforced]
    [Fact]
    public static void Parse_Null()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeValue item;

        Debug.WriteLine("");
        func = x => null!; // Returns the value without invoking the dynamic argument...
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeValue>(node);
        Assert.Equal("'NULL'", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Arbitrary()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeValue item;

        Debug.WriteLine("");
        func = x => 7; // Returns the value without invoking the dynamic argument...
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeValue>(node);
        Assert.Equal("'7'", node.ToString());

        Debug.WriteLine("");
        func = x => "any"; // Returns the value without invoking the dynamic argument...
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeValue>(node);
        Assert.Equal("'any'", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_With_Cast_Constant()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeValue item;

        Debug.WriteLine("");
        func = x => (string?)null!;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeValue>(node);
        Assert.Equal("'NULL'", node.ToString());

        Debug.WriteLine("");
        func = x => (int)10.5!;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeValue>(node);
        Assert.Equal("'10'", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_With_Cast_Dynamic()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeConvert item;

        Debug.WriteLine("");
        func = x => (string?)x!;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeConvert>(node);
        Assert.Equal("((String) x)", node.ToString());

        Debug.WriteLine("");
        func = x => (int)x.Delta.Beta.Alpha;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeConvert>(node);
        Assert.Equal("((Int32) x.Delta.Beta.Alpha)", node.ToString());
    }
}
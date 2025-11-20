namespace Yotei.Tools.LambdaParser.Tests;

// ========================================================
//[Enforced]
public static class Test_Constant
{
    //[Enforced]
    [Fact]
    public static void Parse_Null()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeValue item;

        Debug.WriteLine("");
        func = x => null!; // Returns the value without invoking the dynamic argument...
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        Assert.Equal("'NULL'", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Arbitrary()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeValue item;

        Debug.WriteLine("");
        func = x => 7; // Returns the value without invoking the dynamic argument...
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        Assert.Equal("'7'", node.ToString());

        Debug.WriteLine("");
        func = x => "any"; // Returns the value without invoking the dynamic argument...
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        Assert.Equal("'any'", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_With_Cast_Constant()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeValue item;

        Debug.WriteLine("");
        func = x => (string?)null!;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        Assert.Equal("'NULL'", node.ToString());

        Debug.WriteLine("");
        func = x => (int)10.5!;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        Assert.Equal("'10'", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_With_Cast_Dynamic()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeConvert item;

        Debug.WriteLine("");
        func = x => (string?)x!;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeConvert>(node);
        Assert.Equal("((String) x)", node.ToString());

        Debug.WriteLine("");
        func = x => (int)x.Delta.Beta.Alpha;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeConvert>(node);
        Assert.Equal("((Int32) x.Delta.Beta.Alpha)", node.ToString());
    }
}
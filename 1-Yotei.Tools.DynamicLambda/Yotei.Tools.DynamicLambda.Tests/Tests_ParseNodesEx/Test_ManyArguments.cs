namespace Yotei.Tools.DynamicLambda.Tests;

// ========================================================
//[Enforced]
public static class Test_ManyArguments
{
    //[Enforced]
    [Fact]
    public static void Parse_Only_Concretes()
    {
        Func<int, int> func = (x) => x + 1;
        DLambdaNode node;
        DLambdaNodeValue item;

        Debug.WriteLine("");
        node = DLambdaParser.Parse(func, 1).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeValue>(node);
        Assert.Equal(2, item.DLambdaValue);

        try { node = DLambdaParser.Parse(func).Result; Assert.Fail(); }
        catch (NotFoundException) { }

        try { node = DLambdaParser.Parse(func, 1, 1).Result; Assert.Fail(); }
        catch (InvalidOperationException) { }
    }

    //[Enforced]
    [Fact]
    public static void Parse_Several_Arguments_Equality()
    {
        Func<dynamic, dynamic, object> func = (x, y) => x == y;
        DLambdaNode node;
        DLambdaNodeBinary item;

        Debug.WriteLine("");
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x Equal y)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Several_Arguments_Indexes()
    {
        Func<dynamic, dynamic, object> func = (x, y) => x.Alpha[y.Apha] = y.Alpha[x.Alpha];
        DLambdaNode node;
        DLambdaNodeSetter item;

        Debug.WriteLine("");
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[y.Apha] = y.Alpha[x.Alpha])", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Several_Arguments_Indexes_And_Convert()
    {
        Func<dynamic, dynamic, object>
            func = (x, y) => x.Alpha[(int)y.Apha] = (int)y.Alpha[(DateTime)x.Alpha];

        DLambdaNode node;
        DLambdaNodeSetter item;

        Debug.WriteLine("");
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal(
            "(x.Alpha[((Int32) y.Alpha[((DateTime) x.Alpha)])] = ((Int32) y.Alpha[((DateTime) x.Alpha)]))",
            node.ToString());
    }
}
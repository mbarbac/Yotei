namespace Yotei.Tools.DynamicLambda.Tests;

// ========================================================
//[Enforced]
public static class Test_Ternary
{
    //[Enforced]
    [Fact]
    public static void Standard_Ternary_Does_Not_Work()
    {
        Func<dynamic, object> func;
        DLambdaNode node;

        // Because 'x.Alpha' is dynamically false, the operator skips the second branch and
        // goes to the third one...
        Debug.WriteLine("");
        func = x => x.Alpha ? x.Beta : x.Delta;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        Assert.NotEqual("x.Alpha ? x.Beta : x.Delta", node.ToString());
        Assert.IsType<DLambdaNodeMember>(node);
        Assert.Equal("x.Delta", node.ToString());

        // To validate the hypothesis, use 'true' as the first element...
        Debug.WriteLine("");
        func = x => true ? x.Beta : x.Delta;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        Assert.NotEqual("x.Alpha ? x.Beta : x.Delta", node.ToString());
        Assert.IsType<DLambdaNodeMember>(node);
        Assert.Equal("x.Beta", node.ToString());

        // To validate the hypothesis, use 'false' as the first element...
        Debug.WriteLine("");
        func = x => false ? x.Beta : x.Delta;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        Assert.NotEqual("x.Alpha ? x.Beta : x.Delta", node.ToString());
        Assert.IsType<DLambdaNodeMember>(node);
        Assert.Equal("x.Delta", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Explicit_Ternary_Method()
    {
        Func<dynamic, object> func;
        DLambdaNode node;

        Debug.WriteLine("");
        func = x => x.Ternary(x.Alpha, x.Beta, x.Delta);
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");

        Assert.IsType<DLambdaNodeTernary>(node);
        Assert.Equal("(x.Alpha ? x.Beta : x.Delta)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Explicit_Ternary_As_Index()
    {
        Func<dynamic, object> func;
        DLambdaNode node;

        Debug.WriteLine("");
        func = x => x.Alpha[x.Ternary(x.Alpha, x.Beta, x.Delta)];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");

        Assert.IsType<DLambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[(x.Alpha ? x.Beta : x.Delta)]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Explicit_Ternary_As_Host()
    {
        Func<dynamic, object> func;
        DLambdaNode node;

        Debug.WriteLine("");
        func = x => x.Ternary(x.Alpha, x.Beta, x.Delta).Alpha;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");

        Assert.IsType<DLambdaNodeMember>(node);
        Assert.Equal("(x.Alpha ? x.Beta : x.Delta).Alpha", node.ToString());
    }
}
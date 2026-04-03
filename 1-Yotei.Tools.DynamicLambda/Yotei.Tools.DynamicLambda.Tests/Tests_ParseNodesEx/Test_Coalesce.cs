namespace Yotei.Tools.DynamicLambda.Tests;

// ========================================================
//[Enforced]
public static class Test_Coalesce
{
    //[Enforced]
    [Fact]
    public static void Standard_Coalesce_OnDynamic_Does_Not_Work()
    {
        Func<dynamic, object> func;
        DLambdaNode node;

        // Only executes the first node because 'x.Alpha' is dynamically not null, and so the
        // operator needs not to execute the second branch...
        Debug.WriteLine("");
        func = x => x.Alpha ?? x.Beta;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        Assert.NotEqual("(x.Alpha ?? x.Beta)", node.ToString());
        Assert.IsType<DLambdaNodeMember>(node);
        Assert.Equal("x.Alpha", node.ToString());

        // To validate the hypothesis, use null as the first element...
        Debug.WriteLine("");
        func = x => null ?? x.Beta;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        Assert.NotEqual("(x.Alpha ?? x.Beta)", node.ToString());
        Assert.IsType<DLambdaNodeMember>(node);
        Assert.Equal("x.Beta", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Explicit_Coalesce_Method()
    {
        Func<dynamic, object> func;
        DLambdaNode node;

        Debug.WriteLine("");
        func = x => x.Coalesce(x.Alpha, x.Beta);
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");

        Assert.IsType<DLambdaNodeCoalesce>(node);
        Assert.Equal("(x.Alpha ?? x.Beta)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Explicit_Coalesce_As_Index()
    {
        Func<dynamic, object> func;
        DLambdaNode node;

        Debug.WriteLine("");
        func = x => x.Alpha[x.Coalesce(x.Alpha, x.Beta)];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");

        Assert.IsType<DLambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[(x.Alpha ?? x.Beta)]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Explicit_Coalesce_As_Host()
    {
        Func<dynamic, object> func;
        DLambdaNode node;

        Debug.WriteLine("");
        func = x => x.Coalesce(x.Alpha, x.Beta).Alpha;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");

        Assert.IsType<DLambdaNodeMember>(node);
        Assert.Equal("(x.Alpha ?? x.Beta).Alpha", node.ToString());
    }
}
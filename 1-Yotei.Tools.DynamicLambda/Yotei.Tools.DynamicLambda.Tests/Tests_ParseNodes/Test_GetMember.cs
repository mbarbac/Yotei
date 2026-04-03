namespace Yotei.Tools.DynamicLambda.Tests;

// ========================================================
//[Enforced]
public static class Test_GetMember
{
    //[Enforced]
    [Fact]
    public static void Parse_Dynamic_Member()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeMember item;

        Debug.WriteLine("");
        func = x => x.Alpha;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeMember>(node);
        Assert.Equal("x.Alpha", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Member_Chained()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeMember item;

        Debug.WriteLine("");
        func = x => x.Alpha.Beta;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeMember>(node);
        Assert.Equal("x.Alpha.Beta", node.ToString());
    }
}
namespace Yotei.Tools.LambdaParser.Tests;

// ========================================================
//[Enforced]
public static class Test_GetMember
{
    //[Enforced]
    [Fact]
    public static void Parse_Dynamic_Member()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeMember item;

        Debug.WriteLine("");
        func = x => x.Alpha;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Alpha", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Member_Chained()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeMember item;

        Debug.WriteLine("");
        func = x => x.Alpha.Beta;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Alpha.Beta", node.ToString());
    }
}
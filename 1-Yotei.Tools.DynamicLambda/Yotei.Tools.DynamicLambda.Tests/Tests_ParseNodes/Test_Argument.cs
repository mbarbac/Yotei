namespace Yotei.Tools.DynamicLambda.Tests;

// ========================================================
//[Enforced]
public static class Test_Argument
{
    //[Enforced]
    [Fact]
    public static void Parse_Argument()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeArgument item;

        // Does not return an expression, but rather the argument itself...
        func = x => x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeArgument>(node);
        Assert.Equal("x", node.ToString());
    }
}
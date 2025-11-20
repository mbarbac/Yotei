namespace Yotei.Tools.LambdaParser.Tests;

// ========================================================
////[Enforced]
public static class Test_Argument
{
    //[Enforced]
    [Fact]
    public static void Parse_Argument()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeArgument item;

        // Does not return an expression, but rather the argument itself...
        func = x => x;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeArgument>(node);
        Assert.Equal("x", node.ToString());
    }
}
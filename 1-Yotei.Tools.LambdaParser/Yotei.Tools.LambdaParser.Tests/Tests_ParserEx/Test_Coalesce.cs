using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Coalesce
{
    //[Enforced]
    [Fact]
    public static void Test_Standard_Coalesce_Operator_Does_Not_Work()
    {
        Func<dynamic, object> func;
        LambdaNode node;

        WriteLine(true);
        func = x => x.Alpha ?? x.Beta;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");

        Assert.IsNotType<LambdaNodeSetter>(node);
        Assert.IsType<LambdaNodeMember>(node);
        Assert.NotEqual("(x.Alpha ?? x.Beta)", node.ToString());
        Assert.Equal("x.Alpha", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Test_Intercepting_Coalesce_Method()
    {
        Func<dynamic, object> func;
        LambdaNode node;

        WriteLine(true);
        func = x => x.Coalesce(x.Alpha, x.Beta);
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");

        Assert.IsType<LambdaNodeCoalesce>(node);
        Assert.Equal("(x.Alpha ?? x.Beta)", node.ToString());
    }
}
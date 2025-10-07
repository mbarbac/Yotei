using static Yotei.Tools.Diagnostics.DebugEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Node_Coalesce
{
    //[Enforced]
    [Fact]
    public static void Standard_Coalesce_OnDynamic_Does_Not_Work()
    {
        Func<dynamic, object> func;
        LambdaNode node;

        // Only executes the first node because 'x.Alpha' is dynamically not null, and so the
        // operator needs not to execute the second branch...
        WriteLine(true);
        func = x => x.Alpha ?? x.Beta;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        Assert.NotEqual("(x.Alpha ?? x.Beta)", node.ToString());
        Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Alpha", node.ToString());

        // To validate the hypothesis, use null as the first element...
        WriteLine(true);
        func = x => null ?? x.Beta;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        Assert.NotEqual("(x.Alpha ?? x.Beta)", node.ToString());
        Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Beta", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Explicit_Coalesce_Method()
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

    //[Enforced]
    [Fact]
    public static void Explicit_Coalesce_As_Index()
    {
        Func<dynamic, object> func;
        LambdaNode node;

        WriteLine(true);
        func = x => x.Alpha[x.Coalesce(x.Alpha, x.Beta)];
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");

        Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[(x.Alpha ?? x.Beta)]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Explicit_Coalesce_As_Host()
    {
        Func<dynamic, object> func;
        LambdaNode node;

        WriteLine(true);
        func = x => x.Coalesce(x.Alpha, x.Beta).Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");

        Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("(x.Alpha ?? x.Beta).Alpha", node.ToString());
    }
}
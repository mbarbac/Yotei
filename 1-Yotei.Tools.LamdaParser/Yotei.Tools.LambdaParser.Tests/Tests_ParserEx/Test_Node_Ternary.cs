using static Yotei.Tools.Diagnostics.DebugEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Node_Ternary
{
    //[Enforced]
    [Fact]
    public static void Standard_Ternary_Does_Not_Work()
    {
        Func<dynamic, object> func;
        LambdaNode node;

        // Because 'x.Alpha' is dynamically false, the operator skips the second branch and
        // goes to the third one...
        WriteLine(true);
        func = x => x.Alpha ? x.Beta : x.Delta;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        Assert.NotEqual("x.Alpha ? x.Beta : x.Delta", node.ToString());
        Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Delta", node.ToString());

        // To validate the hypothesis, use 'true' as the first element...
        WriteLine(true);
        func = x => true ? x.Beta : x.Delta;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        Assert.NotEqual("x.Alpha ? x.Beta : x.Delta", node.ToString());
        Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Beta", node.ToString());

        // To validate the hypothesis, use 'false' as the first element...
        WriteLine(true);
        func = x => false ? x.Beta : x.Delta;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        Assert.NotEqual("x.Alpha ? x.Beta : x.Delta", node.ToString());
        Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Delta", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Explicit_Ternary_Method()
    {
        Func<dynamic, object> func;
        LambdaNode node;

        WriteLine(true);
        func = x => x.Ternary(x.Alpha, x.Beta, x.Delta);
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");

        Assert.IsType<LambdaNodeTernary>(node);
        Assert.Equal("(x.Alpha ? x.Beta : x.Delta)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Explicit_Ternary_As_Index()
    {
        Func<dynamic, object> func;
        LambdaNode node;

        WriteLine(true);
        func = x => x.Alpha[x.Ternary(x.Alpha, x.Beta, x.Delta)];
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");

        Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[(x.Alpha ? x.Beta : x.Delta)]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Explicit_Ternary_As_Host()
    {
        Func<dynamic, object> func;
        LambdaNode node;

        WriteLine(true);
        func = x => x.Ternary(x.Alpha, x.Beta, x.Delta).Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");

        Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("(x.Alpha ? x.Beta : x.Delta).Alpha", node.ToString());
    }
}
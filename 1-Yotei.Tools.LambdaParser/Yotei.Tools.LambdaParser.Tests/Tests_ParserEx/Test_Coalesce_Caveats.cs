using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Coalesce_Caveats
{
    //[Enforced]
    [Fact]
    public static void Standard_Coalesce_OnDynamic_Does_Not_Work()
    {
        Func<dynamic, object> func;
        LambdaNode node;

        WriteLine();
        func = x => x.Alpha ?? x.Beta;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");

        // Hypothesis is that because 'x.Alpha' is not null, then the operator does not need
        // to execute the second branch, and so, it returns the first element.

        Assert.NotEqual("(x.Alpha ?? x.Beta)", node.ToString());
        Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Alpha", node.ToString());

        // To validate the hypothesis, let's see if using a null first element then we obtain
        // the dynamic second one. Any case, we cannot intercept anything that let us understand
        // that the expression involves a coalesce operator...

        WriteLine();
        func = x => null ?? x.Beta;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");

        Assert.NotEqual("(x.Alpha ?? x.Beta)", node.ToString());
        Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Beta", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Coalesce_Method()
    {
        Func<dynamic, object> func;
        LambdaNode node;

        WriteLine();
        func = x => x.Coalesce(x.Alpha, x.Beta);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");

        Assert.IsType<LambdaNodeCoalesce>(node);
        Assert.Equal("(x.Alpha ?? x.Beta)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Coalesce_As_Index()
    {
        Func<dynamic, object> func;
        LambdaNode node;

        WriteLine();
        func = x => x.Alpha[x.Coalesce(x.Alpha, x.Beta)];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");

        Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[(x.Alpha ?? x.Beta)]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Coalesce_As_Host()
    {
        Func<dynamic, object> func;
        LambdaNode node;

        WriteLine();
        func = x => x.Coalesce(x.Alpha, x.Beta).Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");

        Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("(x.Alpha ?? x.Beta).Alpha", node.ToString());
    }
}
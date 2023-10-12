using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Caveats
{
    //[Enforced]
    [Fact]
    public static void Setter_Concatenated_On_Same_Dynamic()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine(true);
        func = x => x.Alpha = (x.Alpha = x.Beta);
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha = x.Beta)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Unary_PostIncrement_Resolves_To_Setter()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine(true);
        func = x => x.Alpha++;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha = (Increment x.Alpha))", node.ToString());
    }
}
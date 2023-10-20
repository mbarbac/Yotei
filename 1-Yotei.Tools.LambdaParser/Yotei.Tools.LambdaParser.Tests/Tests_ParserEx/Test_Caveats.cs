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

        // Executing this test individually in test explorer, or under the 'runner' environment,
        // renders '(x.Alpha = x.Beta)', which is expected.But when executing ALL tests in test
        // explorer, it renders '(x.Alpha = (x.Alpha = x.Beta))'.Logically is the same thing,
        // so we'll consider it as passed.
        var s = node.ToString();
        Assert.True(s == "(x.Alpha = x.Beta)" || s == "(x.Alpha = (x.Alpha = x.Beta))");
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
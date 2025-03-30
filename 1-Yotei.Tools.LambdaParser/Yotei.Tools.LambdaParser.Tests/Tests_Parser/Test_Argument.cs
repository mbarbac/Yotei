using static Yotei.Tools.Diagnostics.DebugEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Argument
{
    //[Enforced]
    [Fact]
    public static void Parse_Argument() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeArgument item;

        WriteLine();
        func = x => x; // Does not return an expression, but rather the argument itself...
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeArgument>(node);
        Assert.Equal("x", node.ToString());
    });
}
using static Yotei.Tools.Diagnostics.DebugEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Convert
{
    [Enforced]
    [Fact]
    public static void Parse_Convert_Argument() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeConvert item;

        WriteLine();
        func = x => (int)x;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeConvert>(node);
        Assert.Equal("((Int32) x)", node.ToString());
    });
}
using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Argument
{
    //[Enforced]
    [Fact]
    public static void Parse_Argument()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeArgument item;

        WriteLine();
        func = x => x;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeArgument>(node);
        Assert.Equal("x", node.ToString());
    }
}
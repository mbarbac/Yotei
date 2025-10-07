using static Yotei.Tools.Diagnostics.DebugEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_GetMember
{
    //[Enforced]
    [Fact]
    public static void Parse_Dynamic_Member()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeMember item;

        WriteLine(true);
        func = x => x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Alpha", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Member_Chained()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeMember item;

        WriteLine(true);
        func = x => x.Alpha.Beta;
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Alpha.Beta", node.ToString());
    }
}
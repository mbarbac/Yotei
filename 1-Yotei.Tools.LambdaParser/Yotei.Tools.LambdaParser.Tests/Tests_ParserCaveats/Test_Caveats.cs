using static Yotei.Tools.Diagnostics.DebugEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Caveats
{
    //[Enforced]
    [Fact]
    public static void SetMember_Parse_Assign_Assigment_To_Itself_Dynamic() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine();
        func = x => x.Alpha = (x.Alpha = x.Beta);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);

        // For unknown reasons the DLR understands both are equivalent and prefers the second...
        Assert.NotEqual("(x.Alpha = (x.Alpha = x.Beta))", node.ToString());
        Assert.Equal("(x.Alpha = x.Beta)", node.ToString());
    });
}
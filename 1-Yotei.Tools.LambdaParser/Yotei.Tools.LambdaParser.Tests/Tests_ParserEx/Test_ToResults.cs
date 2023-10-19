using static Yotei.Tools.Diagnostics.ConsoleWrapper;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_ToResults
{
    //[Enforced]
    [Fact]
    public static void Parse_To_Anonymous()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeConstant item;

        WriteLine(true);
        func = x => new { x.Name, x.Age, Id = 50 };
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeConstant>(node);
        Assert.Equal("'{ Name = x.Name, Age = x.Age, Id = 50 }'", node.ToString());

        Assert.True(item.LambdaValue!.GetType().IsAnonymous());
        dynamic value = item.LambdaValue!;
        Assert.IsType<LambdaNodeMember>(value.Name);
        Assert.IsType<LambdaNodeMember>(value.Age);
        Assert.IsType<int>(value.Id);
    }

    //[Enforced]
    [Fact]
    public static void Parse_To_Array()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeConstant item;

        WriteLine(true);
        func = x => new[] { x.Name, 7, null };
        node = LambdaParser.Parse(func).Result;
        WriteLine(true, $"> Result: {node}");
        item = Assert.IsType<LambdaNodeConstant>(node);
        Assert.Equal("'[x.Name, 7, NULL]'", node.ToString());

        var items = (object[])item.LambdaValue!;
        Assert.IsType<LambdaNodeMember>(items[0]);
        Assert.IsType<int>(items[1]);
        Assert.Null(items[2]);
    }
}
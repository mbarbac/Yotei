using static Yotei.Tools.Diagnostics.DebugEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_ToResults
{
    //[Enforced]
    [Fact]
    public static void Parse_To_Anonymous() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeValue item;

        WriteLine();
        func = x => new { x.Name, x.Age, Id = 50 };
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        WriteLine($"Result Type: {node.GetType().EasyName()}");
        WriteLine($"Node Type: {item.LambdaValue!?.GetType().EasyName()}");
        Assert.Equal("'{ Name = x.Name, Age = x.Age, Id = 50 }'", node.ToString());

        Assert.True(item.LambdaValue!.GetType().IsAnonymous());
        dynamic value = item.LambdaValue!;
        Assert.IsType<LambdaNodeMember>(value.Name);
        Assert.IsType<LambdaNodeMember>(value.Age);
        Assert.IsType<int>(value.Id);
    });

    //[Enforced]
    [Fact]
    public static void Parse_To_Array() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeValue item;

        WriteLine();
        func = x => new[] { x.Name, 7, null };
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        WriteLine($"Result Type: {node.GetType().EasyName()}");
        WriteLine($"Node Type: {item.LambdaValue!?.GetType().EasyName()}");
        Assert.Equal("'[x.Name, '7', 'NULL']'", node.ToString());

        var items = (object[])item.LambdaValue!;
        Assert.IsType<LambdaNodeMember>(items[0]);
        item = Assert.IsType<LambdaNodeValue>(items[1]); Assert.Equal(7, item.LambdaValue);
        item = Assert.IsType<LambdaNodeValue>(items[2]); Assert.Null(item.LambdaValue);
    });

    //[Enforced]
    [Fact]
    public static void Parse_To_List() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeValue item;

        WriteLine();
        func = x => new List<object> { x.Name, 7, null! };
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        WriteLine($"Result Type: {node.GetType().EasyName()}");
        WriteLine($"Node Type: {item.LambdaValue!?.GetType().EasyName()}");
        Assert.Equal("'[x.Name, '7', 'NULL']'", node.ToString());

        var items = (List<LambdaNode>)item.LambdaValue!;
        Assert.IsType<LambdaNodeMember>(items[0]);
        item = Assert.IsType<LambdaNodeValue>(items[1]); Assert.Equal(7, item.LambdaValue);
        item = Assert.IsType<LambdaNodeValue>(items[2]); Assert.Null(item.LambdaValue);
    });
}
namespace Yotei.Tools.DynamicLambda.Tests;

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
        LambdaNodeValue item;

        Debug.WriteLine("");
        func = x => new { x.Name, x.Age, Id = 50 };
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);

        Debug.WriteLine($"Result Type: {node.GetType().EasyName()}");
        Debug.WriteLine($"Node Type: {item.DLambdaValue!?.GetType().EasyName()}");
        Assert.Equal("'{ Name = x.Name, Age = x.Age, Id = 50 }'", node.ToString());

        Assert.True(item.DLambdaValue!.GetType().IsAnonymous);
        dynamic value = item.DLambdaValue!;
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
        LambdaNodeValue item;

        Debug.WriteLine("");
        func = x => new[] { x.Name, 7, null };
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);

        Debug.WriteLine($"Result Type: {node.GetType().EasyName()}");
        Debug.WriteLine($"Node Type: {item.DLambdaValue!?.GetType().EasyName()}");
        Assert.Equal("'[x.Name, '7', 'NULL']'", node.ToString());

        var items = (object[])item.DLambdaValue!;
        Assert.IsType<LambdaNodeMember>(items[0]);
        item = Assert.IsType<LambdaNodeValue>(items[1]); Assert.Equal(7, item.DLambdaValue);
        item = Assert.IsType<LambdaNodeValue>(items[2]); Assert.Null(item.DLambdaValue);
    }

    //[Enforced]
    [Fact]
    public static void Parse_To_List()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeValue item;

        Debug.WriteLine("");
        func = x => new List<object> { x.Name, 7, null! };
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);

        Debug.WriteLine($"Result Type: {node.GetType().EasyName()}");
        Debug.WriteLine($"Node Type: {item.DLambdaValue!?.GetType().EasyName()}");
        Assert.Equal("'[x.Name, '7', 'NULL']'", node.ToString());

        var items = (List<LambdaNode>)item.DLambdaValue!;
        Assert.IsType<LambdaNodeMember>(items[0]);
        item = Assert.IsType<LambdaNodeValue>(items[1]); Assert.Equal(7, item.DLambdaValue);
        item = Assert.IsType<LambdaNodeValue>(items[2]); Assert.Null(item.DLambdaValue);
    }
}
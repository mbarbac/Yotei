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
        DLambdaNode node;
        DLambdaNodeValue item;

        Debug.WriteLine("");
        func = x => new { x.Name, x.Age, Id = 50 };
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeValue>(node);

        Debug.WriteLine($"Result Type: {node.GetType().EasyName()}");
        Debug.WriteLine($"Node Type: {item.DLambdaValue!?.GetType().EasyName()}");
        Assert.Equal("'{ Name = x.Name, Age = x.Age, Id = 50 }'", node.ToString());

        Assert.True(item.DLambdaValue!.GetType().IsAnonymous);
        dynamic value = item.DLambdaValue!;
        Assert.IsType<DLambdaNodeMember>(value.Name);
        Assert.IsType<DLambdaNodeMember>(value.Age);
        Assert.IsType<int>(value.Id);
    }

    //[Enforced]
    [Fact]
    public static void Parse_To_Array()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeValue item;

        Debug.WriteLine("");
        func = x => new[] { x.Name, 7, null };
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeValue>(node);

        Debug.WriteLine($"Result Type: {node.GetType().EasyName()}");
        Debug.WriteLine($"Node Type: {item.DLambdaValue!?.GetType().EasyName()}");
        Assert.Equal("'[x.Name, '7', 'NULL']'", node.ToString());

        var items = (object[])item.DLambdaValue!;
        Assert.IsType<DLambdaNodeMember>(items[0]);
        item = Assert.IsType<DLambdaNodeValue>(items[1]); Assert.Equal(7, item.DLambdaValue);
        item = Assert.IsType<DLambdaNodeValue>(items[2]); Assert.Null(item.DLambdaValue);
    }

    //[Enforced]
    [Fact]
    public static void Parse_To_List()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeValue item;

        Debug.WriteLine("");
        func = x => new List<object> { x.Name, 7, null! };
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeValue>(node);

        Debug.WriteLine($"Result Type: {node.GetType().EasyName()}");
        Debug.WriteLine($"Node Type: {item.DLambdaValue!?.GetType().EasyName()}");
        Assert.Equal("'[x.Name, '7', 'NULL']'", node.ToString());

        var items = (List<DLambdaNode>)item.DLambdaValue!;
        Assert.IsType<DLambdaNodeMember>(items[0]);
        item = Assert.IsType<DLambdaNodeValue>(items[1]); Assert.Equal(7, item.DLambdaValue);
        item = Assert.IsType<DLambdaNodeValue>(items[2]); Assert.Null(item.DLambdaValue);
    }
}
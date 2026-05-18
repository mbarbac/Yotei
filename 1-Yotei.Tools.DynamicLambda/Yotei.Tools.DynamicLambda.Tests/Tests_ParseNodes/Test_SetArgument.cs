#pragma warning disable CS1717

namespace Yotei.Tools.DynamicLambda.Tests;

// ========================================================
//[Enforced]
public static class Test_SetArgument
{
    //[Enforced]
    [Fact]
    public static void Parse_ToArgument_Resolves_Into_Argument()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeArgument item;

        Debug.WriteLine("");
        func = x => x = x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeArgument>(node);
        Assert.Equal("x", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_ToDynamic()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeMember item;

        Debug.WriteLine("");
        func = x => x = x.Alpha;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeMember>(node);
        Assert.Equal("x.Alpha", node.ToString());

        Debug.WriteLine("");
        func = x => x = x.Alpha.Beta;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeMember>(node);
        Assert.Equal("x.Alpha.Beta", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_ToConstant_Resolves_Into_Constant()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeValue item;

        Debug.WriteLine("");
        func = x => x = 7;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeValue>(node);
        Assert.Equal("'7'", node.ToString());

        Debug.WriteLine("");
        func = x => x = "any";
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeValue>(node);
        Assert.Equal("'any'", node.ToString());
    }
}
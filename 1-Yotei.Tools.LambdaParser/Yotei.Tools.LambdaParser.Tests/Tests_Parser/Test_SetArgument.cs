#pragma warning disable CS1717

namespace Yotei.Tools.LambdaParser.Tests;

// ========================================================
//[Enforced]
public static class Test_SetArgument
{
    //[Enforced]
    [Fact]
    public static void Parse_ToArgument_Resolves_Into_Argument()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeArgument item;

        Debug.WriteLine("");
        func = x => x = x;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeArgument>(node);
        Assert.Equal("x", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_ToDynamic()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeMember item;

        Debug.WriteLine("");
        func = x => x = x.Alpha;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Alpha", node.ToString());

        Debug.WriteLine("");
        func = x => x = x.Alpha.Beta;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Alpha.Beta", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_ToConstant_Resolves_Into_Constant()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeValue item;

        Debug.WriteLine("");
        func = x => x = 7;
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        Assert.Equal("'7'", node.ToString());

        Debug.WriteLine("");
        func = x => x = "any";
        node = LambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        Assert.Equal("'any'", node.ToString());
    }
}
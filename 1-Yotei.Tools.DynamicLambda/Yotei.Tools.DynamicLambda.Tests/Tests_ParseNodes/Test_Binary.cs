#pragma warning disable CS1718

namespace Yotei.Tools.DynamicLambda.Tests;

// ========================================================
//[Enforced]
public static class Test_Binary
{
    //[Enforced]
    [Fact]
    public static void Parse_Equal_Argument_Argument()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeBinary item;

        Debug.WriteLine("");

        func = x => x == x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x Equal x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Equal_Argument_Constant()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeBinary item;

        Debug.WriteLine("");
        func = x => x == null;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x Equal 'NULL')", node.ToString());

        Debug.WriteLine("");
        func = x => x == 7;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x Equal '7')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Equal_Argument_Dynamic()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeBinary item;

        Debug.WriteLine("");
        func = x => x == x.Alpha;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x Equal x.Alpha)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Equal_Dynamic_Argument()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeBinary item;

        Debug.WriteLine("");
        func = x => x.Alpha == x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x.Alpha Equal x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Equal_Dynamic_Dynamic()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeBinary item;

        Debug.WriteLine("");
        func = x => x.Alpha == x.Beta;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x.Alpha Equal x.Beta)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Equal_Dynamic_Same_Dynamic()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeBinary item;

        Debug.WriteLine("");
        func = x => x.Alpha == x.Alpha;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x.Alpha Equal x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Equal_Dynamic_Setter()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeBinary item;

        Debug.WriteLine("");
        func = x => x.Alpha == (x.Alpha = x.Beta);
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x.Alpha Equal (x.Alpha = x.Beta))", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha == (x.Alpha = x.Alpha);
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x.Alpha Equal (x.Alpha = x.Alpha))", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_SingleAnd_Argument_Constant()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeBinary item;

        Debug.WriteLine("");
        func = x => x & 7;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x And '7')", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SingleAnd_Argument_Argument()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeBinary item;

        Debug.WriteLine("");
        func = x => x & x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x And x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SingleAnd_Argument_Dynamic()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeBinary item;

        Debug.WriteLine("");
        func = x => x & x.Alpha;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x And x.Alpha)", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha & x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x.Alpha And x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_SingleAnd_Dynamic_Dynamic()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeBinary item;

        Debug.WriteLine("");
        func = x => x.Alpha & x.Beta;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x.Alpha And x.Beta)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_RegularAnd_Argument_Constant()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeBinary item;

        Debug.WriteLine("");
        func = x => x && 7;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x And '7')", node.ToString()); // Not AndAlso :(
    }

    //[Enforced]
    [Fact]
    public static void Parse_RegularAnd_Argument_Argument()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeBinary item;

        Debug.WriteLine("");
        func = x => x && x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x And x)", node.ToString()); // Not AndAlso :(
    }

    //[Enforced]
    [Fact]
    public static void Parse_RegularAnd_Argument_Dynamic()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeBinary item;

        Debug.WriteLine("");
        func = x => x && x.Alpha;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x And x.Alpha)", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha & x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x.Alpha And x)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_RegularAnd_Dynamic_Dynamic()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeBinary item;

        Debug.WriteLine("");
        func = x => x.Alpha && x.Beta;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x.Alpha And x.Beta)", node.ToString());

        Debug.WriteLine("");
        func = x => x.Delta.Alpha && x.Beta.Delta;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeBinary>(node);
        Assert.Equal("(x.Delta.Alpha And x.Beta.Delta)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Binary_AddAssign_Resolves_To_Setter()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x.Alpha += x.Beta;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha = (x.Alpha AddAssign x.Beta))", node.ToString());
    }
}
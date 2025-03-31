#pragma warning disable CS1718 // Comparison made to same variable

using static Yotei.Tools.Diagnostics.DebugEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Binary
{
    //[Enforced]
    [Fact]
    public static void Parse_Equal_Argument_Argument() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeBinary item;

        WriteLine();

        func = x => x == x;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x Equal x)", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_Equal_Argument_Constant() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeBinary item;

        WriteLine();
        func = x => x == null;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x Equal 'NULL')", node.ToString());

        WriteLine();
        func = x => x == 7;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x Equal '7')", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_Equal_Argument_Dynamic() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeBinary item;

        WriteLine();
        func = x => x == x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x Equal x.Alpha)", node.ToString());
    });

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Equal_Dynamic_Argument() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeBinary item;

        WriteLine();
        func = x => x.Alpha == x;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x.Alpha Equal x)", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_Equal_Dynamic_Dynamic() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeBinary item;

        WriteLine();
        func = x => x.Alpha == x.Beta;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x.Alpha Equal x.Beta)", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_Equal_Dynamic_Same_Dynamic() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeBinary item;

        WriteLine();
        func = x => x.Alpha == x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x.Alpha Equal x.Alpha)", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_Equal_Dynamic_Setter() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeBinary item;

        WriteLine();
        func = x => x.Alpha == (x.Alpha = x.Beta);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x.Alpha Equal (x.Alpha = x.Beta))", node.ToString());

        WriteLine();
        func = x => x.Alpha == (x.Alpha = x.Alpha);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x.Alpha Equal (x.Alpha = x.Alpha))", node.ToString());
    });

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_SingleAnd_Argument_Constant() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeBinary item;

        WriteLine();
        func = x => x & 7;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x And '7')", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_SingleAnd_Argument_Argument() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeBinary item;

        WriteLine();
        func = x => x & x;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x And x)", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_SingleAnd_Argument_Dynamic() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeBinary item;

        WriteLine();
        func = x => x & x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x And x.Alpha)", node.ToString());

        WriteLine();
        func = x => x.Alpha & x;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x.Alpha And x)", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_SingleAnd_Dynamic_Dynamic() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeBinary item;

        WriteLine();
        func = x => x.Alpha & x.Beta;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x.Alpha And x.Beta)", node.ToString());
    });

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_RegularAnd_Argument_Constant() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeBinary item;

        WriteLine();
        func = x => x && 7;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x And '7')", node.ToString()); // Not AndAlso :(
    });

    //[Enforced]
    [Fact]
    public static void Parse_RegularAnd_Argument_Argument() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeBinary item;

        WriteLine();
        func = x => x && x;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x And x)", node.ToString()); // Not AndAlso :(
    });

    //[Enforced]
    [Fact]
    public static void Parse_RegularAnd_Argument_Dynamic() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeBinary item;

        WriteLine();
        func = x => x && x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x And x.Alpha)", node.ToString());

        WriteLine();
        func = x => x.Alpha & x;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x.Alpha And x)", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_RegularAnd_Dynamic_Dynamic() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeBinary item;

        WriteLine();
        func = x => x.Alpha && x.Beta;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x.Alpha And x.Beta)", node.ToString());

        WriteLine();
        func = x => x.Delta.Alpha && x.Beta.Delta;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x.Delta.Alpha And x.Beta.Delta)", node.ToString());
    });

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Binary_AddAssign_Resolves_To_Setter() => Repeater.Repeat(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine();
        func = x => x.Alpha += x.Beta;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha = (x.Alpha AddAssign x.Beta))", node.ToString());
    });
}
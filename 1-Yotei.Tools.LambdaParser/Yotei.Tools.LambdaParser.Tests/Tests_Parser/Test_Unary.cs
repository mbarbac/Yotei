using static Yotei.Tools.Diagnostics.ConsoleWrapper;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Unary
{
    [SuppressMessage("", "xUnit1013")]
    public static void Executor(Action action)
    {
        lock (LambdaParser.SyncRoot)
        {
            LambdaParser.OnDebug += OnDebug;
            try { action(); }
            finally { LambdaParser.OnDebug = null!; }
        }

        static void OnDebug(object? _, string message) => WriteLine(message);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Not_Constant() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeConstant item;

        WriteLine();
        var value = true;
        func = x => !value;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeConstant>(node);
        Assert.Equal("'False'", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_Not_Argument() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeUnary item;

        WriteLine();
        func = x => !x;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeUnary>(node);
        Assert.Equal("(Not x)", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_Not_Dynamic() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeUnary item;

        WriteLine();
        func = x => !x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeUnary>(node);
        Assert.Equal("(Not x.Alpha)", node.ToString());
    });

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Negate_Constant() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeConstant item;

        WriteLine();
        var value = 7;
        func = x => -value;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeConstant>(node);
        Assert.Equal("'-7'", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_Negate_Argument() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeUnary item;

        WriteLine();
        func = x => -x;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeUnary>(node);
        Assert.Equal("(Negate x)", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_Negate_Dynamic() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeUnary item;

        WriteLine();
        func = x => -x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeUnary>(node);
        Assert.Equal("(Negate x.Alpha)", node.ToString());
    });

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_PreIncrement_Argument() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeUnary item;

        WriteLine();
        func = x => ++x;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeUnary>(node);
        Assert.Equal("(Increment x)", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_PreIncrement_Dynamic_Resolves_To_Setter() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        // A pre-increment is logically an increment followed by a setter, which exactly is
        // what the parser renders. So we consider this test as passed.

        WriteLine();
        func = x => ++x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha = (Increment x.Alpha))", node.ToString());
    });

    ////[Enforced]
    //[Fact]
    //public static void Parse_() => Executor(() =>
    //{
    //});
}
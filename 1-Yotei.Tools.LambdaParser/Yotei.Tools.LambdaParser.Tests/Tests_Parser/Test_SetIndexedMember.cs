using static Yotei.Tools.Diagnostics.ConsoleWrapper;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_SetIndexedMember
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
    public static void Parse_IxArgument_ToConstant() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine();
        func = x => x.Alpha[x] = null!;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x] = 'NULL')", node.ToString());

        WriteLine();
        func = x => x.Alpha[x] = 7;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x] = '7')", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_IxArgument_ToArgument() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine();
        func = x => x.Alpha[x] = x;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x] = x)", node.ToString());

        WriteLine();
        func = x => x.Alpha[x][x] = x;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x][x] = x)", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_IxArgument_ToDynamic() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine();
        func = x => x.Alpha[x] = x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x] = x.Alpha)", node.ToString());

        WriteLine();
        func = x => x.Alpha.Beta[x] = x.Beta;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha.Beta[x] = x.Beta)", node.ToString());
    });

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_IxDynamic_ToConstant() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine();
        func = x => x.Alpha[x.Alpha] = null!;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Alpha] = 'NULL')", node.ToString());

        WriteLine();
        func = x => x.Alpha[x.Beta] = 7;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Beta] = '7')", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_IxDynamic_ToArgument() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine();
        func = x => x.Alpha[x.Alpha] = x;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Alpha] = x)", node.ToString());

        WriteLine();
        func = x => x.Alpha[x.Beta][x.Delta] = x;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Beta][x.Delta] = x)", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_IxDynamic_ToDynamic() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine();
        func = x => x.Alpha[x.Alpha] = x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Alpha] = x.Alpha)", node.ToString());

        WriteLine();
        func = x => x.Alpha[x.Beta] = x.Beta.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Beta] = x.Beta.Alpha)", node.ToString());
    });

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Nested() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine();
        func = x => x.Alpha[x.Beta[x.Delta]] = x;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Beta[x.Delta]] = x)", node.ToString());

        WriteLine();
        func = x => x.Alpha[x.Beta[x.Delta]] = x.Delta[x.Beta[x.Alpha]];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Beta[x.Delta]] = x.Delta[x.Beta[x.Alpha]])", node.ToString());

        WriteLine();
        func = x => x.Alpha[x.Alpha[x.Alpha]] = x.Alpha[x.Alpha[x.Alpha]];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[x.Alpha[x.Alpha]] = x.Alpha[x.Alpha[x.Alpha]])", node.ToString());
    });

    ////[Enforced]
    //[Fact]
    //public static void Parse_() => Executor(() =>
    //{
    //});
}
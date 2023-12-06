using static Yotei.Tools.Diagnostics.ConsoleWrapper;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Invoke
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
    public static void Parse_IxEmpty() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeInvoke item;

        WriteLine();
        func = x => x();
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x()", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_IxArbitrary() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeInvoke item;

        WriteLine();
        func = x => x(x.Alpha, null, 7);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x(x.Alpha, 'NULL', '7')", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_Chained() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeInvoke item;

        WriteLine();
        func = x => x(x.Alpha)(x.Beta);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x(x.Alpha)(x.Beta)", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_Nested() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeInvoke item;

        WriteLine();
        func = x => x(x());
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x(x())", node.ToString());

        WriteLine();
        func = x => x(x(x(x.Alpha)));
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x(x(x(x.Alpha)))", node.ToString());
    });

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Invoke_IxSetter() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeInvoke item;

        WriteLine();
        func = x => x(x = x.Alpha);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x(x.Alpha)", node.ToString());

        WriteLine();
        func = x => x(x.Alpha = x.Alpha);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x((x.Alpha = x.Alpha))", node.ToString());

        WriteLine();
        func = x => x(x.Alpha = x.Beta)(x.Beta = x.Alpha);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x((x.Alpha = x.Beta))((x.Beta = x.Alpha))", node.ToString());

        WriteLine();
        func = x => x(x[x] = x);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeInvoke>(node);
        Assert.Equal("x((x[x] = x))", node.ToString());
    });

    ////[Enforced]
    //[Fact]
    //public static void Parse_() => Executor(() =>
    //{
    //});
}
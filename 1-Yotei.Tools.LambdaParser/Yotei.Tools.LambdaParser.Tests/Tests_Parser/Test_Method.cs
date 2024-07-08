using static Yotei.Tools.Diagnostics.ConsoleWrapper;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Method
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
    public static void Parse_No_Arguments() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeMethod item;

        WriteLine();
        func = x => x.Alpha();
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMethod>(node);
        Assert.Equal("x.Alpha()", node.ToString());

        WriteLine();
        func = x => x.x();
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMethod>(node);
        Assert.Equal("x.x()", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_Chained_No_Arguments() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeMethod item;

        WriteLine();
        func = x => x.Alpha.Beta();
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMethod>(node);
        Assert.Equal("x.Alpha.Beta()", node.ToString());

        WriteLine();
        func = x => x.x.x();
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMethod>(node);
        Assert.Equal("x.x.x()", node.ToString());
    });

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_With_Arguments() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeMethod item;

        WriteLine();
        func = x => x.Alpha(x.Beta, null, 7);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMethod>(node);
        Assert.Equal("x.Alpha(x.Beta, 'NULL', '7')", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_Nested() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeMethod item;

        WriteLine();
        func = x => x.Alpha(x.Alpha());
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMethod>(node);
        Assert.Equal("x.Alpha(x.Alpha())", node.ToString());

        WriteLine();
        func = x => x.Alpha(x.Beta());
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMethod>(node);
        Assert.Equal("x.Alpha(x.Beta())", node.ToString());
    });

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Typed() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeMethod item;

        WriteLine();
        func = x => x.Alpha<int>(x.Beta);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMethod>(node);
        Assert.Equal("x.Alpha<Int32>(x.Beta)", node.ToString());

        WriteLine();
        func = x => x.Alpha<int, string>(x.Beta);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMethod>(node);
        Assert.Equal("x.Alpha<Int32, String>(x.Beta)", node.ToString());
    });

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Method_IxSetter() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeMethod item;

        WriteLine();
        func = x => x.Alpha(x.Alpha = x.Alpha()[x]);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMethod>(node);
        Assert.Equal("x.Alpha((x.Alpha = x.Alpha()[x]))", node.ToString());

        WriteLine();
        func = x => x.Alpha(x.Alpha = x.Alpha()[x.Alpha = x.Beta]);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMethod>(node);
        Assert.Equal("x.Alpha((x.Alpha = x.Alpha()[(x.Alpha = x.Beta)]))", node.ToString());

        WriteLine();
        func = x => x.Alpha(x.Alpha = x.Alpha(x.Beta = x.Alpha)[x.Alpha = x.Beta]);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMethod>(node);
        Assert.Equal("x.Alpha((x.Alpha = x.Alpha((x.Beta = x.Alpha))[(x.Alpha = x.Beta)]))", node.ToString());
    });

    ////[Enforced]
    //[Fact]
    //public static void Parse_() => Executor(() =>
    //{
    //});
}
using static Yotei.Tools.Diagnostics.ConsoleWrapper;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Coalesce
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
    public static void Test_Standard_Coalesce_Operator_Does_Not_Work() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;

        WriteLine();
        func = x => x.Alpha ?? x.Beta;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");

        Assert.IsNotType<LambdaNodeSetter>(node);
        Assert.IsType<LambdaNodeMember>(node);
        Assert.NotEqual("(x.Alpha ?? x.Beta)", node.ToString());
        Assert.Equal("x.Alpha", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Test_Coalesce_Method() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;

        WriteLine();
        func = x => x.Coalesce(x.Alpha, x.Beta);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");

        Assert.IsType<LambdaNodeCoalesce>(node);
        Assert.Equal("(x.Alpha ?? x.Beta)", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Test_Coalesce_As_Index() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;

        WriteLine();
        func = x => x.Alpha[x.Coalesce(x.Alpha, x.Beta)];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");

        Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x.Alpha[(x.Alpha ?? x.Beta)]", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Test_Coalesce_As_Host() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;

        WriteLine();
        func = x => x.Coalesce(x.Alpha, x.Beta).Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");

        Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("(x.Alpha ?? x.Beta).Alpha", node.ToString());
    });

    ////[Enforced]
    //[Fact]
    //public static void Parse_() => Executor(() =>
    //{
    //});
}
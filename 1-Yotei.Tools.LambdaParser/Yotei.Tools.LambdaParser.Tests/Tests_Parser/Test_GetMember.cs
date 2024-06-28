using static Yotei.Tools.Diagnostics.ConsoleWrapper;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_GetMember
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
    public static void Parse_Member() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeMember item;

        WriteLine();
        func = x => x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Alpha", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Parse_Member_Chained() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeMember item;

        WriteLine();
        func = x => x.Alpha.Beta;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeMember>(node);
        Assert.Equal("x.Alpha.Beta", node.ToString());
    });

    ////[Enforced]
    //[Fact]
    //public static void Parse_() => Executor(() =>
    //{
    //});
}
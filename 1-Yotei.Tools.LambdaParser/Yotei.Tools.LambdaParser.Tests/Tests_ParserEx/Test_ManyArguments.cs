using static Yotei.Tools.Diagnostics.ConsoleWrapper;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_ManyArguments
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
    public static void Test_Delegate_Is_Action() => Executor(() =>
    {
        Action<dynamic> func = (x) => x.ToString();
        LambdaNode node;

        WriteLine();
        try { node = LambdaParser.Parse(func).Result; Assert.Fail(); }
        catch (NotSupportedException) { }
    });

    //[Enforced]
    [Fact]
    public static void Test_Only_Concretes() => Executor(() =>
    {
        Func<int, int> func = (x) => x + 1;
        LambdaNode node;
        LambdaNodeValue item;

        WriteLine();
        node = LambdaParser.Parse(func, 1).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeValue>(node);
        Assert.Equal(2, item.LambdaValue);

        try { node = LambdaParser.Parse(func).Result; Assert.Fail(); }
        catch (NotFoundException) { }

        try { node = LambdaParser.Parse(func, 1, 1).Result; Assert.Fail(); }
        catch (InvalidOperationException) { }
    });

    //[Enforced]
    [Fact]
    public static void Test_Several_Equality() => Executor(() =>
    {
        Func<dynamic, dynamic, object> func = (x, y) => x == y;
        LambdaNode node;
        LambdaNodeBinary item;

        WriteLine();
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeBinary>(node);
        Assert.Equal("(x Equal y)", node.ToString());
    });

    //[Enforced]
    [Fact]
    public static void Test_Several_Indexes() => Executor(() =>
    {
        Func<dynamic, dynamic, object> func = (x, y) => x.Alpha[y.Apha] = y.Alpha[x.Alpha];
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine();
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha[y.Apha] = y.Alpha[x.Alpha])", node.ToString());
    });
}
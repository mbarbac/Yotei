using static Yotei.Tools.Diagnostics.ConsoleWrapper;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_ToResults
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
    public static void Parse_To_Anonymous() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeConstant item;

        WriteLine();
        func = x => new { x.Name, x.Age, Id = 50 };
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeConstant>(node);
        Assert.Equal("'{ Name = x.Name, Age = x.Age, Id = 50 }'", node.ToString());

        Assert.True(item.LambdaValue!.GetType().IsAnonymous());
        dynamic value = item.LambdaValue!;
        Assert.IsType<LambdaNodeMember>(value.Name);
        Assert.IsType<LambdaNodeMember>(value.Age);
        Assert.IsType<int>(value.Id);
    });

    //[Enforced]
    [Fact]
    public static void Parse_To_Array() => Executor(() =>
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeConstant item;

        WriteLine();
        func = x => new[] { x.Name, 7, null };
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeConstant>(node);
        Assert.Equal("'[x.Name, 7, NULL]'", node.ToString());

        var items = (object[])item.LambdaValue!;
        Assert.IsType<LambdaNodeMember>(items[0]);
        Assert.IsType<int>(items[1]);
        Assert.Null(items[2]);
    });

    ////[Enforced]
    //[Fact]
    //public static void Parse_() => Executor(() =>
    //{
    //});
}
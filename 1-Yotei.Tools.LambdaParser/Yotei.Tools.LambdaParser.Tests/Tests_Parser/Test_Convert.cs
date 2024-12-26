using static Yotei.Tools.Diagnostics.ConsoleEx;
using static System.ConsoleColor;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_Convert
{
    // Used to have a type with no parameterless constructor.
    public class Person(string name)
    {
        public override string ToString() => Name;
        public string Name { get; set; } = name ?? string.Empty;
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Convert_Argument()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeConvert item;

        WriteLine();
        func = x => (int)x;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeConvert>(node);
        Assert.Equal("((Int32) x)", node.ToString());

        WriteLine();
        func = x => (string)x;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeConvert>(node);
        Assert.Equal("((String) x)", node.ToString());

        WriteLine();
        func = x => (Person)x;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeConvert>(node);
        Assert.Equal("((Person) x)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Convert_IndexedArgument()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeIndexed item;

        WriteLine();
        func = x => x[(Person)x.Alpha, (string)x.Beta];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x[((Person) x.Alpha), ((String) x.Beta)]", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Convert_Member()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeConvert item;

        WriteLine();
        func = x => (int)x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeConvert>(node);
        Assert.Equal("((Int32) x.Alpha)", node.ToString());

        WriteLine();
        func = x => (Person)x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeConvert>(node);
        Assert.Equal("((Person) x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Convert_Member_And_Assign()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine();
        func = x => x.Beta = (int)x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Beta = ((Int32) x.Alpha))", node.ToString());

        WriteLine();
        func = x => x.Alpha = (Person)x.Alpha;
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha = ((Person) x.Alpha))", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Conversions_As_Indexes()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeIndexed item;

        WriteLine();
        func = x => x[(Person)x.Alpha, x.Alpha = (string)x.Beta];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x[((Person) x.Alpha), (x.Alpha = ((String) x.Beta))]", node.ToString());

        WriteLine();
        func = x => x[(Person)x.Alpha, x.Alpha = (float)x.Alpha];
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeIndexed>(node);
        Assert.Equal("x[((Person) x.Alpha), (x.Alpha = ((Single) x.Alpha))]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Conversions_As_Arguments()
    {
        Func<dynamic, object> func;
        LambdaNode node;
        LambdaNodeSetter item;

        WriteLine();
        func = x => x.Alpha = x.Alpha(null, x.Alpha = (int)x.Alpha);
        node = LambdaParser.Parse(func).Result;
        WriteLine($"> Result: {node}");
        item = Assert.IsType<LambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha = x.Alpha('NULL', (x.Alpha = ((Int32) x.Alpha))))", node.ToString());
    }
}
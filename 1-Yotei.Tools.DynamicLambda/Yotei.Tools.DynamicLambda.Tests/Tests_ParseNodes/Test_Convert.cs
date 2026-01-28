namespace Yotei.Tools.DynamicLambda.Tests;

// ========================================================
//[Enforced]
public static class Test_Convert
{
    // Type with no parameterless constructor.
    public class Person(string name)
    {
        public override string ToString() => $"(Person:'{Name}')";
        public string Name { get; set; } = name ?? string.Empty;
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Convert_Argument()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeConvert item;

        Debug.WriteLine("");
        func = x => (int)x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeConvert>(node);
        Assert.Equal("((Int32) x)", node.ToString());

        Debug.WriteLine("");
        func = x => (string)x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeConvert>(node);
        Assert.Equal("((String) x)", node.ToString());

        Debug.WriteLine("");
        func = x => (Person)x;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeConvert>(node);
        Assert.Equal("((Person) x)", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Convert_Member()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeConvert item;

        Debug.WriteLine("");
        func = x => (int)x.Alpha;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeConvert>(node);
        Assert.Equal("((Int32) x.Alpha)", node.ToString());

        Debug.WriteLine("");
        func = x => (Person)x.Alpha;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeConvert>(node);
        Assert.Equal("((Person) x.Alpha)", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Convert_Member_And_Assign()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x.Beta = (int)x.Alpha;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Beta = ((Int32) x.Alpha))", node.ToString());

        Debug.WriteLine("");
        func = x => x.Alpha = (Person)x.Alpha;
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha = ((Person) x.Alpha))", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Convert_IndexedArgument()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeIndexed item;

        Debug.WriteLine("");
        func = x => x[(Person)x.Alpha, (string)x.Beta];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeIndexed>(node);
        Assert.Equal("x[((Person) x.Alpha), ((String) x.Beta)]", node.ToString());
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_Conversions_As_Indexes()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeIndexed item;

        Debug.WriteLine("");
        func = x => x[(Person)x.Alpha, x.Alpha = (string)x.Beta];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeIndexed>(node);
        Assert.Equal("x[((Person) x.Alpha), (x.Alpha = ((String) x.Beta))]", node.ToString());

        Debug.WriteLine("");
        func = x => x[(Person)x.Alpha, x.Alpha = (float)x.Alpha];
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeIndexed>(node);
        Assert.Equal("x[((Person) x.Alpha), (x.Alpha = ((Single) x.Alpha))]", node.ToString());
    }

    //[Enforced]
    [Fact]
    public static void Parse_Conversions_As_Arguments()
    {
        Func<dynamic, object> func;
        DLambdaNode node;
        DLambdaNodeSetter item;

        Debug.WriteLine("");
        func = x => x.Alpha = x.Alpha(null, x.Alpha = (int)x.Alpha);
        node = DLambdaParser.Parse(func).Result;
        Debug.WriteLine($"> Result: {node}");
        item = Assert.IsType<DLambdaNodeSetter>(node);
        Assert.Equal("(x.Alpha = x.Alpha('NULL', (x.Alpha = ((Int32) x.Alpha))))", node.ToString());
    }
}
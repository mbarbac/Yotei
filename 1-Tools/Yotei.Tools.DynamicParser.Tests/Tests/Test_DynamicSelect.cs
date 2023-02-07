using static System.Console;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_DynamicSelect
{
    public class Person
    {
        public Person() { }
        public Person(string name, int age) { Name = name; Age = age; }
        public override string ToString() => $"{Name}, {Age}";
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; } = 0;
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Parse_New_To_Constant()
    {
        Func<dynamic, object> func;
        DynamicParser parser;
        DynamicNode node;
        DynamicNodeValued valued;

        func = x => new Person("Cervantes", 50);
        parser = DynamicParser.Parse(func);
        WriteLine($"\n> Parser: {parser}");

        node = parser.Result;
        valued = Assert.IsType<DynamicNodeValued>(node);
        Assert.IsType<Person>(valued.DynamicValue);
    }

    //[Enforced]
    [Fact]
    public static void Parse_New_To_Anonymous()
    {
        Func<dynamic, object> func;
        DynamicParser parser;
        DynamicNode node;
        DynamicNodeValued valued;

        func = x => new { x.Name, x.Age, Id = "50" };
        parser = DynamicParser.Parse(func);
        WriteLine($"\n> Parser: {parser}");
        Assert.Equal("(x) => '{ Name = x.Name, Age = x.Age, Id = 50 }'", parser.ToString());

        node = parser.Result;
        valued = Assert.IsType<DynamicNodeValued>(node);
        Assert.True(valued.DynamicValue!.GetType().IsAnonymous());
    }

    //[Enforced]
    //[Fact]
    //public static void Parse_New_To_Dynamic()
    //{
    //    Func<dynamic, object> func;
    //    DynamicParser parser;
    //    DynamicNode node;

    //    func = x => new Person() { Name = x.Name, Age = x.Age };
    //    parser = DynamicParser.Parse(func);
    //    WriteLine($"\n> Parser: {parser}");
    //    node = parser.Result;

    //}
}
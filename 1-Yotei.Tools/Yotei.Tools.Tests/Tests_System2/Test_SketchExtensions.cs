#pragma warning disable IDE0079
#pragma warning disable IDE0290
#pragma warning disable IDE0044
#pragma warning disable CA1822

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_SketchExtensions
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = nameof(Test_SketchExtensions);

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Value_Null()
    {
        // Default...
        var source = (object?)null;
        var options = SketchOptions.Default;
        var name = source.Sketch(options); Assert.Equal("NULL", name);

        // Empty...
        options = SketchOptions.Empty;
        name = source.Sketch(options); Assert.Equal(string.Empty, name);

        // Full...
        options = SketchOptions.Full;
        name = source.Sketch(options);
        Assert.Equal("(System.Object) NULL", name);

        var other = (int?)null;
        name = other.Sketch(options);
        Assert.Equal("(System.Nullable<System.Int32>) NULL", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Value_String()
    {
        // Default...
        var source = "Hello";
        var options = SketchOptions.Default;
        var name = source.Sketch(options); Assert.Equal("Hello", name);

        // Empty...
        options = SketchOptions.Empty;
        name = source.Sketch(options); Assert.Equal("Hello", name);

        // Full...
        options = SketchOptions.Full;
        name = source.Sketch(options); Assert.Equal("(System.String) Hello", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Value_Guid()
    {
        // Default...
        var source = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
        var options = SketchOptions.Default;
        var name = source.Sketch(options);
        Assert.Equal("00000001-0002-0003-0405-060708090a0b", name);

        // Empty...
        options = SketchOptions.Empty;
        name = source.Sketch(options); Assert.Equal("00000001-0002-0003-0405-060708090a0b", name);

        // Full...
        options = SketchOptions.Full;
        name = source.Sketch(options);
        Assert.Equal("(System.Guid) 00000001-0002-0003-0405-060708090a0b", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Value_Int()
    {
        // Default...
        var source = 5;
        var options = SketchOptions.Default;
        var name = source.Sketch(options); Assert.Equal("5", name); 

        // Empty...
        options = SketchOptions.Empty;
        name = source.Sketch(options); Assert.Equal("5", name);

        // Full...
        options = SketchOptions.Full;
        name = source.Sketch(options); Assert.Equal("(System.Int32) 5", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Value_Double()
    {
        // Default...
        var source = 1234.567;
        var options = SketchOptions.Default with { Provider = CultureInfo.InvariantCulture };
        var name = source.Sketch(options); Assert.Equal("1234.567", name);

        options = options with { Format = "0.00" };
        name = source.Sketch(options); Assert.Equal("1234.57", name);

        options = SketchOptions.Default with { Provider = CultureInfo.GetCultureInfo("es-ES") };
        name = source.Sketch(options); Assert.Equal("1234,567", name);

        options = options with { Format = "0.00" };
        name = source.Sketch(options); Assert.Equal("1234,57", name);

        options = options with { Format = "0,000.00" };
        name = source.Sketch(options); Assert.Equal("1.234,57", name);

        // Empty...
        options = SketchOptions.Empty with { Provider = CultureInfo.InvariantCulture };
        name = source.Sketch(options); Assert.Equal("1234.567", name);

        // Full...
        options = SketchOptions.Full with { Provider = CultureInfo.InvariantCulture };
        name = source.Sketch(options); Assert.Equal("(System.Double) 1234.567", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Value_Decimal()
    {
        // Default...
        var source = new decimal(1234.567);
        var options = SketchOptions.Default with { Provider = CultureInfo.InvariantCulture };
        var name = source.Sketch(options); Assert.Equal("1234.567", name);

        options = options with { Format = "0.00" };
        name = source.Sketch(options); Assert.Equal("1234.57", name);

        options = SketchOptions.Default with { Provider = CultureInfo.GetCultureInfo("es-ES") };
        name = source.Sketch(options); Assert.Equal("1234,567", name);

        options = options with { Format = "0.00" };
        name = source.Sketch(options); Assert.Equal("1234,57", name);

        options = options with { Format = "0,000.00" };
        name = source.Sketch(options); Assert.Equal("1.234,57", name);

        // Empty...
        options = SketchOptions.Empty with { Provider = CultureInfo.InvariantCulture };
        name = source.Sketch(options); Assert.Equal("1234.567", name);

        // Full...
        options = SketchOptions.Full with { Provider = CultureInfo.InvariantCulture };
        name = source.Sketch(options); Assert.Equal("(System.Decimal) 1234.567", name);
    }

    // ----------------------------------------------------

    [Flags] public enum MyEnum { One = 1, Two = 2, Three = 4 }

    //[Enforced]
    [Fact]
    public static void Test_Enum_Simple()
    {
        // Default...
        var source = MyEnum.One;
        var options = SketchOptions.Default;
        var name = source.Sketch(options); Assert.Equal("One", name);

        // Empty...
        options = SketchOptions.Empty;
        name = source.Sketch(options); Assert.Equal("One", name);

        // Full...
        options = SketchOptions.Full;
        name = source.Sketch(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.MyEnum.One", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Enum_Flags()
    {
        // Default...
        var source = MyEnum.One | MyEnum.Two | MyEnum.Three;
        var options = SketchOptions.Default;
        var name = source.Sketch(options); Assert.Equal("One | Two | Three", name);

        options = options with
        { SourceTypeOptions = EasyNameOptions.Default with { } };
        name = source.Sketch(options);
        Assert.Equal("MyEnum.One | MyEnum.Two | MyEnum.Three", name);

        // Empty...
        options = SketchOptions.Empty;
        name = source.Sketch(options); Assert.Equal("One | Two | Three", name);

        // Full...
        options = SketchOptions.Full;
        name = source.Sketch(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.MyEnum.One | " +
            $"{NAMESPACE}.{CLASSNAME}.MyEnum.Two | " +
            $"{NAMESPACE}.{CLASSNAME}.MyEnum.Three",
            name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Array_Empty()
    {
        // Default...
        var source = Array.Empty<char>();
        var options = SketchOptions.Default;
        var name = source.Sketch(options); Assert.Equal("[]", name);

        options = options with
        { SourceTypeOptions = EasyNameOptions.Default with { } };
        name = source.Sketch(options); Assert.Equal("(Char[]) []", name);

        // Empty...
        options = SketchOptions.Empty;
        name = source.Sketch(options); Assert.Equal("[]", name);

        // Full...
        options = SketchOptions.Full;
        name = source.Sketch(options);
        Assert.Equal("(System.Char[]) []", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_Populated()
    {
        // Default...
        var source = new int[] { 1, 2, 3 };
        var options = SketchOptions.Default;
        var name = source.Sketch(options); Assert.Equal("[1, 2, 3]", name);

        options = options with
        { SourceTypeOptions = EasyNameOptions.Default with { } };
        name = source.Sketch(options); Assert.Equal("(Int32[]) [(Int32) 1, (Int32) 2, (Int32) 3]", name);

        // Empty...
        options = SketchOptions.Empty;
        name = source.Sketch(options); Assert.Equal("[1, 2, 3]", name);

        // Full...
        options = SketchOptions.Full;
        name = source.Sketch(options);
        Assert.Equal("(System.Int32[]) [(System.Int32) 1, (System.Int32) 2, (System.Int32) 3]", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dictionary()
    {
        // Default...
        var source = new Dictionary<string, int>() { { "James", 50 }, { "Maria", 25 } };
        var options = SketchOptions.Default;
        var name = source.Sketch(options); Assert.Equal("{James = 50, Maria = 25}", name);

        options = options with
        { SourceTypeOptions = EasyNameOptions.Default with { } };
        name = source.Sketch(options);
        Assert.Equal(
            "(Dictionary<String, Int32>) " +
            "{(String) James = (Int32) 50, (String) Maria = (Int32) 25}",
            name);

        // Empty...
        options = SketchOptions.Empty;
        name = source.Sketch(options); Assert.Equal("{James = 50, Maria = 25}", name);

        // Full...
        options = SketchOptions.Full;
        name = source.Sketch(options);
        Assert.Equal(
            "(System.Collections.Generic.Dictionary<System.String, System.Int32>) " +
            "{(System.String) James = (System.Int32) 50, (System.String) Maria = (System.Int32) 25}",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_List()
    {
        // Default...
        var source = new List<string>() { "James", "Maria" };
        var options = SketchOptions.Default;
        var name = source.Sketch(options); Assert.Equal("[James, Maria]", name);

        options = options with
        { SourceTypeOptions = EasyNameOptions.Default with { } };
        name = source.Sketch(options);
        Assert.Equal(
            "(List<String>) [(String) James, (String) Maria]",
            name);

        // Empty...
        options = SketchOptions.Empty;
        name = source.Sketch(options); Assert.Equal("[James, Maria]", name);

        // Full...
        options = SketchOptions.Full;
        name = source.Sketch(options);
        Assert.Equal(
            "(System.Collections.Generic.List<System.String>) " +
            "[(System.String) James, (System.String) Maria]",
            name);
    }

    // ----------------------------------------------------

    public class AX
    {
        public AX(string name, int age) { Name = name; Age = age; }
        public int GetAge() => Age;
        public string GetOrg() => Org;

        public string this[int index] => $"Name{index}";
        public string Name { get; set; }

        int Age;
        static string Org = "MI6";
    }
    public class AY : AX
    {
        public AY(string name, int age) : base(name, age) { }
        public override string ToString() => $"AY&{Name}&{GetAge()}";
    }
    public class AZ : AY
    {
        public AZ(string name, int age) : base(name, age) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Shape()
    {
        // Default...
        var source = new AX("James", 50);
        var options = SketchOptions.Default;
        var name = source.Sketch(options);
        Assert.Equal("{ this[Int32], Name = James }", name);

        // Empty...
        options = SketchOptions.Empty;
        name = source.Sketch(options); Assert.Equal("AX", name);

        // Full...
        options = SketchOptions.Full;
        name = source.Sketch(options);
        Assert.Equal(
            $"({NAMESPACE}.{CLASSNAME}.AX) " +
            "{ this[Int32], Name = (System.String) James, " +
            "Age = (System.Int32) 50, Org = (System.String) MI6 }",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Shape_ToString_Precedence()
    {
        // Default...
        var source = new AZ("James", 50);
        var options = SketchOptions.Default;
        var name = source.Sketch(options);
        Assert.Equal("AY&James&50", name);

        // Empty...
        options = SketchOptions.Empty;
        name = source.Sketch(options); Assert.Equal("AY&James&50", name);

        // Full...
        options = SketchOptions.Full;
        name = source.Sketch(options);
        Assert.Equal($"({NAMESPACE}.{CLASSNAME}.AZ) AY&James&50", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dynamics()
    {
        // Default...
        var source = new ExpandoObject();
        dynamic obj = source;
        obj.Name = "James";
        obj.Age = 50;

        var options = SketchOptions.Default;
        var name = source.Sketch(options); Assert.Equal("{ [Name, James], [Age, 50] }", name);

        options = options with
        { SourceTypeOptions = EasyNameOptions.Default with { } };
        name = source.Sketch(options);
        Assert.Equal(
            "(ExpandoObject) { " +
            "(KeyValuePair<String, Object>) [Name, James], " +
            "(KeyValuePair<String, Object>) [Age, 50] " +
            "}",
            name);

        // Empty...
        options = SketchOptions.Empty;
        name = source.Sketch(options);
        Assert.Equal("{ [Name, James], [Age, 50] }", name);

        // Full...
        options = SketchOptions.Full;
        name = source.Sketch(options);
        Assert.Equal(
            "(System.Dynamic.ExpandoObject) { " +
            "(System.Collections.Generic.KeyValuePair<System.String, System.Object>) [Name, James], " +
            "(System.Collections.Generic.KeyValuePair<System.String, System.Object>) [Age, 50] " +
            "}",
            name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Anonymous()
    {
        // Default...
        var source = new { Name = "James", Age = 50 };
        var options = SketchOptions.Default;
        var name = source.Sketch(options); Assert.Equal("{ Name = James, Age = 50 }", name);

        // LOW: Sketch anonymous: would be nice to have the values' types.
        options = options with
        { SourceTypeOptions = EasyNameOptions.Default with { } };
        name = source.Sketch(options);
        Assert.EndsWith("<String, Int32>) { Name = James, Age = 50 }", name);

        // Empty...
        options = SketchOptions.Empty;
        name = source.Sketch(options);
        Assert.Equal("{ Name = James, Age = 50 }", name);

        // Full...
        options = SketchOptions.Full;
        name = source.Sketch(options);
        Assert.EndsWith("<System.String, System.Int32>) { Name = James, Age = 50 }", name);
    }
}
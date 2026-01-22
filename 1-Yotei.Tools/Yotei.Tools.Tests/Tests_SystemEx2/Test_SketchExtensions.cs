#pragma warning disable IDE0290
#pragma warning disable IDE0044
#pragma warning disable CA1822

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_SketchExtensions
{
    static readonly string NAMESPACE = typeof(Test_SketchExtensions).Namespace!;
    static readonly string TESTNAME = typeof(Test_SketchExtensions).Name;

    readonly static SketchOptions EMPTY = SketchOptions.Empty;
    readonly static SketchOptions DEFAULT = SketchOptions.Default;
    readonly static SketchOptions FULL = SketchOptions.Full;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_System_Null()
    {
        SketchOptions options;
        string name;
        var item = (string?)null;

        options = EMPTY;
        name = item.Sketch(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.Sketch(options); Assert.Equal("NULL", name);

        options = DEFAULT with { UseTypeHead = true };
        name = item.Sketch(options); Assert.Equal("(String) NULL", name);

        options = FULL;
        name = item.Sketch(options); Assert.Equal("(System.String) NULL", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_System_Byte()
    {
        SketchOptions options;
        string name;
        var item = (byte)15;

        options = EMPTY;
        name = item.Sketch(options); Assert.Equal("15", name);

        options = DEFAULT;
        name = item.Sketch(options); Assert.Equal("15", name);

        options = DEFAULT with { UseTypeHead = true };
        name = item.Sketch(options); Assert.Equal("(Byte) 15", name);

        options = FULL;
        name = item.Sketch(options); Assert.Equal("(System.Byte) 15", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_System_String()
    {
        SketchOptions options;
        string name;
        var item = "Hello";

        options = EMPTY;
        name = item.Sketch(options); Assert.Equal("Hello", name);

        options = DEFAULT;
        name = item.Sketch(options); Assert.Equal("Hello", name);

        options = DEFAULT with { UseTypeHead = true };
        name = item.Sketch(options); Assert.Equal("(String) Hello", name);

        options = FULL;
        name = item.Sketch(options); Assert.Equal("(System.String) Hello", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_System_Guid()
    {
        SketchOptions options;
        string name;
        var item = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);
        var result = "00000001-0002-0003-0405-060708090a0b";

        options = EMPTY;
        name = item.Sketch(options); Assert.Equal(result, name);

        options = DEFAULT;
        name = item.Sketch(options); Assert.Equal(result, name);

        options = DEFAULT with { UseTypeHead = true };
        name = item.Sketch(options); Assert.Equal($"(Guid) {result}", name);

        options = FULL;
        name = item.Sketch(options); Assert.Equal($"(System.Guid) {result}", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_System_Type()
    {
        SketchOptions options;
        string name;
        var item = typeof(string);

        options = EMPTY;
        name = item.Sketch(options); Assert.Equal("String", name);

        options = DEFAULT;
        name = item.Sketch(options); Assert.Equal("String", name);

        options = FULL;
        name = item.Sketch(options); Assert.Equal("System.String", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_System_Type_NullableWrapper()
    {
        SketchOptions options;
        string name;
        var item = typeof(EasyNullable<string>);

        options = EMPTY;
        name = item.Sketch(options); Assert.Equal("String", name);

        options = DEFAULT;
        name = item.Sketch(options); Assert.Equal("String?", name);

        options = FULL;
        name = item.Sketch(options);
        Assert.Equal("Yotei.Tools.EasyNullable<System.String>", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_System_Double()
    {
        SketchOptions options;
        string name;
        var item = 1234.567;

        options = EMPTY with { FormatProvider = CultureInfo.InvariantCulture };
        name = item.Sketch(options); Assert.Equal("1234.567", name);

        options = options with { FormatString = "0.00" };
        name = item.Sketch(options); Assert.Equal("1234.57", name);

        options = DEFAULT with { FormatProvider = CultureInfo.GetCultureInfo("es-ES") };
        name = item.Sketch(options); Assert.Equal("1234,567", name);

        options = options with { FormatString = "0.00" };
        name = item.Sketch(options); Assert.Equal("1234,57", name);

        options = FULL with { FormatProvider = CultureInfo.InvariantCulture };
        name = item.Sketch(options); Assert.Equal("(System.Double) 1234.567", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_System_Decimal()
    {
        SketchOptions options;
        string name;
        var item = new decimal(1234.567);

        options = EMPTY with { FormatProvider = CultureInfo.InvariantCulture };
        name = item.Sketch(options); Assert.Equal("1234.567", name);

        options = options with { FormatString = "0.00" };
        name = item.Sketch(options); Assert.Equal("1234.57", name);

        options = DEFAULT with { FormatProvider = CultureInfo.GetCultureInfo("es-ES") };
        name = item.Sketch(options); Assert.Equal("1234,567", name);

        options = options with { FormatString = "0.00" };
        name = item.Sketch(options); Assert.Equal("1234,57", name);

        options = FULL with { FormatProvider = CultureInfo.InvariantCulture };
        name = item.Sketch(options); Assert.Equal("(System.Decimal) 1234.567", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_System_DateTime()
    {
        SketchOptions options;
        string name;
        var item = new DateTime(2000, 12, 31);

        options = EMPTY with { FormatProvider = CultureInfo.InvariantCulture };
        name = item.Sketch(options); Assert.Equal("12/31/2000 00:00:00", name);

        options = DEFAULT with { FormatProvider = CultureInfo.GetCultureInfo("es-ES") };
        name = item.Sketch(options); Assert.Equal("31/12/2000 0:00:00", name);

        options = FULL with { FormatProvider = CultureInfo.InvariantCulture };
        name = item.Sketch(options); Assert.Equal("(System.DateTime) 12/31/2000 00:00:00", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_System_DateOnly()
    {
        SketchOptions options;
        string name;
        var item = new DateOnly(2000, 12, 31);

        options = EMPTY with { FormatProvider = CultureInfo.InvariantCulture };
        name = item.Sketch(options); Assert.Equal("12/31/2000", name);

        options = DEFAULT with { FormatProvider = CultureInfo.GetCultureInfo("es-ES") };
        name = item.Sketch(options); Assert.Equal("31/12/2000", name);

        options = FULL with { FormatProvider = CultureInfo.InvariantCulture };
        name = item.Sketch(options); Assert.Equal("(System.DateOnly) 12/31/2000", name);
    }

    // ----------------------------------------------------

    [Flags] public enum MyEnum { One = 1, Two = 2, Three = 4 }

    //[Enforced]
    [Fact]
    public static void Test_Enum_Simple()
    {
        SketchOptions options;
        string name;
        var source = MyEnum.One;

        options = EMPTY;
        name = source.Sketch(options); Assert.Equal("One", name);

        options = DEFAULT;
        name = source.Sketch(options); Assert.Equal("One", name);

        options = DEFAULT with { UseTypeHead = true };
        name = source.Sketch(options); Assert.Equal("MyEnum.One", name);

        options = FULL;
        name = source.Sketch(options); Assert.Equal($"{NAMESPACE}.{TESTNAME}.MyEnum.One", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Enum_Flags()
    {
        SketchOptions options;
        string name;
        var source = MyEnum.One | MyEnum.Two | MyEnum.Three;

        options = EMPTY;
        name = source.Sketch(options); Assert.Equal("One | Two | Three", name);

        options = DEFAULT;
        name = source.Sketch(options); Assert.Equal("One | Two | Three", name);

        options = DEFAULT with { UseTypeHead = true };
        name = source.Sketch(options);
        Assert.Equal("MyEnum.One | MyEnum.Two | MyEnum.Three", name);

        options = FULL;
        name = source.Sketch(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}.MyEnum.One | " +
            $"{NAMESPACE}.{TESTNAME}.MyEnum.Two | " +
            $"{NAMESPACE}.{TESTNAME}.MyEnum.Three",
            name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Array_Empty()
    {
        SketchOptions options;
        string name;
        var source = Array.Empty<char>();

        options = EMPTY;
        name = source.Sketch(options); Assert.Equal("[]", name);

        options = DEFAULT;
        name = source.Sketch(options); Assert.Equal("[]", name);

        options = DEFAULT with { UseTypeHead = true };
        name = source.Sketch(options); Assert.Equal("(Char[]) []", name);

        options = FULL;
        name = source.Sketch(options); Assert.Equal("(System.Char[]) []", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_Populated()
    {
        SketchOptions options;
        string name;
        var source = new int[] { 1, 2, 3 };

        options = EMPTY;
        name = source.Sketch(options); Assert.Equal("[1, 2, 3]", name);

        options = DEFAULT;
        name = source.Sketch(options); Assert.Equal("[1, 2, 3]", name);

        options = DEFAULT with { UseTypeHead = true };
        name = source.Sketch(options);
        Assert.Equal("(Int32[]) [(Int32) 1, (Int32) 2, (Int32) 3]", name);

        options = FULL;
        name = source.Sketch(options);
        Assert.Equal(
            "(System.Int32[]) [(System.Int32) 1, (System.Int32) 2, (System.Int32) 3]",
            name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dictionary()
    {
        SketchOptions options;
        string name;
        var source = new Dictionary<string, int>() { { "James", 50 }, { "Maria", 25 } };

        options = EMPTY;
        name = source.Sketch(options); Assert.Equal("{James = 50, Maria = 25}", name);

        options = DEFAULT;
        name = source.Sketch(options); Assert.Equal("{James = 50, Maria = 25}", name);

        options = DEFAULT with { UseTypeHead = true };
        name = source.Sketch(options);
        Assert.Equal(
            "(Dictionary<String, Int32>) {(String) James = (Int32) 50, (String) Maria = (Int32) 25}",
            name);

        options = DEFAULT with { UseTypeHead = true };
        name = source.Sketch(options);
        Assert.Equal(
            "(Dictionary<String, Int32>) {(String) James = (Int32) 50, (String) Maria = (Int32) 25}",
            name);

        options = FULL;
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
        SketchOptions options;
        string name;
        var source = new List<string>() { "James", "Maria" };

        options = EMPTY;
        name = source.Sketch(options); Assert.Equal("[James, Maria]", name);

        options = DEFAULT;
        name = source.Sketch(options); Assert.Equal("[James, Maria]", name);

        options = DEFAULT with { UseTypeHead = true };
        name = source.Sketch(options);
        Assert.Equal("(List<String>) [(String) James, (String) Maria]", name);

        options = FULL;
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

    //[Enforced]
    [Fact]
    public static void Test_Shape()
    {
        SketchOptions options;
        string name;
        var source = new AX("James", 50);

        options = EMPTY;
        name = source.Sketch(options); Assert.Equal("AX", name);

        options = DEFAULT;
        name = source.Sketch(options); Assert.Equal("{ this, Name = James }", name);

        options = DEFAULT with { UseTypeHead = true };
        name = source.Sketch(options);
        Assert.Equal("(AX) { this, Name = (String) James }", name);

        options = FULL;
        name = source.Sketch(options);
        Assert.Equal(
            $"({NAMESPACE}.{TESTNAME}.AX) " +
            "{ this, " +
            "Name = (System.String) James, Age = (System.Int32) 50, Org = (System.String) MI6 }",
            name);
    }

    // ----------------------------------------------------

    public class AY : AX
    {
        public AY(string name, int age) : base(name, age) { }
        public override string ToString() => $"<AY> & {Name} & {GetAge()}";
    }

    public class AZ : AY
    {
        public AZ(string name, int age) : base(name, age) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Shape_ToString_Precedence()
    {
        SketchOptions options;
        string name;
        var source = new AZ("James", 50);

        options = EMPTY;
        name = source.Sketch(options); Assert.Equal("<AY> & James & 50", name);

        options = DEFAULT;
        name = source.Sketch(options); Assert.Equal("<AY> & James & 50", name);

        options = DEFAULT with { UseTypeHead = true };
        name = source.Sketch(options); Assert.Equal("(AZ) <AY> & James & 50", name);

        options = FULL;
        name = source.Sketch(options);
        Assert.Equal($"({NAMESPACE}.{TESTNAME}.AZ) <AY> & James & 50", name);

        options = DEFAULT with { PreventToString = true };
        name = source.Sketch(options); Assert.Equal("{ this, Name = James }", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Value_Dynamics()
    {
        SketchOptions options;
        string name;
        var source = new ExpandoObject();
        dynamic obj = source;
        obj.Name = "James";
        obj.Age = 50;

        options = EMPTY;
        name = source.Sketch(options); Assert.Equal("{ [Name, James], [Age, 50] }", name);

        options = DEFAULT;
        name = source.Sketch(options); Assert.Equal("{ [Name, James], [Age, 50] }", name);

        options = DEFAULT with { UseTypeHead = true };
        name = source.Sketch(options);
        Assert.Equal(
            "(ExpandoObject) { " +
            "(KeyValuePair<String, Object>) [Name, James], " +
            "(KeyValuePair<String, Object>) [Age, 50] }",
            name);

        options = FULL;
        name = source.Sketch(options);
        Assert.Equal(
            "(System.Dynamic.ExpandoObject) { " +
            "(System.Collections.Generic.KeyValuePair<System.String, System.Object>) [Name, James], " +
            "(System.Collections.Generic.KeyValuePair<System.String, System.Object>) [Age, 50] }",
            name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Value_Anonymous()
    {
        SketchOptions options;
        string name;
        var source = new { Name = "James", Age = 50 };

        options = EMPTY;
        name = source.Sketch(options); Assert.Equal("{ Name = James, Age = 50 }", name);


        options = DEFAULT;
        name = source.Sketch(options); Assert.Equal("{ Name = James, Age = 50 }", name);

        options = DEFAULT with { UseTypeHead = true };
        name = source.Sketch(options);
        Assert.EndsWith("{ Name = James, Age = 50 }", name);
        Assert.Contains("AnonymousType", name);
    }
}
namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_SketchExtensions
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = nameof(Test_SketchExtensions);

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Null_Object()
    {
        object? source = null;

        var options = SketchOptions.Empty;
        var name = source.Sketch(options); Assert.Equal("", name);

        options = options with { NullStr = "NULL" };
        name = source.Sketch(options); Assert.Equal("NULL", name);

        options = options with { UseSourceType = EasyTypeOptions.Default };
        name = source.Sketch(options); Assert.Equal("(Object) NULL", name);

        options = options with { UseSourceType = options.UseSourceType with { UseNamespace = true } };
        name = source.Sketch(options); Assert.Equal("(System.Object) NULL", name);

        // Default...
        options = SketchOptions.Default;
        name = source.Sketch(options); Assert.Equal("NULL", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Null_String()
    {
        string? source = null;

        var options = SketchOptions.Empty;
        var name = source.Sketch(options); Assert.Equal("", name);

        options = options with { NullStr = "NULL" };
        name = source.Sketch(options); Assert.Equal("NULL", name);

        options = options with { UseSourceType = EasyTypeOptions.Default };
        name = source.Sketch(options); Assert.Equal("(String) NULL", name);

        options = options with { UseSourceType = options.UseSourceType with { UseNamespace = true } };
        name = source.Sketch(options); Assert.Equal("(System.String) NULL", name);

        // Default...
        options = SketchOptions.Default;
        name = source.Sketch(options); Assert.Equal("NULL", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Null_Others()
    {
        int? source = null;

        var options = SketchOptions.Empty;
        var name = source.Sketch(options); Assert.Equal("", name);

        options = options with { NullStr = "NULL" };
        name = source.Sketch(options); Assert.Equal("NULL", name);

        options = options with { UseSourceType = EasyTypeOptions.Default };
        name = source.Sketch(options); Assert.Equal("(Nullable<Int32>) NULL", name);

        options = options with { UseSourceType = options.UseSourceType with { UseNamespace = true } };
        name = source.Sketch(options); Assert.Equal("(System.Nullable<Int32>) NULL", name);

        // Default...
        options = SketchOptions.Default;
        name = source.Sketch(options); Assert.Equal("NULL", name);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Regular_String()
    {
        var source = "Hello";

        var options = SketchOptions.Empty;
        var name = source.Sketch(options); Assert.Equal("Hello", name);

        options = options with { NullStr = "NULL" };
        name = source.Sketch(options); Assert.Equal("Hello", name);

        options = options with { UseSourceType = EasyTypeOptions.Default };
        name = source.Sketch(options); Assert.Equal("(String) Hello", name);

        options = options with { UseSourceType = options.UseSourceType with { UseNamespace = true } };
        name = source.Sketch(options); Assert.Equal("(System.String) Hello", name);

        // Default...
        options = SketchOptions.Default;
        name = source.Sketch(options); Assert.Equal("Hello", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Regular_Guid()
    {
        var source = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);

        var options = SketchOptions.Empty;
        var name = source.Sketch(options); Assert.Equal("00000001-0002-0003-0405-060708090a0b", name);

        options = options with { NullStr = "NULL" };
        name = source.Sketch(options); Assert.Equal("00000001-0002-0003-0405-060708090a0b", name);

        options = options with { UseSourceType = EasyTypeOptions.Default };
        name = source.Sketch(options); Assert.Equal("(Guid) 00000001-0002-0003-0405-060708090a0b", name);

        options = options with { UseSourceType = options.UseSourceType with { UseNamespace = true } };
        name = source.Sketch(options); Assert.Equal("(System.Guid) 00000001-0002-0003-0405-060708090a0b", name);

        // Default...
        options = SketchOptions.Default;
        name = source.Sketch(options); Assert.Equal("00000001-0002-0003-0405-060708090a0b", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Regular_Int()
    {
        var source = 5;

        var options = SketchOptions.Empty;
        var name = source.Sketch(options); Assert.Equal("5", name);

        options = options with { NullStr = "NULL" };
        name = source.Sketch(options); Assert.Equal("5", name);

        options = options with { UseSourceType = EasyTypeOptions.Default };
        name = source.Sketch(options); Assert.Equal("(Int32) 5", name);

        options = options with { UseSourceType = options.UseSourceType with { UseNamespace = true } };
        name = source.Sketch(options); Assert.Equal("(System.Int32) 5", name);

        // Default...
        options = SketchOptions.Default;
        name = source.Sketch(options); Assert.Equal("5", name);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Double()
    {
        var source = 1234.567;

        var options = SketchOptions.Empty with { Provider = CultureInfo.InvariantCulture };
        var name = source.Sketch(options); Assert.Equal("1234.567", name);

        options = options with { NullStr = "NULL" };
        name = source.Sketch(options); Assert.Equal("1234.567", name);

        options = options with { UseSourceType = EasyTypeOptions.Default };
        name = source.Sketch(options); Assert.Equal("(Double) 1234.567", name);

        options = options with { UseSourceType = options.UseSourceType with { UseNamespace = true } };
        name = source.Sketch(options); Assert.Equal("(System.Double) 1234.567", name);

        // Default...
        options = SketchOptions.Default;
        name = source.Sketch(options); Assert.Equal("1234.567", name);

        // Explicit culture...
        options = SketchOptions.Default with { Provider = CultureInfo.GetCultureInfo("es-ES") };
        name = source.Sketch(options); Assert.Equal("1234,567", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Decimal()
    {
        var source = new decimal(7.5);

        var options = SketchOptions.Empty with { Provider = CultureInfo.InvariantCulture };
        var name = source.Sketch(options); Assert.Equal("7.5", name);

        options = options with { NullStr = "NULL" };
        name = source.Sketch(options); Assert.Equal("7.5", name);

        options = options with { UseSourceType = EasyTypeOptions.Default };
        name = source.Sketch(options); Assert.Equal("(Decimal) 7.5", name);

        options = options with { UseSourceType = options.UseSourceType with { UseNamespace = true } };
        name = source.Sketch(options); Assert.Equal("(System.Decimal) 7.5", name);

        // Default...
        options = SketchOptions.Default;
        name = source.Sketch(options); Assert.Equal("7.5", name);

        // Explicit culture...
        options = SketchOptions.Default with { Provider = CultureInfo.GetCultureInfo("es-ES") };
        name = source.Sketch(options); Assert.Equal("7,5", name);
    }

    // ----------------------------------------------------

    [Flags] public enum MyEnum { One = 1, Two = 2, Three = 4 }

    //[Enforced]
    [Fact]
    public static void Test_Enum_Simple()
    {
        var source = MyEnum.One;

        var options = SketchOptions.Empty;
        var name = source.Sketch(options); Assert.Equal("One", name);

        options = options with { NullStr = "NULL" };
        name = source.Sketch(options); Assert.Equal("One", name);

        options = options with { UseSourceType = EasyTypeOptions.Default };
        name = source.Sketch(options); Assert.Equal("MyEnum.One", name);

        options = options with { UseSourceType = options.UseSourceType with { UseNamespace = true } };
        name = source.Sketch(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.MyEnum.One", name);

        // Default...
        options = SketchOptions.Default;
        name = source.Sketch(options); Assert.Equal("One", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Enum_Flags()
    {
        var source = MyEnum.One | MyEnum.Two | MyEnum.Three;

        var options = SketchOptions.Empty;
        var name = source.Sketch(options); Assert.Equal("One | Two | Three", name);

        options = options with { NullStr = "NULL" };
        name = source.Sketch(options); Assert.Equal("One | Two | Three", name);

        options = options with { UseSourceType = EasyTypeOptions.Default };
        name = source.Sketch(options); Assert.Equal("MyEnum.One | MyEnum.Two | MyEnum.Three", name);

        options = options with { UseSourceType = options.UseSourceType with { UseNamespace = true } };
        name = source.Sketch(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.MyEnum.One | {NAMESPACE}.{CLASSNAME}.MyEnum.Two | {NAMESPACE}.{CLASSNAME}.MyEnum.Three", name);

        // Default...
        options = SketchOptions.Default;
        name = source.Sketch(options); Assert.Equal("One | Two | Three", name);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Array_Empty()
    {
        var source = Array.Empty<char>();

        var options = SketchOptions.Empty;
        var name = source.Sketch(options); Assert.Equal("[]", name);

        options = options with { NullStr = "NULL" };
        name = source.Sketch(options); Assert.Equal("[]", name);

        options = options with { UseSourceType = EasyTypeOptions.Default };
        name = source.Sketch(options); Assert.Equal("(Char[]) []", name);

        options = options with { UseSourceType = options.UseSourceType with { UseNamespace = true } };
        name = source.Sketch(options); Assert.Equal("(System.Char[]) []", name);

        // Default...
        options = SketchOptions.Default;
        name = source.Sketch(options); Assert.Equal("[]", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_Populated()
    {
        var source = new int[] { 1, 2, 3 };

        var options = SketchOptions.Empty;
        var name = source.Sketch(options); Assert.Equal("[1, 2, 3]", name);

        options = options with { NullStr = "NULL" };
        name = source.Sketch(options); Assert.Equal("[1, 2, 3]", name);

        options = options with { UseSourceType = EasyTypeOptions.Default };
        name = source.Sketch(options); Assert.Equal("(Int32[]) [1, 2, 3]", name);

        options = options with { UseSourceType = options.UseSourceType with { UseNamespace = true } };
        name = source.Sketch(options); Assert.Equal("(System.Int32[]) [1, 2, 3]", name);

        // Default...
        options = SketchOptions.Default;
        name = source.Sketch(options); Assert.Equal("[1, 2, 3]", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dictionary()
    {
        var source = new Dictionary<string, int>() { { "James", 50 }, { "Maria", 25 } };

        var options = SketchOptions.Empty;
        var name = source.Sketch(options); Assert.Equal("{James = 50, Maria = 25}", name);

        options = options with { NullStr = "NULL" };
        name = source.Sketch(options); Assert.Equal("{James = 50, Maria = 25}", name);

        options = options with { UseSourceType = EasyTypeOptions.Default };
        name = source.Sketch(options); Assert.Equal("(Dictionary<String, Int32>) {James = 50, Maria = 25}", name);

        options = options with { UseSourceType = options.UseSourceType with { UseNamespace = true } };
        name = source.Sketch(options); Assert.Equal("(System.Collections.Generic.Dictionary<String, Int32>) {James = 50, Maria = 25}", name);

        // Default...
        options = SketchOptions.Default;
        name = source.Sketch(options); Assert.Equal("{James = 50, Maria = 25}", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Other_Collections()
    {
        var source = new List<string>() { "James", "Maria" };

        var options = SketchOptions.Empty;
        var name = source.Sketch(options); Assert.Equal("[James, Maria]", name);

        options = options with { NullStr = "NULL" };
        name = source.Sketch(options); Assert.Equal("[James, Maria]", name);

        options = options with { UseSourceType = EasyTypeOptions.Default };
        name = source.Sketch(options); Assert.Equal("(List<String>) [James, Maria]", name);

        options = options with { UseSourceType = options.UseSourceType with { UseNamespace = true } };
        name = source.Sketch(options); Assert.Equal("(System.Collections.Generic.List<String>) [James, Maria]", name);

        // Default...
        options = SketchOptions.Default;
        name = source.Sketch(options); Assert.Equal("[James, Maria]", name);
    }

    // ----------------------------------------------------

    public class A
    {
        public string Name = "James";
        readonly int Age = 50;
        readonly static string Org = "MI6";
        protected string GetString() => $"({Name}, {Age}, {Org})";
    }

    //[Enforced]
    [Fact]
    public static void Test_Shape()
    {
        var source = new A();

        var options = SketchOptions.Empty;
        var name = source.Sketch(options); Assert.Equal("A", name);

        options = options with { UseShape = true };
        name = source.Sketch(options); Assert.Equal("{ Name = James }", name);

        options = options with { UsePrivateMembers = true };
        name = source.Sketch(options); Assert.Equal("{ Name = James, Age = 50 }", name);

        options = options with { UseStaticMembers = true };
        name = source.Sketch(options); Assert.Equal("{ Name = James, Age = 50, Org = MI6 }", name);

        //Default...
        options = SketchOptions.Default;
        name = source.Sketch(options); Assert.Equal("{ Name = James }", name);
    }

    // ----------------------------------------------------

    public class B : A { public override string ToString() => $"B: {GetString()}"; }
    public class C : B { }

    //[Enforced]
    [Fact]
    public static void Test_Overriden_ToString()
    {
        var source = new B();

        var options = SketchOptions.Empty;
        var name = source.Sketch(options); Assert.Equal("B: (James, 50, MI6)", name);

        options = options with { UseSourceType = EasyTypeOptions.Default };
        name = source.Sketch(options); Assert.Equal("(B) B: (James, 50, MI6)", name);

        //Default...
        options = SketchOptions.Default;
        name = source.Sketch(options); Assert.Equal("B: (James, 50, MI6)", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Base_Overriden_ToString()
    {
        var source = new C();

        var options = SketchOptions.Empty;
        var name = source.Sketch(options); Assert.Equal("B: (James, 50, MI6)", name);

        options = options with { UseSourceType = EasyTypeOptions.Default };
        name = source.Sketch(options); Assert.Equal("(C) B: (James, 50, MI6)", name);

        //Default...
        options = SketchOptions.Default;
        name = source.Sketch(options); Assert.Equal("B: (James, 50, MI6)", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dynamics()
    {
        var source = new ExpandoObject(); // dynamics do not bind against extension methods!
        dynamic obj = source;
        obj.Name = "James";
        obj.Age = 50;

        var options = SketchOptions.Empty;
        var name = source.Sketch(options); Assert.Equal("{ [Name, James], [Age, 50] }", name);

        options = options with { UseSourceType = EasyTypeOptions.Default };
        name = source.Sketch(options); Assert.Equal("(ExpandoObject) { [Name, James], [Age, 50] }", name);

        // Default...
        options = SketchOptions.Default;
        name = source.Sketch(options); Assert.Equal("{ [Name, James], [Age, 50] }", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Anonymous()
    {
        var source = new { Name = "James", Age = 50 };

        var options = SketchOptions.Empty;
        var name = source.Sketch(options); Assert.Equal("{ Name = James, Age = 50 }", name);

        options = options with { UseSourceType = EasyTypeOptions.Default };
        name = source.Sketch(options);
        Assert.EndsWith("{ Name = James, Age = 50 }", name);
        Assert.Contains("AnonymousType", name);

        // Default...
        options = SketchOptions.Default;
        name = source.Sketch(options); Assert.Equal("{ Name = James, Age = 50 }", name);
    }
}
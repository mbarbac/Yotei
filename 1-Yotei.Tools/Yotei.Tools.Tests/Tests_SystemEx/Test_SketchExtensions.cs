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
        var options = SketchOptions.Default;
        string target;
        object? source = null;

        target = source.Sketch(options);
        Assert.Equal("NULL", target);

        options = options with { TypeOptions = options.TypeOptions with { UseName = true } };
        target = source.Sketch(options);
        Assert.Equal("(Object) NULL", target);

        options = options with { TypeOptions = options.TypeOptions with { UseNamespace = true } };
        target = source.Sketch(options);
        Assert.Equal("(System.Object) NULL", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Null_String()
    {
        var options = SketchOptions.Default;
        string target;
        string? source = null;

        target = source.Sketch(options);
        Assert.Equal("NULL", target);

        options = options with { TypeOptions = options.TypeOptions with { UseName = true } };
        target = source.Sketch(options);
        Assert.Equal("(String) NULL", target);

        options = options with { TypeOptions = options.TypeOptions with { UseNamespace = true } };
        target = source.Sketch(options);
        Assert.Equal("(System.String) NULL", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Null_Others()
    {
        var options = SketchOptions.Default;
        string target;
        int? source = null;

        target = source.Sketch(options);
        Assert.Equal("NULL", target);

        options = options with { TypeOptions = options.TypeOptions with { UseName = true } };
        target = source.Sketch(options);
        Assert.Equal("(Nullable) NULL", target);

        options = options with { TypeOptions = options.TypeOptions with { UseName = true, UseArgumentsNames = true } };
        target = source.Sketch(options);
        Assert.Equal("(Nullable<Int32>) NULL", target);

        options = options with { TypeOptions = options.TypeOptions with { UseNamespace = true } };
        target = source.Sketch(options);
        Assert.Equal("(System.Nullable<Int32>) NULL", target);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Regular_String()
    {
        var options = SketchOptions.Default;
        string target;
        var source = "Hello";

        target = source.Sketch(options);
        Assert.Equal("Hello", target);

        options = options with { TypeOptions = options.TypeOptions with { UseName = true } };
        target = source.Sketch(options);
        Assert.Equal("(String) Hello", target);

        options = options with { TypeOptions = options.TypeOptions with { UseNamespace = true } };
        target = source.Sketch(options);
        Assert.Equal("(System.String) Hello", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Guid()
    {
        var options = SketchOptions.Default;
        string target;
        var source = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);

        target = source.Sketch(options);
        Assert.Equal("00000001-0002-0003-0405-060708090a0b", target);

        options = options with { TypeOptions = options.TypeOptions with { UseName = true  } };
        target = source.Sketch(options);
        Assert.Equal("(Guid) 00000001-0002-0003-0405-060708090a0b", target);

        options = options with { TypeOptions = options.TypeOptions with { UseNamespace = true } };
        target = source.Sketch(options);
        Assert.Equal("(System.Guid) 00000001-0002-0003-0405-060708090a0b", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Int()
    {
        var options = SketchOptions.Default;
        string target;
        var source = 5;

        target = source.Sketch(options);
        Assert.Equal("5", target);

        options = options with { TypeOptions = options.TypeOptions with { UseName = true } };
        target = source.Sketch(options);
        Assert.Equal("(Int32) 5", target);

        options = options with { TypeOptions = options.TypeOptions with { UseNamespace = true } };
        target = source.Sketch(options);
        Assert.Equal("(System.Int32) 5", target);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Double()
    {
        var options = new SketchOptions { Provider = CultureInfo.InvariantCulture };
        string target;
        var source = 1234.567;

        target = source.Sketch(options);
        Assert.Equal("1234.567", target);

        options = options with { TypeOptions = options.TypeOptions with { UseName = true } };
        target = source.Sketch(options);
        Assert.Equal("(Double) 1234.567", target);

        options = options with { TypeOptions = options.TypeOptions with { UseNamespace = true } };
        target = source.Sketch(options);
        Assert.Equal("(System.Double) 1234.567", target);

        options = options with { Provider = CultureInfo.GetCultureInfo("es-es") };
        target = source.Sketch(options);
        Assert.Equal("(System.Double) 1234,567", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Decimal()
    {
        var options = new SketchOptions { Provider = CultureInfo.InvariantCulture };
        string target;
        var source = new decimal(7.5);

        target = source.Sketch(options);
        Assert.Equal("7.5", target);

        options = options with { TypeOptions = options.TypeOptions with { UseName = true  } };
        target = source.Sketch(options);
        Assert.Equal("(Decimal) 7.5", target);

        options = options with { TypeOptions = options.TypeOptions with { UseNamespace = true } };
        target = source.Sketch(options);
        Assert.Equal("(System.Decimal) 7.5", target);

        options = options with { Provider = CultureInfo.GetCultureInfo("es-es") };
        target = source.Sketch(options);
        Assert.Equal("(System.Decimal) 7,5", target);
    }

    // ----------------------------------------------------

    [Flags] public enum MyEnum { None = 0, One = 1, Two = 2, Three = 4 }

    //[Enforced]
    [Fact]
    public static void Test_Enum_Simple()
    {
        var options = SketchOptions.Default;
        string target;

        var source = MyEnum.None;
        target = source.Sketch(options);
        Assert.Equal("None", target);

        source = MyEnum.One;
        target = source.Sketch(options);
        Assert.Equal("One", target);

        options = options with { TypeOptions = options.TypeOptions with { UseName = true } };
        target = source.Sketch(options);
        Assert.Equal("MyEnum.One", target);

        options = options with { TypeOptions = options.TypeOptions with { UseHost = true } };
        target = source.Sketch(options);
        Assert.Equal($"{CLASSNAME}.MyEnum.One", target);

        options = options with { TypeOptions = options.TypeOptions with { UseNamespace = true } };
        target = source.Sketch(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.MyEnum.One", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Enum_Flags()
    {
        var options = SketchOptions.Default;
        string target;
        var source = MyEnum.One | MyEnum.Two | MyEnum.Three;

        target = source.Sketch(options);
        Assert.Equal("One | Two | Three", target);

        options = options with { TypeOptions = options.TypeOptions with { UseName = true } };
        target = source.Sketch(options);
        Assert.Equal("MyEnum.One | MyEnum.Two | MyEnum.Three", target);

        options = options with { TypeOptions = options.TypeOptions with { UseHost = true } };
        target = source.Sketch(options);
        Assert.Equal($"{CLASSNAME}.MyEnum.One | {CLASSNAME}.MyEnum.Two | {CLASSNAME}.MyEnum.Three", target);

        options = options with { TypeOptions = options.TypeOptions with { UseNamespace = true } };
        target = source.Sketch(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.MyEnum.One | {NAMESPACE}.{CLASSNAME}.MyEnum.Two | {NAMESPACE}.{CLASSNAME}.MyEnum.Three", target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Array_Empty()
    {
        var options = SketchOptions.Default;
        string target;
        var source = Array.Empty<char>();

        target = source.Sketch(options);
        Assert.Equal("[]", target);

        options = options with { TypeOptions = options.TypeOptions with { UseName = true } };
        target = source.Sketch(options);
        Assert.Equal("(Char[]) []", target);

        options = options with { TypeOptions = options.TypeOptions with { UseNamespace = true } };
        target = source.Sketch(options);
        Assert.Equal("(System.Char[]) []", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_Populated()
    {
        var options = SketchOptions.Default;
        string target;
        var source = new int[] { 1, 2, 3 };

        target = source.Sketch(options);
        Assert.Equal("[1, 2, 3]", target);

        options = options with { TypeOptions = options.TypeOptions with { UseName = true } };
        target = source.Sketch(options);
        Assert.Equal("(Int32[]) [1, 2, 3]", target);

        options = options with { TypeOptions = options.TypeOptions with { UseNamespace = true } };
        target = source.Sketch(options);
        Assert.Equal("(System.Int32[]) [1, 2, 3]", target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dictionary()
    {
        var options = SketchOptions.Default;
        string target;
        var source = new Dictionary<string, int>() { { "James", 50 }, { "Maria", 25 } };

        target = source.Sketch(options);
        Assert.Equal("{James = 50, Maria = 25}", target);

        options = options with { TypeOptions = options.TypeOptions with { UseName = true } };
        target = source.Sketch(options);
        Assert.Equal("(Dictionary) {James = 50, Maria = 25}", target);

        options = new SketchOptions { TypeOptions = options.TypeOptions with { UseArgumentsNames = true } };
        target = source.Sketch(options);
        Assert.Equal("(Dictionary<String, Int32>) {James = 50, Maria = 25}", target);

        options = options with { TypeOptions = options.TypeOptions with { UseNamespace = true } };
        target = source.Sketch(options);
        Assert.Equal("(System.Collections.Generic.Dictionary<String, Int32>) {James = 50, Maria = 25}", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Other_Collections()
    {
        var options = SketchOptions.Default;
        string target;
        var source = new List<string>() { "James", "Maria" };

        target = source.Sketch(options);
        Assert.Equal("[James, Maria]", target);

        options = options with { TypeOptions = options.TypeOptions with { UseName = true } };
        target = source.Sketch(options);
        Assert.Equal("(List) [James, Maria]", target);

        options = new SketchOptions { TypeOptions = options.TypeOptions with { UseArgumentsNames = true } };
        target = source.Sketch(options);
        Assert.Equal("(List<String>) [James, Maria]", target);

        options = options with { TypeOptions = options.TypeOptions with { UseNamespace = true } };
        target = source.Sketch(options);
        Assert.Equal("(System.Collections.Generic.List<String>) [James, Maria]", target);
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
        var options = SketchOptions.Default;
        string target;
        var source = new A();

        target = source.Sketch(options);
        Assert.Equal("{ Name='James' }", target);

        options = options with { TypeOptions = options.TypeOptions with { UseName = true } };
        target = source.Sketch(options);
        Assert.Equal("(A) { Name='James' }", target);

        options = options with { UsePrivateMembers = true };
        target = source.Sketch(options);
        Assert.Equal("(A) { Name='James', Age='50' }", target);

        options = options with { UseStaticMembers = true };
        target = source.Sketch(options);
        Assert.Equal("(A) { Name='James', Age='50', Org='MI6' }", target);
    }

    // ----------------------------------------------------

    public class B : A { public override string ToString() => $"B: {GetString()}"; }
    public class C : B { }

    //[Enforced]
    [Fact]
    public static void Test_Overriden_ToString()
    {
        var options = SketchOptions.Default;
        string target;

        var source = new A();
        options = options with { PreventShape = true, TypeOptions = options.TypeOptions with { UseName = true } };
        target = source.Sketch(options);
        Assert.Equal("A", target);

        source = new B();
        options = SketchOptions.Default;
        target = source.Sketch(options);
        Assert.Equal("B: (James, 50, MI6)", target);

        options = options with { TypeOptions = options.TypeOptions with { UseName = true } };
        target = source.Sketch(options);
        Assert.Equal("(B) B: (James, 50, MI6)", target);

        source = new C();
        options = SketchOptions.Default;
        target = source.Sketch(options);
        Assert.Equal("B: (James, 50, MI6)", target);

        options = options with { TypeOptions = options.TypeOptions with { UseName = true } };
        target = source.Sketch(options);
        Assert.Equal("(C) B: (James, 50, MI6)", target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dynamics()
    {
        var options = SketchOptions.Default;
        string target;

        var source = new ExpandoObject(); // dynamics do not bind against extension methods!
        dynamic obj = source;
        obj.Name = "James";
        obj.Age = 50;

        target = source.Sketch(options);
        Assert.Equal("{ [Name, James], [Age, 50] }", target);

        options = options with { TypeOptions = options.TypeOptions with { UseName = true } };
        target = source.Sketch(options);
        Assert.Equal("(ExpandoObject) { [Name, James], [Age, 50] }", target);
    }
}
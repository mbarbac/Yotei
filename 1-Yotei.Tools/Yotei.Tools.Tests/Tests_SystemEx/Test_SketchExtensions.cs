using static Yotei.Tools.Diagnostics.ConsoleWrapper;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_SketchExtensions
{
    static readonly string NAMESPACE = "Yotei.Tools.Tests";
    static readonly string CLASSNAME = "Test_SketchExtensions";

    //[Enforced]
    [Fact]
    public static void Test_Null_Object()
    {
        var options = SketchOptions.Default;
        object? source = null;

        string target = source.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("NULL", target);

        target = source.Sketch(options with { UseTypeName = true });
        WriteLine(true, target);
        Assert.Equal("(Object) NULL", target);

        target = source.Sketch(options with { UseTypeName = true, UseNameSpace = true });
        WriteLine(true, target);
        Assert.Equal("(System.Object) NULL", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Null_String()
    {
        var options = SketchOptions.Default;
        string? source = null;

        string target = source.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("NULL", target);

        target = source.Sketch(options with { UseTypeName = true });
        WriteLine(true, target);
        Assert.Equal("(String) NULL", target);

        target = source.Sketch(options with { UseTypeName = true, UseNameSpace = true });
        WriteLine(true, target);
        Assert.Equal("(System.String) NULL", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Null_Others()
    {
        var options = SketchOptions.Default;
        int? source = null;

        string target = source.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("NULL", target);

        target = source.Sketch(options with { UseTypeName = true });
        WriteLine(true, target);
        Assert.Equal("(Nullable<Int32>) NULL", target);

        target = source.Sketch(options with { UseTypeName = true, UseNameSpace = true });
        WriteLine(true, target);
        Assert.Equal("(System.Nullable<System.Int32>) NULL", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Regular_String()
    {
        var options = SketchOptions.Default;
        var source = "hello";

        string target = source.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("hello", target);

        target = source.Sketch(options with { UseTypeName = true });
        WriteLine(true, target);
        Assert.Equal("(String) hello", target);

        target = source.Sketch(options with { UseTypeName = true, UseNameSpace = true });
        WriteLine(true, target);
        Assert.Equal("(System.String) hello", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Guid()
    {
        var options = SketchOptions.Default;
        var source = new Guid(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11);

        string target = source.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("00000001-0002-0003-0405-060708090a0b", target);

        target = source.Sketch(options with { UseTypeName = true });
        WriteLine(true, target);
        Assert.Equal("(Guid) 00000001-0002-0003-0405-060708090a0b", target);

        target = source.Sketch(options with { UseTypeName = true, UseNameSpace = true });
        WriteLine(true, target);
        Assert.Equal("(System.Guid) 00000001-0002-0003-0405-060708090a0b", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Int()
    {
        var options = SketchOptions.Default;
        var source = 5;

        string target = source.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("5", target);

        target = source.Sketch(options with { UseTypeName = true });
        WriteLine(true, target);
        Assert.Equal("(Int32) 5", target);

        target = source.Sketch(options with { UseTypeName = true, UseNameSpace = true });
        WriteLine(true, target);
        Assert.Equal("(System.Int32) 5", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Double()
    {
        var options = SketchOptions.Default with { Provider = CultureInfo.InvariantCulture };
        var source = 1234.567;

        string target = source.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("1234.567", target);

        target = source.Sketch(options with { UseTypeName = true });
        WriteLine(true, target);
        Assert.Equal("(Double) 1234.567", target);

        target = source.Sketch(options with { UseTypeName = true, UseNameSpace = true });
        WriteLine(true, target);
        Assert.Equal("(System.Double) 1234.567", target);

        target = source.Sketch(options with { Provider = CultureInfo.GetCultureInfo("es-es") });
        WriteLine(true, target);
        Assert.Equal("1234,567", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Decimal()
    {
        var options = SketchOptions.Default with { Provider = CultureInfo.InvariantCulture };
        var source = new decimal(7.5);

        string target = source.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("7.5", target);

        target = source.Sketch(options with { UseTypeName = true });
        WriteLine(true, target);
        Assert.Equal("(Decimal) 7.5", target);

        target = source.Sketch(options with { UseTypeName = true, UseNameSpace = true });
        WriteLine(true, target);
        Assert.Equal("(System.Decimal) 7.5", target);

        target = source.Sketch(options with { Provider = CultureInfo.GetCultureInfo("es-es") });
        WriteLine(true, target);
        Assert.Equal("7,5", target);
    }

    // ----------------------------------------------------

    [Flags] public enum MyEnum { None = 0, One = 1, Two = 2, Three = 4 }

    //[Enforced]
    [Fact]
    public static void Test_Enum_Simple()
    {
        var options = SketchOptions.Default;
        var source = MyEnum.None;

        string target = source.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("None", target);

        source = MyEnum.One;

        target = source.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("One", target);

        target = source.Sketch(options with { UseTypeName = true });
        WriteLine(true, target);
        Assert.Equal("MyEnum.One", target);

        target = source.Sketch(options with { UseFullTypeName = true });
        WriteLine(true, target);
        Assert.Equal($"{CLASSNAME}.MyEnum.One", target);

        target = source.Sketch(options with { UseNameSpace = true });
        WriteLine(true, target);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.MyEnum.One", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Enum_Flags()
    {
        var options = SketchOptions.Default;
        var source = MyEnum.One | MyEnum.Two | MyEnum.Three;

        string target = source.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("One | Two | Three", target);

        target = source.Sketch(options with { UseTypeName = true });
        WriteLine(true, target);
        Assert.Equal("MyEnum.One | MyEnum.Two | MyEnum.Three", target);

        target = source.Sketch(options with { UseFullTypeName = true });
        WriteLine(true, target);
        Assert.Equal($"{CLASSNAME}.MyEnum.One | {CLASSNAME}.MyEnum.Two | {CLASSNAME}.MyEnum.Three", target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Array_Empty()
    {
        var options = SketchOptions.Default;
        var source = Array.Empty<char>();

        string target = source.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("[]", target);

        target = source.Sketch(options with { UseTypeName = true });
        WriteLine(true, target);
        Assert.Equal("(Char[]) []", target);

        target = source.Sketch(options with { UseNameSpace = true });
        WriteLine(true, target);
        Assert.Equal("(System.Char[]) []", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_Populated()
    {
        var options = SketchOptions.Default;
        var source = new int[] { 1, 2, 3 };

        string target = source.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("[1, 2, 3]", target);

        target = source.Sketch(options with { UseTypeName = true });
        WriteLine(true, target);
        Assert.Equal("(Int32[]) [1, 2, 3]", target);

        target = source.Sketch(options with { UseNameSpace = true });
        WriteLine(true, target);
        Assert.Equal("(System.Int32[]) [1, 2, 3]", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Dictionary()
    {
        var options = SketchOptions.Default;
        var source = new Dictionary<string, int>() { { "James", 50 }, { "Maria", 25 } };

        string target = source.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("{James = 50, Maria = 25}", target);

        target = source.Sketch(options with { UseTypeName = true });
        WriteLine(true, target);
        Assert.Equal("(Dictionary<String, Int32>) {James = 50, Maria = 25}", target);

        target = source.Sketch(options with { UseNameSpace = true });
        WriteLine(true, target);
        Assert.Equal("(System.Collections.Generic.Dictionary<System.String, System.Int32>) {James = 50, Maria = 25}", target);
    }

    //[Enforced]
    [Fact]
    public static void Test_Other_Collections()
    {
        var options = SketchOptions.Default;
        var source = new List<string>() { "James", "Maria" };

        string target = source.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("[James, Maria]", target);

        target = source.Sketch(options with { UseTypeName = true });
        WriteLine(true, target);
        Assert.Equal("(List<String>) [James, Maria]", target);

        target = source.Sketch(options with { UseNameSpace = true });
        WriteLine(true, target);
        Assert.Equal("(System.Collections.Generic.List<System.String>) [James, Maria]", target);
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
        var source = new A();

        string target = source.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("{ Name = James }", target);

        target = source.Sketch(options with { UseTypeName = true });
        WriteLine(true, target);
        Assert.Equal("(A) { Name = (String) James }", target);

        target = source.Sketch(options with { UseFullTypeName = true });
        WriteLine(true, target);
        Assert.Equal($"({CLASSNAME}.A) {{ Name = (String) James }}", target);

        target = source.Sketch(options with { UsePrivateMembers = true });
        WriteLine(true, target);
        Assert.Equal("{ Name = James, Age = 50 }", target);

        target = source.Sketch(options with { UsePrivateMembers = true, UseStaticMembers = true });
        WriteLine(true, target);
        Assert.Equal("{ Name = James, Age = 50, Org = MI6 }", target);
    }

    // ----------------------------------------------------

    public class B : A { public override string ToString() => $"B: {GetString()}"; }

    public class C : B { }

    //[Enforced]
    [Fact]
    public static void Test_Overriden_ToString()
    {
        var options = SketchOptions.Default with { PreventShape = true };
        var source = new A();

        string target = source.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("A", target);

        source = new B();

        target = source.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("B: (James, 50, MI6)", target);

        target = source.Sketch(options with { UseTypeName = true });
        WriteLine(true, target);
        Assert.Equal("(B) B: (James, 50, MI6)", target);

        source = new C();

        target = source.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("B: (James, 50, MI6)", target);

        target = source.Sketch(options with { UseTypeName = true });
        WriteLine(true, target);
        Assert.Equal("(C) B: (James, 50, MI6)", target);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Dynamics()
    {
        var options = SketchOptions.Default;
        var obj = new ExpandoObject();
        dynamic source = obj;
        source.Name = "James";
        source.Age = 50;

        string target = obj.Sketch(options);
        WriteLine(true, target);
        Assert.Equal("{ [Name, James], [Age, 50] }", target);

        target = obj.Sketch(options with { UseTypeName = true });
        WriteLine(true, target);
        Assert.Equal("(ExpandoObject) { [Name, James], [Age, 50] }", target);

        target = obj.Sketch(options with { UseNameSpace = true });
        WriteLine(true, target);
        Assert.Equal("(System.Dynamic.ExpandoObject) { [Name, James], [Age, 50] }", target);
    }
}
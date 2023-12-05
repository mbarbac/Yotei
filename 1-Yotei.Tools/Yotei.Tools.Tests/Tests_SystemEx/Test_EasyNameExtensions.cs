using static Yotei.Tools.Diagnostics.ConsoleWrapper;

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_EasyNameExtensions
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = "Test_EasyNameExtensions";

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_System_Types()
    {
        var options = new EasyNameOptions();
        var type = typeof(string);
        string str;

        str = type.EasyName();
        WriteLine(true, str);
        Assert.Equal("String", str);

        str = type.EasyName(options with { UseFullTypeName = true });
        WriteLine(true, str);
        Assert.Equal("String", str);

        str = type.EasyName(options with { UseNameSpace = true });
        WriteLine(true, str);
        Assert.Equal("System.String", str);
    }

    // ----------------------------------------------------
    public class TypeA { public class TypeB { } }

    //[Enforced]
    [Fact]
    public static void Test_Simple_Type()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeA);
        string str;

        str = type.EasyName();
        WriteLine(true, str);
        Assert.Equal("TypeA", str);

        str = type.EasyName(options with { UseFullTypeName = true });
        WriteLine(true, str);
        Assert.Equal($"{CLASSNAME}.TypeA", str);

        str = type.EasyName(options with { UseNameSpace = true });
        WriteLine(true, str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeA", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Type()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeA.TypeB);
        string str;

        str = type.EasyName();
        WriteLine(true, str);
        Assert.Equal("TypeB", str);

        str = type.EasyName(options with { UseFullTypeName = true });
        WriteLine(true, str);
        Assert.Equal($"{CLASSNAME}.TypeA.TypeB", str);

        str = type.EasyName(options with { UseNameSpace = true });
        WriteLine(true, str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeA.TypeB", str);
    }

    // ----------------------------------------------------

    public class TypeC<T> { }

    //[Enforced]
    [Fact]
    public static void Test_Simple_Generics()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeC<>);
        string str;

        str = type.EasyName();
        WriteLine(true, str);
        Assert.Equal("TypeC<T>", str);

        str = type.EasyName(options with { UseFullTypeName = true });
        WriteLine(true, str);
        Assert.Equal($"{CLASSNAME}.TypeC<T>", str);

        str = type.EasyName(options with { UseNameSpace = true });
        WriteLine(true, str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeC<T>", str);

        str = type.EasyName(options with { UseFullTypeName = true, PreventGenericTypeNames = true });
        WriteLine(true, str);
        Assert.Equal($"{CLASSNAME}.TypeC<>", str);

        str = type.EasyName(options with { UseNameSpace = true, PreventGenericTypeNames = true });
        WriteLine(true, str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeC<>", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_Simple_Generics_Closed()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeC<string>);
        string str;

        str = type.EasyName();
        WriteLine(true, str);
        Assert.Equal("TypeC<String>", str);

        str = type.EasyName(options with { UseFullTypeName = true });
        WriteLine(true, str);
        Assert.Equal($"{CLASSNAME}.TypeC<String>", str);

        str = type.EasyName(options with { UseNameSpace = true });
        WriteLine(true, str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeC<System.String>", str);

        str = type.EasyName(options with { UseFullTypeName = true, PreventGenericTypeNames = true });
        WriteLine(true, str);
        Assert.Equal($"{CLASSNAME}.TypeC<String>", str);

        str = type.EasyName(options with { UseNameSpace = true, PreventGenericTypeNames = true });
        WriteLine(true, str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeC<System.String>", str);
    }

    // ----------------------------------------------------

    public class TypeD<K, T> { public class TypeE<S> { } }

    //[Enforced]
    [Fact]
    public static void Test_Multiple_Generics()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeD<,>);
        string str;

        str = type.EasyName();
        WriteLine(true, str);
        Assert.Equal("TypeD<K, T>", str);

        str = type.EasyName(options with { UseFullTypeName = true });
        WriteLine(true, str);
        Assert.Equal($"{CLASSNAME}.TypeD<K, T>", str);

        str = type.EasyName(options with { UseNameSpace = true });
        WriteLine(true, str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeD<K, T>", str);

        str = type.EasyName(options with { UseFullTypeName = true, PreventGenericTypeNames = true });
        WriteLine(true, str);
        Assert.Equal($"{CLASSNAME}.TypeD<,>", str);

        str = type.EasyName(options with { UseNameSpace = true, PreventGenericTypeNames = true });
        WriteLine(true, str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeD<,>", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_Multiple_Generics_Closed()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeD<int, string>);
        string str;

        str = type.EasyName();
        WriteLine(true, str);
        Assert.Equal("TypeD<Int32, String>", str);

        str = type.EasyName(options with { UseFullTypeName = true });
        WriteLine(true, str);
        Assert.Equal($"{CLASSNAME}.TypeD<Int32, String>", str);

        str = type.EasyName(options with { UseNameSpace = true });
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeD<System.Int32, System.String>", str);
        WriteLine(true, str);

        str = type.EasyName(options with { UseFullTypeName = true, PreventGenericTypeNames = true });
        WriteLine(true, str);
        Assert.Equal($"{CLASSNAME}.TypeD<Int32, String>", str);

        str = type.EasyName(options with { UseNameSpace = true, PreventGenericTypeNames = true });
        WriteLine(true, str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeD<System.Int32, System.String>", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_Generics_Nested()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeD<,>.TypeE<>);
        string str;

        str = type.EasyName();
        WriteLine(true, str);
        Assert.Equal("TypeE<S>", str);

        str = type.EasyName(options with { UseFullTypeName = true });
        WriteLine(true, str);
        Assert.Equal($"{CLASSNAME}.TypeD<K, T>.TypeE<S>", str);

        str = type.EasyName(options with { UseNameSpace = true });
        WriteLine(true, str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeD<K, T>.TypeE<S>", str);

        str = type.EasyName(options with { UseFullTypeName = true, PreventGenericTypeNames = true });
        WriteLine(true, str);
        Assert.Equal($"{CLASSNAME}.TypeD<,>.TypeE<>", str);

        str = type.EasyName(options with { UseNameSpace = true, PreventGenericTypeNames = true });
        WriteLine(true, str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeD<,>.TypeE<>", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_Generics_Nested_Closed()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeD<int, string>.TypeE<DateTime>);
        string str;

        str = type.EasyName();
        WriteLine(true, str);
        Assert.Equal("TypeE<DateTime>", str);

        str = type.EasyName(options with { UseFullTypeName = true });
        WriteLine(true, str);
        Assert.Equal($"{CLASSNAME}.TypeD<Int32, String>.TypeE<DateTime>", str);

        str = type.EasyName(options with { UseNameSpace = true });
        WriteLine(true, str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeD<System.Int32, System.String>.TypeE<System.DateTime>", str);

        str = type.EasyName(options with { UseFullTypeName = true, PreventGenericTypeNames = true });
        WriteLine(true, str);
        Assert.Equal($"{CLASSNAME}.TypeD<Int32, String>.TypeE<DateTime>", str);

        str = type.EasyName(options with { UseNameSpace = true, PreventGenericTypeNames = true });
        WriteLine(true, str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeD<System.Int32, System.String>.TypeE<System.DateTime>", str);
    }

    // ----------------------------------------------------

    public class TypeF { }

    //[Enforced]
    [Fact]
    public static void Test_Implicit_Constructor()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeF);
        string str;
        var info = type.GetConstructor(Type.EmptyTypes)!;

        str = info.EasyName(options);
        WriteLine(true, str);
        Assert.Equal(".ctor()", str);

        str = info.EasyName(options with { UseTypeName = true });
        WriteLine(true, str);
        Assert.Equal("TypeF..ctor()", str);
    }

    // ----------------------------------------------------

#pragma warning disable CS9113 // Parameter is unread.
    public class TypeG<T>(T one, string two) { }
#pragma warning restore CS9113 // Parameter is unread.

    //[Enforced]
    [Fact]
    public static void Test_Explicit_Constructors()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeG<>);
        string str;
        var infos = type.GetConstructors();

        str = infos[0].EasyName(options);
        WriteLine(true, str);
        Assert.Equal(".ctor(T one, String two)", str);

        str = infos[0].EasyName(options with { UseTypeName = true });
        WriteLine(true, str);
        Assert.Equal("TypeG<T>..ctor(T one, String two)", str);

        str = infos[0].EasyName(options with { UseTypeName = true, UseFullTypeName = true });
        WriteLine(true, str);
        Assert.Equal($"{CLASSNAME}.TypeG<T>..ctor(T one, String two)", str);
    }

    // ----------------------------------------------------

    public class TypeH<K, T>
    {
        public void MethodA() => throw null!;
        public void MethodB(K key, DateTime date) => throw null!;
        public T MethodC<S, R>(S source, K key, DateTime date) => throw null!;
    }

    //[Enforced]
    [Fact]
    public static void Test_Methods_Parameterless_Return_Void()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeH<,>);
        string str;
        var info = type.GetMethod("MethodA")!;

        str = info.EasyName(options);
        WriteLine(true, str);
        Assert.Equal("Void MethodA()", str);

        str = info.EasyName(options with { UseTypeName = true });
        WriteLine(true, str);
        Assert.Equal("Void TypeH<K, T>.MethodA()", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_Methods_With_Parameters_Return_Void()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeH<,>);
        string str;
        var info = type.GetMethod("MethodB")!;

        str = info.EasyName(options);
        WriteLine(true, str);
        Assert.Equal("Void MethodB(K key, DateTime date)", str);

        str = info.EasyName(options with { UseTypeName = true });
        WriteLine(true, str);
        Assert.Equal("Void TypeH<K, T>.MethodB(K key, DateTime date)", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_Methods_With_Return_And_Generics()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeH<,>);
        string str;
        var info = type.GetMethod("MethodC")!;

        str = info.EasyName(options);
        WriteLine(true, str);
        Assert.Equal("T MethodC<S, R>(S source, K key, DateTime date)", str);

        str = info.EasyName(options with { UseTypeName = true });
        WriteLine(true, str);
        Assert.Equal("T TypeH<K, T>.MethodC<S, R>(S source, K key, DateTime date)", str);
    }

    // ----------------------------------------------------

    public class TypeJ<K, T>
    {
        public int Age => throw null!;

        [IndexerName("MyItem")]
        public T this[K key] => throw null!;

        public K? Other;
    }

    //[Enforced]
    [Fact]
    public static void Test_Regular_Property()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeJ<,>);
        PropertyInfo info;
        string str;

        info = type.GetProperty("Age")!;

        str = info.EasyName(options);
        WriteLine(true, str);
        Assert.Equal("Int32 Age", str);

        str = info.EasyName(options with { UseTypeName = true });
        WriteLine(true, str);
        Assert.Equal("Int32 TypeJ<K, T>.Age", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_Indexer_Property()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeJ<,>);
        PropertyInfo info;
        string str;

        info = type.GetProperty("MyItem")!;

        str = info.EasyName(options);
        WriteLine(true, str);
        Assert.Equal("T this[K key]", str);

        str = info.EasyName(options with { UseTypeName = true });
        WriteLine(true, str);
        Assert.Equal("T TypeJ<K, T>.this[K key]", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_Field()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeJ<,>);
        FieldInfo info;
        string str;

        info = type.GetField("Other")!;

        str = info.EasyName(options);
        WriteLine(true, str);
        Assert.Equal("K Other", str);

        str = info.EasyName(options with { UseTypeName = true });
        WriteLine(true, str);
        Assert.Equal("K TypeJ<K, T>.Other", str);
    }
}
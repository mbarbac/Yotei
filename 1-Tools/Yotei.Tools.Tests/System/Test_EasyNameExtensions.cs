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
        Console.WriteLine(str);
        Assert.Equal("String", str);

        str = type.EasyName(options with { UseFullTypeName = true });
        Console.WriteLine(str);
        Assert.Equal("String", str);

        str = type.EasyName(options with { UseNameSpace = true });
        Console.WriteLine(str);
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
        Console.WriteLine(str);
        Assert.Equal("TypeA", str);

        str = type.EasyName(options with { UseFullTypeName = true });
        Console.WriteLine(str);
        Assert.Equal($"{CLASSNAME}.TypeA", str);

        str = type.EasyName(options with { UseNameSpace = true });
        Console.WriteLine(str);
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
        Console.WriteLine(str);
        Assert.Equal("TypeB", str);

        str = type.EasyName(options with { UseFullTypeName = true });
        Console.WriteLine(str);
        Assert.Equal($"{CLASSNAME}.TypeA.TypeB", str);

        str = type.EasyName(options with { UseNameSpace = true });
        Console.WriteLine(str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeA.TypeB", str);
    }

    // ----------------------------------------------------

    public class TypeC<T> { }
    public class TypeC<K, T> { public class TypeD<S> { } }

    //[Enforced]
    [Fact]
    public static void Test_Simple_Generics()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeC<>);
        string str;

        str = type.EasyName();
        Console.WriteLine(str);
        Assert.Equal("TypeC<T>", str);

        str = type.EasyName(options with { UseFullTypeName = true });
        Console.WriteLine(str);
        Assert.Equal($"{CLASSNAME}.TypeC<T>", str);

        str = type.EasyName(options with { UseNameSpace = true });
        Console.WriteLine(str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeC<T>", str);

        str = type.EasyName(options with { UseFullTypeName = true, PreventGenericTypeNames = true });
        Console.WriteLine(str);
        Assert.Equal($"{CLASSNAME}.TypeC<>", str);

        str = type.EasyName(options with { UseNameSpace = true, PreventGenericTypeNames = true });
        Console.WriteLine(str);
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
        Console.WriteLine(str);
        Assert.Equal("TypeC<String>", str);

        str = type.EasyName(options with { UseFullTypeName = true });
        Console.WriteLine(str);
        Assert.Equal($"{CLASSNAME}.TypeC<String>", str);

        str = type.EasyName(options with { UseNameSpace = true });
        Console.WriteLine(str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeC<System.String>", str);

        str = type.EasyName(options with { UseFullTypeName = true, PreventGenericTypeNames = true });
        Console.WriteLine(str);
        Assert.Equal($"{CLASSNAME}.TypeC<String>", str);

        str = type.EasyName(options with { UseNameSpace = true, PreventGenericTypeNames = true });
        Console.WriteLine(str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeC<System.String>", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_Multiple_Generics()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeC<,>);
        string str;

        str = type.EasyName();
        Console.WriteLine(str);
        Assert.Equal("TypeC<K, T>", str);

        str = type.EasyName(options with { UseFullTypeName = true });
        Console.WriteLine(str);
        Assert.Equal($"{CLASSNAME}.TypeC<K, T>", str);

        str = type.EasyName(options with { UseNameSpace = true });
        Console.WriteLine(str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeC<K, T>", str);

        str = type.EasyName(options with { UseFullTypeName = true, PreventGenericTypeNames = true });
        Console.WriteLine(str);
        Assert.Equal($"{CLASSNAME}.TypeC<,>", str);

        str = type.EasyName(options with { UseNameSpace = true, PreventGenericTypeNames = true });
        Console.WriteLine(str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeC<,>", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_Multiple_Generics_Closed()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeC<int, string>);
        string str;

        str = type.EasyName();
        Console.WriteLine(str);
        Assert.Equal("TypeC<Int32, String>", str);

        str = type.EasyName(options with { UseFullTypeName = true });
        Console.WriteLine(str);
        Assert.Equal($"{CLASSNAME}.TypeC<Int32, String>", str);

        str = type.EasyName(options with { UseNameSpace = true });
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeC<System.Int32, System.String>", str);
        Console.WriteLine(str);

        str = type.EasyName(options with { UseFullTypeName = true, PreventGenericTypeNames = true });
        Console.WriteLine(str);
        Assert.Equal($"{CLASSNAME}.TypeC<Int32, String>", str);

        str = type.EasyName(options with { UseNameSpace = true, PreventGenericTypeNames = true });
        Console.WriteLine(str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeC<System.Int32, System.String>", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_Generics_Nested()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeC<,>.TypeD<>);
        string str;

        str = type.EasyName();
        Console.WriteLine(str);
        Assert.Equal("TypeD<S>", str);

        str = type.EasyName(options with { UseFullTypeName = true });
        Console.WriteLine(str);
        Assert.Equal($"{CLASSNAME}.TypeC<K, T>.TypeD<S>", str);

        str = type.EasyName(options with { UseNameSpace = true });
        Console.WriteLine(str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeC<K, T>.TypeD<S>", str);

        str = type.EasyName(options with { UseFullTypeName = true, PreventGenericTypeNames = true });
        Console.WriteLine(str);
        Assert.Equal($"{CLASSNAME}.TypeC<,>.TypeD<>", str);

        str = type.EasyName(options with { UseNameSpace = true, PreventGenericTypeNames = true });
        Console.WriteLine(str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeC<,>.TypeD<>", str);
    }

    //[Enforced]
    [Fact]
    public static void Test_Generics_Nested_Closed()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeC<int, string>.TypeD<DateTime>);
        string str;

        str = type.EasyName();
        Console.WriteLine(str);
        Assert.Equal("TypeD<DateTime>", str);

        str = type.EasyName(options with { UseFullTypeName = true });
        Console.WriteLine(str);
        Assert.Equal($"{CLASSNAME}.TypeC<Int32, String>.TypeD<DateTime>", str);

        str = type.EasyName(options with { UseNameSpace = true });
        Console.WriteLine(str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeC<System.Int32, System.String>.TypeD<System.DateTime>", str);

        str = type.EasyName(options with { UseFullTypeName = true, PreventGenericTypeNames = true });
        Console.WriteLine(str);
        Assert.Equal($"{CLASSNAME}.TypeC<Int32, String>.TypeD<DateTime>", str);

        str = type.EasyName(options with { UseNameSpace = true, PreventGenericTypeNames = true });
        Console.WriteLine(str);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TypeC<System.Int32, System.String>.TypeD<System.DateTime>", str);
    }

    // ----------------------------------------------------

    public class TypeE<T>
    {
        public TypeE() { }
        public TypeE(T one, string two) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Constructors()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeE<>);
        string str;
        var infos = type.GetConstructors();

        str = infos[0].EasyName(options);
        Console.WriteLine(str);
        Assert.Equal("TypeE<T>.ctor()", str);

        str = infos[1].EasyName(options);
        Console.WriteLine(str);
        Assert.Equal("TypeE<T>.ctor(T one, String two)", str);
    }

    // ----------------------------------------------------

    public class TypeF<K, T>
    {
        public void MethodA(K key, DateTime date) => throw null!;
        public T MethodB<S>(S source, K key, DateTime date) => throw null!;
    }

    //[Enforced]
    [Fact]
    public static void Test_Methods()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeF<,>);
        string str;
        MethodInfo info;

        info = type.GetMethod("MethodA")!;
        str = info.EasyName(options);
        Console.WriteLine(str);
        Assert.Equal("Void MethodA(K key, DateTime date)", str);

        str = info.EasyName(options with { UseTypeName = true });
        Console.WriteLine(str);
        Assert.Equal("Void TypeF<K, T>.MethodA(K key, DateTime date)", str);

        str = info.EasyName(options with { UseFullTypeName = true });
        Console.WriteLine(str);
        Assert.Equal($"Void {CLASSNAME}.TypeF<K, T>.MethodA(K key, DateTime date)", str);

        str = info.EasyName(options with { UseNameSpace = true });
        Console.WriteLine(str);
        Assert.Equal($"System.Void {NAMESPACE}.{CLASSNAME}.TypeF<K, T>.MethodA(K key, System.DateTime date)", str);

        info = type.GetMethod("MethodB")!;
        str = info.EasyName(options);
        Console.WriteLine(str);
        Assert.Equal("T MethodB<S>(S source, K key, DateTime date)", str);
    }

    // ----------------------------------------------------

    public class TypeG<K, T>
    {
        public int Age => throw null!;
        public T this[K key] => throw null!;
    }

    //[Enforced]
    [Fact]
    public static void Test_Properties()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeG<,>);
        string str;
        var infos = type.GetProperties();

        str = infos[0].EasyName(options);
        Console.WriteLine(str);
        Assert.Equal("Int32 Age", str);

        str = infos[1].EasyName(options);
        Console.WriteLine(str);
        Assert.Equal("T this[K key]", str);
    }

    public class TypeH<K, T>
    {
        [IndexerName("MyItem")] public T this[K key, DateTime date] => throw null!;
    }

    //[Enforced]
    [Fact]
    public static void Test_Properties_With_Indexer_Name()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeH<,>);
        string str;

        var info = type.GetProperties()[0];
        str = info.EasyName(options);
        Console.WriteLine(str);
        Assert.Equal("T MyItem[K key, DateTime date]", str);
    }

    // ----------------------------------------------------

    public class TypeI<K, T>
    {
        public K? Age;
    }

    //[Enforced]
    [Fact]
    public static void Test_Field()
    {
        var options = new EasyNameOptions();
        var type = typeof(TypeI<,>);
        string str;

        var info = type.GetFields()[0];
        str = info.EasyName(options);
        Console.WriteLine(str);
        Assert.Equal("K Age", str);
    }
}
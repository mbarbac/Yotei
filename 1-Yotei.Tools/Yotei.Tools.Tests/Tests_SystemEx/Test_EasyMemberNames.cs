#pragma warning disable CS9113

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_EasyMemberNames
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = nameof(Test_EasyMemberNames);

    // -----------------------------------------------------

    public class TA<T, K>
    {
        public T MTKS<S>(K one, S two) => throw new NotImplementedException();
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Unbounded()
    {
        var type = typeof(TA<,>);
        MethodInfo item = type.GetMethod("MTKS")!;
        
        EasyMemberOptions options = EasyMemberOptions.Empty;
        string name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = item.EasyName(options); Assert.Equal("MTKS", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("MTKS(,)", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("MTKS(K, S)", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("MTKS(K one, S two)", name);

        options = options with { UseTypeArguments = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("MTKS<S>(K one, S two)", name);

        options = options with { UseHostType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TA<T, K>.MTKS<S>(K one, S two)", name);

        options = options with { UseReturnType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T TA<T, K>.MTKS<S>(K one, S two)", name);

        options = EasyMemberOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"T {NAMESPACE}.{CLASSNAME}.TA<T, K>.MTKS<S>(K one, S two)", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Bounded()
    {
        var type = typeof(TA<byte, int>);
        MethodInfo item = type.GetMethod("MTKS")!;
        item = item.MakeGenericMethod(typeof(long));

        EasyMemberOptions options = EasyMemberOptions.Empty;
        string name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = item.EasyName(options); Assert.Equal("MTKS", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("MTKS(,)", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("MTKS(Int32, Int64)", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("MTKS(Int32 one, Int64 two)", name);

        options = options with { UseTypeArguments = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("MTKS<Int64>(Int32 one, Int64 two)", name);

        options = options with { UseHostType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TA<Byte, Int32>.MTKS<Int64>(Int32 one, Int64 two)", name);

        options = options with { UseReturnType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("Byte TA<Byte, Int32>.MTKS<Int64>(Int32 one, Int64 two)", name);

        options = EasyMemberOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"System.Byte {NAMESPACE}.{CLASSNAME}.TA<System.Byte, System.Int32>.MTKS<System.Int64>(System.Int32 one, System.Int64 two)", name);
    }
    
    // -----------------------------------------------------

    public class TB<T> { }

    //[Enforced]
    [Fact]
    public static void Test_Implicit_Constructor_Unbounded()
    {
        var type = typeof(TB<>);
        ConstructorInfo item = type.GetConstructor(Type.EmptyTypes)!;

        EasyMemberOptions options = EasyMemberOptions.Empty;
        string name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = item.EasyName(options); Assert.Equal(".ctor", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal(".ctor()", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal(".ctor()", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal(".ctor()", name);

        options = options with { UseTypeArguments = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal(".ctor()", name);

        options = options with { UseHostType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TB<T>..ctor()", name);

        options = options with { UseReturnType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TB<T> TB<T>..ctor()", name);

        options = EasyMemberOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TB<T> {NAMESPACE}.{CLASSNAME}.TB<T>..ctor()", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Implicit_Constructor_Bounded()
    {
        var type = typeof(TB<byte>);
        ConstructorInfo item = type.GetConstructor(Type.EmptyTypes)!;

        EasyMemberOptions options = EasyMemberOptions.Empty;
        string name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = item.EasyName(options); Assert.Equal(".ctor", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal(".ctor()", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal(".ctor()", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal(".ctor()", name);

        options = options with { UseTypeArguments = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal(".ctor()", name);

        options = options with { UseHostType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TB<Byte>..ctor()", name);

        options = options with { UseReturnType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TB<Byte> TB<Byte>..ctor()", name);

        options = EasyMemberOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TB<System.Byte> {NAMESPACE}.{CLASSNAME}.TB<System.Byte>..ctor()", name);
    }

    // -----------------------------------------------------

    public class TC<T, K>(T one, K two) { }

    //[Enforced]
    [Fact]
    public static void Test_Explicit_Constructor_Unbounded()
    {
        var type = typeof(TC<,>);
        ConstructorInfo item = type.GetConstructors()[0];

        EasyMemberOptions options = EasyMemberOptions.Empty;
        string name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = item.EasyName(options); Assert.Equal(".ctor", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal(".ctor(,)", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal(".ctor(T, K)", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal(".ctor(T one, K two)", name);

        options = options with { UseTypeArguments = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal(".ctor(T one, K two)", name);

        options = options with { UseHostType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TC<T, K>..ctor(T one, K two)", name);

        options = options with { UseReturnType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TC<T, K> TC<T, K>..ctor(T one, K two)", name);

        options = EasyMemberOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TC<T, K> {NAMESPACE}.{CLASSNAME}.TC<T, K>..ctor(T one, K two)", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Explicit_Constructor_Bounded()
    {
        var type = typeof(TC<byte, int>);
        ConstructorInfo item = type.GetConstructors()[0];

        EasyMemberOptions options = EasyMemberOptions.Empty;
        string name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = item.EasyName(options); Assert.Equal(".ctor", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal(".ctor(,)", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal(".ctor(Byte, Int32)", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal(".ctor(Byte one, Int32 two)", name);

        options = options with { UseTypeArguments = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal(".ctor(Byte one, Int32 two)", name);

        options = options with { UseHostType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TC<Byte, Int32>..ctor(Byte one, Int32 two)", name);

        options = options with { UseReturnType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TC<Byte, Int32> TC<Byte, Int32>..ctor(Byte one, Int32 two)", name);

        options = EasyMemberOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TC<System.Byte, System.Int32> {NAMESPACE}.{CLASSNAME}.TC<System.Byte, System.Int32>..ctor(System.Byte one, System.Int32 two)", name);
    }

    // -----------------------------------------------------

    public class TD<T, K>
    {
        public T this[K one] => throw new NotImplementedException();
        public T PT => throw new NotImplementedException();
    }

    //[Enforced]
    [Fact]
    public static void Test_Indexer_Unbounded()
    {
        var type = typeof(TD<,>);
        PropertyInfo item = type.GetProperty("Item")!;

        EasyMemberOptions options = EasyMemberOptions.Empty;
        string name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = item.EasyName(options); Assert.Equal("this", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("this[]", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("this[K]", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("this[K one]", name);

        options = options with { UseHostType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TD<T, K>.this[K one]", name);

        options = options with { UseReturnType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T TD<T, K>.this[K one]", name);

        options = EasyMemberOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"T {NAMESPACE}.{CLASSNAME}.TD<T, K>.this[K one]", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Indexer_Bounded()
    {
        var type = typeof(TD<byte, int>);
        PropertyInfo item = type.GetProperty("Item")!;

        EasyMemberOptions options = EasyMemberOptions.Empty;
        string name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = item.EasyName(options); Assert.Equal("this", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("this[]", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("this[Int32]", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("this[Int32 one]", name);

        options = options with { UseHostType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TD<Byte, Int32>.this[Int32 one]", name);

        options = options with { UseReturnType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("Byte TD<Byte, Int32>.this[Int32 one]", name);

        options = EasyMemberOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"System.Byte {NAMESPACE}.{CLASSNAME}.TD<System.Byte, System.Int32>.this[System.Int32 one]", name);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Property_Unbounded()
    {
        var type = typeof(TD<,>);
        PropertyInfo item = type.GetProperty("PT")!;

        EasyMemberOptions options = EasyMemberOptions.Empty;
        string name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = item.EasyName(options); Assert.Equal("PT", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("PT", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("PT", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("PT", name);

        options = options with { UseHostType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TD<T, K>.PT", name);

        options = options with { UseReturnType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T TD<T, K>.PT", name);

        options = EasyMemberOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"T {NAMESPACE}.{CLASSNAME}.TD<T, K>.PT", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Property_Bounded()
    {
        var type = typeof(TD<byte, int>);
        PropertyInfo item = type.GetProperty("PT")!;

        EasyMemberOptions options = EasyMemberOptions.Empty;
        string name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = item.EasyName(options); Assert.Equal("PT", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("PT", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("PT", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("PT", name);

        options = options with { UseHostType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TD<Byte, Int32>.PT", name);

        options = options with { UseReturnType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("Byte TD<Byte, Int32>.PT", name);

        options = EasyMemberOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"System.Byte {NAMESPACE}.{CLASSNAME}.TD<System.Byte, System.Int32>.PT", name);
    }

    // -----------------------------------------------------

    public class TE<T, K>
    {
        [IndexerName("MyItem")]
        public T this[K one] => throw new NotImplementedException();
    }

    //[Enforced]
    [Fact]
    public static void Test_Custom_Indexer_Unbounded()
    {
        var type = typeof(TE<,>);
        PropertyInfo item = type.GetProperty("MyItem")!;

        EasyMemberOptions options = EasyMemberOptions.Empty;
        string name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = item.EasyName(options); Assert.Equal("this", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("this[]", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("this[K]", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("this[K one]", name);

        options = options with { UseHostType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TE<T, K>.this[K one]", name);

        options = options with { UseReturnType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T TE<T, K>.this[K one]", name);

        options = EasyMemberOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"T {NAMESPACE}.{CLASSNAME}.TE<T, K>.this[K one]", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Custom_Indexer_Bounded()
    {
        var type = typeof(TE<byte, int>);
        PropertyInfo item = type.GetProperty("MyItem")!;

        EasyMemberOptions options = EasyMemberOptions.Empty;
        string name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = item.EasyName(options); Assert.Equal("this", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("this[]", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("this[Int32]", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("this[Int32 one]", name);

        options = options with { UseHostType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TE<Byte, Int32>.this[Int32 one]", name);

        options = options with { UseReturnType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("Byte TE<Byte, Int32>.this[Int32 one]", name);

        options = EasyMemberOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"System.Byte {NAMESPACE}.{CLASSNAME}.TE<System.Byte, System.Int32>.this[System.Int32 one]", name);
    }

    // -----------------------------------------------------

    public class TF<T, K>
    {
        public T Age = default!;
    }

    //[Enforced]
    [Fact]
    public static void Test_Field_Unbounded()
    {
        var type = typeof(TF<,>);
        FieldInfo item = type.GetField("Age")!;

        EasyMemberOptions options = EasyMemberOptions.Empty;
        string name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = item.EasyName(options); Assert.Equal("Age", name);

        options = options with { UseHostType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TF<T, K>.Age", name);

        options = options with { UseReturnType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T TF<T, K>.Age", name);

        options = EasyMemberOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"T {NAMESPACE}.{CLASSNAME}.TF<T, K>.Age", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Field_Bounded()
    {
        var type = typeof(TF<byte, int>);
        FieldInfo item = type.GetField("Age")!;

        EasyMemberOptions options = EasyMemberOptions.Empty;
        string name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = item.EasyName(options); Assert.Equal("Age", name);

        options = options with { UseHostType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TF<Byte, Int32>.Age", name);

        options = options with { UseReturnType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("Byte TF<Byte, Int32>.Age", name);

        options = EasyMemberOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"System.Byte {NAMESPACE}.{CLASSNAME}.TF<System.Byte, System.Int32>.Age", name);
    }
}
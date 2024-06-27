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
        EasyMemberOptions options;
        string name;
        var type = typeof(TA<,>);
        MethodInfo item = type.GetMethod("MTKS")!;

        name = item.EasyName(); Assert.Equal("MTKS", name);

        options = new EasyMemberOptions { UseName = false };
        name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("(,)", name);

        options = new EasyMemberOptions { UseGenericArguments = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("MTKS<S>", name);

        options = options with { UseReturnType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T MTKS<S>", name);

        options = new EasyMemberOptions { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("MTKS(one, two)", name);

        options = new EasyMemberOptions { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("MTKS(K, S)", name);

        options = EasyMemberOptions.NoNamespaces;
        name = item.EasyName(options); Assert.Equal($"T {CLASSNAME}.TA<T, K>.MTKS<S>(K one, S two)", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Bounded()
    {
        EasyMemberOptions options;
        string name;
        var type = typeof(TA<byte, int>);
        MethodInfo item = type.GetMethod("MTKS")!;
        item = item.MakeGenericMethod(typeof(long));

        name = item.EasyName(); Assert.Equal("MTKS", name);

        options = new EasyMemberOptions { UseName = false };
        name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("(,)", name);

        options = new EasyMemberOptions { UseGenericArguments = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("MTKS<Int64>", name);

        options = options with { UseReturnType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("Byte MTKS<Int64>", name);

        options = new EasyMemberOptions { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("MTKS(one, two)", name);

        options = new EasyMemberOptions { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("MTKS(Int32, Int64)", name);

        options = EasyMemberOptions.NoNamespaces;
        name = item.EasyName(options); Assert.Equal($"Byte {CLASSNAME}.TA<Byte, Int32>.MTKS<Int64>(Int32 one, Int64 two)", name);
    }

    // -----------------------------------------------------

    public class TB<T> { }

    //[Enforced]
    [Fact]
    public static void Test_Implicit_Constructor_Unbounded()
    {
        EasyMemberOptions options;
        string name;
        var type = typeof(TB<>);
        ConstructorInfo item = type.GetConstructor(Type.EmptyTypes)!;

        name = item.EasyName(); Assert.Equal(".ctor", name);

        options = new EasyMemberOptions { UseName = false };
        name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("()", name);

        options = new EasyMemberOptions { UseHostType = new EasyTypeOptions { UseArgumentsNames = true } };
        name = item.EasyName(options); Assert.Equal("TB<T>..ctor", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("TB<T>..ctor()", name);

        options = EasyMemberOptions.NoNamespaces;
        name = item.EasyName(options); Assert.Equal($"{CLASSNAME}.TB<T> {CLASSNAME}.TB<T>..ctor()", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Implicit_Constructor_Bounded()
    {
        EasyMemberOptions options;
        string name;
        var type = typeof(TB<byte>);
        ConstructorInfo item = type.GetConstructor(Type.EmptyTypes)!;

        name = item.EasyName(); Assert.Equal(".ctor", name);

        options = new EasyMemberOptions { UseName = false };
        name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("()", name);

        options = new EasyMemberOptions { UseHostType = new EasyTypeOptions { UseArgumentsNames = true } };
        name = item.EasyName(options); Assert.Equal("TB<Byte>..ctor", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("TB<Byte>..ctor()", name);

        options = EasyMemberOptions.NoNamespaces;
        name = item.EasyName(options); Assert.Equal($"{CLASSNAME}.TB<Byte> {CLASSNAME}.TB<Byte>..ctor()", name);
    }

    // -----------------------------------------------------

    public class TC<T, K>(T one, K two) { }

    //[Enforced]
    [Fact]
    public static void Test_Explicit_Constructor_Unbounded()
    {
        EasyMemberOptions options;
        string name;
        var type = typeof(TC<,>);
        ConstructorInfo item = type.GetConstructors()[0];

        name = item.EasyName(); Assert.Equal(".ctor", name);

        options = new EasyMemberOptions { UseName = false };
        name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("(,)", name);

        options = new EasyMemberOptions { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal(".ctor(one, two)", name);

        options = new EasyMemberOptions { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal(".ctor(T, K)", name);

        options = new EasyMemberOptions { UseHostType = new EasyTypeOptions { UseArgumentsNames = true } };
        name = item.EasyName(options); Assert.Equal("TC<T, K>..ctor", name);

        options = EasyMemberOptions.NoNamespaces;
        name = item.EasyName(options); Assert.Equal($"{CLASSNAME}.TC<T, K> {CLASSNAME}.TC<T, K>..ctor(T one, K two)", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Explicit_Constructor_Bounded()
    {
        EasyMemberOptions options;
        string name;
        var type = typeof(TC<byte, int>);
        ConstructorInfo item = type.GetConstructors()[0];

        name = item.EasyName(); Assert.Equal(".ctor", name);

        options = new EasyMemberOptions { UseName = false };
        name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("(,)", name);

        options = new EasyMemberOptions { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal(".ctor(one, two)", name);

        options = new EasyMemberOptions { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal(".ctor(Byte, Int32)", name);

        options = new EasyMemberOptions { UseHostType = new EasyTypeOptions { UseArgumentsNames = true } };
        name = item.EasyName(options); Assert.Equal("TC<Byte, Int32>..ctor", name);

        options = EasyMemberOptions.NoNamespaces;
        name = item.EasyName(options); Assert.Equal($"{CLASSNAME}.TC<Byte, Int32> {CLASSNAME}.TC<Byte, Int32>..ctor(Byte one, Int32 two)", name);
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
        EasyMemberOptions options;
        string name;
        var type = typeof(TD<,>);
        PropertyInfo item = type.GetProperty("Item")!;

        name = item.EasyName(); Assert.Equal("this", name);

        options = new EasyMemberOptions { UseName = false };
        name = item.EasyName(options); Assert.Equal("", name);

        options = new EasyMemberOptions { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("this[]", name);

        options = new EasyMemberOptions { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("this[one]", name);

        options = new EasyMemberOptions { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("this[K]", name);

        options = new EasyMemberOptions { UseArgumentsNames = true, UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("this[K one]", name);

        options = EasyMemberOptions.NoNamespaces;
        name = item.EasyName(options); Assert.Equal($"T {CLASSNAME}.TD<T, K>.this[K one]", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Indexer_Bounded()
    {
        EasyMemberOptions options;
        string name;
        var type = typeof(TD<byte, int>);
        PropertyInfo item = type.GetProperty("Item")!;

        name = item.EasyName(); Assert.Equal("this", name);

        options = new EasyMemberOptions { UseName = false };
        name = item.EasyName(options); Assert.Equal("", name);

        options = new EasyMemberOptions { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("this[]", name);

        options = new EasyMemberOptions { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("this[one]", name);

        options = new EasyMemberOptions { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("this[Int32]", name);

        options = new EasyMemberOptions { UseArgumentsNames = true, UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("this[Int32 one]", name);

        options = EasyMemberOptions.NoNamespaces;
        name = item.EasyName(options); Assert.Equal($"Byte {CLASSNAME}.TD<Byte, Int32>.this[Int32 one]", name);
    }

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Property_Unbounded()
    {
        EasyMemberOptions options;
        string name;
        var type = typeof(TD<,>);
        PropertyInfo item = type.GetProperty("PT")!;

        name = item.EasyName(); Assert.Equal("PT", name);

        options = new EasyMemberOptions { UseName = false };
        name = item.EasyName(options); Assert.Equal("", name);

        options = new EasyMemberOptions { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("PT", name);

        options = new EasyMemberOptions { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("PT", name);

        options = new EasyMemberOptions { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("PT", name);

        options = EasyMemberOptions.NoNamespaces;
        name = item.EasyName(options); Assert.Equal($"T {CLASSNAME}.TD<T, K>.PT", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Property_Bounded()
    {
        EasyMemberOptions options;
        string name;
        var type = typeof(TD<byte, int>);
        PropertyInfo item = type.GetProperty("PT")!;

        name = item.EasyName(); Assert.Equal("PT", name);

        options = new EasyMemberOptions { UseName = false };
        name = item.EasyName(options); Assert.Equal("", name);

        options = new EasyMemberOptions { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("PT", name);

        options = new EasyMemberOptions { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("PT", name);

        options = new EasyMemberOptions { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("PT", name);

        options = EasyMemberOptions.NoNamespaces;
        name = item.EasyName(options); Assert.Equal($"Byte {CLASSNAME}.TD<Byte, Int32>.PT", name);
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
        EasyMemberOptions options;
        string name;
        var type = typeof(TE<,>);
        PropertyInfo item = type.GetProperty("MyItem")!;

        name = item.EasyName(); Assert.Equal("this", name);

        options = new EasyMemberOptions { UseName = false };
        name = item.EasyName(options); Assert.Equal("", name);

        options = new EasyMemberOptions { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("this[]", name);

        options = new EasyMemberOptions { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("this[one]", name);

        options = new EasyMemberOptions { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("this[K]", name);

        options = new EasyMemberOptions { UseArgumentsNames = true, UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("this[K one]", name);

        options = EasyMemberOptions.NoNamespaces;
        name = item.EasyName(options); Assert.Equal($"T {CLASSNAME}.TE<T, K>.this[K one]", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Custom_Indexer_Bounded()
    {
        EasyMemberOptions options;
        string name;
        var type = typeof(TE<byte, int>);
        PropertyInfo item = type.GetProperty("MyItem")!;

        name = item.EasyName(); Assert.Equal("this", name);

        options = new EasyMemberOptions { UseName = false };
        name = item.EasyName(options); Assert.Equal("", name);

        options = new EasyMemberOptions { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("this[]", name);

        options = new EasyMemberOptions { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("this[one]", name);

        options = new EasyMemberOptions { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("this[Int32]", name);

        options = new EasyMemberOptions { UseArgumentsNames = true, UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("this[Int32 one]", name);

        options = EasyMemberOptions.NoNamespaces;
        name = item.EasyName(options); Assert.Equal($"Byte {CLASSNAME}.TE<Byte, Int32>.this[Int32 one]", name);
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
        EasyMemberOptions options;
        string name;
        var type = typeof(TF<,>);
        FieldInfo item = type.GetField("Age")!;

        name = item.EasyName(); Assert.Equal("Age", name);

        options = new EasyMemberOptions { UseName = false };
        name = item.EasyName(options); Assert.Equal("", name);

        options = new EasyMemberOptions { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("Age", name);

        options = EasyMemberOptions.NoNamespaces;
        name = item.EasyName(options); Assert.Equal($"T {CLASSNAME}.TF<T, K>.Age", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Field_Bounded()
    {
        EasyMemberOptions options;
        string name;
        var type = typeof(TF<byte, int>);
        FieldInfo item = type.GetField("Age")!;

        name = item.EasyName(); Assert.Equal("Age", name);

        options = new EasyMemberOptions { UseName = false };
        name = item.EasyName(options); Assert.Equal("", name);

        options = new EasyMemberOptions { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("Age", name);

        options = EasyMemberOptions.NoNamespaces;
        name = item.EasyName(options); Assert.Equal($"Byte {CLASSNAME}.TF<Byte, Int32>.Age", name);
    }
}
#pragma warning disable CS9113

namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_EasyName_Members
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = nameof(Test_EasyName_Members);

    // -----------------------------------------------------

    public class TA<K, T>
    {
        public T? M<S>(K? one, S two) => throw new NotImplementedException();
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Unbounded()
    {
        var type = typeof(TA<,>);
        var item = type.GetMethod("M")!;

        var options = EasyMemberOptions.Empty;
        var name = item.EasyName(options); Assert.Equal("M", name);

        options = options with { UseMemberType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T M", name);

        options = options with { UseMemberHost = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T TA<K, T>.M", name);

        options = options with { UseTypeArguments = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T TA<K, T>.M<S>", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("T TA<K, T>.M<S>(,)", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T TA<K, T>.M<S>(K, S)", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("T TA<K, T>.M<S>(K one, S two)", name);

        options = options with { UseMemberType = options.UseMemberType with { UseNamespace = true } };
        name = item.EasyName(options); Assert.Equal("T TA<K, T>.M<S>(K one, S two)", name);

        // Default...
        options = EasyMemberOptions.Default;
        name = item.EasyName(options); Assert.Equal("M<S>(K, S)", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Method_Bounded()
    {
        var type = typeof(TA<byte, string>);
        var item = type.GetMethod("M")!;
        item = item.MakeGenericMethod(typeof(int));

        var options = EasyMemberOptions.Empty;
        var name = item.EasyName(options); Assert.Equal("M", name);

        options = options with { UseMemberType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("String M", name);

        options = options with { UseMemberHost = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("String TA<Byte, String>.M", name);

        options = options with { UseTypeArguments = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("String TA<Byte, String>.M<Int32>", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("String TA<Byte, String>.M<Int32>(,)", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("String TA<Byte, String>.M<Int32>(Byte, Int32)", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("String TA<Byte, String>.M<Int32>(Byte one, Int32 two)", name);

        options = options with { UseMemberType = options.UseMemberType with { UseNamespace = true } };
        name = item.EasyName(options); Assert.Equal("System.String TA<Byte, String>.M<Int32>(Byte one, Int32 two)", name);

        // Default...
        options = EasyMemberOptions.Default;
        name = item.EasyName(options); Assert.Equal("M<Int32>(Byte, Int32)", name);
    }

    // -----------------------------------------------------

    public class TB<T> { }

    //[Enforced]
    [Fact]
    public static void Test_Implicit_Constructor_Unbounded()
    {
        var type = typeof(TB<>);
        var item = type.GetConstructor(Type.EmptyTypes)!;

        var options = EasyMemberOptions.Empty;
        var name = item.EasyName(options); Assert.Equal("new", name);

        options = options with { UseMemberType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("new", name);

        options = options with { UseMemberHost = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TB<T>.new", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("TB<T>.new()", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TB<T>.new()", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("TB<T>.new()", name);

        options = options with { UseMemberType = options.UseMemberType with { UseNamespace = true } };
        name = item.EasyName(options); Assert.Equal("TB<T>.new()", name);

        // Default...
        options = EasyMemberOptions.Default;
        name = item.EasyName(options); Assert.Equal("new()", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Implicit_Constructor_Bounded()
    {
        var type = typeof(TB<byte>);
        var item = type.GetConstructor(Type.EmptyTypes)!;

        var options = EasyMemberOptions.Empty;
        var name = item.EasyName(options); Assert.Equal("new", name);

        options = options with { UseMemberType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("new", name);

        options = options with { UseMemberHost = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TB<Byte>.new", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("TB<Byte>.new()", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TB<Byte>.new()", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("TB<Byte>.new()", name);

        options = options with { UseMemberType = options.UseMemberType with { UseNamespace = true } };
        name = item.EasyName(options); Assert.Equal("TB<Byte>.new()", name);

        // Default...
        options = EasyMemberOptions.Default;
        name = item.EasyName(options); Assert.Equal("new()", name);
    }

    // -----------------------------------------------------

    public class TC<K, T>(K one, T two) { }

    //[Enforced]
    [Fact]
    public static void Test_Explicit_Constructor_Unbounded()
    {
        var type = typeof(TC<,>);
        var item = type.GetConstructors()[0];

        var options = EasyMemberOptions.Empty;
        var name = item.EasyName(options); Assert.Equal("new", name);

        options = options with { UseMemberType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("new", name);

        options = options with { UseMemberHost = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TC<K, T>.new", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("TC<K, T>.new(,)", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TC<K, T>.new(K, T)", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("TC<K, T>.new(K one, T two)", name);

        options = options with { UseMemberType = options.UseMemberType with { UseNamespace = true } };
        name = item.EasyName(options); Assert.Equal("TC<K, T>.new(K one, T two)", name);

        // Default...
        options = EasyMemberOptions.Default;
        name = item.EasyName(options); Assert.Equal("new(K, T)", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Explicit_Constructor_Bounded()
    {
        var type = typeof(TC<byte, string>);
        var item = type.GetConstructors()[0];

        var options = EasyMemberOptions.Empty;
        var name = item.EasyName(options); Assert.Equal("new", name);

        options = options with { UseMemberType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("new", name);

        options = options with { UseMemberHost = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TC<Byte, String>.new", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("TC<Byte, String>.new(,)", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("TC<Byte, String>.new(Byte, String)", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("TC<Byte, String>.new(Byte one, String two)", name);

        options = options with { UseMemberType = options.UseMemberType with { UseNamespace = true } };
        name = item.EasyName(options); Assert.Equal("TC<Byte, String>.new(Byte one, String two)", name);

        // Default...
        options = EasyMemberOptions.Default;
        name = item.EasyName(options); Assert.Equal("new(Byte, String)", name);
    }

    // -----------------------------------------------------

    public class TD<K, T>
    {
        public T this[K one, T two] => throw new NotImplementedException();
    }

    //[Enforced]
    [Fact]
    public static void Test_Indexer_Unbounded()
    {
        var type = typeof(TD<,>);
        var item = type.GetProperty("Item")!;

        var options = EasyMemberOptions.Empty;
        var name = item.EasyName(options); Assert.Equal("this", name);

        options = options with { UseMemberType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T this", name);

        options = options with { UseMemberHost = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T TD<K, T>.this", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("T TD<K, T>.this[,]", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T TD<K, T>.this[K, T]", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("T TD<K, T>.this[K one, T two]", name);

        options = options with { UseMemberType = options.UseMemberType with { UseNamespace = true } };
        name = item.EasyName(options); Assert.Equal("T TD<K, T>.this[K one, T two]", name);

        // Default...
        options = EasyMemberOptions.Default;
        name = item.EasyName(options); Assert.Equal("this[K, T]", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Indexer_Bounded()
    {
        var type = typeof(TD<byte, string>);
        var item = type.GetProperty("Item")!;

        var options = EasyMemberOptions.Empty;
        var name = item.EasyName(options); Assert.Equal("this", name);

        options = options with { UseMemberType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("String this", name);

        options = options with { UseMemberHost = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("String TD<Byte, String>.this", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("String TD<Byte, String>.this[,]", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("String TD<Byte, String>.this[Byte, String]", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("String TD<Byte, String>.this[Byte one, String two]", name);

        options = options with { UseMemberType = options.UseMemberType with { UseNamespace = true } };
        name = item.EasyName(options); Assert.Equal("System.String TD<Byte, String>.this[Byte one, String two]", name);

        // Default...
        options = EasyMemberOptions.Default;
        name = item.EasyName(options); Assert.Equal("this[Byte, String]", name);
    }

    // -----------------------------------------------------

    public class TE<K, T>
    {
        [IndexerName("MyItem")]
        public T this[K one, T two] => throw new NotImplementedException();
    }

    //[Enforced]
    [Fact]
    public static void Test_Custom_Indexer_Unbounded()
    {
        var type = typeof(TE<,>);
        var item = type.GetProperty("MyItem")!;

        var options = EasyMemberOptions.Empty;
        var name = item.EasyName(options); Assert.Equal("this", name);

        options = options with { UseMemberType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T this", name);

        options = options with { UseMemberHost = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T TE<K, T>.this", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("T TE<K, T>.this[,]", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T TE<K, T>.this[K, T]", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("T TE<K, T>.this[K one, T two]", name);

        options = options with { UseMemberType = options.UseMemberType with { UseNamespace = true } };
        name = item.EasyName(options); Assert.Equal("T TE<K, T>.this[K one, T two]", name);

        // Default...
        options = EasyMemberOptions.Default;
        name = item.EasyName(options); Assert.Equal("this[K, T]", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_CustomIndexer_Bounded()
    {
        var type = typeof(TE<byte, string>);
        var item = type.GetProperty("MyItem")!;

        var options = EasyMemberOptions.Empty;
        var name = item.EasyName(options); Assert.Equal("this", name);

        options = options with { UseMemberType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("String this", name);

        options = options with { UseMemberHost = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("String TE<Byte, String>.this", name);

        options = options with { UseArguments = true };
        name = item.EasyName(options); Assert.Equal("String TE<Byte, String>.this[,]", name);

        options = options with { UseArgumentsTypes = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("String TE<Byte, String>.this[Byte, String]", name);

        options = options with { UseArgumentsNames = true };
        name = item.EasyName(options); Assert.Equal("String TE<Byte, String>.this[Byte one, String two]", name);

        options = options with { UseMemberType = options.UseMemberType with { UseNamespace = true } };
        name = item.EasyName(options); Assert.Equal("System.String TE<Byte, String>.this[Byte one, String two]", name);

        // Default...
        options = EasyMemberOptions.Default;
        name = item.EasyName(options); Assert.Equal("this[Byte, String]", name);
    }

    // -----------------------------------------------------

    public class TF<K, T>
    {
        public T P => throw new NotImplementedException();
    }

    //[Enforced]
    [Fact]
    public static void Test_Property_Unbounded()
    {
        var type = typeof(TF<,>);
        var item = type.GetProperty("P")!;

        var options = EasyMemberOptions.Empty;
        var name = item.EasyName(options); Assert.Equal("P", name);

        options = options with { UseMemberType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T P", name);

        options = options with { UseMemberHost = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T TF<K, T>.P", name);

        options = options with { UseMemberType = options.UseMemberType with { UseNamespace = true } };
        name = item.EasyName(options); Assert.Equal("T TF<K, T>.P", name);

        // Default...
        options = EasyMemberOptions.Default;
        name = item.EasyName(options); Assert.Equal("P", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Property_Bounded()
    {
        var type = typeof(TF<byte, string>);
        var item = type.GetProperty("P")!;

        var options = EasyMemberOptions.Empty;
        var name = item.EasyName(options); Assert.Equal("P", name);

        options = options with { UseMemberType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("String P", name);

        options = options with { UseMemberHost = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("String TF<Byte, String>.P", name);

        options = options with { UseMemberType = options.UseMemberType with { UseNamespace = true } };
        name = item.EasyName(options); Assert.Equal("System.String TF<Byte, String>.P", name);

        // Default...
        options = EasyMemberOptions.Default;
        name = item.EasyName(options); Assert.Equal("P", name);
    }

    // -----------------------------------------------------

    public class TG<K, T>
    {
        public T F = default!;
    }

    //[Enforced]
    [Fact]
    public static void Test_Field_Unbounded()
    {
        var type = typeof(TG<,>);
        var item = type.GetField("F")!;

        var options = EasyMemberOptions.Empty;
        var name = item.EasyName(options); Assert.Equal("F", name);

        options = options with { UseMemberType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T F", name);

        options = options with { UseMemberHost = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("T TG<K, T>.F", name);

        options = options with { UseMemberType = options.UseMemberType with { UseNamespace = true } };
        name = item.EasyName(options); Assert.Equal("T TG<K, T>.F", name);

        // Default...
        options = EasyMemberOptions.Default;
        name = item.EasyName(options); Assert.Equal("F", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Field_Bounded()
    {
        var type = typeof(TG<byte, string>);
        var item = type.GetField("F")!;

        var options = EasyMemberOptions.Empty;
        var name = item.EasyName(options); Assert.Equal("F", name);

        options = options with { UseMemberType = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("String F", name);

        options = options with { UseMemberHost = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("String TG<Byte, String>.F", name);

        options = options with { UseMemberType = options.UseMemberType with { UseNamespace = true } };
        name = item.EasyName(options); Assert.Equal("System.String TG<Byte, String>.F", name);

        // Default...
        options = EasyMemberOptions.Default;
        name = item.EasyName(options); Assert.Equal("F", name);
    }
}
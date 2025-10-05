namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyNames_Types
{
    const string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    const string CLASSNAME = nameof(Test_EasyNames_Types);
    static readonly EasyNameOptions OPTIONS = EasyNameOptions.Default;

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Type_System()
    {
        var item = typeof(string);
        var options = OPTIONS;
        var name = item.EasyName(options); Assert.Equal("String", name);
    }

    // -----------------------------------------------------

    public class TXA { public class TA {  } }

    //[Enforced]
    [Fact]
    public static void Test_Type_Standard()
    {
        // Default...
        var item = typeof(TXA.TA);
        var options = OPTIONS;
        var name = item.EasyName(options); Assert.Equal("TA", name);

        options = options with { UseTypeHost = OPTIONS };
        name = item.EasyName(options); Assert.Equal($"{CLASSNAME}.TXA.TA", name);

        options = OPTIONS;
        options = options with { UseTypeNamespace = true };
        name = item.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TXA.TA", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("", name);

        // Full...
        options = EasyNameOptions.Full;
        name = item.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TXA.TA", name);
    }

    // -----------------------------------------------------

    public class TXB<K, T> { public class TB<S> { } }

    //[Enforced]
    [Fact]
    public static void Test_Type_Generic_Unbound()
    {
        // Default...
        var item = typeof(TXB<,>.TB<>);
        var options = OPTIONS;
        var name = item.EasyName(options); Assert.Equal("TB<S>", name);

        options = options with { UseTypeHost = OPTIONS };
        name = item.EasyName(options); Assert.Equal($"{CLASSNAME}.TXB<K, T>.TB<S>", name);

        options = OPTIONS;
        options = options with { UseTypeNamespace = true };
        name = item.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TXB<K, T>.TB<S>", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeName = true };
        name = item.EasyName(options); Assert.Equal("TB", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Empty };
        name = item.EasyName(options); Assert.Equal("TB<>", name);

        // Full...
        options = EasyNameOptions.Full;
        name = item.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TXB<K, T>.TB<S>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_Generic_Bound()
    {
        // Default...
        var item = typeof(TXB<byte, string>.TB<int>);
        var options = OPTIONS;
        var name = item.EasyName(options); Assert.Equal("TB<Int32>", name);

        options = options with { UseTypeHost = OPTIONS };
        name = item.EasyName(options); Assert.Equal($"{CLASSNAME}.TXB<Byte, String>.TB<Int32>", name);

        options = OPTIONS;
        options = options with { UseTypeNamespace = true };
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TXB<Byte, String>.TB<Int32>", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeName = true };
        name = item.EasyName(options); Assert.Equal("TB", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Empty };
        name = item.EasyName(options); Assert.Equal("TB<>", name);

        // Full...
        options = EasyNameOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TXB<System.Byte, System.String>.TB<System.Int32>", name);
    }

    // -----------------------------------------------------

    public class TXC<K, T> { public class TYC<S> { public class TC<V> { } } }

    //[Enforced]
    [Fact]
    public static void Test_Type_DeepNested_Unbound()
    {
        // Default...
        var item = typeof(TXC<,>.TYC<>.TC<>);
        var options = OPTIONS;
        var name = item.EasyName(options); Assert.Equal("TC<V>", name);

        options = options with { UseTypeHost = OPTIONS };
        name = item.EasyName(options); Assert.Equal($"{CLASSNAME}.TXC<K, T>.TYC<S>.TC<V>", name);

        options = OPTIONS;
        options = options with { UseTypeNamespace = true };
        name = item.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TXC<K, T>.TYC<S>.TC<V>", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = item.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeName = true };
        name = item.EasyName(options); Assert.Equal("TC", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Empty };
        name = item.EasyName(options); Assert.Equal("TC<>", name);

        // Full...
        options = EasyNameOptions.Full;
        name = item.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TXC<K, T>.TYC<S>.TC<V>", name);
    }
}
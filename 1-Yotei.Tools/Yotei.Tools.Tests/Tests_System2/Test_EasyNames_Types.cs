namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_EasyNames_Types
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = nameof(Test_EasyNames_Types);

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_System_Types()
    {
        var type = typeof(string);
        string name = default!;
        var options = EasyNameOptions.Default;

        // Default...
        name = type.EasyName(options); Assert.Equal("String", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal("System.String", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal("System.String", name);
    }

    // ----------------------------------------------------

    public class TA { public class TB { } }

    //[Enforced]
    [Fact]
    public static void Test_Simple_Type()
    {
        var type = typeof(TA);
        string name = default!;
        var options = EasyNameOptions.Default;

        // Default...
        name = type.EasyName(options); Assert.Equal("TA", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TA", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TA", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TA", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TA", name);

        // Full...
        options = EasyNameOptions.Full;
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TA", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Simple_NestedType()
    {
        var type = typeof(TA.TB);
        string name = default!;
        var options = EasyNameOptions.Default;

        // Default...
        name = type.EasyName(options); Assert.Equal("TB", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TA.TB", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TA.TB", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TA.TB", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TA.TB", name);

        // Full...
        options = EasyNameOptions.Full;
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TA.TB", name);
    }

    // -----------------------------------------------------

    public class TC<K, T> { }

    //[Enforced]
    [Fact]
    public static void Test_Type_Unbound_Generics()
    {
        var type = typeof(TC<,>);
        string name = default!;
        var options = EasyNameOptions.Default;

        // Default...
        name = type.EasyName(options); Assert.Equal("TC", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Empty };
        name = type.EasyName(options); Assert.Equal("TC<,>", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Default };
        name = type.EasyName(options); Assert.Equal("TC<K, T>", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TC<K, T>", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TC<K, T>", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Empty };
        name = type.EasyName(options); Assert.Equal("$<,>", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TC<,>", name);

        // Full...
        options = EasyNameOptions.Full;
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TC<K, T>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_Bound_Generics()
    {
        var type = typeof(TC<byte, string>);
        string name = default!;
        var options = EasyNameOptions.Default;

        // Default...
        name = type.EasyName(options); Assert.Equal("TC", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Empty };
        name = type.EasyName(options); Assert.Equal("TC<,>", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Default };
        name = type.EasyName(options); Assert.Equal("TC<Byte, String>", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TC<Byte, String>", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TC<Byte, String>", name);

        var temp = EasyNameOptions.Default with { UseTypeNamespace = true };
        options = options with { UseTypeGenericArguments = temp };
        name = type.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TC<System.Byte, System.String>", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Empty };
        name = type.EasyName(options); Assert.Equal("$<,>", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Default };
        name = type.EasyName(options); Assert.Equal("$<Byte, String>", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TC<Byte, String>", name);

        // Full...
        options = EasyNameOptions.Full;
        name = type.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TC<System.Byte, System.String>", name);
    }

    // -----------------------------------------------------

    public class TD<K, T> { public class TE<S> { } }

    //[Enforced]
    [Fact]
    public static void Test_Type_Unbound_Nested_Generics()
    {
        var type = typeof(TD<,>.TE<>);
        string name = default!;
        var options = EasyNameOptions.Default;

        // Default...
        name = type.EasyName(options); Assert.Equal("TE", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Empty };
        name = type.EasyName(options); Assert.Equal("TE<>", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Default };
        name = type.EasyName(options); Assert.Equal("TE<S>", name);

        // Default with host...
        options = EasyNameOptions.Default with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD.TE", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Empty };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD<,>.TE<>", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Default };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD<K, T>.TE<S>", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TD<K, T>.TE<S>", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Empty };
        name = type.EasyName(options); Assert.Equal("$<>", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD<,>.TE<>", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Default };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD<K, T>.TE<S>", name);

        // Full...
        options = EasyNameOptions.Full;
        name = type.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TD<K, T>.TE<S>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_Bound_Nested_Generics()
    {
        var type = typeof(TD<byte, string>.TE<int>);
        string name = default!;
        var options = EasyNameOptions.Default;

        // Default...
        name = type.EasyName(options); Assert.Equal("TE", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Empty };
        name = type.EasyName(options); Assert.Equal("TE<>", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Default };
        name = type.EasyName(options); Assert.Equal("TE<Int32>", name);

        // Default with host...
        options = EasyNameOptions.Default with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD.TE", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Empty };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD<,>.TE<>", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Default };
        name = type.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TD<Byte, String>.TE<Int32>", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TD<Byte, String>.TE<Int32>", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Empty };
        name = type.EasyName(options); Assert.Equal("$<>", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD<,>.TE<>", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Default };
        name = type.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TD<Byte, String>.TE<Int32>", name);

        // Full...
        options = EasyNameOptions.Full;
        name = type.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TD<System.Byte, System.String>.TE<System.Int32>", name);
    }

    // -----------------------------------------------------

    public class TF<K, T> { public class TG { public class TH<S> { } } }

    //[Enforced]
    [Fact]
    public static void Test_Type_Unbound_TwoNested_Generics()
    {
        var type = typeof(TF<,>.TG.TH<>);
        string name = default!;
        var options = EasyNameOptions.Default;

        // Default...
        name = type.EasyName(options); Assert.Equal("TH", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Empty };
        name = type.EasyName(options); Assert.Equal("TH<>", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Default };
        name = type.EasyName(options); Assert.Equal("TH<S>", name);

        // Default with host...
        options = EasyNameOptions.Default with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF.TG.TH", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Empty };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF<,>.TG.TH<>", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Default };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF<K, T>.TG.TH<S>", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TF<K, T>.TG.TH<S>", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Empty };
        name = type.EasyName(options); Assert.Equal("$<>", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF<,>.TG.TH<>", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Default };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF<K, T>.TG.TH<S>", name);

        // Full...
        options = EasyNameOptions.Full;
        name = type.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TF<K, T>.TG.TH<S>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_Bound_TwoNested_Generics()
    {
        var type = typeof(TF<byte, string>.TG.TH<int>);
        string name = default!;
        var options = EasyNameOptions.Default;

        // Default...
        name = type.EasyName(options); Assert.Equal("TH", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Empty };
        name = type.EasyName(options); Assert.Equal("TH<>", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Default };
        name = type.EasyName(options); Assert.Equal("TH<Int32>", name);

        // Default with host...
        options = EasyNameOptions.Default with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF.TG.TH", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Empty };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF<,>.TG.TH<>", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Default };
        name = type.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TF<Byte, String>.TG.TH<Int32>", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TF<Byte, String>.TG.TH<Int32>", name);

        // Empty...
        options = EasyNameOptions.Empty;
        name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Empty };
        name = type.EasyName(options); Assert.Equal("$<>", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF<,>.TG.TH<>", name);

        options = options with { UseTypeGenericArguments = EasyNameOptions.Default };
        name = type.EasyName(options);
        Assert.Equal($"{CLASSNAME}.TF<Byte, String>.TG.TH<Int32>", name);

        // Full...
        options = EasyNameOptions.Full;
        name = type.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.TF<System.Byte, System.String>.TG.TH<System.Int32>",
            name);
    }
}
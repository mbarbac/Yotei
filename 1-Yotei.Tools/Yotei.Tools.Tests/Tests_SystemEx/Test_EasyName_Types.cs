namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_EasyName_Types
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = nameof(Test_EasyName_Types);

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_System_Types()
    {
        var type = typeof(string);

        var options = EasyNameOptions.False;
        var name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeName = true };
        name = type.EasyName(options); Assert.Equal("String", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal("String", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal("System.String", name);

        // Default...
        options = EasyNameOptions.Default;
        name = type.EasyName(options); Assert.Equal("String", name);
    }

    // -----------------------------------------------------

    public class TA { public class TB { } }

    //[Enforced]
    [Fact]
    public static void Test_Simple_Type()
    {
        var type = typeof(TA);

        var options = EasyNameOptions.False;
        var name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeName = true };
        name = type.EasyName(options); Assert.Equal("TA", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TA", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TA", name);

        // Default...
        options = EasyNameOptions.Default;
        name = type.EasyName(options); Assert.Equal("TA", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Simple_Nested_Type()
    {
        var type = typeof(TA.TB);

        var options = EasyNameOptions.False;
        var name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeName = true };
        name = type.EasyName(options); Assert.Equal("TB", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TA.TB", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TA.TB", name);

        // Default...
        options = EasyNameOptions.Default;
        name = type.EasyName(options); Assert.Equal("TB", name);
    }

    // -----------------------------------------------------

    public class TC<K, T> { }

    //[Enforced]
    [Fact]
    public static void Test_Type_Unbound_Generics()
    {
        var type = typeof(TC<,>);

        var options = EasyNameOptions.False;
        var name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeName = true };
        name = type.EasyName(options); Assert.Equal("TC", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TC", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TC", name);

        options = EasyNameOptions.False with { UseTypeArguments = true };
        name = type.EasyName(options); Assert.Equal($"TC<,>", name);

        options = options with { UseTypeName = true };
        name = type.EasyName(options); Assert.Equal($"TC<K, T>", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TC<K, T>", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TC<K, T>", name);

        // Default...
        options = EasyNameOptions.Default;
        name = type.EasyName(options); Assert.Equal("TC<K, T>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_Bound_Generics()
    {
        var type = typeof(TC<byte, string>);

        var options = EasyNameOptions.False;
        var name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeName = true };
        name = type.EasyName(options); Assert.Equal("TC", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TC", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TC", name);

        options = EasyNameOptions.False with { UseTypeArguments = true };
        name = type.EasyName(options); Assert.Equal($"TC<,>", name);

        options = options with { UseTypeName = true };
        name = type.EasyName(options); Assert.Equal($"TC<Byte, String>", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TC<Byte, String>", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TC<System.Byte, System.String>", name);

        // Default...
        options = EasyNameOptions.Default;
        name = type.EasyName(options); Assert.Equal("TC<Byte, String>", name);
    }

    // -----------------------------------------------------

    public class TD<K, T> { public class TE<S> { } }

    //[Enforced]
    [Fact]
    public static void Test_Type_Unbound_Nested_Generics()
    {
        var type = typeof(TD<,>.TE<>);

        var options = EasyNameOptions.False;
        var name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeName = true };
        name = type.EasyName(options); Assert.Equal("TE", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD.TE", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TD.TE", name);

        options = EasyNameOptions.False with { UseTypeArguments = true };
        name = type.EasyName(options); Assert.Equal($"TE<>", name);

        options = options with { UseTypeName = true };
        name = type.EasyName(options); Assert.Equal($"TE<S>", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD<K, T>.TE<S>", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TD<K, T>.TE<S>", name);

        // Default...
        options = EasyNameOptions.Default;
        name = type.EasyName(options); Assert.Equal("TE<S>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_Bound_Nested_Generics()
    {
        var type = typeof(TD<byte, string>.TE<int>);

        var options = EasyNameOptions.False;
        var name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeName = true };
        name = type.EasyName(options); Assert.Equal("TE", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD.TE", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TD.TE", name);

        options = EasyNameOptions.False with { UseTypeArguments = true };
        name = type.EasyName(options); Assert.Equal($"TE<>", name);

        options = options with { UseTypeName = true };
        name = type.EasyName(options); Assert.Equal($"TE<Int32>", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD<Byte, String>.TE<Int32>", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TD<System.Byte, System.String>.TE<System.Int32>", name);

        // Default...
        options = EasyNameOptions.Default;
        name = type.EasyName(options); Assert.Equal("TE<Int32>", name);
    }

    // -----------------------------------------------------

    public class TF<K, T> { public class TG { public class TH<S> { } } }

    //[Enforced]
    [Fact]
    public static void Test_Type_Unbound_TwoNested_Generics()
    {
        var type = typeof(TF<,>.TG.TH<>);

        var options = EasyNameOptions.False;
        var name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeName = true };
        name = type.EasyName(options); Assert.Equal("TH", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF.TG.TH", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TF.TG.TH", name);

        options = EasyNameOptions.False with { UseTypeArguments = true };
        name = type.EasyName(options); Assert.Equal($"TH<>", name);

        options = options with { UseTypeName = true };
        name = type.EasyName(options); Assert.Equal($"TH<S>", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF<K, T>.TG.TH<S>", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TF<K, T>.TG.TH<S>", name);

        // Default...
        options = EasyNameOptions.Default;
        name = type.EasyName(options); Assert.Equal("TH<S>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_Bound_TwoNested_Generics()
    {
        var type = typeof(TF<byte, string>.TG.TH<int>);

        var options = EasyNameOptions.False;
        var name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseTypeName = true };
        name = type.EasyName(options); Assert.Equal("TH", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF.TG.TH", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TF.TG.TH", name);

        options = EasyNameOptions.False with { UseTypeArguments = true };
        name = type.EasyName(options); Assert.Equal($"TH<>", name);

        options = options with { UseTypeName = true };
        name = type.EasyName(options); Assert.Equal($"TH<Int32>", name);

        options = options with { UseTypeHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF<Byte, String>.TG.TH<Int32>", name);

        options = options with { UseTypeNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TF<System.Byte, System.String>.TG.TH<System.Int32>", name);

        // Default...
        options = EasyNameOptions.Default;
        name = type.EasyName(options); Assert.Equal("TH<Int32>", name);
    }
}
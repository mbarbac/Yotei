namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_EasyTypeNames
{
    const string NAMESPACE = "Yotei.Tools.Tests";
    const string CLASSNAME = nameof(Test_EasyTypeNames);

    // -----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_System_Types()
    {
        EasyTypeOptions options;
        Type type = typeof(string);
        string name = type.EasyName(); Assert.Equal("String", name);

        options = new EasyTypeOptions { UseHost = true };
        name = type.EasyName(options); Assert.Equal("String", name);

        options = new EasyTypeOptions { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal("System.String", name);
    }

    // -----------------------------------------------------

    public class TA { public class TB { } }

    [Enforced]
    [Fact]
    public static void Test_Simple_Type()
    {
        EasyTypeOptions options;
        Type type = typeof(TA);
        string name = type.EasyName(); Assert.Equal("TA", name);

        options = new EasyTypeOptions { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TA", name);

        options = new EasyTypeOptions { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TA", name);

        type = typeof(TA.TB);
        name = type.EasyName(); Assert.Equal("TB", name);

        options = new EasyTypeOptions { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TA.TB", name);

        options = new EasyTypeOptions { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TA.TB", name);

        options = new EasyTypeOptions { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TA.TB", name);

        options = new EasyTypeOptions { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TA.TB", name);
    }

    // -----------------------------------------------------

    public class TC<K, T> { }

    //[Enforced]
    [Fact]
    public static void Test_Type_Unbound_Generics()
    {
        EasyTypeOptions options;
        Type type = typeof(TC<,>);
        string name = type.EasyName(); Assert.Equal("TC", name);

        options = new EasyTypeOptions { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TC", name);

        options = new EasyTypeOptions { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TC", name);

        options = new EasyTypeOptions { UseArguments = true };
        name = type.EasyName(options); Assert.Equal("TC<,>", name);

        options = new EasyTypeOptions { UseArgumentNames = true };
        name = type.EasyName(options); Assert.Equal("TC<K, T>", name);

        options = options with { UseArgumentsHosts = true };
        name = type.EasyName(options); Assert.Equal("TC<K, T>", name);

        options = options with { UseArgumentsNamespaces = true };
        name = type.EasyName(options); Assert.Equal("TC<K, T>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_Bound_Generics()
    {
        EasyTypeOptions options;
        Type type = typeof(TC<int, string>);
        string name = type.EasyName(); Assert.Equal("TC", name);

        options = new EasyTypeOptions { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TC", name);

        options = new EasyTypeOptions { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TC", name);

        options = new EasyTypeOptions { UseArguments = true };
        name = type.EasyName(options); Assert.Equal("TC<,>", name);

        options = new EasyTypeOptions { UseArgumentNames = true };
        name = type.EasyName(options); Assert.Equal("TC<Int32, String>", name);

        options = options with { UseArgumentsHosts = true };
        name = type.EasyName(options); Assert.Equal("TC<Int32, String>", name);

        options = options with { UseArgumentsNamespaces = true };
        name = type.EasyName(options); Assert.Equal("TC<System.Int32, System.String>", name);
    }

    // -----------------------------------------------------

    public class TD<K, T> { public class TE<S> { } }

    //[Enforced]
    [Fact]
    public static void Test_Type_Unbound_Nested_Generics_No_Names()
    {
        EasyTypeOptions options;
        Type type = typeof(TD<,>.TE<>);
        string name = type.EasyName(); Assert.Equal("TE", name);

        options = new EasyTypeOptions { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD.TE", name);

        options = new EasyTypeOptions { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TD.TE", name);

        options = new EasyTypeOptions { UseArguments = true };
        name = type.EasyName(options); Assert.Equal("TE<>", name);

        options = options with { UseHost = true, UseArgumentsHosts = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD<,>.TE<>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_Unbound_Nested_Generics_With_Names()
    {
        EasyTypeOptions options;
        Type type = typeof(TD<,>.TE<>);
        string name = type.EasyName(); Assert.Equal("TE", name);

        options = new EasyTypeOptions { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TD.TE", name);

        options = new EasyTypeOptions { UseArgumentNames = true };
        name = type.EasyName(options); Assert.Equal("TE<S>", name);

        options = options with { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD<K, T>.TE<S>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_Bound_Nested_Generics_No_Names()
    {
        EasyTypeOptions options;
        Type type = typeof(TD<int, long>.TE<byte>);
        string name = type.EasyName(); Assert.Equal("TE", name);

        options = new EasyTypeOptions { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD.TE", name);

        options = new EasyTypeOptions { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TD.TE", name);

        options = new EasyTypeOptions { UseArguments = true };
        name = type.EasyName(options); Assert.Equal("TE<>", name);

        options = options with { UseHost = true, UseArgumentsHosts = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD<,>.TE<>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_Bound_Nested_Generics_With_Names()
    {
        EasyTypeOptions options;
        Type type = typeof(TD<int, long>.TE<byte>);
        string name = type.EasyName(); Assert.Equal("TE", name);

        options = new EasyTypeOptions { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TD.TE", name);

        options = new EasyTypeOptions { UseArgumentNames = true };
        name = type.EasyName(options); Assert.Equal("TE<Byte>", name);

        options = options with { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD<Int32, Int64>.TE<Byte>", name);
    }

    // -----------------------------------------------------

    public class TF<K, T> { public class TG { public class TH<S> { } } }

    //[Enforced]
    [Fact]
    public static void Test_Type_Unbound_TwoNested_Generics_No_Names()
    {
        EasyTypeOptions options;
        Type type = typeof(TF<,>.TG.TH<>);
        string name = type.EasyName(); Assert.Equal("TH", name);

        options = new EasyTypeOptions { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF.TG.TH", name);

        options = new EasyTypeOptions { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TF.TG.TH", name);

        options = new EasyTypeOptions { UseArguments = true };
        name = type.EasyName(options); Assert.Equal("TH<>", name);

        options = new EasyTypeOptions { UseArguments = true, UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF<,>.TG.TH<>", name);

        options = new EasyTypeOptions { UseArguments = true, UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TF<,>.TG.TH<>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_Unbound_TwoNested_Generics_With_Names()
    {
        EasyTypeOptions options;
        Type type = typeof(TF<,>.TG.TH<>);
        string name = type.EasyName(); Assert.Equal("TH", name);

        options = new EasyTypeOptions { UseArgumentNames = true };
        name = type.EasyName(options); Assert.Equal("TH<S>", name);

        options = options with { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF<K, T>.TG.TH<S>", name);

        options = options with { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TF<K, T>.TG.TH<S>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_Bound_TwoNested_Generics_No_Names()
    {
        EasyTypeOptions options;
        Type type = typeof(TF<,>.TG.TH<>);
        string name = type.EasyName(); Assert.Equal("TH", name);

        options = new EasyTypeOptions() { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF.TG.TH", name);

        options = new EasyTypeOptions { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TF.TG.TH", name);

        options = new EasyTypeOptions { UseArguments = true };
        name = type.EasyName(options); Assert.Equal("TH<>", name);

        options = options with { UseHost = true, UseArgumentsHosts = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF<,>.TG.TH<>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_Bound_TwoNested_Generics_With_Names()
    {
        EasyTypeOptions options;
        Type type = typeof(TF<int, long>.TG.TH<byte>);
        string name = type.EasyName(); Assert.Equal("TH", name);

        options = new EasyTypeOptions { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TF.TG.TH", name);

        options = new EasyTypeOptions { UseArgumentNames = true };
        name = type.EasyName(options); Assert.Equal("TH<Byte>", name);

        options = options with { UseHost = true, UseArgumentsHosts = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF<Int32, Int64>.TG.TH<Byte>", name);
    }
}
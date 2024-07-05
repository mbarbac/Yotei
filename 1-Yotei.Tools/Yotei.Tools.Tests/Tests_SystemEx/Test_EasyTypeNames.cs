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
        Type type = typeof(string);

        EasyTypeOptions options = EasyTypeOptions.Empty;
        string name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = type.EasyName(options); Assert.Equal("String", name);

        options = new EasyTypeOptions { UseHost = true };
        name = type.EasyName(options); Assert.Equal("String", name);

        options = new EasyTypeOptions { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal("System.String", name);
    }

    // -----------------------------------------------------

    public class TA { public class TB { } }

    //[Enforced]
    [Fact]
    public static void Test_Simple_Type()
    {
        Type type = typeof(TA);
        
        EasyTypeOptions options = EasyTypeOptions.Empty;
        string name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = type.EasyName(options); Assert.Equal("TA", name);

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
        Type type = typeof(TC<,>);
        
        EasyTypeOptions options = EasyTypeOptions.Empty;
        string name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = type.EasyName(options); Assert.Equal("TC", name);

        options = options with { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TC", name);

        options = options with { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TC", name);

        options = EasyTypeOptions.Empty with { UseName = true, UseTypeArguments = true };
        name = type.EasyName(options); Assert.Equal("TC<,>", name);

        options = options with { UseTypeArgumentsNames = true };
        name = type.EasyName(options); Assert.Equal("TC<K, T>", name);

        options = options with { UseTypeArgumentsHosts = true };
        name = type.EasyName(options); Assert.Equal("TC<K, T>", name);

        options = options with { UseTypeArgumentsNamespaces = true };
        name = type.EasyName(options); Assert.Equal("TC<K, T>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_Bound_Generics()
    {
        Type type = typeof(TC<int, string>);
        
        EasyTypeOptions options = EasyTypeOptions.Empty;
        string name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = type.EasyName(options); Assert.Equal("TC", name);

        options = options with { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TC", name);

        options = options with { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TC", name);

        options = EasyTypeOptions.Empty with { UseName = true, UseTypeArguments = true };
        name = type.EasyName(options); Assert.Equal("TC<,>", name);

        options = options with { UseTypeArgumentsNames = true };
        name = type.EasyName(options); Assert.Equal("TC<Int32, String>", name);

        options = options with { UseTypeArgumentsHosts = true };
        name = type.EasyName(options); Assert.Equal("TC<Int32, String>", name);

        options = options with { UseTypeArgumentsNamespaces = true };
        name = type.EasyName(options); Assert.Equal("TC<System.Int32, System.String>", name);
    }

    // -----------------------------------------------------

    public class TD<K, T> { public class TE<S> { } }

    //[Enforced]
    [Fact]
    public static void Test_Type_Unbound_Nested_Generics()
    {
        Type type = typeof(TD<,>.TE<>);
        
        EasyTypeOptions options = EasyTypeOptions.Empty;
        string name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = type.EasyName(options); Assert.Equal("TE", name);

        options = options with { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD.TE", name);

        options = options with { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TD.TE", name);

        options = EasyTypeOptions.Empty with { UseName = true, UseTypeArguments = true };
        name = type.EasyName(options); Assert.Equal("TE<>", name);

        options = options with { UseTypeArgumentsNames = true };
        name = type.EasyName(options); Assert.Equal("TE<S>", name);

        options = options with { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD<K, T>.TE<S>", name);

        options = options with { UseTypeArgumentsNamespaces = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD<K, T>.TE<S>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_Bound_Nested_Generics()
    {
        Type type = typeof(TD<int, long>.TE<byte>);
        
        EasyTypeOptions options = EasyTypeOptions.Empty;
        string name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = type.EasyName(options); Assert.Equal("TE", name);

        options = options with { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD.TE", name);

        options = options with { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TD.TE", name);

        options = EasyTypeOptions.Empty with { UseName = true, UseTypeArguments = true };
        name = type.EasyName(options); Assert.Equal("TE<>", name);

        options = options with { UseTypeArgumentsNames = true };
        name = type.EasyName(options); Assert.Equal("TE<Byte>", name);

        options = options with { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD<Int32, Int64>.TE<Byte>", name);

        options = options with { UseTypeArgumentsNamespaces = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TD<System.Int32, System.Int64>.TE<System.Byte>", name);
    }

    // -----------------------------------------------------

    public class TF<K, T> { public class TG { public class TH<S> { } } }

    //[Enforced]
    [Fact]
    public static void Test_Type_Unbound_TwoNested_Generics()
    {
        Type type = typeof(TF<,>.TG.TH<>);
        
        EasyTypeOptions options = EasyTypeOptions.Empty;
        string name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = type.EasyName(options); Assert.Equal("TH", name);

        options = options with { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF.TG.TH", name);

        options = options with { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TF.TG.TH", name);

        options = EasyTypeOptions.Empty with { UseName = true, UseTypeArguments = true };
        name = type.EasyName(options); Assert.Equal("TH<>", name);

        options = options with { UseTypeArgumentsNames = true };
        name = type.EasyName(options); Assert.Equal("TH<S>", name);

        options = options with { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF<K, T>.TG.TH<S>", name);

        options = options with { UseTypeArgumentsNamespaces = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF<K, T>.TG.TH<S>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_Bound_TwoNested_Generics()
    {
        Type type = typeof(TF<int, long>.TG.TH<byte>);
        
        EasyTypeOptions options = EasyTypeOptions.Empty;
        string name = type.EasyName(options); Assert.Equal("", name);

        options = options with { UseName = true };
        name = type.EasyName(options); Assert.Equal("TH", name);

        options = options with { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF.TG.TH", name);

        options = options with { UseNamespace = true };
        name = type.EasyName(options); Assert.Equal($"{NAMESPACE}.{CLASSNAME}.TF.TG.TH", name);

        options = EasyTypeOptions.Empty with { UseName = true, UseTypeArguments = true };
        name = type.EasyName(options); Assert.Equal("TH<>", name);

        options = options with { UseTypeArgumentsNames = true };
        name = type.EasyName(options); Assert.Equal("TH<Byte>", name);

        options = options with { UseHost = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF<Int32, Int64>.TG.TH<Byte>", name);

        options = options with { UseTypeArgumentsNamespaces = true };
        name = type.EasyName(options); Assert.Equal($"{CLASSNAME}.TF<System.Int32, System.Int64>.TG.TH<System.Byte>", name);
    }
}
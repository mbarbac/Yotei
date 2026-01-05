namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
////[Enforced]
public static class Test_EasyName_Indexer
{
    const string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    const string TESTNAME = nameof(Test_EasyName_Indexer);

    readonly static EasyNamePropertyOptions DEFAULT = EasyNamePropertyOptions.Default;
    readonly static EasyNamePropertyOptions FULL = EasyNamePropertyOptions.Full;

    // ----------------------------------------------------

    public class T0A { public class T0B { public string? this[int? one, string? two] => default!; } }

    //[Enforced]
    [Fact]
    public static void Test0_Standard()
    {
        EasyNamePropertyOptions options;
        string name;
        var type = typeof(T0A.T0B);
        var item = type.GetProperty("Item");
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = DEFAULT with { UseBrackets = true };
        name = item.EasyName(options); Assert.Equal("this[]", name);

        options = options with { ParameterOptions = EasyNameParameterOptions.Empty };
        name = item.EasyName(options); Assert.Equal("this[,]", name);

        options = options with { ParameterOptions = EasyNameParameterOptions.Default };
        name = item.EasyName(options); Assert.Equal("this[Int32?, String?]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.String {NAMESPACE}.{TESTNAME}." +
            "T0A.T0B.Item[System.Nullable<System.Int32> one, System.String? two]",
            name);
    }

    public class T0C { public class T0D { public IsNullable<string> this[int? one, string? two] => default!; } }

    //[Enforced]
    [Fact]
    public static void Test0_Standard_NullableReturnType()
    {
        EasyNamePropertyOptions options;
        string name;
        var type = typeof(T0C.T0D);
        var item = type.GetProperty("Item");
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = DEFAULT with
        { ReturnTypeOptions = EasyNameTypeOptions.Default with { UseNullability = true } };
        name = item.EasyName(options); Assert.Equal("String? this", name);
    }

    // ----------------------------------------------------

    public class T1A { [IndexerName("MyItems")] public string this[int? one, string? two] => default!; }

    //[Enforced]
    [Fact]
    public static void Test1_CustomIndexerName()
    {
        EasyNamePropertyOptions options;
        string name;
        var type = typeof(T1A);
        var item = type.GetProperty("MyItems");
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = DEFAULT with { UseBrackets = true };
        name = item.EasyName(options); Assert.Equal("this[]", name);

        options = options with { UseTechName = true };
        name = item.EasyName(options); Assert.Equal("MyItems[]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.String {NAMESPACE}.{TESTNAME}." +
            "T1A.MyItems[System.Nullable<System.Int32> one, System.String? two]",
            name);
    }

    // ----------------------------------------------------

    public class T2A<K, T> { public class T2B<S> { public K this[T? one, S two] => default!; } }

    //[Enforced]
    [Fact]
    public static void Test2_Generic_Unbound()
    {
        EasyNamePropertyOptions options;
        string name;
        var type = typeof(T2A<,>.T2B<>);
        var item = type.GetProperty("Item");
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = DEFAULT with { UseTechName = true };
        name = item.EasyName(options); Assert.Equal("Item", name);

        options = DEFAULT with { ParameterOptions = EasyNameParameterOptions.Default };
        name = item.EasyName(options); Assert.Equal("this[T?, S]", name);

        options = options with { HostTypeOptions = EasyNameTypeOptions.Default with { UseGenericArguments = true } };
        name = item.EasyName(options); Assert.Equal("T2B<S>.this[T?, S]", name);

        options = options with { ReturnTypeOptions = EasyNameTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("K T2B<S>.this[T?, S]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"K {NAMESPACE}.{TESTNAME}.T2A<K, T>.T2B<S>.Item[T? one, S two]", name);
    }

    //[Enforced]
    [Fact]
    public static void Test2_Generic_Bound()
    {
        EasyNamePropertyOptions options;
        string name;
        var type = typeof(T2A<byte, int?>.T2B<string?>);
        var item = type.GetProperty("Item");
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = DEFAULT with { UseTechName = true };
        name = item.EasyName(options); Assert.Equal("Item", name);

        options = DEFAULT with { ParameterOptions = EasyNameParameterOptions.Default };
        name = item.EasyName(options); Assert.Equal("this[Int32?, String]", name);

        options = options with { HostTypeOptions = EasyNameTypeOptions.Default with { UseGenericArguments = true } };
        name = item.EasyName(options); Assert.Equal("T2B<String>.this[Int32?, String]", name);

        options = options with { ReturnTypeOptions = EasyNameTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("Byte T2B<String>.this[Int32?, String]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Byte {NAMESPACE}.{TESTNAME}." +
            "T2A<System.Byte, System.Nullable<System.Int32>>.T2B<System.String>." +
            "Item[System.Nullable<System.Int32> one, System.String two]",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test2_Generic_Bound_WithNullableReferenceType()
    {
        EasyNamePropertyOptions options;
        string name;
        var type = typeof(T2A<byte, int?>.T2B<IsNullable<string?>>);
        var item = type.GetProperty("Item");
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = DEFAULT with { UseTechName = true };
        name = item.EasyName(options); Assert.Equal("Item", name);

        options = DEFAULT with { ParameterOptions = EasyNameParameterOptions.Default };
        name = item.EasyName(options); Assert.Equal("this[Int32?, String?]", name);

        options = options with { HostTypeOptions = EasyNameTypeOptions.Default with { UseGenericArguments = true } };
        name = item.EasyName(options); Assert.Equal("T2B<String?>.this[Int32?, String?]", name);

        options = options with { ReturnTypeOptions = EasyNameTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("Byte T2B<String?>.this[Int32?, String?]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Byte {NAMESPACE}.{TESTNAME}." +
            "T2A<System.Byte, System.Nullable<System.Int32>>.T2B<Yotei.Tools.IsNullable<System.String>>." +
            "Item[System.Nullable<System.Int32> one, Yotei.Tools.IsNullable<System.String> two]",
            name);
    }
}
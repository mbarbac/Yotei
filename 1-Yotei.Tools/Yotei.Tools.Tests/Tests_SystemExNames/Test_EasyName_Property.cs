#pragma warning disable CA1822

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
////[Enforced]
public static class Test_EasyName_Property
{
    const string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    const string TESTNAME = nameof(Test_EasyName_Property);

    readonly static EasyNamePropertyOptions DEFAULT = EasyNamePropertyOptions.Default;
    readonly static EasyNamePropertyOptions FULL = EasyNamePropertyOptions.Full;

    // ----------------------------------------------------

    public class T0A { public class T0B { public string? Name => default; } }

    //[Enforced]
    [Fact]
    public static void Test0_Standard()
    {
        EasyNamePropertyOptions options;
        string name;
        var type = typeof(T0A.T0B);
        var item = type.GetProperty("Name");
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { UseBrackets = true };
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = options with { ParameterOptions = EasyNameParameterOptions.Empty };
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = options with { ParameterOptions = EasyNameParameterOptions.Default };
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { ReturnTypeOptions = EasyNameTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("String Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"System.String {NAMESPACE}.{TESTNAME}.T0A.T0B.Name", name);
    }

    // ----------------------------------------------------

    public class T1A<K, T> { public class T1B<S> { public K Name => default!; } }

    //[Enforced]
    [Fact]
    public static void Test2_Generic_Unbound()
    {
        EasyNamePropertyOptions options;
        string name;
        var type = typeof(T1A<,>.T1B<>);
        var item = type.GetProperty("Name");
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { UseTechName = true };
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { ParameterOptions = EasyNameParameterOptions.Default };
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = options with { HostTypeOptions = EasyNameTypeOptions.Default with { UseGenericArguments = true } };
        name = item.EasyName(options); Assert.Equal("T1B<S>.Name", name);

        options = options with { ReturnTypeOptions = EasyNameTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("K T1B<S>.Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"K {NAMESPACE}.{TESTNAME}.T1A<K, T>.T1B<S>.Name", name);
    }

    //[Enforced]
    [Fact]
    public static void Test2_Generic_Bound()
    {
        EasyNamePropertyOptions options;
        string name;
        var type = typeof(T1A<byte, int?>.T1B<string?>);
        var item = type.GetProperty("Name");
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { UseTechName = true };
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { ParameterOptions = EasyNameParameterOptions.Default };
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = options with { HostTypeOptions = EasyNameTypeOptions.Default with { UseGenericArguments = true } };
        name = item.EasyName(options); Assert.Equal("T1B<String>.Name", name);

        options = options with { ReturnTypeOptions = EasyNameTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("Byte T1B<String>.Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Byte {NAMESPACE}.{TESTNAME}." +
            "T1A<System.Byte, System.Nullable<System.Int32>>.T1B<System.String>." +
            "Name",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test2_Generic_Bound_WithNullableReferenceType()
    {
        EasyNamePropertyOptions options;
        string name;
        var type = typeof(T1A<byte, int?>.T1B<IsNullable<string?>>);
        var item = type.GetProperty("Name");
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { UseTechName = true };
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { ParameterOptions = EasyNameParameterOptions.Default };
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = options with { HostTypeOptions = EasyNameTypeOptions.Default with { UseGenericArguments = true } };
        name = item.EasyName(options); Assert.Equal("T1B<String?>.Name", name);

        options = options with { ReturnTypeOptions = EasyNameTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("Byte T1B<String?>.Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Byte {NAMESPACE}.{TESTNAME}." +
            "T1A<System.Byte, System.Nullable<System.Int32>>.T1B<Yotei.Tools.IsNullable<System.String>>." +
            "Name",
            name);
    }
}
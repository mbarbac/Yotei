namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
////[Enforced]
public static class Test_EasyName_Field
{
    const string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    const string TESTNAME = nameof(Test_EasyName_Field);

    readonly static EasyNameFieldInfo DEFAULT = EasyNameFieldInfo.Default;
    readonly static EasyNameFieldInfo FULL = EasyNameFieldInfo.Full;

    // ----------------------------------------------------

    public class T0A { public class T0B { public string? Name; } }

    //[Enforced]
    [Fact]
    public static void Test0_Standard()
    {
        EasyNameFieldInfo options;
        string name;
        var type = typeof(T0A.T0B);
        var item = type.GetField("Name");
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { ReturnTypeOptions = EasyNameType.Default };
        name = item.EasyName(options); Assert.Equal("String Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"System.String {NAMESPACE}.{TESTNAME}.T0A.T0B.Name", name);
    }

    // ----------------------------------------------------

    public class T1A<K, T> { public class T1B<S> { public K Name = default!; } }

    //[Enforced]
    [Fact]
    public static void Test2_Generic_Unbound()
    {
        EasyNameFieldInfo options;
        string name;
        var type = typeof(T1A<,>.T1B<>);
        var item = type.GetField("Name");
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = options with { HostTypeOptions = EasyNameType.Default with { UseGenericArguments = true } };
        name = item.EasyName(options); Assert.Equal("T1B<S>.Name", name);

        options = options with { ReturnTypeOptions = EasyNameType.Default };
        name = item.EasyName(options); Assert.Equal("K T1B<S>.Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"K {NAMESPACE}.{TESTNAME}.T1A<K, T>.T1B<S>.Name", name);
    }

    //[Enforced]
    [Fact]
    public static void Test2_Generic_Bound()
    {
        EasyNameFieldInfo options;
        string name;
        var type = typeof(T1A<byte, int?>.T1B<string?>);
        var item = type.GetField("Name");
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = options with { HostTypeOptions = EasyNameType.Default with { UseGenericArguments = true } };
        name = item.EasyName(options); Assert.Equal("T1B<String>.Name", name);

        options = options with { ReturnTypeOptions = EasyNameType.Default };
        name = item.EasyName(options); Assert.Equal("Byte T1B<String>.Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Byte {NAMESPACE}.{TESTNAME}." +
            "T1A<System.Byte, System.Nullable<System.Int32>>.T1B<System.String>." +
            "Name",
            name);
    }
}
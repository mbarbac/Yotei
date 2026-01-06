namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
////[Enforced]
public static class Test_EasyName_Method
{
    const string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    const string TESTNAME = nameof(Test_EasyName_Method);

    readonly static EasyNameMethodInfo DEFAULT = EasyNameMethodInfo.Default;
    readonly static EasyNameMethodInfo FULL = EasyNameMethodInfo.Full;

    // ----------------------------------------------------

    public interface IFace0 { void Name(byte one, out int? two, ref string? three, in long four); }

    //[Enforced]
    [Fact]
    public static void Test0_Standard()
    {
        EasyNameMethodInfo options;
        string name;
        var type = typeof(IFace0);
        var item = type.GetMethod("Name");
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { UseBrackets = true };
        name = item.EasyName(options); Assert.Equal("Name()", name);

        options = DEFAULT with { ParameterOptions = EasyNameParameterInfo.Empty };
        name = item.EasyName(options); Assert.Equal("Name(,,,)", name);

        options = DEFAULT with { ParameterOptions = EasyNameParameterInfo.Default };
        name = item.EasyName(options);
        Assert.Equal("Name(Byte, out Int32?, ref String?, in Int64)", name);

        options = DEFAULT with { ParameterOptions = new() { UseName = true } };
        name = item.EasyName(options);
        Assert.Equal("Name(Byte one, out Int32? two, ref String? three, in Int64 four)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Void {NAMESPACE}.{TESTNAME}." +
            "IFace0.Name(System.Byte one, out System.Nullable<System.Int32> two, ref System.String? three, in System.Int64 four)",
            name);
    }

    // ----------------------------------------------------

    public interface I1A<K, T> { public interface I1B<S> { public K Name<V>(ref T one, S? two); } }

    //[Enforced]
    [Fact]
    public static void Test1_Generic_Unbound()
    {
        EasyNameMethodInfo options;
        string name;
        var type = typeof(I1A<,>.I1B<>);
        var item = type.GetMethod("Name");
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { GenericArgumentOptions = new() };
        name = item.EasyName(options); Assert.Equal("Name<V>", name);

        options = options with { ParameterOptions = new() };
        name = item.EasyName(options); Assert.Equal("Name<V>(ref T, S?)", name);

        options = options with { HostTypeOptions = new() };
        name = item.EasyName(options);
        Assert.Equal("I1B.Name<V>(ref T, S?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"K {NAMESPACE}.{TESTNAME}.I1A<K, T>.I1B<S>.Name<V>(ref T one, S? two)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test1_Generic_Bound()
    {
        EasyNameMethodInfo options;
        string name;
        var type = typeof(I1A<byte, int>.I1B<IsNullable<string>>);
        var item = type.GetMethod("Name");
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { GenericArgumentOptions = new() };
        name = item.EasyName(options); Assert.Equal("Name<V>", name);

        options = options with { ParameterOptions = new() };
        name = item.EasyName(options); Assert.Equal("Name<V>(ref Int32, String?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Byte {NAMESPACE}.{TESTNAME}." +
            "I1A<System.Byte, System.Int32>.I1B<Yotei.Tools.IsNullable<System.String>>." +
            "Name<V>(ref System.Int32 one, Yotei.Tools.IsNullable<System.String> two)",
            name);
    }
}
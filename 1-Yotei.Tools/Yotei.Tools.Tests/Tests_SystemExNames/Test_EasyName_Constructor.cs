#pragma warning disable IDE0060
#pragma warning disable CS9113

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
////[Enforced]
public static class Test_EasyName_Constructor
{
    const string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    const string TESTNAME = nameof(Test_EasyName_Constructor);

    readonly static EasyNameConstructorInfo DEFAULT = EasyNameConstructorInfo.Default;
    readonly static EasyNameConstructorInfo FULL = EasyNameConstructorInfo.Full;

    // ----------------------------------------------------

    public class T0A
    { public class T0B { public T0B(ref int one, out string? two) { two = default!; } } }

    //[Enforced]
    [Fact]
    public static void Test0_Standard()
    {
        EasyNameConstructorInfo options;
        string name;
        var type = typeof(T0A.T0B);
        var item = type.GetConstructor([typeof(int).MakeByRefType(), typeof(string).MakeByRefType()]);
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("new", name);

        options = DEFAULT with { UseBrackets = true };
        name = item.EasyName(options); Assert.Equal("new()", name);

        options = DEFAULT with { ParameterOptions = EasyNameParameterInfo.Empty };
        name = item.EasyName(options); Assert.Equal("new(,)", name);

        options = DEFAULT with { UseTechName = true };
        name = item.EasyName(options); Assert.Equal(".ctor", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}." +
            "T0A.T0B..ctor(ref System.Int32 one, out System.String? two)",
            name);
    }

    public class T0C { public class T0D { static T0D() { } } }

    //[Enforced]
    [Fact]
    public static void Test0_Standard_Static()
    {
        EasyNameConstructorInfo options;
        string name;
        var type = typeof(T0C.T0D);
        var item = type.TypeInitializer;
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("T0D", name);

        options = DEFAULT with { UseBrackets = true };
        name = item.EasyName(options); Assert.Equal("T0D()", name);

        options = DEFAULT with { ParameterOptions = EasyNameParameterInfo.Empty };
        name = item.EasyName(options); Assert.Equal("T0D()", name);

        options = DEFAULT with { UseTechName = true };
        name = item.EasyName(options); Assert.Equal(".cctor", name);

        options = options with { HostTypeOptions = EasyNameType.Default };
        name = item.EasyName(options); Assert.Equal("T0D..cctor", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTNAME}.T0C.T0D..cctor()", name);
    }

    // ----------------------------------------------------

    public class T1A<K, T> { public class T1B<S> { public T1B(ref T? one, out S? two) { two = default!; } } }

    //[Enforced]
    [Fact]
    public static void Test0_Generic_Unbound()
    {
        EasyNameConstructorInfo options;
        string name;
        var type = typeof(T1A<,>.T1B<>);
        var item = type.GetConstructors().Where(x => x.GetParameters().Length == 2).Single();
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("new", name);

        options = DEFAULT with { UseBrackets = true };
        name = item.EasyName(options); Assert.Equal("new()", name);

        options = DEFAULT with { ParameterOptions = EasyNameParameterInfo.Empty };
        name = item.EasyName(options); Assert.Equal("new(,)", name);

        options = DEFAULT with { UseTechName = true };
        name = item.EasyName(options); Assert.Equal(".ctor", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}." +
            "T1A<K, T>.T1B<S>..ctor(ref T? one, out S? two)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test0_Generic_Bound()
    {
        EasyNameConstructorInfo options;
        string name;
        var type = typeof(T1A<byte, int?>.T1B<string?>);
        var item = type.GetConstructors().Where(x => x.GetParameters().Length == 2).Single();
        Assert.NotNull(item);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("new", name);

        options = DEFAULT with { UseBrackets = true };
        name = item.EasyName(options); Assert.Equal("new()", name);

        options = DEFAULT with { ParameterOptions = EasyNameParameterInfo.Empty };
        name = item.EasyName(options); Assert.Equal("new(,)", name);

        options = DEFAULT with { UseTechName = true };
        name = item.EasyName(options); Assert.Equal(".ctor", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}." +
            "T1A<System.Byte, System.Nullable<System.Int32>>.T1B<System.String>." +
            ".ctor(ref System.Nullable<System.Int32> one, out System.String? two)",
            name);
    }
}
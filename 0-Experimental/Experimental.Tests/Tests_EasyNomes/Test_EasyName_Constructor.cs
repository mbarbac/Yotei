#pragma warning disable CS9113

using Experimental;

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyName_Constructor
{
    const string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    const string CLASSNAME = nameof(Test_EasyName_Constructor);

    readonly static EasyNameOptions EMPTY = EasyNameOptions.Empty;
    readonly static EasyNameOptions DEFAULT = EasyNameOptions.Default;
    readonly static EasyNameOptions FULL = EasyNameOptions.Full;

    // ----------------------------------------------------

    public class T0A { public class T0B(byte one, int two) { } }

    //[Enforced]
    [Fact]
    public static void Test0_Standard()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T0A.T0B);
        var item = type.GetConstructor([typeof(byte), typeof(int)])!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("new", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("new(Byte, Int32)", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("new(Byte, Int32)", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T0B.new(Byte, Int32)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.T0A.T0B..ctor(System.Byte one, System.Int32 two)",
            name);
    }

    // ----------------------------------------------------

    public class T1A<K, T> { public class T1B<S>(T? one, S two) { } }

    //[Enforced]
    [Fact]
    public static void TesT1_Generic_Unbound()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T1A<,>.T1B<>);
        var item = type.GetConstructors().Where(x => x.GetParameters().Length == 2).Single();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("new", name);

        options = EMPTY with { ConstructorTechName = true };
        name = item.EasyName(options); Assert.Equal(".ctor", name);

        options = EMPTY with { MemberHostTypeOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("T1B.new", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("new(T?, S)", name);

        options = DEFAULT with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options); Assert.Equal("new(T?, S)", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T1B<S>.new(T?, S)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.T1A<K, T>.T1B<S>..ctor(T? one, S two)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void TesT1_Generic_Bound()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T1A<byte, int>.T1B<string?>);
        var item = type.GetConstructors().Where(x => x.GetParameters().Length == 2).Single();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("new", name);

        options = EMPTY with { ConstructorTechName = true };
        name = item.EasyName(options); Assert.Equal(".ctor", name);

        options = EMPTY with { MemberHostTypeOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("T1B.new", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("new(Int32, String?)", name);

        options = DEFAULT with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options); Assert.Equal("new(Int32, String?)", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T1B<String>.new(Int32, String?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.T1A<System.Byte, System.Int32>.T1B<System.String>" +
            "..ctor(System.Int32 one, System.String? two)",
            name);
    }
}
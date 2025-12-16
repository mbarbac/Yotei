#pragma warning disable CS9113

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

    public class T1A { public class T1B(byte one, int two) { } }

    //[Enforced]
    [Fact]
    public static void Test1_Standard()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T1A.T1B);
        var item = type.GetConstructor([typeof(byte), typeof(int)])!;

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("new", name);

        options = EMPTY with { ConstructorTechName = true };
        name = item.EasyName(options); Assert.Equal(".ctor", name);

        options = EMPTY with { MemberUseParameters = true };
        name = item.EasyName(options);
        Assert.Equal("new(,)", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("new(Byte, Int32)", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("new(Byte, Int32)", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T1B.new(Byte, Int32)", name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.T1A.T1B..ctor(System.Byte one, System.Int32 two)",
            name);
    }

    // ----------------------------------------------------

    public class T2A<K, T> { public class T2B<S>(T one, S two) { } }

    //[Enforced]
    [Fact]
    public static void Test2_Generic_Unbound()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T2A<,>.T2B<>);
        var item = type.GetConstructors().Where(x => x.GetParameters().Length == 2).Single();

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("new", name);

        options = EMPTY with { ConstructorTechName = true };
        name = item.EasyName(options); Assert.Equal(".ctor", name);

        options = EMPTY with { MemberUseParameters = true };
        name = item.EasyName(options);
        Assert.Equal("new(,)", name);

        options = EMPTY with { MemberHostTypeOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("T2B.new", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("new(T, S)", name);

        options = DEFAULT with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options); Assert.Equal("new(T, S)", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T2B<S>.new(T, S)", name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.T2A<K, T>.T2B<S>..ctor(T one, S two)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test2_Generic_Bound()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T2A<byte, int>.T2B<string>);
        var item = type.GetConstructors().Where(x => x.GetParameters().Length == 2).Single();

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("new", name);

        options = EMPTY with { ConstructorTechName = true };
        name = item.EasyName(options); Assert.Equal(".ctor", name);

        options = EMPTY with { MemberUseParameters = true };
        name = item.EasyName(options);
        Assert.Equal("new(,)", name);

        options = EMPTY with { MemberHostTypeOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("T2B.new", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("new(Int32, String)", name);

        options = DEFAULT with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options); Assert.Equal("new(Int32, String)", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T2B<String>.new(Int32, String)", name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}.T2A<System.Byte, System.Int32>.T2B<System.String>..ctor(System.Int32 one, System.String two)",
            name);
    }
}
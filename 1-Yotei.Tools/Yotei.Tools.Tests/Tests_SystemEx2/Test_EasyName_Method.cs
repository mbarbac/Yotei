#pragma warning disable IDE0060
#pragma warning disable CA1822

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyName_Method
{
    const string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    const string CLASSNAME = nameof(Test_EasyName_Method);

    readonly static EasyNameOptions EMPTY = EasyNameOptions.Empty;
    readonly static EasyNameOptions DEFAULT = EasyNameOptions.Default;
    readonly static EasyNameOptions FULL = EasyNameOptions.Full;

    // ----------------------------------------------------

    public class T1A { public class T1B { public void Name(byte one, int two) { } } }

    //[Enforced]
    [Fact]
    public static void Test1_Standard()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T1A.T1B);
        var item = type.GetMethod("Name")!;

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EMPTY with { MemberUseParameters = true };
        name = item.EasyName(options);
        Assert.Equal("Name(,)", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name(Byte, Int32)", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("Void Name(Byte, Int32)", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T1B.Name(Byte, Int32)", name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Void {NAMESPACE}.{CLASSNAME}.T1A.T1B.Name(System.Byte one, System.Int32 two)",
            name);
    }

    // ----------------------------------------------------

    public class T2A<K, T> { public class T2B<S> { public K Name<V>(T one, S two) => default!; } }

    //[Enforced]
    [Fact]
    public static void Test2_Generic_Unbound()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T2A<,>.T2B<>);
        var item = type.GetMethod("Name")!;

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EMPTY with { MemberUseParameters = true };
        name = item.EasyName(options);
        Assert.Equal("Name(,)", name);

        options = EMPTY with { ParameterTypeOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("Name(T, S)", name);

        options = options with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("K Name(T, S)", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name<V>(T, S)", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("K Name<V>(T, S)", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T2B<S>.Name<V>(T, S)", name);

        options = options with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("K T2B<S>.Name<V>(T, S)", name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"K {NAMESPACE}.{CLASSNAME}.T2A<K, T>.T2B<S>.Name<V>(T one, S two)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test2_Generic_Bound_Method_Unbound()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(T2A<byte, int>.T2B<string>);
        var item = type.GetMethod("Name")!;

        // Empty...
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EMPTY with { MemberUseParameters = true };
        name = item.EasyName(options);
        Assert.Equal("Name(,)", name);

        options = EMPTY with { ParameterTypeOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("Name(Int32, String)", name);

        options = options with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("Byte Name(Int32, String)", name);

        // Default...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name<V>(Int32, String)", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("Byte Name<V>(Int32, String)", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("T2B<String>.Name<V>(Int32, String)", name);

        options = options with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("Byte T2B<String>.Name<V>(Int32, String)", name);

        // Full...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Byte {NAMESPACE}.{CLASSNAME}.T2A<System.Byte, System.Int32>.T2B<System.String>.Name<V>(System.Int32 one, System.String two)",
            name);
    }
}
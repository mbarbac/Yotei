using Experimental;

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

    public interface I0A { void Name(byte one, out int? two, ref string? three, in long four); }

    //[Enforced]
    [Fact]
    public static void Test0_Standard()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(I0A);
        var item = type.GetMethod("Name")!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options);
        Assert.Equal("Name(Byte, out Int32?, ref String?, in Int64)", name);

        options = DEFAULT with { MemberUseArgumentNames = true };
        name = item.EasyName(options);
        Assert.Equal("Name(Byte one, out Int32? two, ref String? three, in Int64 four)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Void {NAMESPACE}.{CLASSNAME}.I0A.Name(" +
            "System.Byte one, out System.Int32? two, ref System.String? three, in System.Int64 four)",
            name);
    }

    // ----------------------------------------------------

    public interface I1A<K, T> { public interface I1B<S> { public K Name<V>(ref T one, S? two); } }

    //[Enforced]
    [Fact]
    public static void Test2_Generic_Unbound()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(I1A<,>.I1B<>);
        var item = type.GetMethod("Name")!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EMPTY with { MemberUseArgumentTypes = true };
        name = item.EasyName(options);
        Assert.Equal("Name(ref T, S)", name);

        options = options with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("K Name(ref T, S)", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name<V>(ref T, S?)", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("K Name<V>(ref T, S?)", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("I1B<S>.Name<V>(ref T, S?)", name);

        options = options with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("K I1B<S>.Name<V>(ref T, S?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"K {NAMESPACE}.{CLASSNAME}.I1A<K, T>.I1B<S>.Name<V>(ref T one, S? two)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test2_Generic_Bound()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(I1A<byte, int?>.I1B<string>);
        var item = type.GetMethod("Name")!;

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EMPTY with { MemberUseArgumentTypes = true };
        name = item.EasyName(options);
        Assert.Equal("Name(ref Nullable, String)", name);

        options = options with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options);
        Assert.Equal("Byte Name(ref Nullable, String)", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name<V>(ref Int32?, String?)", name);

        options = DEFAULT with { MemberReturnTypeOptions = EMPTY };
        name = item.EasyName(options); Assert.Equal("Byte Name<V>(ref Int32?, String?)", name);

        options = DEFAULT with { MemberHostTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("I1B<String>.Name<V>(ref Int32?, String?)", name);

        options = options with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options);
        Assert.Equal("Byte I1B<String>.Name<V>(ref Int32?, String?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Byte {NAMESPACE}.{CLASSNAME}.I1A<System.Byte, System.Int32?>.I1B<System.String>." +
            "Name<V>(ref System.Int32? one, System.String? two)",
            name);
    }
}
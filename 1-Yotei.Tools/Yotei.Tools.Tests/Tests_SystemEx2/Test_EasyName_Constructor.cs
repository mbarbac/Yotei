#pragma warning disable IDE0060

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyName_Constructor
{
    static readonly string NAMESPACE = typeof(Test_EasyName_Constructor).Namespace!;
    static readonly string TESTNAME = typeof(Test_EasyName_Constructor).Name;

    readonly static EasyNameOptions EMPTY = new();
    readonly static EasyNameOptions DEFAULT = EasyNameOptions.Default;
    readonly static EasyNameOptions FULL = EasyNameOptions.Full;

    // ----------------------------------------------------

    public class Type0A { public Type0A(ref int? one, out string? two) { two = default!; } }

    //[Enforced]
    [Fact]
    public static void Test0A_Standard()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(Type0A);
        var item = type.GetConstructors().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type0A", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type0A(ref Int32?, out String?)", name);

        var xoptions = EMPTY with { UseArgumentName = true };
        options = EMPTY with { MemberArgumentOptions = xoptions };
        name = item.EasyName(options); Assert.Equal("Type0A(one, two)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}.Type0A..ctor" +
            "(ref System.Nullable<System.Int32> one, out System.String? two)",
            name);
    }

    // ----------------------------------------------------

    public class Type0B { static Type0B() { } }

    //[Enforced]
    [Fact]
    public static void Test0B_Static()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(Type0B);
        var item = type.TypeInitializer; Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type0B", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type0B()", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTNAME}.Type0B..cctor()", name);
    }

    // ----------------------------------------------------

    public class Type1A<[IsNullable] K, [IsNullable] T>
    {
        public class Type1B<[IsNullable] S>
        { public Type1B(ref K one, in T two, out S three) { three = default!; } }
    }

    //[Enforced]
    [Fact]
    public static void Test1_Nullable_From_Outer()
    {
        EasyNameOptions options;
        string name;

        // Unbound...
        var type = typeof(Type1A<,>.Type1B<>);
        var item = type.GetConstructors().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type1B", name);

        options = DEFAULT;
        name = item.EasyName(options);
        Assert.Equal("Type1B<S?>(ref K?, in T?, out S?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}.Type1A<K?, T?>.Type1B<S?>..ctor" +
            "(ref K? one, in T? two, out S? three)",
            name);

        // Bound...
        type = typeof(Type1A<byte?, int?>.Type1B<string?>);
        item = type.GetConstructors().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type1B", name);

        options = DEFAULT;
        name = item.EasyName(options);
        Assert.Equal("Type1B<String>(ref Byte?, in Int32?, out String?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}." +
            "Type1A<System.Nullable<System.Byte>, System.Nullable<System.Int32>>." +
            "Type1B<System.String>..ctor" +
            "(ref System.Nullable<System.Byte> one, " +
            "in System.Nullable<System.Int32> two, out System.String? three)",
            name);
    }

    // ----------------------------------------------------

    public class Type2A<K, T>
    {
        public class Type2B<S>
        { public Type2B(ref K? one, in T? two, out S? three) { three = default!; } }
    }

    //[Enforced]
    [Fact]
    public static void Test2_Nullable_From_Inner()
    {
        EasyNameOptions options;
        string name;

        // Unbound...
        var type = typeof(Type2A<,>.Type2B<>);
        var item = type.GetConstructors().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type2B", name);

        options = DEFAULT;
        name = item.EasyName(options);
        Assert.Equal("Type2B<S>(ref K?, in T?, out S?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}.Type2A<K, T>.Type2B<S>..ctor" +
            "(ref K? one, in T? two, out S? three)",
            name);

        // Bound...
        type = typeof(Type2A<byte?, int?>.Type2B<string?>);
        item = type.GetConstructors().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type2B", name);

        options = DEFAULT;
        name = item.EasyName(options);
        Assert.Equal("Type2B<String>(ref Byte?, in Int32?, out String?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}." +
            "Type2A<System.Nullable<System.Byte>, System.Nullable<System.Int32>>." +
            "Type2B<System.String>..ctor" +
            "(ref System.Nullable<System.Byte> one, " +
            "in System.Nullable<System.Int32> two, out System.String? three)",
            name);
    }
}
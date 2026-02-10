namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyName_Method
{
    static readonly string NAMESPACE = typeof(Test_EasyName_Method).Namespace!;
    static readonly string TESTNAME = typeof(Test_EasyName_Method).Name;

    readonly static EasyNameOptions EMPTY = EasyNameOptions.Empty;
    readonly static EasyNameOptions DEFAULT = EasyNameOptions.Default;
    readonly static EasyNameOptions FULL = EasyNameOptions.Full;

    // ----------------------------------------------------

    public interface IFace0A { void Name(byte one, out int? two, ref string? three, in long four); }

    //[Enforced]
    [Fact]
    public static void Test0A_Standard()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(IFace0A);
        var item = type.GetMethod("Name"); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options);
        Assert.Equal("Name(Byte, out Int32?, ref String?, in Int64)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Void {NAMESPACE}.{TESTNAME}.IFace0A.Name(" +
            "System.Byte one, out System.Nullable<System.Int32> two, " +
            "ref System.String? three, in System.Int64 four)",
            name);
    }

    // ----------------------------------------------------

    public interface IFace0B { void Name([IsNullable] ref string? one); }

    //[Enforced]
    [Fact]
    public static void Test0B_NullableAttribute()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(IFace0B);
        var item = type.GetMethod("Name"); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name(ref String?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Void {NAMESPACE}.{TESTNAME}.IFace0B.Name(ref System.String? one)",
            name);
    }

    // ----------------------------------------------------

    public interface IFace0C { void Name(ref IsNullable<string?> one); }

    //[Enforced]
    [Fact]
    public static void Test0C_NullableWrapper()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(IFace0C);
        var item = type.GetMethod("Name"); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name(ref String?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Void {NAMESPACE}.{TESTNAME}.IFace0C." +
            "Name(ref Yotei.Tools.EasyNullable<System.String> one)",
            name);
    }

    // ----------------------------------------------------

    public interface IFace1A<[IsNullable] K, [IsNullable] T>
    { public interface IFace1B<S> { K Name<R>(ref T? one, S? two); } }

    //[Enforced]
    [Fact]
    public static void Test1_Nullable_From_Outer()
    {
        EasyNameOptions options;
        string name;

        // Unbound...
        var type = typeof(IFace1A<,>.IFace1B<>);
        var item = type.GetMethod("Name"); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name<R>(ref T?, S?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"K? {NAMESPACE}.{TESTNAME}.IFace1A<K?, T?>.IFace1B<S>." +
            "Name<R>(ref T? one, S? two)",
            name);

        // Bound...
        type = typeof(IFace1A<byte?, int?>.IFace1B<string?>);
        item = type.GetMethod("Name"); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name<R>(ref Int32?, String?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Nullable<System.Byte> {NAMESPACE}.{TESTNAME}." +
            "IFace1A<System.Nullable<System.Byte>, System.Nullable<System.Int32>>." +
            "IFace1B<System.String>." +
            "Name<R>(ref System.Nullable<System.Int32> one, System.String? two)",
            name);
    }

    // ----------------------------------------------------

    public interface IFace2A<K, T>
    { public interface IFace2B<S> { K Name<R>(ref T? one, S? two); } }

    //[Enforced]
    [Fact]
    public static void Test2_Nullable_From_Inner()
    {
        EasyNameOptions options;
        string name;

        // Unbound...
        var type = typeof(IFace2A<,>.IFace2B<>);
        var item = type.GetMethod("Name"); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name<R>(ref T?, S?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"K {NAMESPACE}.{TESTNAME}.IFace2A<K, T>.IFace2B<S>.Name<R>(ref T? one, S? two)",
            name);

        // Bound...
        type = typeof(IFace2A<byte?, int?>.IFace2B<string?>);
        item = type.GetMethod("Name"); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name<R>(ref Int32?, String?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Nullable<System.Byte> {NAMESPACE}.{TESTNAME}." +
            "IFace2A<System.Nullable<System.Byte>, System.Nullable<System.Int32>>." +
            "IFace2B<System.String>." +
            "Name<R>(ref System.Nullable<System.Int32> one, System.String? two)",
            name);
    }
}
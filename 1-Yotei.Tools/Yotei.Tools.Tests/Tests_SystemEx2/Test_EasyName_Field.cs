namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyName_Field
{
    static readonly string NAMESPACE = typeof(Test_EasyName_Field).Namespace!;
    static readonly string TESTNAME = typeof(Test_EasyName_Field).Name;

    readonly static EasyNameOptions EMPTY = EasyNameOptions.Empty;
    readonly static EasyNameOptions DEFAULT = EasyNameOptions.Default;
    readonly static EasyNameOptions FULL = EasyNameOptions.Full;

    // ----------------------------------------------------

    public class Type0A { public string? Name = default!; }

    //[Enforced]
    [Fact]
    public static void Test0A_Standard_Nullability()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(Type0A);
        var item = type.GetFields().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options); Assert.Equal("String? Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"System.String? {NAMESPACE}.{TESTNAME}.Type0A.Name", name);
    }

    // ----------------------------------------------------

    public class Type0B { [EasyNullable] public string? Name = default!; }

    //[Enforced]
    [Fact]
    public static void Test0B_Standard_Nullability_FromAttribute()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(Type0B);
        var item = type.GetFields().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options); Assert.Equal("String? Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"System.String? {NAMESPACE}.{TESTNAME}.Type0B.Name", name);
    }

    // ----------------------------------------------------

    public class Type0C { public EasyNullable<string?> Name = default!; }

    //[Enforced]
    [Fact]
    public static void Test0C_Standard_Nullability_FromWrapper()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(Type0C);
        var item = type.GetFields().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options); Assert.Equal("String? Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"Yotei.Tools.EasyNullable<System.String> {NAMESPACE}.{TESTNAME}.Type0C.Name",
            name);
    }

    // ----------------------------------------------------

    public class Type1A<[EasyNullable] K, [EasyNullable] T>
    {
        public class Type1B<[EasyNullable] S> { public S Name = default!; }
    }

    //[Enforced]
    [Fact]
    public static void Test1_Nullable_From_Outer()
    {
        EasyNameOptions options;
        string name;

        // Unbound
        var type = typeof(Type1A<,>.Type1B<>);
        var item = type.GetFields().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options); Assert.Equal("S? Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"S? {NAMESPACE}.{TESTNAME}.Type1A<K?, T?>.Type1B<S?>.Name",
            name);

        // Bound...
        type = typeof(Type1A<byte?, int?>.Type1B<string?>);
        item = type.GetFields().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options); Assert.Equal("String? Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.String? {NAMESPACE}.{TESTNAME}." +
            "Type1A<System.Nullable<System.Byte>, System.Nullable<System.Int32>>." +
            "Type1B<System.String>.Name",
            name);
    }

    // ----------------------------------------------------

    public class Type2A<K, T> { public class Type2B<S> { [EasyNullable] public S? Name = default!; } }

    //[Enforced]
    [Fact]
    public static void Test2_Nullable_From_Inner()
    {
        EasyNameOptions options;
        string name;

        // Unbound
        var type = typeof(Type2A<,>.Type2B<>);
        var item = type.GetFields().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options); Assert.Equal("S? Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"S? {NAMESPACE}.{TESTNAME}.Type2A<K, T>.Type2B<S>.Name",
            name);

        // Bound
        type = typeof(Type2A<byte?, int?>.Type2B<string?>);
        item = type.GetFields().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options); Assert.Equal("String? Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.String? {NAMESPACE}.{TESTNAME}." +
            "Type2A<System.Nullable<System.Byte>, System.Nullable<System.Int32>>." +
            "Type2B<System.String>.Name",
            name);
    }
}
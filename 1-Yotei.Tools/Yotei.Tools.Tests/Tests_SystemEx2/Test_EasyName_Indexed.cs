namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyName_Indexed
{
    static readonly string NAMESPACE = typeof(Test_EasyName_Indexed).Namespace!;
    static readonly string TESTNAME = typeof(Test_EasyName_Indexed).Name;

    readonly static EasyNameOptions EMPTY = EasyNameOptions.Empty;
    readonly static EasyNameOptions DEFAULT = EasyNameOptions.Default;
    readonly static EasyNameOptions FULL = EasyNameOptions.Full;

    // ----------------------------------------------------

    public class Type0A { public string? this[int? one, in string? two] => default!; }

    //[Enforced]
    [Fact]
    public static void Test0A_Standard_Nullability()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(Type0A);
        var item = type.GetProperties().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this[Int32?, in String?]", name);

        options = DEFAULT with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options); Assert.Equal("String? this[Int32?, in String?]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.String? {NAMESPACE}.{TESTNAME}." +
            "Type0A.Item[System.Nullable<System.Int32> one, in System.String? two]",
            name);
    }

    // ----------------------------------------------------

    public class Type0B
    {
        [IndexerName("MyItem")]
        [EasyNullable]
        public string? this[int? one, in string? two] => default!;
    }

    // Using '[EasyNullable]' on the property to make its return type nullable...
    //[Enforced]
    [Fact]
    public static void Test0B_Custom_Indexer_Name()
    {
        EasyNameOptions options;
        string name;
        var type = typeof(Type0B);
        var item = type.GetProperties().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this[Int32?, in String?]", name);

        options = DEFAULT with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options); Assert.Equal("String? this[Int32?, in String?]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.String? {NAMESPACE}.{TESTNAME}." +
            "Type0B.MyItem[System.Nullable<System.Int32> one, in System.String? two]",
            name);
    }

    // ----------------------------------------------------

    public class Type1A<[EasyNullable] K, [EasyNullable] T>
    {
        public class Type1B<[EasyNullable] S> { public K this[T one, S two] => default!; }
    }

    //[Enforced]
    [Fact]
    public static void Test1_Nullable_From_Outer()
    {
        EasyNameOptions options;
        string name;

        // Unbound
        var type = typeof(Type1A<,>.Type1B<>);
        var item = type.GetProperties().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this[T?, S?]", name);

        options = DEFAULT with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options); Assert.Equal("K? this[T?, S?]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"K? {NAMESPACE}.{TESTNAME}.Type1A<K?, T?>.Type1B<S?>.Item[T? one, S? two]",
            name);

        // Bound (nullability lost)...
        type = typeof(Type1A<byte?, int?>.Type1B<string?>);
        item = type.GetProperties().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this[Int32?, String]", name);

        options = DEFAULT with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options); Assert.Equal("Byte? this[Int32?, String]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Nullable<System.Byte> {NAMESPACE}.{TESTNAME}." +
            "Type1A<System.Nullable<System.Byte>, System.Nullable<System.Int32>>." +
            "Type1B<System.String>." +
            "Item[System.Nullable<System.Int32> one, System.String two]",
            name);

        // Bound (wrapped nullability)...
        type = typeof(Type1A<byte?, int?>.Type1B<EasyNullable<string>>);
        item = type.GetProperties().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this[Int32?, String?]", name);

        options = DEFAULT with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options); Assert.Equal("Byte? this[Int32?, String?]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Nullable<System.Byte> {NAMESPACE}.{TESTNAME}." +
            "Type1A<System.Nullable<System.Byte>, System.Nullable<System.Int32>>." +
            "Type1B<Yotei.Tools.EasyNullable<System.String>>." +
            "Item[System.Nullable<System.Int32> one, Yotei.Tools.EasyNullable<System.String> two]",
            name);
    }

    // ----------------------------------------------------

    public class Type2A<K, T> { public class Type2B<S> { [EasyNullable] public K? this[T? one, S? two] => default!; } }

    //[Enforced]
    [Fact]
    public static void Test2_Nullable_From_Inner()
    {
        EasyNameOptions options;
        string name;

        // Unbound
        var type = typeof(Type2A<,>.Type2B<>);
        var item = type.GetProperties().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this[T?, S?]", name);

        options = DEFAULT with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options); Assert.Equal("K? this[T?, S?]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"K? {NAMESPACE}.{TESTNAME}.Type2A<K, T>.Type2B<S>.Item[T? one, S? two]",
            name);

        // Bound
        type = typeof(Type2A<byte?, int?>.Type2B<string?>);
        item = type.GetProperties().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this[Int32?, String?]", name);

        options = DEFAULT with { MemberReturnTypeOptions = DEFAULT };
        name = item.EasyName(options); Assert.Equal("Byte? this[Int32?, String?]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Nullable<System.Byte> {NAMESPACE}.{TESTNAME}." +
            "Type2A<System.Nullable<System.Byte>, System.Nullable<System.Int32>>." +
            "Type2B<System.String>." +
            "Item[System.Nullable<System.Int32> one, System.String? two]",
            name);
    }
}
namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyProperty
{
    readonly static string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    readonly static string TESTHOST = "Test_EasyProperty";

    readonly static EasyPropertyOptions EMPTY = EasyPropertyOptions.Empty;
    readonly static EasyPropertyOptions DEFAULT = EasyPropertyOptions.Default;
    readonly static EasyPropertyOptions FULL = EasyPropertyOptions.Full;

    // ----------------------------------------------------

    public class Type0A
    {
        [IsNullable]
        public string? this[int? one, [IsNullable] in string? two] => default!;
    }

    //[Enforced]
    [Fact]
    public static void Test_Indexed_NullableArguments_NullableMemberType()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(Type0A);
        var item = type.GetProperties().Single();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this[int?, in string?]", name);

        options = DEFAULT with { MemberTypeOptions = EasyTypeOptions.Default };
        name = item.EasyName(options);
        Assert.Equal("string? this[int?, in string?]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"public System.String? {NAMESPACE}.{TESTHOST}." +
            "Type0A.Item[System.Nullable<System.Int32> one, in System.String? two]",
            name);
    }

    // ----------------------------------------------------

    public class Type0B
    {
        [IndexerName("MyItem")]
        [IsNullable]
        public string? this[int? one, [IsNullable] in string? two] => default!;
    }

    //[Enforced]
    [Fact]
    public static void Test_Indexed_WithName()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(Type0B);
        var item = type.GetProperties().Single();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this[int?, in string?]", name);

        options = DEFAULT with { MemberTypeOptions = EasyTypeOptions.Default };
        name = item.EasyName(options);
        Assert.Equal("string? this[int?, in string?]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"public System.String? {NAMESPACE}.{TESTHOST}." +
            "Type0B.MyItem[System.Nullable<System.Int32> one, in System.String? two]",
            name);
    }

    // ----------------------------------------------------

    class Type1A { public int Name { get; set; } }
    class Type1B : Type1A { public new int Name { get; set; } }

    //[Enforced]
    [Fact]
    public static void Test_Standar_NewOverNonVirtual()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(Type1B);
        var item = type.GetProperties().Single();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { MemberTypeOptions = EasyTypeOptions.Default };
        name = item.EasyName(options);
        Assert.Equal("int Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"public new System.Int32 {NAMESPACE}.{TESTHOST}.Type1B.Name", name);
    }

    // ----------------------------------------------------

    class Type2A { public virtual int Name { get; set; } }
    class Type2B : Type2A { public new int Name { get; set; } }

    //[Enforced]
    [Fact]
    public static void Test_Standar_NewOverVirtual()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(Type2B);
        var item = type.GetProperties().Single();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { MemberTypeOptions = EasyTypeOptions.Default };
        name = item.EasyName(options);
        Assert.Equal("int Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"public new System.Int32 {NAMESPACE}.{TESTHOST}.Type2B.Name", name);
    }

    // ----------------------------------------------------

    class Type3A { [IsNullable] public virtual string? Name { get; set; } = default!; }
    class Type3B : Type3A { [IsNullable] public override string? Name { get; set; } = default!; }

    //[Enforced]
    [Fact]
    public static void Test_Standar_Override_NullableAttribute()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(Type3B);
        var item = type.GetProperties().Single();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { MemberTypeOptions = EasyTypeOptions.Default };
        name = item.EasyName(options);
        Assert.Equal("string? Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"public override System.String? {NAMESPACE}.{TESTHOST}.Type3B.Name", name);
    }

    // ----------------------------------------------------

    class Type4A { public virtual IsNullable<string?> Name { get; set; } = default!; }
    class Type4B : Type4A { public override IsNullable<string?> Name { get; set; } = default!; }

    //[Enforced]
    [Fact]
    public static void Test_Standar_Override_NullableWrapper()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(Type4B);
        var item = type.GetProperties().Single();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { MemberTypeOptions = EasyTypeOptions.Default };
        name = item.EasyName(options);
        Assert.Equal("string? Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"public override Yotei.Tools.IsNullable<System.String> {NAMESPACE}.{TESTHOST}.Type4B.Name",
            name);
    }

    // ----------------------------------------------------

    public class Type5A<K, T>
    {
        public class Type5B<S> { [IsNullable] public K? this[T? one, S? two] => default!; }
    }

    //[Enforced]
    [Fact]
    public static void Test_NestedGenerics_Unbound()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(Type5A<,>.Type5B<>);
        var item = type.GetProperties().Single();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this[T?, S?]", name);

        options = DEFAULT with { MemberTypeOptions = EasyTypeOptions.Default };
        name = item.EasyName(options);
        Assert.Equal("K? this[T?, S?]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"public K? {NAMESPACE}.{TESTHOST}.Type5A<K, T>.Type5B<S>.Item[T? one, S? two]",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_NestedGenerics_Bound()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(Type5A<byte?, int?>.Type5B<string?>);
        var item = type.GetProperties().Single();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("this[int?, string?]", name);

        options = DEFAULT with { MemberTypeOptions = EasyTypeOptions.Default };
        name = item.EasyName(options);
        Assert.Equal("byte? this[int?, string?]", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"public System.Nullable<System.Byte> {NAMESPACE}.{TESTHOST}." +
            "Type5A<System.Nullable<System.Byte>, System.Nullable<System.Int32>>." +
            "Type5B<System.String>.Item[System.Nullable<System.Int32> one, System.String? two]",
            name);
    }
}
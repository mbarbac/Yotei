#pragma warning disable CA1822, IDE0060

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyProperty
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyProperty";

    readonly static EasyPropertyOptions EMPTY = EasyPropertyOptions.Empty;
    readonly static EasyPropertyOptions DEFAULT = EasyPropertyOptions.Default;
    readonly static EasyPropertyOptions FULL = EasyPropertyOptions.Full;

    // ----------------------------------------------------

    public class Type0a
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
        var type = typeof(Type0a);
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
            $"public System.String? {PREFIX}." +
            "Type0a.Item[System.Nullable<System.Int32> one, in System.String? two]",
            name);
    }

    // ----------------------------------------------------

    public class Type0b
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
        var type = typeof(Type0b);
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
            $"public System.String? {PREFIX}." +
            "Type0b.MyItem[System.Nullable<System.Int32> one, in System.String? two]",
            name);
    }

    // ----------------------------------------------------

    class Type1a { public int Name { get; set; } }
    class Type1b : Type1a { public new int Name { get; set; } }

    //[Enforced]
    [Fact]
    public static void Test_Standar_NewOverNonVirtual()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(Type1b);
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
        Assert.Equal($"public new System.Int32 {PREFIX}.Type1b.Name", name);
    }

    // ----------------------------------------------------

    class Type2a { public virtual int Name { get; set; } }
    class Type2b : Type2a { public new int Name { get; set; } }

    //[Enforced]
    [Fact]
    public static void Test_Standar_NewOverVirtual()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(Type2b);
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
        Assert.Equal($"public new System.Int32 {PREFIX}.Type2b.Name", name);
    }

    // ----------------------------------------------------

    class Type3a { [IsNullable] public virtual string? Name { get; set; } = default!; }
    class Type3b : Type3a { [IsNullable] public override string? Name { get; set; } = default!; }

    //[Enforced]
    [Fact]
    public static void Test_Standar_Override_NullableAttribute()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(Type3b);
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
        Assert.Equal($"public override System.String? {PREFIX}.Type3b.Name", name);
    }

    // ----------------------------------------------------

    class Type4a { public virtual IsNullable<string?> Name { get; set; } = default!; }
    class Type4b : Type4a { public override IsNullable<string?> Name { get; set; } = default!; }

    //[Enforced]
    [Fact]
    public static void Test_Standar_Override_NullableWrapper()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(Type4b);
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
            $"public override Yotei.Tools.IsNullable<System.String> {PREFIX}.Type4b.Name",
            name);
    }

    // ----------------------------------------------------

    public class Type5a<K, T>
    {
        public class Type5b<S> { [IsNullable] public K? this[T? one, S? two] => default!; }
    }

    //[Enforced]
    [Fact]
    public static void Test_NestedGenerics_Unbound()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(Type5a<,>.Type5b<>);
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
            $"public K? {PREFIX}.Type5a<K, T>.Type5b<S>.Item[T? one, S? two]",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_NestedGenerics_Bound()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(Type5a<byte?, int?>.Type5b<string?>);
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
            $"public System.Nullable<System.Byte> {PREFIX}." +
            "Type5a<System.Nullable<System.Byte>, System.Nullable<System.Int32>>." +
            "Type5b<System.String>.Item[System.Nullable<System.Int32> one, System.String? two]",
            name);
    }
}
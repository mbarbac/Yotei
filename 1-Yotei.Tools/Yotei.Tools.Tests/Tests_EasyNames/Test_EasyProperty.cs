namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyProperty
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyProperty";

    // ----------------------------------------------------

    public class RType0a
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
        var type = typeof(RType0a);
        var item = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = item.EasyName(options); Assert.Equal("this[int?, in string?]", name);

        options = EasyPropertyOptions.DefaultEx;
        name = item.EasyName(options);
        Assert.Equal($"{PREFIX}.RType0a.this[int?, in string?]", name);

        options.MemberTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = item.EasyName(options);
        Assert.Equal($"public string? {PREFIX}.RType0a.this[int?, in string?]", name);

        options = EasyPropertyOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"public System.String? {PREFIX}." +
            "RType0a.Item[System.Nullable<System.Int32> one, in System.String? two]",
            name);
    }

    // ----------------------------------------------------

    public class RType0b
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
        var type = typeof(RType0b);
        var item = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = item.EasyName(options); Assert.Equal("this[int?, in string?]", name);

        options = EasyPropertyOptions.DefaultEx;
        name = item.EasyName(options);
        Assert.Equal($"{PREFIX}.RType0b.this[int?, in string?]", name);

        options.MemberTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = item.EasyName(options);
        Assert.Equal($"public string? {PREFIX}.RType0b.this[int?, in string?]", name);

        options = EasyPropertyOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"public System.String? {PREFIX}." +
            "RType0b.MyItem[System.Nullable<System.Int32> one, in System.String? two]",
            name);
    }

    // ----------------------------------------------------

    class RType1a { public int Name { get; set; } }
    class RType1b : RType1a { public new int Name { get; set; } }

    //[Enforced]
    [Fact]
    public static void Test_Standar_New_Over_NonVirtual()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType1b);
        var item = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyPropertyOptions.Default;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyPropertyOptions.DefaultEx;
        name = item.EasyName(options);
        Assert.Equal($"{PREFIX}.RType1b.Name", name);

        options.MemberTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = item.EasyName(options);
        Assert.Equal($"public new int {PREFIX}.RType1b.Name", name);

        options = EasyPropertyOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"public new System.Int32 {PREFIX}.RType1b.Name", name);
    }

    // ----------------------------------------------------

    class RType2a { public virtual int Name { get; set; } }
    class RType2b : RType2a { public new int Name { get; set; } }

    //[Enforced]
    [Fact]
    public static void Test_Standar_NewOverVirtual()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType2b);
        var item = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyPropertyOptions.Default;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyPropertyOptions.DefaultEx;
        name = item.EasyName(options);
        Assert.Equal($"{PREFIX}.RType2b.Name", name);

        options.MemberTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = item.EasyName(options);
        Assert.Equal($"public new int {PREFIX}.RType2b.Name", name);

        options = EasyPropertyOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"public new System.Int32 {PREFIX}.RType2b.Name", name);
    }

    // ----------------------------------------------------

    class RType3a { [IsNullable] public virtual string? Name { get; set; } = default!; }
    class RType3b : RType3a { [IsNullable] public override string? Name { get; set; } = default!; }

    //[Enforced]
    [Fact]
    public static void Test_Standar_Override_NullableAttribute()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType3b);
        var item = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyPropertyOptions.Default;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyPropertyOptions.DefaultEx;
        name = item.EasyName(options);
        Assert.Equal($"{PREFIX}.RType3b.Name", name);

        options.MemberTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = item.EasyName(options);
        Assert.Equal($"public override string? {PREFIX}.RType3b.Name", name);

        options = EasyPropertyOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"public override System.String? {PREFIX}.RType3b.Name", name);
    }

    // ----------------------------------------------------

    class RType4a { public virtual IsNullable<string?> Name { get; set; } = default!; }
    class RType4b : RType4a { public override IsNullable<string?> Name { get; set; } = default!; }

    //[Enforced]
    [Fact]
    public static void Test_Standar_Override_NullableWrapper()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType4b);
        var item = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyPropertyOptions.Default;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyPropertyOptions.DefaultEx;
        name = item.EasyName(options);
        Assert.Equal($"{PREFIX}.RType4b.Name", name);

        options.MemberTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = item.EasyName(options);
        Assert.Equal($"public override string? {PREFIX}.RType4b.Name", name);

        options = EasyPropertyOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"public override Yotei.Tools.IsNullable<System.String> {PREFIX}.RType4b.Name",
            name);
    }

    // ----------------------------------------------------

    public class RType5a<K, T>
    {
        public class RType5b<S> { [IsNullable] public K? this[T? one, S? two] => default!; }
    }

    //[Enforced]
    [Fact]
    public static void Test_NestedGenerics_Unbound()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType5a<,>.RType5b<>);
        var item = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = item.EasyName(options); Assert.Equal("this[T?, S?]", name);

        options = EasyPropertyOptions.DefaultEx;
        name = item.EasyName(options);
        Assert.Equal($"{PREFIX}.RType5a<K, T>.RType5b<S>.this[T?, S?]", name);

        options.MemberTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = item.EasyName(options);
        Assert.Equal(
            $"public K? {PREFIX}.RType5a<K, T>.RType5b<S>.this[T?, S?]",
            name);

        options = EasyPropertyOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"public K? {PREFIX}.RType5a<K, T>.RType5b<S>.Item[T? one, S? two]",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_NestedGenerics_Bound()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType5a<byte?, int?>.RType5b<string?>);
        var item = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = item.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = item.EasyName(options); Assert.Equal("this[int?, string?]", name);

        options = EasyPropertyOptions.DefaultEx;
        name = item.EasyName(options);
        Assert.Equal($"{PREFIX}.RType5a<byte?, int?>.RType5b<string>.this[int?, string?]", name);

        options.MemberTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = item.EasyName(options);
        Assert.Equal(
            $"public byte? {PREFIX}.RType5a<byte?, int?>.RType5b<string>.this[int?, string?]",
            name);

        options = EasyPropertyOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"public System.Nullable<System.Byte> {PREFIX}." +
            "RType5a<System.Nullable<System.Byte>, System.Nullable<System.Int32>>." +
            "RType5b<System.String>.Item[System.Nullable<System.Int32> one, System.String? two]",
            name);
    }
}
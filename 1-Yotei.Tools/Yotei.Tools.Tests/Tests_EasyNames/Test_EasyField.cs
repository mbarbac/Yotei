#pragma warning disable IDE0044, CS0414, CS9265

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyField
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyField";

    // ----------------------------------------------------

    class RType0a { public string? Name = default!; }

    //[Enforced]
    [Fact]
    public static void Test_ReferenceType_Nullable()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType0a);
        var source = type.GetFields().Single();

        options = EasyFieldOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public string? Name", name);

        options = EasyFieldOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"public System.String? {PREFIX}.RType0a.Name", name);
    }

    // ----------------------------------------------------

    class RType0b { public IsNullable<string?> Name = default!; }

    //[Enforced]
    [Fact]
    public static void Test_ReferenceType_Nullable_Wrapped()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType0b);
        var source = type.GetFields().Single();

        options = EasyFieldOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public string? Name", name);

        options = EasyFieldOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"public Yotei.Tools.IsNullable<System.String> {PREFIX}.RType0b.Name", name);
    }

    // ----------------------------------------------------

    class RType0c { [IsNullable] public string Name = default!; }

    //[Enforced]
    [Fact]
    public static void Test_ReferenceType_Nullable_Attribute()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType0c);
        var source = type.GetFields().Single();

        options = EasyFieldOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public string? Name", name);

        options = EasyFieldOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"public System.String? {PREFIX}.RType0c.Name", name);
    }

    // ----------------------------------------------------

    public class RType1a<K, T>
    { public class RType1b<S> { public S? Name => default!; } }

    //[Enforced]
    [Fact]
    public static void Test_NestedGenerics_Unbound()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType1a<,>.RType1b<>);
        var source = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public S? Name", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public S? {PREFIX}.RType1a<K, T>.RType1b<S>.Name",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_NestedGenerics_Bound()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType1a<byte?, int?>.RType1b<string?>);
        var source = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public string? Name", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public System.String? {PREFIX}." +
            "RType1a<System.Nullable<System.Byte>, System.Nullable<System.Int32>>." +
            "RType1b<System.String>.Name",
            name);
    }

    // ----------------------------------------------------

    class RType2a { static string? Name = default!; }

    //[Enforced]
    [Fact]
    public static void Test_Static()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType2a);
        var flags = BindingFlags.Static | BindingFlags.NonPublic;
        var source = type.GetFields(flags).Single();

        options = EasyFieldOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("private static string? Name", name);

        options = EasyFieldOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"private static System.String? {PREFIX}.RType2a.Name", name);
    }

    // ----------------------------------------------------

    class RType2b { public const string? Name = default!; }

    //[Enforced]
    [Fact]
    public static void Test_Constant()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType2b);
        var source = type.GetFields().Single();

        options = EasyFieldOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public const string? Name", name);

        options = EasyFieldOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"public const System.String? {PREFIX}.RType2b.Name", name);
    }

    // ----------------------------------------------------

    class RType2c { public readonly string? Name = default!; }

    //[Enforced]
    [Fact]
    public static void Test_Readonly()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType2c);
        var source = type.GetFields().Single();

        options = EasyFieldOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public readonly string? Name", name);

        options = EasyFieldOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"public readonly System.String? {PREFIX}.RType2c.Name", name);
    }

    // ----------------------------------------------------

    ref struct RType2d { public ref string? Name; }

    //[Enforced]
    [Fact]
    public static void Test_Ref_Field()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType2d);
        var source = type.GetFields().Single();

        options = EasyFieldOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public ref string? Name", name);

        options = EasyFieldOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"public ref System.String? {PREFIX}.RType2d.Name", name);
    }

    // ----------------------------------------------------

    ref struct RType2e { public ref readonly string? Name; }

    //[Enforced]
    [Fact]
    public static void Test_RefReadonly_Field()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType2e);
        var source = type.GetFields().Single();

        options = EasyFieldOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public ref readonly string? Name", name);

        options = EasyFieldOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"public ref readonly System.String? {PREFIX}.RType2e.Name", name);
    }

    // ----------------------------------------------------

    class RType3a { public string? Name = default; }
    class RType3b : RType3a { public new string? Name = default; }



    //[Enforced]
    [Fact]
    public static void Test_New()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType3b);
        var source = type.GetField("Name")!;

        options = EasyFieldOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public new string? Name", name);

        options = EasyFieldOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"public new System.String? {PREFIX}.RType3b.Name", name);
    }
}
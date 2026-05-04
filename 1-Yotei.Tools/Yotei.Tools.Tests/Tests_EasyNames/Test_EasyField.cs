#pragma warning disable IDE0044, CS0414

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyField
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyField";

    // ----------------------------------------------------

    class RType0a { public string? Name = default; }
    class RType0b { [IsNullable] public string Name = default!; }
    class RType0c { public IsNullable<string> Name = default!; }

    //[Enforced]
    [Fact]
    public static void Test_Standard_Nullable()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType0a);
        var item = type.GetFields().Single();

        options = EasyFieldOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.DefaultEx;
        name = item.EasyName(options);
        Assert.Equal($"{PREFIX}.RType0a.Name", name);

        options.MemberTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = item.EasyName(options);
        Assert.Equal($"public string? {PREFIX}.RType0a.Name", name);

        options = EasyFieldOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"public System.String? {PREFIX}.RType0a.Name", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_Nullable_ByAttribute()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType0b);
        var item = type.GetFields().Single();

        options = EasyFieldOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.DefaultEx;
        name = item.EasyName(options);
        Assert.Equal($"{PREFIX}.RType0b.Name", name);

        options.MemberTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = item.EasyName(options);
        Assert.Equal($"public string? {PREFIX}.RType0b.Name", name);

        options = EasyFieldOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"public System.String? {PREFIX}.RType0b.Name", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_Nullable_ByWrapper()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType0c);
        var item = type.GetFields().Single();

        options = EasyFieldOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.DefaultEx;
        name = item.EasyName(options);
        Assert.Equal($"{PREFIX}.RType0c.Name", name);

        options.MemberTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = item.EasyName(options);
        Assert.Equal($"public string? {PREFIX}.RType0c.Name", name);

        options = EasyFieldOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"public Yotei.Tools.IsNullable<System.String> {PREFIX}.RType0c.Name", name);
    }

    // ----------------------------------------------------

    public class RType1a<[IsNullable] K, [IsNullable] T>
    { public class RType1b<[IsNullable] S> { public S Name = default!; } }

    //[Enforced]
    [Fact]
    public static void Test_NestedGeneric_Unbound()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType1a<,>.RType1b<>);
        var item = type.GetFields().Single();

        options = EasyFieldOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.DefaultEx;
        name = item.EasyName(options);
        Assert.Equal($"{PREFIX}.RType1a<K?, T?>.RType1b<S?>.Name", name);

        options.MemberTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = item.EasyName(options);
        Assert.Equal($"public S? {PREFIX}.RType1a<K?, T?>.RType1b<S?>.Name", name);

        options = EasyFieldOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"public S? {PREFIX}.RType1a<K?, T?>.RType1b<S?>.Name", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_NestedGeneric_Bound_ReferenceNullabilityLost()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType1a<byte?, int?>.RType1b<string?>);
        var item = type.GetFields().Single();

        options = EasyFieldOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.DefaultEx;
        name = item.EasyName(options);
        Assert.Equal($"{PREFIX}.RType1a<byte?, int?>.RType1b<string>.Name", name);

        options.MemberTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = item.EasyName(options);
        Assert.Equal($"public string? {PREFIX}.RType1a<byte?, int?>.RType1b<string>.Name", name);

        options = EasyFieldOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"public System.String? {PREFIX}." +
            "RType1a<System.Nullable<System.Byte>, System.Nullable<System.Int32>>." +
            "RType1b<System.String>.Name",
            name);
    }

    // ----------------------------------------------------

    
    class RType2a { static string? Name = default; }

    //[Enforced]
    [Fact]
    public static void Test_Static()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType2a);
        var flags = BindingFlags.Static | BindingFlags.NonPublic;
        var item = type.GetFields(flags).Single();

        options = EasyFieldOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.DefaultEx;
        name = item.EasyName(options);
        Assert.Equal($"{PREFIX}.RType2a.Name", name);

        options.MemberTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = item.EasyName(options);
        Assert.Equal($"static string? {PREFIX}.RType2a.Name", name);

        options = EasyFieldOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"static System.String? {PREFIX}.RType2a.Name", name);
    }

    // ----------------------------------------------------

    class RType2b { public const string? Name = default; }

    //[Enforced]
    [Fact]
    public static void Test_Constant()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType2b);
        var item = type.GetFields().Single();

        options = EasyFieldOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.DefaultEx;
        name = item.EasyName(options);
        Assert.Equal($"{PREFIX}.RType2b.Name", name);

        options.MemberTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = item.EasyName(options);
        Assert.Equal($"public const string? {PREFIX}.RType2b.Name", name);

        options = EasyFieldOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"public const System.String? {PREFIX}.RType2b.Name", name);
    }

    // ----------------------------------------------------

    class RType2c { public readonly string? Name = default; }

    //[Enforced]
    [Fact]
    public static void Test_ReadOnly()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType2c);
        var item = type.GetFields().Single();

        options = EasyFieldOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.DefaultEx;
        name = item.EasyName(options);
        Assert.Equal($"{PREFIX}.RType2c.Name", name);

        options.MemberTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = item.EasyName(options);
        Assert.Equal($"public readonly string? {PREFIX}.RType2c.Name", name);

        options = EasyFieldOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"public readonly System.String? {PREFIX}.RType2c.Name", name);
    }

    // ----------------------------------------------------

    ref struct RType2d { public ref int? Name; }

    //[Enforced]
    [Fact]
    public static void Test_RefField()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType2d);
        var item = type.GetFields().Single();

        options = EasyFieldOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.DefaultEx;
        name = item.EasyName(options);
        Assert.Equal($"{PREFIX}.RType2d.Name", name);

        options.MemberTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = item.EasyName(options);
        Assert.Equal($"public ref int? {PREFIX}.RType2d.Name", name);

        options = EasyFieldOptions.Full;
        name = item.EasyName(options);
        Assert.Equal($"public ref System.Nullable<System.Int32> {PREFIX}.RType2d.Name", name);
    }

    // ----------------------------------------------------

    ref struct RType2e { public ref readonly int? Name; }

    //[Enforced]
    [Fact]
    public static void Test_RefReadonlyField()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType2e);
        var item = type.GetFields().Single();

        options = EasyFieldOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.DefaultEx;
        name = item.EasyName(options);
        Assert.Equal($"{PREFIX}.RType2e.Name", name);

        options.MemberTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = item.EasyName(options);
        Assert.Equal($"public ref readonly int? {PREFIX}.RType2e.Name", name);

        options = EasyFieldOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"public ref readonly System.Nullable<System.Int32> {PREFIX}.RType2e.Name",
            name);
    }

    // ----------------------------------------------------

    class RType3a { public int? Name = 0; }
    class RType3b : RType3a { public new int? Name = 0; }

    //[Enforced]
    [Fact]
    public static void Test_NewOverride()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(RType3b);
        var item = type.GetField("Name");
        Assert.NotNull(item);
        Assert.True(item.DeclaringType == typeof(RType3b));

        options = EasyFieldOptions.Empty;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.Default;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = EasyFieldOptions.DefaultEx;
        name = item.EasyName(options);
        Assert.Equal($"{PREFIX}.RType3b.Name", name);

        options.MemberTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = item.EasyName(options);
        Assert.Equal($"public new int? {PREFIX}.RType3b.Name", name);

        options = EasyFieldOptions.Full;
        name = item.EasyName(options);
        Assert.Equal(
            $"public new System.Nullable<System.Int32> {PREFIX}.RType3b.Name",
            name);
    }
}
#pragma warning disable CA1822, IDE0060

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyField
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyField";

    readonly static EasyFieldOptions EMPTY = EasyFieldOptions.Empty;
    readonly static EasyFieldOptions DEFAULT = EasyFieldOptions.Default;
    readonly static EasyFieldOptions FULL = EasyFieldOptions.Full;

    /*

    // ----------------------------------------------------

    class Type0a { public string? Name = default; }
    class Type0b { [IsNullable] public string Name = default!; }
    class Type0c { public IsNullable<string> Name = default!; }

    //[Enforced]
    [Fact]
    public static void Test_Standard_Nullable()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type0a);
        var item = type.GetFields().Single();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { MemberTypeOptions = EasyTypeOptions.Default };
        name = item.EasyName(options);
        Assert.Equal("string? Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"public System.String? {PREFIX}.Type0a.Name", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_Nullable_ByAttribute()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type0b);
        var item = type.GetFields().Single();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT with { MemberTypeOptions = EasyTypeOptions.Default };
        name = item.EasyName(options);
        Assert.Equal("string? Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"public System.String? {PREFIX}.Type0b.Name", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_Nullable_ByWrapper()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type0c);
        var item = type.GetFields().Single();

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
            $"public Yotei.Tools.IsNullable<System.String> {PREFIX}.Type0c.Name",
            name);
    }

    // ----------------------------------------------------

    public class Type1a<[IsNullable] K, [IsNullable] T>
    { public class Type1b<[IsNullable] S> { public S Name = default!; } }

    //[Enforced]
    [Fact]
    public static void Test_NestedGeneric_Unbound()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type1a<,>.Type1b<>);
        var item = type.GetFields().Single();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        // Gets the nullable annotation from the attribute...
        options = DEFAULT with { MemberTypeOptions = EasyTypeOptions.Default };
        name = item.EasyName(options);
        Assert.Equal("S? Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"public S? {PREFIX}.Type1a<K?, T?>.Type1b<S?>.Name",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_NestedGeneric_Bound_ReferenceNullabilityLost()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type1a<byte?, int?>.Type1b<string?>);
        var item = type.GetFields().Single();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name", name);

        // Gets the nullable annotation from the attribute...
        options = DEFAULT with { MemberTypeOptions = EasyTypeOptions.Default };
        name = item.EasyName(options);
        Assert.Equal("string? Name", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"public System.String? {PREFIX}." +
            "Type1a<System.Nullable<System.Byte>, System.Nullable<System.Int32>>." +
            "Type1b<System.String>.Name",
            name);
    }

    // ----------------------------------------------------

    class Type2a { static string? Name = default; }

    //[Enforced]
    [Fact]
    public static void Test_Static()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type2a);
        var flags = BindingFlags.Static | BindingFlags.NonPublic;
        var item = type.GetFields(flags).Single();

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"static System.String? {PREFIX}.Type2a.Name", name);
    }

    // ----------------------------------------------------

    class Type2b { public const string? Name = default; }

    //[Enforced]
    [Fact]
    public static void Test_Constant()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type2b);
        var item = type.GetFields().Single();

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"public const System.String? {PREFIX}.Type2b.Name", name);
    }

    // ----------------------------------------------------

    class Type2c { public readonly string? Name = default; }

    //[Enforced]
    [Fact]
    public static void Test_ReadOnly()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type2c);
        var item = type.GetFields().Single();

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"public readonly System.String? {PREFIX}.Type2c.Name", name);
    }

    // ----------------------------------------------------

    ref struct Type2d { public ref int Name; }

    //[Enforced]
    [Fact]
    public static void Test_RefField()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type2d);
        var item = type.GetFields().Single();

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"public ref System.Int32 {PREFIX}.Type2d.Name", name);
    }

    // ----------------------------------------------------

    ref struct Type2e { public ref readonly int Name; }

    //[Enforced]
    [Fact]
    public static void Test_RefReadonlyField()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type2e);
        var item = type.GetFields().Single();

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"public ref readonly System.Int32 {PREFIX}.Type2e.Name", name);
    }

    // ----------------------------------------------------

    class Type3a { public int Name = 0; }
    class Type3b : Type3a { public new int Name = 0; }

    //[Enforced]
    [Fact]
    public static void Test_NewOverride()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type3b);
        var item = type.GetField("Name");
        Assert.NotNull(item);
        Assert.True(item.DeclaringType == typeof(Type3b));

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"public new System.Int32 {PREFIX}.Type3b.Name", name);
    }

    */
}
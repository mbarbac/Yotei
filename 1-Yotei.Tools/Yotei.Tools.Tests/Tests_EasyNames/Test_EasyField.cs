#pragma warning disable CS0414
#pragma warning disable CS9265
#pragma warning disable IDE0044

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyField
{
    readonly static string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    readonly static string TESTHOST = "Test_EasyField";

    readonly static EasyFieldOptions EMPTY = EasyFieldOptions.Empty;
    readonly static EasyFieldOptions DEFAULT = EasyFieldOptions.Default;
    readonly static EasyFieldOptions FULL = EasyFieldOptions.Full;

    // ----------------------------------------------------

    class Type0A { public string? Name = default; }
    class Type0B { [IsNullable] public string Name = default!; }
    class Type0C { public IsNullable<string> Name = default!; }

    //[Enforced]
    [Fact]
    public static void Test_Standard_Nullable()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type0A);
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
        Assert.Equal($"public System.String? {NAMESPACE}.{TESTHOST}.Type0A.Name", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_Nullable_ByAttribute()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type0B);
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
        Assert.Equal($"public System.String? {NAMESPACE}.{TESTHOST}.Type0B.Name", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_Nullable_ByWrapper()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type0C);
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
            $"public Yotei.Tools.IsNullable<System.String> {NAMESPACE}.{TESTHOST}.Type0C.Name",
            name);
    }

    // ----------------------------------------------------

    public class Type1A<[IsNullable] K, [IsNullable] T>
    { public class Type1B<[IsNullable] S> { public S Name = default!; } }

    //[Enforced]
    [Fact]
    public static void Test_NestedGeneric_Unbound()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type1A<,>.Type1B<>);
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
            $"public S? {NAMESPACE}.{TESTHOST}.Type1A<K?, T?>.Type1B<S?>.Name",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_NestedGeneric_Bound_ReferenceNullabilityLost()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type1A<byte?, int?>.Type1B<string?>);
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
            $"public System.String? {NAMESPACE}.{TESTHOST}." +
            "Type1A<System.Nullable<System.Byte>, System.Nullable<System.Int32>>." +
            "Type1B<System.String>.Name",
            name);
    }

    // ----------------------------------------------------

    class Type2A { static string? Name = default; }

    //[Enforced]
    [Fact]
    public static void Test_Static()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type2A);
        var flags = BindingFlags.Static | BindingFlags.NonPublic;
        var item = type.GetFields(flags).Single();

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"static System.String? {NAMESPACE}.{TESTHOST}.Type2A.Name", name);
    }

    // ----------------------------------------------------

    class Type2B { public const string? Name = default; }

    //[Enforced]
    [Fact]
    public static void Test_Constant()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type2B);
        var item = type.GetFields().Single();

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"public const System.String? {NAMESPACE}.{TESTHOST}.Type2B.Name", name);
    }

    // ----------------------------------------------------

    class Type2C { public readonly string? Name = default; }

    //[Enforced]
    [Fact]
    public static void Test_ReadOnly()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type2C);
        var item = type.GetFields().Single();

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"public readonly System.String? {NAMESPACE}.{TESTHOST}.Type2C.Name", name);
    }

    // ----------------------------------------------------

    ref struct Type2D { public ref int Name; }

    //[Enforced]
    [Fact]
    public static void Test_RefField()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type2D);
        var item = type.GetFields().Single();

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"public ref System.Int32 {NAMESPACE}.{TESTHOST}.Type2D.Name", name);
    }

    // ----------------------------------------------------

    ref struct Type2E { public ref readonly int Name; }

    //[Enforced]
    [Fact]
    public static void Test_RefReadonlyField()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type2E);
        var item = type.GetFields().Single();

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"public ref readonly System.Int32 {NAMESPACE}.{TESTHOST}.Type2E.Name", name);
    }

    // ----------------------------------------------------

    class Type3A { public int Name = 0; }
    class Type3B : Type3A { public new int Name = 0; }

    //[Enforced]
    [Fact]
    public static void Test_NewOverride()
    {
        EasyFieldOptions options;
        string name;
        var type = typeof(Type3B);
        var item = type.GetField("Name");
        Assert.NotNull(item);
        Assert.True(item.DeclaringType == typeof(Type3B));

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"public new System.Int32 {NAMESPACE}.{TESTHOST}.Type3B.Name", name);
    }
}
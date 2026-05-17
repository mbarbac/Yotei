#pragma warning disable CS8618, IDE0044

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyProperty
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyProperty";

    // ----------------------------------------------------

    public class RType1a { public int this[int? one, in string? two] => default!; }

    //[Enforced]
    [Fact]
    public static void Test_Indexed_Nullable_Arguments_OnReferenceLost()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType1a);
        var source = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int?, in string]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public int this[int?, in string]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public System.Int32 {PREFIX}." +
            "RType1a.Item[System.Nullable<System.Int32> one, in System.String two]",
            name);
    }

    // ----------------------------------------------------

    public class RType1b { public int this[int? one, in IsNullable<string?> two] => default!; }

    //[Enforced]
    [Fact]
    public static void Test_Indexed_Nullable_Arguments_OnReferenceWrapped()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType1b);
        var source = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int?, in string?]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public int this[int?, in string?]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public System.Int32 {PREFIX}.RType1b.Item[" +
            "System.Nullable<System.Int32> one, " +
            "in Yotei.Tools.IsNullable<System.String> two]",
            name);
    }

    // ----------------------------------------------------

    public class RType1c { public int this[int? one, [IsNullable] in string? two] => default!; }

    //[Enforced]
    [Fact]
    public static void Test_Indexed_Nullable_Arguments_OnReferenceAttribute()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType1c);
        var source = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int?, in string?]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public int this[int?, in string?]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public System.Int32 {PREFIX}.RType1c.Item[" +
            "System.Nullable<System.Int32> one, " +
            "in System.String? two]",
            name);
    }

    // ----------------------------------------------------

    public class RType2 { [IndexerName("MyName")] public int this[int one] => default!; }

    //[Enforced]
    [Fact]
    public static void Test_Indexed_CustomName()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType2);
        var source = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public int this[int]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public System.Int32 {PREFIX}.RType2.MyName[System.Int32 one]",
            name);
    }

    // ----------------------------------------------------

    public class RType3a { public int? this[int one] => default!; }

    //[Enforced]
    [Fact]
    public static void Test_NullableMemberType_ValueType()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType3a);
        var source = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public int? this[int]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public System.Nullable<System.Int32> {PREFIX}.RType3a.Item[System.Int32 one]",
            name);
    }

    // ----------------------------------------------------

    public class RType3b { public string? this[int one] => default!; }

    //[Enforced]
    [Fact]
    public static void Test_NullableMemberType_ReferenceType()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType3b);
        var source = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public string? this[int]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public System.String? {PREFIX}.RType3b.Item[System.Int32 one]",
            name);
    }

    // ----------------------------------------------------

    public class RType3c { public IsNullable<string?> this[int one] => default!; }

    //[Enforced]
    [Fact]
    public static void Test_NullableMemberType_ReferenceType_WithWrapper()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType3c);
        var source = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public string? this[int]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public Yotei.Tools.IsNullable<System.String> {PREFIX}.RType3c.Item[System.Int32 one]",
            name);
    }

    // ----------------------------------------------------

    public class RType3d { [IsNullable] public string? this[int one] => default!; }

    //[Enforced]
    [Fact]
    public static void Test_NullableMemberType_ReferenceType_WithAttribute()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType3d);
        var source = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public string? this[int]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public System.String? {PREFIX}.RType3d.Item[System.Int32 one]",
            name);
    }

    // ----------------------------------------------------

    public class RType4a
    {
        public ref string? this[int one] => ref Value!;
        static string Value;
    }

    //[Enforced]
    [Fact]
    public static void Test_Ref_MemberType()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType4a);
        var source = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public ref string? this[int]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public ref System.String? {PREFIX}.RType4a.Item[System.Int32 one]",
            name);
    }

    // ----------------------------------------------------

    public class RType4b
    {
        public ref readonly string? this[int one] => ref Value!;
        static string Value;
    }

    //[Enforced]
    [Fact]
    public static void Test_Ref_Readonly_MemberType()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType4b);
        var source = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public ref readonly string? this[int]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public ref readonly System.String? {PREFIX}.RType4b.Item[System.Int32 one]",
            name);
    }

    // ----------------------------------------------------

    public interface IFace1a { int this[int one] { get; } }

    //[Enforced]
    [Fact]
    public static void Test_Public_OnInterfaceDeclaration()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(IFace1a);
        var source = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("int this[int]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"System.Int32 {PREFIX}.IFace1a.Item[System.Int32 one]",
            name);
    }

    // ----------------------------------------------------

    public interface IFace1b { internal int this[int one] { get; } }

    //[Enforced]
    [Fact]
    public static void Test_Internal_OnInterfaceDeclaration()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(IFace1b);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var source = type.GetProperties(flags).Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("internal int this[int]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"internal System.Int32 {PREFIX}.IFace1b.Item[System.Int32 one]",
            name);
    }

    // ----------------------------------------------------

    public interface IFace1c { protected int this[int one] { get; } }

    //[Enforced]
    [Fact]
    public static void Test_Protected_OnInterfaceDeclaration()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(IFace1c);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var source = type.GetProperties(flags).Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("protected int this[int]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"protected System.Int32 {PREFIX}.IFace1c.Item[System.Int32 one]",
            name);
    }

    // ----------------------------------------------------

    public interface IFace1d { internal protected int this[int one] { get; } }

    //[Enforced]
    [Fact]
    public static void Test_InternalProtected_OnInterfaceDeclaration()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(IFace1d);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var source = type.GetProperties(flags).Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("internal protected int this[int]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"internal protected System.Int32 {PREFIX}.IFace1d.Item[System.Int32 one]",
            name);
    }

    // ----------------------------------------------------

    public interface IFace1e { private protected int this[int one] { get; } }

    //[Enforced]
    [Fact]
    public static void Test_PrivateProtected_OnInterfaceDeclaration()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(IFace1e);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var source = type.GetProperties(flags).Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("private protected int this[int]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"private protected System.Int32 {PREFIX}.IFace1e.Item[System.Int32 one]",
            name);
    }

    // ----------------------------------------------------

    public class RType5a { public int this[int one] { get => throw null!; } }

    //[Enforced]
    [Fact]
    public static void Test_Public_OnTypeDeclaration()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType5a);
        var source = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public int this[int]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public System.Int32 {PREFIX}.RType5a.Item[System.Int32 one]",
            name);
    }

    // ----------------------------------------------------

    public class RType5b { internal int this[int one] { get => throw null!; } }

    //[Enforced]
    [Fact]
    public static void Test_Internal_OnTypeDeclaration()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType5b);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var source = type.GetProperties(flags).Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("internal int this[int]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"internal System.Int32 {PREFIX}.RType5b.Item[System.Int32 one]",
            name);
    }

    // ----------------------------------------------------

    public class RType5c { protected int this[int one] { get => throw null!; } }

    //[Enforced]
    [Fact]
    public static void Test_Protected_OnTypeDeclaration()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType5c);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var source = type.GetProperties(flags).Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("protected int this[int]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"protected System.Int32 {PREFIX}.RType5c.Item[System.Int32 one]",
            name);
    }

    // ----------------------------------------------------

    public class RType5d { internal protected int this[int one] { get => throw null!; } }

    //[Enforced]
    [Fact]
    public static void Test_InternalProtected_OnTypeDeclaration()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType5d);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var source = type.GetProperties(flags).Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("internal protected int this[int]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"internal protected System.Int32 {PREFIX}.RType5d.Item[System.Int32 one]",
            name);
    }

    // ----------------------------------------------------

    public class RType5e { private protected int this[int one] { get => throw null!; } }

    //[Enforced]
    [Fact]
    public static void Test_PrivateProtected_OnTypeDeclaration()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType5e);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var source = type.GetProperties(flags).Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("private protected int this[int]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"private protected System.Int32 {PREFIX}.RType5e.Item[System.Int32 one]",
            name);
    }

    // ----------------------------------------------------

    public class RType5f { int this[int one] { get => throw null!; } }

    //[Enforced]
    [Fact]
    public static void Test_Private_OnTypeDeclaration()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType5f);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var source = type.GetProperties(flags).Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("private int this[int]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"private System.Int32 {PREFIX}.RType5f.Item[System.Int32 one]",
            name);
    }

    // ----------------------------------------------------

    class RType6a { public int Name { get; } }
    class RType6b : RType6a { public new int Name { get; } }

    //[Enforced]
    [Fact]
    public static void Test_New_Over_NotVirtual()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType6b);
        var source = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public new int Name", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"public new System.Int32 {PREFIX}.RType6b.Name", name);
    }

    // ----------------------------------------------------

    class RType7 { public virtual int Name { get; } }

    //[Enforced]
    [Fact]
    public static void Test_Virtual()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType7);
        var source = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public virtual int Name", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"public virtual System.Int32 {PREFIX}.RType7.Name", name);
    }

    // ----------------------------------------------------

    class RType8a { public virtual int Name { get; } }
    class RType8b : RType8a { public new int Name { get; } }

    //[Enforced]
    [Fact]
    public static void Test_New_Over_Virtual()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType8b);
        var source = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("Name", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public new int Name", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"public new System.Int32 {PREFIX}.RType8b.Name", name);
    }

    // ----------------------------------------------------

    public class RType9a<K, T>
    { public class RType9b<S> { public K? this[T? one, S? two] => default!; } }

    //[Enforced]
    [Fact]
    public static void Test_NestedGenerics_Unbound()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType9a<,>.RType9b<>);
        var source = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[T?, S?]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public K? this[T?, S?]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public K? {PREFIX}.RType9a<K, T>.RType9b<S>.Item[T? one, S? two]",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_NestedGenerics_Bound()
    {
        EasyPropertyOptions options;
        string name;
        var type = typeof(RType9a<byte?, int?>.RType9b<string?>);
        var source = type.GetProperties().Single();

        options = EasyPropertyOptions.Empty;
        name = source.EasyName(options); Assert.Equal("this", name);

        options = EasyPropertyOptions.Default;
        name = source.EasyName(options); Assert.Equal("this[int?, string?]", name);

        options = options with
        { MemberTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public byte? this[int?, string?]", name);

        options = EasyPropertyOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public System.Nullable<System.Byte> {PREFIX}." +
            "RType9a<System.Nullable<System.Byte>, System.Nullable<System.Int32>>." +
            "RType9b<System.String>.Item[System.Nullable<System.Int32> one, System.String? two]",
            name);
    }
}
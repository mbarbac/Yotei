#pragma warning disable CA1822
#pragma warning disable IDE0044
#pragma warning disable IDE0060

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyMethod
{
    readonly static string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    readonly static string TESTHOST = "Test_EasyMethod";

    readonly static EasyMethodOptions EMPTY = EasyMethodOptions.Empty;
    readonly static EasyMethodOptions DEFAULT = EasyMethodOptions.Default;
    readonly static EasyMethodOptions FULL = EasyMethodOptions.Full;

    // ----------------------------------------------------

    interface IFace0
    {
        // Need [IsNullable] to persist annotation...
        void Name1(byte one, out int? two, [IsNullable] ref string? three, in long four);

        static int RetValue;
        static ref int Name2() => ref RetValue;
        static ref readonly int Name3() => ref RetValue;
        sealed void Name4() { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_OnInterface_Arguments()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace0);
        var item = type.GetMethod("Name1"); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name1", name);

        options = DEFAULT;
        name = item.EasyName(options);
        Assert.Equal("Name1(byte, out int?, ref string?, in long)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Void {NAMESPACE}.{TESTHOST}.IFace0.Name1(" +
            "System.Byte one, out System.Nullable<System.Int32> two, " +
            "ref System.String? three, in System.Int64 four)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_OnInterface_Returns_Ref()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace0);
        var item = type.GetMethod("Name2"); Assert.NotNull(item);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"static ref System.Int32 {NAMESPACE}.{TESTHOST}.IFace0.Name2()",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_OnInterface_Returns_RefReadOnly()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace0);
        var item = type.GetMethod("Name3"); Assert.NotNull(item);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"static ref readonly System.Int32 {NAMESPACE}.{TESTHOST}.IFace0.Name3()",
            name);
    }
    
    //[Enforced]
    [Fact]
    public static void Test_Standard_OnInterface_Sealed()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace0);
        var item = type.GetMethod("Name4"); Assert.NotNull(item);

        // LOW: I have not found how to obtain 'sealed' in a reliable way...
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Void {NAMESPACE}.{TESTHOST}.IFace0.Name4()",
            name);
    }

    // ----------------------------------------------------

    class Type0A
    {
        // Need [IsNullable] to persist annotation...
        public void Name1(byte one, out int? two, [IsNullable] ref string? three, in long four) { two = default!; }

        protected virtual ref int Name2() => ref RetValue;
        int RetValue;

        internal static ref readonly int Name3() => ref StaticValue;
        static int StaticValue;

        public virtual void Name4() { }
    }

    class Type0B : Type0A
    {
        protected override ref int Name2() => ref base.Name2();
        public sealed override void Name4() => base.Name4();
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_OnType_Arguments()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type0A);
        var item = type.GetMethod("Name1"); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name1", name);

        options = DEFAULT;
        name = item.EasyName(options);
        Assert.Equal("Name1(byte, out int?, ref string?, in long)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"public System.Void {NAMESPACE}.{TESTHOST}.Type0A.Name1(" +
            "System.Byte one, out System.Nullable<System.Int32> two, " +
            "ref System.String? three, in System.Int64 four)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_OnType_Protected_Virtual_Returns_Ref()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type0A);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var item = type.GetMethod("Name2", flags); Assert.NotNull(item);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"protected virtual ref System.Int32 {NAMESPACE}.{TESTHOST}.Type0A.Name2()",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_OnType_Static_Internal_Returns_RefReadOnly()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type0A);
        var flags = BindingFlags.Static | BindingFlags.NonPublic;
        var item = type.GetMethod("Name3", flags); Assert.NotNull(item);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"internal static ref readonly System.Int32 {NAMESPACE}.{TESTHOST}.Type0A.Name3()",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_OnInherit_Protected_Override()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type0B);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var item = type.GetMethod("Name2", flags); Assert.NotNull(item);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"protected override ref System.Int32 {NAMESPACE}.{TESTHOST}.Type0B.Name2()",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_OnInherit_Public_Sealed_Override()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type0B);
        var flags = BindingFlags.Instance | BindingFlags.Public;
        var item = type.GetMethod("Name4", flags); Assert.NotNull(item);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"public override System.Void {NAMESPACE}.{TESTHOST}.Type0B.Name4()",
            name);
    }

    // ----------------------------------------------------

    interface IFace1A<[IsNullable] K, [IsNullable] T>
    { public interface IFace1B<S> { K? Name<R>(ref T? one, S? two); } }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Generics_Unbound()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace1A<,>.IFace1B<>);
        var item = type.GetMethod("Name"); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name<R>(ref T?, S?)", name);

        options = DEFAULT with { ReturnTypeOptions = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("K? Name<R>(ref T?, S?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"K? {NAMESPACE}.{TESTHOST}." +
            "IFace1A<K?, T?>.IFace1B<S>.Name<R>(ref T? one, S? two)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Generics_Bound_RefNullableNeedsWrapper()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace1A<byte?, short?>.IFace1B<IsNullable<string?>>);
        var item = type.GetMethod("Name"); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name<R>(ref short?, string?)", name);

        options = DEFAULT with { ReturnTypeOptions = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("byte? Name<R>(ref short?, string?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"System.Nullable<System.Byte> {NAMESPACE}.{TESTHOST}." +
            "IFace1A<System.Nullable<System.Byte>, System.Nullable<System.Int16>>." +
            "IFace1B<Yotei.Tools.IsNullable<System.String>>." +
            "Name<R>(ref System.Nullable<System.Int16> one, Yotei.Tools.IsNullable<System.String> two)",
            name);
    }

    // ----------------------------------------------------

    interface IFace2A<[IsNullable] K, [IsNullable] T>
    { public interface IFace2B<S> { K? Name<R>([IsNullable] ref T? one, [IsNullable] S? two); } }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Generics_Unbound_NullabilityOfParametersByAttribute()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace2A<,>.IFace2B<>);
        var item = type.GetMethod("Name"); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name<R>(ref T?, S?)", name);

        options = DEFAULT with { ReturnTypeOptions = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("K? Name<R>(ref T?, S?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"K? {NAMESPACE}.{TESTHOST}." +
            "IFace2A<K?, T?>.IFace2B<S>.Name<R>(ref T? one, S? two)",
            name);
    }

    // ----------------------------------------------------

    class Type3A { public int Name() => 0; }
    class Type3B : Type3A { public new int Name() => 0; }

    //[Enforced]
    [Fact]
    public static void Test_NewMethod()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type3B);
        var item = type.GetMethod("Name"); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name()", name);

        options = DEFAULT with { ReturnTypeOptions = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("int Name()", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"public new System.Int32 {NAMESPACE}.{TESTHOST}.Type3B.Name()",
            name);
    }

    // ----------------------------------------------------

    class Type4A { public virtual int Name() => 0; }
    class Type4B : Type4A { public new int Name() => 0; }

    //[Enforced]
    [Fact]
    public static void Test_NewMethod_FromVirtual()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type4B);
        var item = type.GetMethod("Name"); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Name", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Name()", name);

        options = DEFAULT with { ReturnTypeOptions = EasyTypeOptions.Default };
        name = item.EasyName(options); Assert.Equal("int Name()", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"public new System.Int32 {NAMESPACE}.{TESTHOST}.Type4B.Name()",
            name);
    }
}
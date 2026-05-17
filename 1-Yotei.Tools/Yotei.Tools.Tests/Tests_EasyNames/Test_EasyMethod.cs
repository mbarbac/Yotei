#pragma warning disable IDE0060, CA1822

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyMethod
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyMethod";

    // ----------------------------------------------------

    interface IFace1 { void Method(int? one); }

    //[Enforced]
    [Fact]
    public static void Test_Returns_Void()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace1);
        var source = type.GetMethod("Method")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method(int?)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method(System.Nullable<System.Int32> one)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("void Method(System.Nullable<System.Int32> one)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"System.Void {PREFIX}.IFace1.Method(System.Nullable<System.Int32> one)",
            name);
    }

    // ----------------------------------------------------

    interface IFace2 { int? Method(int one); }

    //[Enforced]
    [Fact]
    public static void Test_Returns_ValueType()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace2);
        var source = type.GetMethod("Method")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method(int)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method(System.Int32 one)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("int? Method(System.Int32 one)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"System.Nullable<System.Int32> {PREFIX}.IFace2.Method(System.Int32 one)",
            name);
    }

    // ----------------------------------------------------

    interface IFace3a { string? Method(int one); }

    //[Enforced]
    [Fact]
    public static void Test_Returns_ReferenceType_NullabilityLost()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace3a);
        var source = type.GetMethod("Method")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method(int)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method(System.Int32 one)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("string Method(System.Int32 one)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"System.String {PREFIX}.IFace3a.Method(System.Int32 one)",
            name);
    }

    // ----------------------------------------------------

    interface IFace3b { IsNullable<string?> Method(int one); }

    //[Enforced]
    [Fact]
    public static void Test_Returns_ReferenceType_NullabilityWrapped()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace3b);
        var source = type.GetMethod("Method")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method(int)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method(System.Int32 one)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("string? Method(System.Int32 one)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"Yotei.Tools.IsNullable<System.String> {PREFIX}.IFace3b.Method(System.Int32 one)",
            name);
    }

    // ----------------------------------------------------

    interface IFace3c { [IsNullable] string? Method(int one); }

    //[Enforced]
    [Fact]
    public static void Test_Returns_ReferenceType_NullabilityAttribute()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace3c);
        var source = type.GetMethod("Method")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method(int)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method(System.Int32 one)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("string? Method(System.Int32 one)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"System.String? {PREFIX}.IFace3c.Method(System.Int32 one)",
            name);
    }

    // ----------------------------------------------------

    class RType4a
    {
        static int? RetValue;
        public static ref int? Method1(byte one) => ref RetValue;
        public static ref readonly int? Method2(byte one) => ref RetValue;
    }

    //[Enforced]
    [Fact]
    public static void Test_Returns_ByRef_ValueType()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType4a);
        var source = type.GetMethod("Method1")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method1", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method1(byte)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method1(System.Byte one)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public static ref int? Method1(System.Byte one)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public static ref System.Nullable<System.Int32> {PREFIX}.RType4a.Method1(System.Byte one)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Returns_ByRef_Readonly_ValueType()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType4a);
        var source = type.GetMethod("Method2")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method2", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method2(byte)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method2(System.Byte one)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public static ref readonly int? Method2(System.Byte one)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public static ref readonly System.Nullable<System.Int32> " +
            $"{PREFIX}.RType4a.Method2(System.Byte one)",
            name);
    }

    // ----------------------------------------------------

    interface IFace4
    {
        static string? RetValue;

        static ref readonly string? Method1a(byte one) => ref RetValue;
        // We cannot use this return type, doesn't match with returned value!
        // static ref IsNullable<string> Method1b(byte one) => ref RetValue;
        [IsNullable] static ref string? Method1b(byte one) => ref RetValue;
    }

    //[Enforced]
    [Fact]
    public static void Test_Returns_ByRef_ReferenceType_NullabilityLost()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace4);
        var source = type.GetMethod("Method1a")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method1a", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method1a(byte)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method1a(System.Byte one)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("static ref readonly string Method1a(System.Byte one)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"static ref readonly System.String {PREFIX}.IFace4.Method1a(System.Byte one)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Returns_ByRef_ReferenceType_NullabilityAttribute()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace4);
        var source = type.GetMethod("Method1b")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method1b", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method1b(byte)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method1b(System.Byte one)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("static ref string? Method1b(System.Byte one)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"static ref System.String? {PREFIX}.IFace4.Method1b(System.Byte one)",
            name);
    }

    // ----------------------------------------------------

    interface IFace5 { public void Method(int age); };

    //[Enforced]
    [Fact]
    public static void Test_Accessibility_OnInterface()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace5);
        var source = type.GetMethod("Method")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method(int)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method(System.Int32 age)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("void Method(System.Int32 age)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"System.Void {PREFIX}.IFace5.Method(System.Int32 age)",
            name);
    }

    // ----------------------------------------------------

    class RType5
    {
        public void Method1(int age) { }
        protected void Method2(int age) { }
        private void Method3(int age) { }
        internal void Method4(int age) { }
        internal protected void Method5(int age) { }
        private protected void Method6(int age) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Accessibility_OnType_Public()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType5);
        var source = type.GetMethod("Method1")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method1", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method1(int)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method1(System.Int32 age)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public void Method1(System.Int32 age)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public System.Void {PREFIX}.RType5.Method1(System.Int32 age)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Accessibility_OnType_Protected()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType5);
        var source = type.GetMethod("Method2", BindingFlags.Instance | BindingFlags.NonPublic)!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method2", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method2(int)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method2(System.Int32 age)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("protected void Method2(System.Int32 age)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"protected System.Void {PREFIX}.RType5.Method2(System.Int32 age)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Accessibility_OnType_Private()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType5);
        var source = type.GetMethod("Method3", BindingFlags.Instance | BindingFlags.NonPublic)!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method3", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method3(int)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method3(System.Int32 age)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("private void Method3(System.Int32 age)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"private System.Void {PREFIX}.RType5.Method3(System.Int32 age)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Accessibility_OnType_Internal()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType5);
        var source = type.GetMethod("Method4", BindingFlags.Instance | BindingFlags.NonPublic)!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method4", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method4(int)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method4(System.Int32 age)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("internal void Method4(System.Int32 age)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"internal System.Void {PREFIX}.RType5.Method4(System.Int32 age)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Accessibility_OnType_Internal_Protected()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType5);
        var source = type.GetMethod("Method5", BindingFlags.Instance | BindingFlags.NonPublic)!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method5", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method5(int)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method5(System.Int32 age)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("internal protected void Method5(System.Int32 age)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"internal protected System.Void {PREFIX}.RType5.Method5(System.Int32 age)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Accessibility_OnType_Private_Protected()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType5);
        var source = type.GetMethod("Method6", BindingFlags.Instance | BindingFlags.NonPublic)!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method6", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method6(int)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method6(System.Int32 age)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("private protected void Method6(System.Int32 age)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"private protected System.Void {PREFIX}.RType5.Method6(System.Int32 age)",
            name);
    }

    // ----------------------------------------------------

    abstract class AType1a
    {
        public abstract void Method1(int? age);
        public abstract void Method2(int? age);
    }

    abstract class AType1b : AType1a
    {
        public override void Method1(int? age) { }
        public abstract override void Method2(int? age);
    }

    //[Enforced]
    [Fact]
    public static void Test_Abstract_Inheritance_Override()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(AType1b);
        var source = type.GetMethod("Method1")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method1", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method1(int?)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method1(System.Nullable<System.Int32> age)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public override void Method1(System.Nullable<System.Int32> age)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public override System.Void {PREFIX}.AType1b.Method1(System.Nullable<System.Int32> age)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Abstract_Inheritance_Abstract_Override()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(AType1b);
        var source = type.GetMethod("Method2")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method2", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method2(int?)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method2(System.Nullable<System.Int32> age)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public abstract override void Method2(System.Nullable<System.Int32> age)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public abstract override System.Void {PREFIX}.AType1b.Method2(System.Nullable<System.Int32> age)",
            name);
    }

    // ----------------------------------------------------

    class RType6a
    {
        public void Method1(int? age) { }
        public virtual void Method2(int? age) { }
        public virtual void Method3(int? age) { }
    }

    class RType6b : RType6a
    {
        public new void Method1(int? age) { }
        public override void Method2(int? age) { }
        public sealed override void Method3(int? age) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Regular_Inheritance_New()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType6b);
        var source = type.GetMethod("Method1")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method1", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method1(int?)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method1(System.Nullable<System.Int32> age)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public new void Method1(System.Nullable<System.Int32> age)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public new System.Void {PREFIX}.RType6b.Method1(System.Nullable<System.Int32> age)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Regular_Inheritance_Override()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType6b);
        var source = type.GetMethod("Method2")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method2", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method2(int?)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method2(System.Nullable<System.Int32> age)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public override void Method2(System.Nullable<System.Int32> age)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public override System.Void {PREFIX}.RType6b.Method2(System.Nullable<System.Int32> age)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Regular_Inheritance_Sealed_Override()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType6b);
        var source = type.GetMethod("Method3")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method3", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method3(int?)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method3(System.Nullable<System.Int32> age)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal(
            "public sealed override void Method3(System.Nullable<System.Int32> age)",
            name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public sealed override System.Void {PREFIX}." +
            "RType6b.Method3(System.Nullable<System.Int32> age)",
            name);
    }

    // ----------------------------------------------------

    public class RType7a<[IsNullable] K, [IsNullable] T>
    {
        public class RType7b<S>
        {
            public S? Method1(ref T? one, in S? two, out K three) => throw new UnExpectedException();
            public IsNullable<S?> Method2(ref T? one, in S? two) => throw new UnExpectedException();
            [IsNullable] public S? Method3(ref T? one, in S? two) => throw new UnExpectedException();
        }
    }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Unbound_ReturnNullabilityLost()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType7a<,>.RType7b<>);
        var source = type.GetMethod("Method1")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method1", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method1(ref T?, in S?, out K?)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method1(ref T? one, in S? two, out K? three)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal(
            "public S Method1(ref T? one, in S? two, out K? three)",
            name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public S {PREFIX}.RType7a<K, T>.RType7b<S>.Method1(ref T? one, in S? two, out K? three)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Unbound_ReturnNullabilityWrapped()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType7a<,>.RType7b<>);
        var source = type.GetMethod("Method2")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method2", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method2(ref T?, in S?)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method2(ref T? one, in S? two)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public S? Method2(ref T? one, in S? two)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public Yotei.Tools.IsNullable<S> {PREFIX}." +
            "RType7a<K, T>.RType7b<S>.Method2(ref T? one, in S? two)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Unbound_ReturnNullabilityAttribute()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType7a<,>.RType7b<>);
        var source = type.GetMethod("Method3")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method3", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method3(ref T?, in S?)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method3(ref T? one, in S? two)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public S? Method3(ref T? one, in S? two)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public S? {PREFIX}." +
            "RType7a<K, T>.RType7b<S>.Method3(ref T? one, in S? two)",
            name);
    }
}
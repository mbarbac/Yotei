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

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace1.Method(int?)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"void {PREFIX}.IFace1.Method(int?)", name);

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

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace2.Method(int)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"int? {PREFIX}.IFace2.Method(int)", name);

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

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace3a.Method(int)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"string {PREFIX}.IFace3a.Method(int)", name);

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

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3b.Method(int)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"string? {PREFIX}.IFace3b.Method(int)", name);

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

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3c.Method(int)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"string? {PREFIX}.IFace3c.Method(int)", name);

        options = EasyMethodOptions.Full;
        options.ReturnTypeOptions!.UseSpecialNames = false;
        options.ParameterOptions!.TypeOptions!.UseSpecialNames = false;
        name = source.EasyName(options);
        Assert.Equal(
            $"System.String? {PREFIX}.IFace3c.Method(System.Int32 one)",
            name);
    }

    // ----------------------------------------------------

    interface IFace4a
    {
        static int? RetValue;
        static ref int? Method1(byte one) => ref RetValue;
        static ref readonly int? Method2(byte one) => ref RetValue;
    }

    //[Enforced]
    [Fact]
    public static void Test_Returns_ByRef_ValueType()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace4a);
        var source = type.GetMethod("Method1")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method1", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method1(byte)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace4a.Method1(byte)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"static ref int? {PREFIX}.IFace4a.Method1(byte)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"static ref System.Nullable<System.Int32> {PREFIX}.IFace4a.Method1(System.Byte one)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Returns_ByRef_Readonly_ValueType()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace4a);
        var source = type.GetMethod("Method2")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method2", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method2(byte)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace4a.Method2(byte)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"static ref readonly int? {PREFIX}.IFace4a.Method2(byte)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"static ref readonly System.Nullable<System.Int32> " +
            $"{PREFIX}.IFace4a.Method2(System.Byte one)",
            name);
    }

    // ----------------------------------------------------

    interface IFace4b
    {
        static string? RetValue;

        static ref string? Method1a(byte one) => ref RetValue;
        [IsNullable] static ref string? Method1b(byte one) => ref RetValue;

        static ref readonly string? Method2a(byte one) => ref RetValue;
        [IsNullable] static ref readonly string? Method2b(byte one) => ref RetValue;
    }

    //[Enforced]
    [Fact]
    public static void Test_Returns_ByRef_ReferenceType_NullabilityLost()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace4b);
        var source = type.GetMethod("Method1a")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method1a", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method1a(byte)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace4b.Method1a(byte)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"static ref string {PREFIX}.IFace4b.Method1a(byte)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"static ref System.String {PREFIX}.IFace4b.Method1a(System.Byte one)",
            name);
    }

    // This is an interesting case where we cannot use 'IsNullable<>' (the ref returned is the
    // wrapper instead of the string), so we must use the [IsNullable] attribute...
    //[Enforced]
    [Fact]
    public static void Test_Returns_ByRef_ReferenceType_NullabilityAttribute()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace4b);
        var source = type.GetMethod("Method1b")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method1b", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method1b(byte)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace4b.Method1b(byte)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"static ref string? {PREFIX}.IFace4b.Method1b(byte)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"static ref System.String? {PREFIX}.IFace4b.Method1b(System.Byte one)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Returns_ByRef_Readonly_ReferenceType_NullabilityLost()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace4b);
        var source = type.GetMethod("Method2a")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method2a", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method2a(byte)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace4b.Method2a(byte)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"static ref readonly string {PREFIX}.IFace4b.Method2a(byte)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"static ref readonly System.String {PREFIX}.IFace4b.Method2a(System.Byte one)",
            name);
    }

    // This is an interesting case where we cannot use 'IsNullable<>' (the ref returned is the
    // wrapper instead of the string), so we must use the [IsNullable] attribute...
    //[Enforced]
    [Fact]
    public static void Test_Returns_ByRef_Readonly_ReferenceType_NullabilityAttribute()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace4b);
        var source = type.GetMethod("Method2b")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method2b", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method2b(byte)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace4b.Method2b(byte)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"static ref readonly string? {PREFIX}.IFace4b.Method2b(byte)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"static ref readonly System.String? {PREFIX}.IFace4b.Method2b(System.Byte one)",
            name);
    }

    // ----------------------------------------------------

    interface IFace5 { public void Method(int age); };
    class RType1
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

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace5.Method(int)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"void {PREFIX}.IFace5.Method(int)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"System.Void {PREFIX}.IFace5.Method(System.Int32 age)", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Accessibility_OnType_Public()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType1);
        var source = type.GetMethod("Method1")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method1", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method1(int)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.RType1.Method1(int)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"public void {PREFIX}.RType1.Method1(int)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"public System.Void {PREFIX}.RType1.Method1(System.Int32 age)", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Accessibility_OnType_Protected()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType1);
        var source = type.GetMethod("Method2", BindingFlags.Instance | BindingFlags.NonPublic)!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method2", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method2(int)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.RType1.Method2(int)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"protected void {PREFIX}.RType1.Method2(int)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"protected System.Void {PREFIX}.RType1.Method2(System.Int32 age)", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Accessibility_OnType_Private()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType1);
        var source = type.GetMethod("Method3", BindingFlags.Instance | BindingFlags.NonPublic)!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method3", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method3(int)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.RType1.Method3(int)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"void {PREFIX}.RType1.Method3(int)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"System.Void {PREFIX}.RType1.Method3(System.Int32 age)", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Accessibility_OnType_Internal()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType1);
        var source = type.GetMethod("Method4", BindingFlags.Instance | BindingFlags.NonPublic)!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method4", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method4(int)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.RType1.Method4(int)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"internal void {PREFIX}.RType1.Method4(int)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"internal System.Void {PREFIX}.RType1.Method4(System.Int32 age)", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Accessibility_OnType_InternalProtected()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType1);
        var source = type.GetMethod("Method5", BindingFlags.Instance | BindingFlags.NonPublic)!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method5", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method5(int)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.RType1.Method5(int)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"internal protected void {PREFIX}.RType1.Method5(int)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"internal protected System.Void {PREFIX}.RType1.Method5(System.Int32 age)", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Accessibility_OnType_PrivateProtected()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType1);
        var source = type.GetMethod("Method6", BindingFlags.Instance | BindingFlags.NonPublic)!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method6", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method6(int)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.RType1.Method6(int)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"private protected void {PREFIX}.RType1.Method6(int)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"private protected System.Void {PREFIX}.RType1.Method6(System.Int32 age)", name);
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
        name = source.EasyName(options);
        Assert.Equal("Method1(int?)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.AType1b.Method1(int?)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"public override void {PREFIX}.AType1b.Method1(int?)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public override System.Void {PREFIX}.AType1b.Method1(System.Nullable<System.Int32> age)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Abstract_Inheritance_AbstractOverride()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(AType1b);
        var source = type.GetMethod("Method2")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method2", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method2(int?)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.AType1b.Method2(int?)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"public abstract override void {PREFIX}.AType1b.Method2(int?)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public abstract override System.Void {PREFIX}." +
            "AType1b.Method2(System.Nullable<System.Int32> age)",
            name);
    }

    // ----------------------------------------------------

    class RType2a
    {
        public void Method1(int? age) { }
        public virtual void Method2(int? age) { }
        public virtual void Method3(int? age) { }
    }

    class RType2b : RType2a
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
        var type = typeof(RType2b);
        var source = type.GetMethod("Method1")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method1", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method1(int?)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.RType2b.Method1(int?)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"public new void {PREFIX}.RType2b.Method1(int?)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public new System.Void {PREFIX}.RType2b.Method1(System.Nullable<System.Int32> age)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Regular_Inheritance_Override()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType2b);
        var source = type.GetMethod("Method2")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method2", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method2(int?)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.RType2b.Method2(int?)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"public override void {PREFIX}.RType2b.Method2(int?)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public override System.Void {PREFIX}.RType2b.Method2(System.Nullable<System.Int32> age)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Regular_Inheritance_SealedOverride()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType2b);
        var source = type.GetMethod("Method3")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method3", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method3(int?)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.RType2b.Method3(int?)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal($"public sealed override void {PREFIX}.RType2b.Method3(int?)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public sealed override System.Void {PREFIX}." +
            "RType2b.Method3(System.Nullable<System.Int32> age)",
            name);
    }

    // ----------------------------------------------------

    public class RType3a<[IsNullable] K, [IsNullable] T>
    {
        public class RType3b<S>
        {
            public S? Method1(ref T? one, in S? two) => throw new UnExpectedException();
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
        var type = typeof(RType3a<,>.RType3b<>);
        var source = type.GetMethod("Method1")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method1", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method1(ref T?, in S?)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.RType3a<K?, T?>.RType3b<S>.Method1(ref T?, in S?)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal(
            $"public S {PREFIX}.RType3a<K?, T?>.RType3b<S>.Method1(ref T?, in S?)",
            name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public S {PREFIX}.RType3a<K?, T?>.RType3b<S>.Method1(ref T? one, in S? two)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Unbound_ReturnNullabilityWrapped()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType3a<,>.RType3b<>);
        var source = type.GetMethod("Method2")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method2", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method2(ref T?, in S?)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.RType3a<K?, T?>.RType3b<S>.Method2(ref T?, in S?)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal(
            $"public S? {PREFIX}.RType3a<K?, T?>.RType3b<S>.Method2(ref T?, in S?)",
            name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            "public Yotei.Tools.IsNullable<S> " +
            $"{PREFIX}.RType3a<K?, T?>.RType3b<S>.Method2(ref T? one, in S? two)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Unbound_ReturnNullabilityAttribute()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType3a<,>.RType3b<>);
        var source = type.GetMethod("Method3")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method3", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method3(ref T?, in S?)", name);

        options = EasyMethodOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.RType3a<K?, T?>.RType3b<S>.Method3(ref T?, in S?)", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        options.UseAccessibility = true;
        options.UseModifiers = true;
        name = source.EasyName(options);
        Assert.Equal(
            $"public S? {PREFIX}.RType3a<K?, T?>.RType3b<S>.Method3(ref T?, in S?)",
            name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            "public S? " +
            $"{PREFIX}.RType3a<K?, T?>.RType3b<S>.Method3(ref T? one, in S? two)",
            name);
    }
}
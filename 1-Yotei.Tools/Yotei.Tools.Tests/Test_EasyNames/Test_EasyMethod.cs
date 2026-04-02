#pragma warning disable CA1822, IDE0060

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyMethod
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyMethod";

    readonly static EasyMethodOptions EMPTY = EasyMethodOptions.Empty;
    readonly static EasyMethodOptions DEFAULT = EasyMethodOptions.Default;
    readonly static EasyMethodOptions FULL = EasyMethodOptions.Full;

    // ----------------------------------------------------

    class Type1A
    {
        public virtual void Method1(int age) { }
        public virtual void Method2(int age) { }
        public virtual void Method3(int age) { }
    }

    class Type1B : Type1A
    {
        public override void Method1(int age) { }
        public sealed override void Method2(int age) { }
        public new void Method3(int age) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Inheritance_Override()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type1B);
        var source = type.GetMethod("Method1"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method1", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method1(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public override System.Void {PREFIX}.Type1B.Method1(System.Int32 age)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Inheritance_Sealed_Override()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type1B);
        var source = type.GetMethod("Method2"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method2", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method2(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public sealed override System.Void {PREFIX}.Type1B.Method2(System.Int32 age)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Inheritance_New()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type1B);
        var source = type.GetMethod("Method3"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method3", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method3(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public new System.Void {PREFIX}.Type1B.Method3(System.Int32 age)",
            name);
    }

    // ----------------------------------------------------

    class Type2
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
    public static void Test_Accessibility_Public()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type2);
        var source = type.GetMethod("Method1"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method1", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method1(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public System.Void {PREFIX}.Type2.Method1(System.Int32 age)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Accessibility_Protected()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type2);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var source = type.GetMethod("Method2", flags); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method2", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method2(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"protected System.Void {PREFIX}.Type2.Method2(System.Int32 age)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Accessibility_Private()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type2);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var source = type.GetMethod("Method3", flags); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method3", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method3(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"private System.Void {PREFIX}.Type2.Method3(System.Int32 age)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Accessibility_Internal()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type2);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var source = type.GetMethod("Method4", flags); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method4", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method4(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"internal System.Void {PREFIX}.Type2.Method4(System.Int32 age)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Accessibility_Internal_Protected()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type2);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var source = type.GetMethod("Method5", flags); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method5", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method5(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"internal protected System.Void {PREFIX}.Type2.Method5(System.Int32 age)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Accessibility_Private_Protected()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type2);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var source = type.GetMethod("Method6", flags); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method6", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method6(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"private protected System.Void {PREFIX}.Type2.Method6(System.Int32 age)",
            name);
    }
}
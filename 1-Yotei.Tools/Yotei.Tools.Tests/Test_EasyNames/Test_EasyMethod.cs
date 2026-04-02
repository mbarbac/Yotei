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

    class Type1a
    {
        public virtual void Method1(int age) { }
        public virtual void Method2(int age) { }
        public virtual void Method3(int age) { }
    }

    class Type1b : Type1a
    {
        public override void Method1(int age) { }
        public sealed override void Method2(int age) { }
        public new void Method3(int age) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Inheritance_OnType()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type1b);

        // Override...
        var source = type.GetMethod("Method1"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method1", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method1(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public override System.Void {PREFIX}.Type1b.Method1(System.Int32 age)",
            name);

        // Sealed override...
        source = type.GetMethod("Method2"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method2", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method2(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public sealed override System.Void {PREFIX}.Type1b.Method2(System.Int32 age)",
            name);

        // New..
        source = type.GetMethod("Method3"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method3", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method3(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public new System.Void {PREFIX}.Type1b.Method3(System.Int32 age)",
            name);
    }

    // ----------------------------------------------------

    interface IFace1a { void Method1(int age); }

    interface IFace1b : IFace1a { new void Method1(int age); }

    //[Enforced]
    [Fact]
    public static void Test_Inheritance_OnInterface()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace1b);

        // New...
        var source = type.GetMethod("Method1"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method1", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method1(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"new System.Void {PREFIX}.IFace1b.Method1(System.Int32 age)",
            name);
    }

    // ----------------------------------------------------

    abstract class AType1a
    {
        public abstract void Method1(int age);
        public abstract void Method2(int age);
        public abstract void Method3(int age);

        public virtual void Method4(int age) { }
    }

    abstract class AType1b : AType1a
    {
        public override void Method1(int age) { }
        public override abstract void Method2(int age);
    }

    //[Enforced]
    [Fact]
    public static void Test_Abstract()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(AType1a);

        // Abstract
        var source = type.GetMethod("Method1"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method1", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method1(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public abstract System.Void {PREFIX}.AType1a.Method1(System.Int32 age)",
            name);

        // Virtual
        source = type.GetMethod("Method4"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method4", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method4(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public virtual System.Void {PREFIX}.AType1a.Method4(System.Int32 age)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Inheritance_OnAbstract()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(AType1b);

        // Override...
        var source = type.GetMethod("Method1"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method1", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method1(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public override System.Void {PREFIX}.AType1b.Method1(System.Int32 age)",
            name);

        // Abstract override...
        source = type.GetMethod("Method2"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method2", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method2(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public abstract override System.Void {PREFIX}.AType1b.Method2(System.Int32 age)",
            name);
    }

    // ----------------------------------------------------

    class Type3
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
    public static void Test_Accessibility_OnType()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type3);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;

        // Public...
        var source = type.GetMethod("Method1"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method1", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method1(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public System.Void {PREFIX}.Type3.Method1(System.Int32 age)",
            name);

        // Protected...
        source = type.GetMethod("Method2", flags); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method2", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method2(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"protected System.Void {PREFIX}.Type3.Method2(System.Int32 age)",
            name);

        // Private...
        source = type.GetMethod("Method3", flags); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method3", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method3(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"private System.Void {PREFIX}.Type3.Method3(System.Int32 age)",
            name);

        // Internal...
        source = type.GetMethod("Method4", flags); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method4", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method4(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"internal System.Void {PREFIX}.Type3.Method4(System.Int32 age)",
            name);

        // Internal protected...
        source = type.GetMethod("Method5", flags); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method5", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method5(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"internal protected System.Void {PREFIX}.Type3.Method5(System.Int32 age)",
            name);

        // Private protected...
        source = type.GetMethod("Method6", flags); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Method6", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Method6(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"private protected System.Void {PREFIX}.Type3.Method6(System.Int32 age)",
            name);
    }
}
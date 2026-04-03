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
        public virtual void Name1(int age) { }
        public virtual void Name2(int age) { }
        public virtual void Name3(int age) { }
    }

    class Type1b : Type1a
    {
        public override void Name1(int age) { }
        public sealed override void Name2(int age) { }
        public new void Name3(int age) { }
    }

    //[Enforced]
    [Fact]
    public static void Test_Inheritance_OnType()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type1b);

        // Override...
        var source = type.GetMethod("Name1"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name1", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name1(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public override System.Void {PREFIX}.Type1b.Name1(System.Int32 age)",
            name);

        // Sealed override...
        source = type.GetMethod("Name2"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name2", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name2(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public sealed override System.Void {PREFIX}.Type1b.Name2(System.Int32 age)",
            name);

        // New..
        source = type.GetMethod("Name3"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name3", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name3(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public new System.Void {PREFIX}.Type1b.Name3(System.Int32 age)",
            name);
    }

    // ----------------------------------------------------

    interface IFace1a { void Name1(int age); }

    interface IFace1b : IFace1a { new void Name1(int age); }

    //[Enforced]
    [Fact]
    public static void Test_Inheritance_OnInterface()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace1b);

        // New...
        var source = type.GetMethod("Name1"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name1", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name1(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"new System.Void {PREFIX}.IFace1b.Name1(System.Int32 age)",
            name);
    }

    // ----------------------------------------------------

    abstract class AType1a
    {
        public abstract void Name1(int age);
        public abstract void Name2(int age);
        public abstract void Name3(int age);

        public virtual void Name4(int age) { }
    }

    abstract class AType1b : AType1a
    {
        public override void Name1(int age) { }
        public override abstract void Name2(int age);
    }

    //[Enforced]
    [Fact]
    public static void Test_Abstract()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(AType1a);

        // Abstract
        var source = type.GetMethod("Name1"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name1", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name1(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public abstract System.Void {PREFIX}.AType1a.Name1(System.Int32 age)",
            name);

        // Virtual
        source = type.GetMethod("Name4"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name4", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name4(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public virtual System.Void {PREFIX}.AType1a.Name4(System.Int32 age)",
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
        var source = type.GetMethod("Name1"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name1", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name1(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public override System.Void {PREFIX}.AType1b.Name1(System.Int32 age)",
            name);

        // Abstract override...
        source = type.GetMethod("Name2"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name2", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name2(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public abstract override System.Void {PREFIX}.AType1b.Name2(System.Int32 age)",
            name);
    }

    // ----------------------------------------------------

    class Type3
    {
        public void Name1(int age) { }
        protected void Name2(int age) { }
        private void Name3(int age) { }
        internal void Name4(int age) { }
        internal protected void Name5(int age) { }
        private protected void Name6(int age) { }
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
        var source = type.GetMethod("Name1"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name1", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name1(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"public System.Void {PREFIX}.Type3.Name1(System.Int32 age)",
            name);

        // Protected...
        source = type.GetMethod("Name2", flags); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name2", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name2(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"protected System.Void {PREFIX}.Type3.Name2(System.Int32 age)",
            name);

        // Private...
        source = type.GetMethod("Name3", flags); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name3", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name3(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"private System.Void {PREFIX}.Type3.Name3(System.Int32 age)",
            name);

        // Internal...
        source = type.GetMethod("Name4", flags); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name4", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name4(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"internal System.Void {PREFIX}.Type3.Name4(System.Int32 age)",
            name);

        // Internal protected...
        source = type.GetMethod("Name5", flags); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name5", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name5(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"internal protected System.Void {PREFIX}.Type3.Name5(System.Int32 age)",
            name);

        // Private protected...
        source = type.GetMethod("Name6", flags); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name6", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name6(int)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"private protected System.Void {PREFIX}.Type3.Name6(System.Int32 age)",
            name);
    }

    // ----------------------------------------------------

    interface IFace2
    {
        static int RetValue;

        void Name1(byte one, out int? two, [IsNullable] ref string? three, in long four);
        static ref int Name2() => ref RetValue;
        static ref readonly int Name3() => ref RetValue;
    }

    //[Enforced]
    [Fact]
    public static void Test_Argument_Modifiers()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace2);
        var source = type.GetMethod("Name1"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name1", name);

        options = DEFAULT;
        name = source.EasyName(options);
        Assert.Equal("Name1(byte, out int?, ref string?, in long)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"System.Void {PREFIX}.IFace2.Name1(" +
            "System.Byte one, out System.Nullable<System.Int32> two, " +
            "ref System.String? three, in System.Int64 four)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Return_Ref()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace2);

        // Returns 'ref'
        var source = type.GetMethod("Name2"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name2", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name2()", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal($"static ref System.Int32 {PREFIX}.IFace2.Name2()", name);

        // Returns 'ref readonly'
        source = type.GetMethod("Name3"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name3", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name3()", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal($"static ref readonly System.Int32 {PREFIX}.IFace2.Name3()", name);
    }

    // ----------------------------------------------------

    interface IFace3a<[IsNullable] K, [IsNullable] T>
    {
        interface IFace3b<S>
        {
            S? Name1<R>(ref T? one, S? two);
            [IsNullable] S? Name2<R>(ref T? one, S? two);
        }
    }

    //[Enforced]
    [Fact]
    public static void Test_Nested()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace3a<,>.IFace3b<>);

        // Nullable return lost...
        var source = type.GetMethod("Name1"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name1", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name1<R>(ref T?, S?)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"S {PREFIX}.IFace3a<K?, T?>.IFace3b<S>.Name1<R>(ref T? one, S? two)",
            name);

        // Nullable return explicit...
        source = type.GetMethod("Name2"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name2", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name2<R>(ref T?, S?)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"S? {PREFIX}.IFace3a<K?, T?>.IFace3b<S>.Name2<R>(ref T? one, S? two)",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Bound()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace3a<byte?, int?>.IFace3b<string?>);

        // Nullable return lost...
        var source = type.GetMethod("Name1"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name1", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name1<R>(ref int?, string?)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"System.String {PREFIX}." +
            "IFace3a<System.Nullable<System.Byte>, System.Nullable<System.Int32>>." +
            "IFace3b<System.String>.Name1<R>(" +
            "ref System.Nullable<System.Int32> one, System.String? two)",
            name);

        // Nullable return explicit...
        source = type.GetMethod("Name2"); Assert.NotNull(source);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Name2", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Name2<R>(ref int?, string?)", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"System.String? {PREFIX}." +
            "IFace3a<System.Nullable<System.Byte>, System.Nullable<System.Int32>>." +
            "IFace3b<System.String>.Name2<R>(" +
            "ref System.Nullable<System.Int32> one, System.String? two)",
            name);
    }
}
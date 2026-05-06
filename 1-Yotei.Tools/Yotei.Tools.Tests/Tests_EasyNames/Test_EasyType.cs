#pragma warning disable CA1822, CS8500

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyType
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyType";

    // ----------------------------------------------------

    class IFace1a<T>
    {
        public int?[]? Foo1() => throw null!;
        public IsNullable<IsNullable<int>[]>? Foo2() => throw null!;
        [IsNullable] public int?[]? Foo3() => throw null!;
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_ValueType_Nullability_Lost()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace1a<>);
        var source = type.GetMethod("Foo1")!;

        options.ReturnTypeOptions = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Nullable[] Foo1", name);

        options.ReturnTypeOptions = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("int?[] Foo1", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("int?[] Foo1", name);

        options.ReturnTypeOptions = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("System.Nullable<System.Int32>[] Foo1", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_ValueType_Nullability_Wrapped()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace1a<>);
        var source = type.GetMethod("Foo2")!;

        options.ReturnTypeOptions = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IsNullable Foo2", name);

        options.ReturnTypeOptions = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("int?[]? Foo2", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("int?[]? Foo2", name);

        options.ReturnTypeOptions = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("Yotei.Tools.IsNullable<Yotei.Tools.IsNullable<System.Int32>[]> Foo2", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_ValueType_Nullability_Decorated()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace1a<>);
        var source = type.GetMethod("Foo3")!;

        options.ReturnTypeOptions = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Nullable[] Foo3", name);

        options.ReturnTypeOptions = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("int?[]? Foo3", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("int?[]? Foo3", name);

        options.ReturnTypeOptions = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("System.Nullable<System.Int32>[]? Foo3", name);
    }

    // ----------------------------------------------------

    class IFace1b<T>
    {
        public string?[]? Foo1() => throw null!;
        public IsNullable<IsNullable<string>[]>? Foo2() => throw null!;
        [IsNullable] public IsNullable<string?>[]? Foo3() => throw null!;
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_ReferenceType_Nullability_Lost()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace1b<>);
        var source = type.GetMethod("Foo1")!;

        options.ReturnTypeOptions = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("string[] Foo1", name);

        options.ReturnTypeOptions = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("string[] Foo1", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("string[] Foo1", name);

        options.ReturnTypeOptions = EasyTypeOptions.Full;
        name = source.EasyName(options); Assert.Equal("System.String[] Foo1", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_ReferenceType_Nullability_Wrapped()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace1b<>);
        var source = type.GetMethod("Foo2")!;

        options.ReturnTypeOptions = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IsNullable Foo2", name);

        options.ReturnTypeOptions = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("string?[]? Foo2", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("string?[]? Foo2", name);

        options.ReturnTypeOptions = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("Yotei.Tools.IsNullable<Yotei.Tools.IsNullable<System.String>[]> Foo2", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_ReferenceType_Nullability_Decorated()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace1b<>);
        var source = type.GetMethod("Foo3")!;

        options.ReturnTypeOptions = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IsNullable[] Foo3", name);

        options.ReturnTypeOptions = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("string?[]? Foo3", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("string?[]? Foo3", name);

        options.ReturnTypeOptions = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("Yotei.Tools.IsNullable<System.String>[]? Foo3", name);
    }

    // ----------------------------------------------------

    class IFace1c<T>
    {
        public T?[]? Foo1() => throw null!;
        public IsNullable<IsNullable<T>[]>? Foo2() => throw null!;
        [IsNullable] public IsNullable<T?>[]? Foo3() => throw null!;
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_GenericType_Nullability_Lost()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace1c<>);
        var source = type.GetMethod("Foo1")!;

        options.ReturnTypeOptions = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("T[] Foo1", name);

        options.ReturnTypeOptions = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("T[] Foo1", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("T[] Foo1", name);

        options.ReturnTypeOptions = EasyTypeOptions.Full;
        name = source.EasyName(options); Assert.Equal("T[] Foo1", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_GenericType_Nullability_Wrapped()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace1c<>);
        var source = type.GetMethod("Foo2")!;

        options.ReturnTypeOptions = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IsNullable Foo2", name);

        options.ReturnTypeOptions = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("T?[]? Foo2", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("T?[]? Foo2", name);

        options.ReturnTypeOptions = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("Yotei.Tools.IsNullable<Yotei.Tools.IsNullable<T>[]> Foo2", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_GenericType_Nullability_Decorated()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace1c<>);
        var source = type.GetMethod("Foo3")!;

        options.ReturnTypeOptions = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IsNullable[] Foo3", name);

        options.ReturnTypeOptions = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("T?[]? Foo3", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("T?[]? Foo3", name);

        options.ReturnTypeOptions = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("Yotei.Tools.IsNullable<T>[]? Foo3", name);
    }

    // ----------------------------------------------------

    // Sentences like 'int*?' are syntax errors.
    unsafe class IFace2a<T> { public int?* Foo1() => throw null!; }

    //[Enforced]
    [Fact]
    public static void Test_Pointer_ValueType()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace2a<>);
        var source = type.GetMethod("Foo1")!;

        options.ReturnTypeOptions = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Nullable* Foo1", name);

        options.ReturnTypeOptions = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("int?* Foo1", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("int?* Foo1", name);

        options.ReturnTypeOptions = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("System.Nullable<System.Int32>* Foo1", name);
    }

    // ----------------------------------------------------

    // Sentences like 'string*?' are syntax errors.
    unsafe class IFace2b<T>
    {
        public string?* Foo1() => throw null!;
        public IsNullable<string?>* Foo2() => throw null!;
        [IsNullable] public string?* Foo3() => throw null!;
    }

    //[Enforced]
    [Fact]
    public static void Test_Pointer_ReferenceType_Nullability_Lost()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace2b<>);
        var source = type.GetMethod("Foo1")!;

        options.ReturnTypeOptions = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("string* Foo1", name);

        options.ReturnTypeOptions = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("string* Foo1", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("string* Foo1", name);

        options.ReturnTypeOptions = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("System.String* Foo1", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Pointer_ReferenceType_Nullability_Wrapped()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace2b<>);
        var source = type.GetMethod("Foo2")!;

        options.ReturnTypeOptions = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IsNullable* Foo2", name);

        options.ReturnTypeOptions = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("string?* Foo2", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("string?* Foo2", name);

        options.ReturnTypeOptions = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("Yotei.Tools.IsNullable<System.String>* Foo2", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Pointer_ReferenceType_Nullability_Decorated()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace2b<>);
        var source = type.GetMethod("Foo3")!;

        options.ReturnTypeOptions = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("string* Foo3", name);

        options.ReturnTypeOptions = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("string?* Foo3", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("string?* Foo3", name);

        options.ReturnTypeOptions = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("System.String?* Foo3", name);
    }

    // ----------------------------------------------------

    // Sentences like 'T*?' are syntax errors.
    unsafe class IFace2c<T>
    {
        public T?* Foo1() => throw null!;
        public IsNullable<T?>* Foo2() => throw null!;
    }

    //[Enforced]
    [Fact]
    public static void Test_Pointer_GenericType_Nullability_Lost()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace2c<>);
        var source = type.GetMethod("Foo1")!;

        options.ReturnTypeOptions = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("T* Foo1", name);

        options.ReturnTypeOptions = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("T* Foo1", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("T* Foo1", name);

        options.ReturnTypeOptions = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("T* Foo1", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Pointer_GenericType_Nullability_Wrapped()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace2c<>);
        var source = type.GetMethod("Foo2")!;

        options.ReturnTypeOptions = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IsNullable* Foo2", name);

        options.ReturnTypeOptions = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("T?* Foo2", name);

        options.ReturnTypeOptions = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("T?* Foo2", name);

        options.ReturnTypeOptions = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("Yotei.Tools.IsNullable<T>* Foo2", name);
    }
}
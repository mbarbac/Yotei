#pragma warning disable CA1822, CS8500

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyType
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyType";

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Special_ValueType()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(int);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("int", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("int", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("int", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options); Assert.Equal("System.Int32", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Special_ValueType_Annotated()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(int?);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Nullable", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("int?", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("int?", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options); Assert.Equal("System.Nullable<System.Int32>", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Other_ValueType()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(DateTime);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("DateTime", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("DateTime", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("System.DateTime", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options); Assert.Equal("System.DateTime", name);
    }

    // Annotated value types are well identified.
    //[Enforced]
    [Fact]
    public static void Test_Other_ValueType_Annotated()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(DateTime?);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Nullable", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("DateTime?", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("System.DateTime?", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options); Assert.Equal("System.Nullable<System.DateTime>", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Special_ReferenceType()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(string);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("string", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("string", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("string", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options); Assert.Equal("System.String", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Special_ReferenceType_Nullable()
    {
        EasyTypeOptions options;
        string name;
        //var source = typeof(string?); // CS8639: typeof cannot be use on nullable reference type
        var source = typeof(IsNullable<string>);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IsNullable", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("string?", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("string?", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options); Assert.Equal("Yotei.Tools.IsNullable<System.String>", name);
    }

    // ----------------------------------------------------

    //interface IFace1<T?> { } // CS1003: Syntax error
    interface IFace1<[IsNullable] T> { }

    // We cannot use '<T?>' when it appears in the 'host' type definition (we can if it apperars in
    // its base list, though...!). But because we can decorate it (only in the base definition) with
    // the attribute, then it can be found.
    //[Enforced]
    [Fact]
    public static void Test_Generic_NullableAttribute_Unbound()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace1<>);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IFace1", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("IFace1<T?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace1<T?>", name);

        options.GenericListOptions = EasyTypeOptions.Empty with { UsePlaceHolder = true };
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace1<>", name);

        options.GenericListOptions = null;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace1", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Generic_NullableAttribute_BoundToValue()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace1<bool?>);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IFace1", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("IFace1<bool?>", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace1<bool?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace1<System.Nullable<System.Boolean>>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Generic_NullableAttribute_BoundToReference_Lost()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace1<string?>);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IFace1", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("IFace1<string>", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace1<string>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace1<System.String>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Generic_NullableAttribute_BoundToReference_Wrapped()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace1<IsNullable<string>>);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IFace1", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("IFace1<string?>", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace1<string?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace1<Yotei.Tools.IsNullable<System.String>>", name);
    }

    // ----------------------------------------------------

    //interface IFace1<T?> { } // CS1003: Syntax error
    interface IFace2<[IsNullable] T> { void MyMethod(Predicate<T?> _); }

    //[Enforced]
    [Fact]
    public static void Test_Argument_NullableHostAttribute_Unbound()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace2<>);
        var method = type.GetMethod("MyMethod")!;
        var source = method.GetParameters()[0].ParameterType;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Predicate", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("Predicate<T?>", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal($"System.Predicate<T?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options); Assert.Equal("System.Predicate<T?>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Argument_NullableHostAttribute_BoundToValue()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace2<bool?>);
        var method = type.GetMethod("MyMethod")!;
        var source = method.GetParameters()[0].ParameterType;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Predicate", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("Predicate<bool?>", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal($"System.Predicate<bool?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("System.Predicate<System.Nullable<System.Boolean>>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Argument_NullableHostAttribute_BoundToReference_Lost()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace2<string?>);
        var method = type.GetMethod("MyMethod")!;
        var source = method.GetParameters()[0].ParameterType;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Predicate", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("Predicate<string>", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal($"System.Predicate<string>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("System.Predicate<System.String>", name);
    }
    
    //[Enforced]
    [Fact]
    public static void Test_Argument_NullableHostAttribute_BoundToReference_Wrapped()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace2<IsNullable<string?>>);
        var method = type.GetMethod("MyMethod")!;
        var source = method.GetParameters()[0].ParameterType;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Predicate", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("Predicate<string?>", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal($"System.Predicate<string?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("System.Predicate<Yotei.Tools.IsNullable<System.String>>", name);
    }

    // ----------------------------------------------------

    interface IFace3a<[IsNullable] T> { }
    interface IFace3b<T> : IFace3a<T?> { }

    //[Enforced]
    [Fact]
    public static void Test_Parent_Nullable_InheritedHostAttribute_Unbound_Lost()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace3b<>);
        var source = type.GetInterface("IFace3a`1")!;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("IFace3a<T>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<T>", name);
    }

    // ----------------------------------------------------

    interface IFace3c<[IsNullable] T> : IFace3a<T?> { }

    //[Enforced]
    [Fact]
    public static void Test_Parent_Nullable_ParentHostAttribute_Unbound()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace3c<>);
        var source = type.GetInterface("IFace3a`1")!;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("IFace3a<T?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<T?>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Parent_Nullable_ParentHostAttribute_BoundToValue()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace3c<bool?>);
        var source = type.GetInterface("IFace3a`1")!;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal($"IFace3a<bool?>", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace3a<bool?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<System.Nullable<System.Boolean>>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Parent_Nullable_ParentHostAttribute_BoundToReference_Lost()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace3c<string?>);
        var source = type.GetInterface("IFace3a`1")!;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("IFace3a<string>", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace3a<string>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<System.String>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Parent_Nullable_ParentHostAttribute_BoundToReference_Wrapped()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace3c<IsNullable<string?>>);
        var source = type.GetInterface("IFace3a`1")!;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("IFace3a<string?>", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace3a<string?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<Yotei.Tools.IsNullable<System.String>>", name);
    }

    // ----------------------------------------------------

    interface IFace3d<T> : IFace3a<IsNullable<T>> { }

    //[Enforced]
    [Fact]
    public static void Test_BaseListNullable_Wrapped_Unbound()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace3d<>);
        var source = type.GetInterface("IFace3a`1")!;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("IFace3a<T?>", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace3a<T?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<Yotei.Tools.IsNullable<T>>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_BaseListNullable_Wrapped_BoundToValue()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace3d<bool?>);
        var source = type.GetInterface("IFace3a`1")!;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("IFace3a<bool?>", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace3a<bool?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"{PREFIX}.IFace3a<Yotei.Tools.IsNullable<System.Nullable<System.Boolean>>>",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_BaseListNullable_Wrapped_BoundToReference()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace3d<string?>);
        var source = type.GetInterface("IFace3a`1")!;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("IFace3a<string?>", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace3a<string?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"{PREFIX}.IFace3a<Yotei.Tools.IsNullable<System.String>>",
            name);
    }

    // ----------------------------------------------------

    interface IFace4a<K, T> { interface IFace4b<R> { interface IFace4c<S, V> { } } }

    //[Enforced]
    [Fact]
    public static void Test_Nested_NoExplicitNullability_Unbound()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace4a<,>.IFace4b<>.IFace4c<,>);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IFace4c", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("IFace4c<S, V>", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace4a<K, T>.IFace4b<R>.IFace4c<S, V>", name);

        options = EasyTypeOptions.Default;
        options.GenericListOptions = new EasyTypeOptions() { UsePlaceHolder = true };
        name = source.EasyName(options); Assert.Equal("IFace4c<,>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace4a<K, T>.IFace4b<R>.IFace4c<S, V>", name);

        options.GenericListOptions = null;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace4a.IFace4b.IFace4c", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Nested_NoExplicitNullability_Bound_LostOnReferences()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace4a<byte?, short?>.IFace4b<int?>.IFace4c<long?, string?>);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IFace4c", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("IFace4c<long?, string>", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace4a<byte?, short?>.IFace4b<int?>.IFace4c<long?, string>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"{PREFIX}." +
            "IFace4a<System.Nullable<System.Byte>, System.Nullable<System.Int16>>." +
            "IFace4b<System.Nullable<System.Int32>>." +
            "IFace4c<System.Nullable<System.Int64>, System.String>",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Nested_NoExplicitNullability_Bound_WrappedReferences()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace4a<byte?, short?>.IFace4b<int?>.IFace4c<long?, IsNullable<string?>>);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IFace4c", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("IFace4c<long?, string?>", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace4a<byte?, short?>.IFace4b<int?>.IFace4c<long?, string?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"{PREFIX}." +
            "IFace4a<System.Nullable<System.Byte>, System.Nullable<System.Int16>>." +
            "IFace4b<System.Nullable<System.Int32>>." +
            "IFace4c<System.Nullable<System.Int64>, Yotei.Tools.IsNullable<System.String>>",
            name);
    }

    // ----------------------------------------------------

    interface IFace5a<K, T> { interface IFace5b<R> { interface IFace5c<[IsNullable] S, [IsNullable] V> { } } }

    //[Enforced]
    [Fact]
    public static void Test_Nested_NullableAttribute_Unbound()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace5a<,>.IFace5b<>.IFace5c<,>);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IFace5c", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("IFace5c<S?, V?>", name);

        options.GenericListOptions = new EasyTypeOptions() { UsePlaceHolder = true };
        name = source.EasyName(options); Assert.Equal("IFace5c<,>", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace5a<K, T>.IFace5b<R>.IFace5c<S?, V?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace5a<K, T>.IFace5b<R>.IFace5c<S?, V?>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Nested_NullableAttribute_Bound_LostOnReferences()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace5a<byte?, short?>.IFace5b<int?>.IFace5c<long?, string?>);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IFace5c", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("IFace5c<long?, string>", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace5a<byte?, short?>.IFace5b<int?>.IFace5c<long?, string>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"{PREFIX}.IFace5a<System.Nullable<System.Byte>, System.Nullable<System.Int16>>." +
            "IFace5b<System.Nullable<System.Int32>>." +
            "IFace5c<System.Nullable<System.Int64>, System.String>",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Nested_NullableAttribute_Bound_WrappedReferences()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace5a<byte?, short?>.IFace5b<int?>.IFace5c<long?, IsNullable<string?>>);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IFace5c", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("IFace5c<long?, string?>", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace5a<byte?, short?>.IFace5b<int?>.IFace5c<long?, string?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"{PREFIX}." +
            "IFace5a<System.Nullable<System.Byte>, System.Nullable<System.Int16>>." +
            "IFace5b<System.Nullable<System.Int32>>." +
            "IFace5c<System.Nullable<System.Int64>, Yotei.Tools.IsNullable<System.String>>",
            name);
    }

    // ----------------------------------------------------

    // We don't remove the 'Attribute' suffix when the type is, itself, the 'Attribute' one...
    //[Enforced]
    [Fact]
    public static void Test_Core_Attribute()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(Attribute);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Attribute", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("Attribute", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("System.Attribute", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options); Assert.Equal("System.Attribute", name);

        options.RemoveAttributeSuffix = true;
        name = source.EasyName(options); Assert.Equal("System.Attribute", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Custom_Attribute()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IsNullableAttribute);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IsNullableAttribute", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("IsNullable", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("Yotei.Tools.IsNullable", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("Yotei.Tools.IsNullableAttribute", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Global_Namespace()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IsNullableAttribute);

        options = EasyTypeOptions.Default with { NamespaceStyle = EasyNamespaceStyle.UseGlobal };
        name = source.EasyName(options);
        Assert.Equal("global::Yotei.Tools.IsNullable", name);

        options = EasyTypeOptions.Full with { NamespaceStyle = EasyNamespaceStyle.UseGlobal };
        name = source.EasyName(options);
        Assert.Equal("global::Yotei.Tools.IsNullableAttribute", name);
    }

    // ----------------------------------------------------

    class RType9a<T>
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
        var type = typeof(RType9a<>);
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
        var type = typeof(RType9a<>);
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
        var type = typeof(RType9a<>);
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

    class RType9b<T>
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
        var type = typeof(RType9b<>);
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
        var type = typeof(RType9b<>);
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
        var type = typeof(RType9b<>);
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

    class RType9c<T>
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
        var type = typeof(RType9c<>);
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
        var type = typeof(RType9c<>);
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
        var type = typeof(RType9c<>);
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
    unsafe class RType8a<T> { public int?* Foo1() => throw null!; }

    //[Enforced]
    [Fact]
    public static void Test_Pointer_ValueType()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(RType8a<>);
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
    unsafe class RType8b<T>
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
        var type = typeof(RType8b<>);
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
        var type = typeof(RType8b<>);
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
        var type = typeof(RType8b<>);
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
    unsafe class RType8c<T>
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
        var type = typeof(RType8c<>);
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
        var type = typeof(RType8c<>);
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
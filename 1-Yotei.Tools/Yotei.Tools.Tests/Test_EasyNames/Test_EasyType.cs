namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyType
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyType";

    readonly static EasyTypeOptions EMPTY = EasyTypeOptions.Empty;
    readonly static EasyTypeOptions DEFAULT = EasyTypeOptions.Default;
    readonly static EasyTypeOptions FULL = EasyTypeOptions.Full;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Standar_ValueType()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(int);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("int", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("int", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal("System.Int32", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standar_ValueType_Annotated()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(int?);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Nullable", name);

        options = EMPTY with { GenericListStyle = EasyGenericListStyle.UseNames };
        name = source.EasyName(options); Assert.Equal("Nullable<int>", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("int?", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal("System.Nullable<System.Int32>", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Standar_ReferenceType()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(string);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("string", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("string", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal("System.String", name);
    }

    // public static void Test_Standar_ReferenceType_Annotated()
    // var source = typeof(string?);
    // => CS8639: typeof operator cannot be used on a nullable reference type

    //[Enforced]
    [Fact]
    public static void Test_Standar_ReferenceType_Wrapped()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IsNullable<string>);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IsNullable", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("string?", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.KeepWrappers };
        name = source.EasyName(options); Assert.Equal("IsNullable<string>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal("Yotei.Tools.IsNullable<System.String>", name);
    }

    // ----------------------------------------------------

    // public interface IFace1<T?> { }
    // => CS1003: Syntax error

    interface IFace1<[IsNullable] T> { }

    //[Enforced]
    [Fact]
    public static void Test_Type_NullableByAttribute_Unbound()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace1<>);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IFace1", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IFace1<T?>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace1<T?>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_NullableByAttribute_Bound_Lost()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace1<string?>);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IFace1", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IFace1<string>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace1<System.String>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Type_NullableByAttribute_Bound_Wrapped()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace1<IsNullable<string>>);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IFace1", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IFace1<string?>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.KeepWrappers };
        name = source.EasyName(options); Assert.Equal("IFace1<IsNullable<string>>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace1<Yotei.Tools.IsNullable<System.String>>", name);
    }

    // ----------------------------------------------------

    interface IFace2<[IsNullable] T> { void MyMethod(Predicate<T> _); }

    //[Enforced]
    [Fact]
    public static void Test_Argument_NullableByTopAttribute_Unbound()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace2<>);
        var method = type.GetMethod("MyMethod")!;
        var arg = method.GetParameters()[0];
        var source = arg.ParameterType;

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Predicate", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Predicate<T?>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal("System.Predicate<T?>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Argument_NullableByTopAttribute_Bound_Lost()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace2<string?>);
        var method = type.GetMethod("MyMethod")!;
        var arg = method.GetParameters()[0];
        var source = arg.ParameterType;

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Predicate", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Predicate<string>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal("System.Predicate<System.String>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Argument_NullableByTopAttribute_Bound_Wrapped()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace2<IsNullable<string>>);
        var method = type.GetMethod("MyMethod")!;
        var arg = method.GetParameters()[0];
        var source = arg.ParameterType;

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Predicate", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Predicate<string?>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.KeepWrappers };
        name = source.EasyName(options); Assert.Equal("Predicate<IsNullable<string>>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal("System.Predicate<Yotei.Tools.IsNullable<System.String>>", name);
    }

    // ----------------------------------------------------

    interface IFace3a<[IsNullable] T> { }
    interface IFace3b<T> : IFace3a<T> { }

    //[Enforced]
    [Fact]
    public static void Test_Inherited_UnboundFromTop_Lost()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace3b<>);
        var source = type.GetInterface("IFace3a`1")!;

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IFace3a<T>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<T>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Inherited_BoundFromTop_Lost()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace3b<string?>);
        var source = type.GetInterface("IFace3a`1")!;

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IFace3a<string>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<System.String>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Inherited_BoundFromTop_Wrapped()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace3b<IsNullable<string>>);
        var source = type.GetInterface("IFace3a`1")!;

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IFace3a<string?>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<Yotei.Tools.IsNullable<System.String>>", name);
    }

    // ----------------------------------------------------

    interface IFace3c<T> : IFace3a<T?> { }

    //[Enforced]
    [Fact]
    public static void Test_Inherited_UnboundFromTop_AndAnnotated_Lost()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace3c<>);
        var source = type.GetInterface("IFace3a`1")!;

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IFace3a<T>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<T>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Inherited_BoundFromTop_AndAnnotated_Lost()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace3c<string?>);
        var source = type.GetInterface("IFace3a`1")!;

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IFace3a<string>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<System.String>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Inherited_BoundFromTop_AndAnnottated_Wrapped()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace3c<IsNullable<string>>);
        var source = type.GetInterface("IFace3a`1")!;

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IFace3a<string?>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<Yotei.Tools.IsNullable<System.String>>", name);
    }

    // ----------------------------------------------------

    interface IFace4c<T> : IFace3a<IsNullable<T>> { }

    //[Enforced]
    [Fact]
    public static void Test_Inherited_UnboundFromTop_AndWrapped()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace4c<>);
        var source = type.GetInterface("IFace3a`1")!;

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IFace3a<T?>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<Yotei.Tools.IsNullable<T>>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Inherited_BoundFromTop_AndWrapped()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace4c<string?>);
        var source = type.GetInterface("IFace3a`1")!;

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IFace3a<string?>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<Yotei.Tools.IsNullable<System.String>>", name);
    }

    // ----------------------------------------------------

    interface IFace5a<K, T> { interface IFace5b<R> { interface IFace5c<S, V> { } } }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Unbound_NoExplicitNullability()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace5a<,>.IFace5b<>.IFace5c<,>);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IFace5c", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IFace5c<S, V>", name);

        options = DEFAULT with { GenericListStyle = EasyGenericListStyle.PlaceHolders };
        name = source.EasyName(options); Assert.Equal("IFace5c<,>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace5a<K, T>.IFace5b<R>.IFace5c<S, V>", name);

        options = FULL with { GenericListStyle = EasyGenericListStyle.PlaceHolders };
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace5a<,>.IFace5b<>.IFace5c<,>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Bound_With_NullableAnnotations_LostOnReferenceTypes()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace5a<byte, short>.IFace5b<int>.IFace5c<long?, string?>);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IFace5c", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IFace5c<long?, string>", name);

        options = DEFAULT with { GenericListStyle = EasyGenericListStyle.PlaceHolders };
        name = source.EasyName(options); Assert.Equal("IFace5c<,>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"{PREFIX}." +
            "IFace5a<System.Byte, System.Int16>." +
            "IFace5b<System.Int32>." +
            "IFace5c<System.Nullable<System.Int64>, System.String>",
            name);

        options = FULL with { GenericListStyle = EasyGenericListStyle.PlaceHolders };
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace5a<,>.IFace5b<>.IFace5c<,>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Bound_With_NullableAnnotations_AndWrapper()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace5a<byte, short>.IFace5b<int>.IFace5c<long?, IsNullable<string>>);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IFace5c", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IFace5c<long?, string?>", name);

        options = DEFAULT with { GenericListStyle = EasyGenericListStyle.PlaceHolders };
        name = source.EasyName(options); Assert.Equal("IFace5c<,>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"{PREFIX}." +
            "IFace5a<System.Byte, System.Int16>." +
            "IFace5b<System.Int32>." +
            "IFace5c<System.Nullable<System.Int64>, Yotei.Tools.IsNullable<System.String>>",
            name);

        options = FULL with { GenericListStyle = EasyGenericListStyle.PlaceHolders };
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace5a<,>.IFace5b<>.IFace5c<,>", name);
    }

    // ----------------------------------------------------

    interface IFace6a<K, T> { interface IFace6b<R> { interface IFace6c<[IsNullable] S, [IsNullable] V> { } } }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Unbound_NullableByAttribute()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace6a<,>.IFace6b<>.IFace6c<,>);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IFace6c", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IFace6c<S?, V?>", name);

        options = DEFAULT with { GenericListStyle = EasyGenericListStyle.PlaceHolders };
        name = source.EasyName(options); Assert.Equal("IFace6c<,>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace6a<K, T>.IFace6b<R>.IFace6c<S?, V?>", name);

        options = FULL with { GenericListStyle = EasyGenericListStyle.PlaceHolders };
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace6a<,>.IFace6b<>.IFace6c<,>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Bound_NullableByAttribute_Annotated_LostOnReferenceTypes()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace6a<byte, short>.IFace6b<int>.IFace6c<long?, string?>);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IFace6c", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IFace6c<long?, string>", name);

        options = DEFAULT with { GenericListStyle = EasyGenericListStyle.PlaceHolders };
        name = source.EasyName(options); Assert.Equal("IFace6c<,>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"{PREFIX}." +
            "IFace6a<System.Byte, System.Int16>." +
            "IFace6b<System.Int32>." +
            "IFace6c<System.Nullable<System.Int64>, System.String>",
            name);

        options = FULL with { GenericListStyle = EasyGenericListStyle.PlaceHolders };
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace6a<,>.IFace6b<>.IFace6c<,>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Bound_NullableByAttribute_Annotated_And_Wrapper()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace6a<byte, short>.IFace6b<int>.IFace6c<long?, IsNullable<string>>);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IFace6c", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IFace6c<long?, string?>", name);

        options = DEFAULT with { GenericListStyle = EasyGenericListStyle.PlaceHolders };
        name = source.EasyName(options); Assert.Equal("IFace6c<,>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal(
            $"{PREFIX}." +
            "IFace6a<System.Byte, System.Int16>." +
            "IFace6b<System.Int32>." +
            "IFace6c<System.Nullable<System.Int64>, Yotei.Tools.IsNullable<System.String>>",
            name);

        options = FULL with { GenericListStyle = EasyGenericListStyle.PlaceHolders };
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace6a<,>.IFace6b<>.IFace6c<,>", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Core_Attribute()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(Attribute);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("Attribute", name);

        options = EMPTY with { RemoveAttributeSuffix = true };
        name = source.EasyName(options); Assert.Equal("Attribute", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("Attribute", name);

        options = DEFAULT with { RemoveAttributeSuffix = true };
        name = source.EasyName(options); Assert.Equal("Attribute", name);

        options = FULL;
        name = source.EasyName(options); Assert.Equal("System.Attribute", name);

        options = FULL with { RemoveAttributeSuffix = true };
        name = source.EasyName(options); Assert.Equal("System.Attribute", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Custom_Attribute()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IsNullableAttribute);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IsNullableAttribute", name);

        options = EMPTY with { RemoveAttributeSuffix = true };
        name = source.EasyName(options); Assert.Equal("IsNullable", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IsNullable", name);

        options = DEFAULT with { RemoveAttributeSuffix = true };
        name = source.EasyName(options); Assert.Equal("IsNullable", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal("Yotei.Tools.IsNullableAttribute", name);

        options = FULL with { RemoveAttributeSuffix = true };
        name = source.EasyName(options);
        Assert.Equal("Yotei.Tools.IsNullable", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Global_Namespace()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IsNullableAttribute);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IsNullableAttribute", name);

        options = EMPTY with { NamespaceStyle = EasyNamespaceStyle.UseGlobal };
        name = source.EasyName(options);
        Assert.Equal("global::Yotei.Tools.IsNullableAttribute", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IsNullable", name);

        options = DEFAULT with { NamespaceStyle = EasyNamespaceStyle.UseGlobal };
        name = source.EasyName(options);
        Assert.Equal("global::Yotei.Tools.IsNullable", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal("Yotei.Tools.IsNullableAttribute", name);

        options = FULL with { NamespaceStyle = EasyNamespaceStyle.UseGlobal };
        name = source.EasyName(options);
        Assert.Equal("global::Yotei.Tools.IsNullableAttribute", name);
    }
}
namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyType
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyType";

    const EasyTypeOptions.Mode EMPTY = EasyTypeOptions.Mode.Empty;
    const EasyTypeOptions.Mode DEFAULT = EasyTypeOptions.Mode.Default;
    const EasyTypeOptions.Mode FULL = EasyTypeOptions.Mode.Full;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Special_ValueType()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(int);

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("int", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("int", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options); Assert.Equal("int", name);

        options.UseSpecialNames = false;
        name = source.EasyName(options); Assert.Equal("System.Int32", name);
    }

    // Annotated value types are well identified. When they are special ones, easy name methods
    // give precedence to their special names, unless explicitly opted out.
    //[Enforced]
    [Fact]
    public static void Test_Special_ValueType_Annotated()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(int?);

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("int?", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("int?", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options); Assert.Equal("int?", name);

        options.UseSpecialNames = false;
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

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("DateTime", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("DateTime", name);

        options = new EasyTypeOptions(FULL);
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

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("DateTime?", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("DateTime?", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options);
        Assert.Equal("System.Nullable<System.DateTime>", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Special_ReferenceType()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(string);

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("string", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("string", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options); Assert.Equal("string", name);

        options.UseSpecialNames = false;
        name = source.EasyName(options); Assert.Equal("System.String", name);
    }

    // Cannot use 'typeof' with nullable reference types, generates a compiler error. Any case,
    // in general, annotations on reference types are lost because are just compiler artifacts
    // not persisted in metadata.
    //[Enforced]
    [Fact]
    public static void Test_Special_ReferenceType_Nullable()
    {
        EasyTypeOptions options;
        string name;
        //var source = typeof(string?); // CS8639: typeof cannot be use on nullable reference type
        var source = typeof(IsNullable<string>);

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("string?", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("string?", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options); Assert.Equal("string?", name);

        options.UseSpecialNames = false;
        name = source.EasyName(options);
        Assert.Equal("Yotei.Tools.IsNullable<System.String>", name);
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

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("IFace1", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("IFace1<T?>", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace1<T?>", name);

        options.GenericListOptions = new EasyTypeOptions(EMPTY) { PlaceHolder = true };
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

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("IFace1", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("IFace1<bool?>", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace1<bool?>", name);

        options.UseSpecialNames = false;
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

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("IFace1", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("IFace1<string>", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace1<string>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Generic_NullableAttribute_BoundToReference_Wrapped()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace1<IsNullable<string>>);

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("IFace1", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("IFace1<string?>", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace1<string?>", name);

        options.UseSpecialNames = false;
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

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("Predicate", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("Predicate<T?>", name);

        options = new EasyTypeOptions(FULL);
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

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("Predicate", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("Predicate<bool?>", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options); Assert.Equal("System.Predicate<bool?>", name);

        options.UseSpecialNames = false;
        name = source.EasyName(options);
        Assert.Equal("System.Predicate<System.Nullable<System.Boolean>>", name);
    }

    // For unknown reasons, the '<T?>' in the generic list of the method looses the nullability
    // information. It does not accept a '<[IsNullable] T>' so the only way to persist this info
    // is by using a wrappers, as happens in the next test method.
    //[Enforced]
    [Fact]
    public static void Test_Argument_NullableHostAttribute_BoundToReference_Lost()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace2<string?>);
        var method = type.GetMethod("MyMethod")!;
        var source = method.GetParameters()[0].ParameterType;

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("Predicate", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("Predicate<string>", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options); Assert.Equal("System.Predicate<string>", name);

        options.UseSpecialNames = false;
        name = source.EasyName(options);
        Assert.Equal("System.Predicate<System.String>", name);
    }

    // As said before, we can persist the nullability information using a wrapper as a workaround.
    // Of course, this is sub-optimal under any reasonable criteria, but at least there is this
    // quick and extremely dirty workaround.
    //[Enforced]
    [Fact]
    public static void Test_Argument_NullableHostAttribute_BoundToReference_Wrapped()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace2<IsNullable<string?>>);
        var method = type.GetMethod("MyMethod")!;
        var source = method.GetParameters()[0].ParameterType;

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("Predicate", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("Predicate<string?>", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options); Assert.Equal("System.Predicate<string?>", name);

        options.UseSpecialNames = false;
        name = source.EasyName(options);
        Assert.Equal("System.Predicate<Yotei.Tools.IsNullable<System.String>>", name);
    }

    // ----------------------------------------------------

    interface IFace3a<[IsNullable] T> { }
    interface IFace3b<T> : IFace3a<T?> { }

    // When the host type definition (IFace3b) has no nullability annotations, it doesn't matter
    // if we hace used them in the 'IFace3a' definition or base list usage, it is lost.
    //[Enforced]
    [Fact]
    public static void Test_Parent_Nullable_InheritedHostAttribute_Unbound_Lost()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace3b<>);
        var source = type.GetInterface("IFace3a`1")!;

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("IFace3a<T>", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<T>", name);
    }

    // ----------------------------------------------------

    interface IFace3c<[IsNullable] T> : IFace3a<T?> { }

    // But when used in the host type definition (IFace3c), even in this case where we must use
    // the attribute (because 'T?' is not allowed in such base definition), then it is persisted.
    //[Enforced]
    [Fact]
    public static void Test_Parent_Nullable_ParentHostAttribute_Unbound()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace3c<>);
        var source = type.GetInterface("IFace3a`1")!;

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("IFace3a<T?>", name);

        options = new EasyTypeOptions(FULL);
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

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("IFace3a<bool?>", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<bool?>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Parent_Nullable_ParentHostAttribute_BoundToReference_Lost()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace3c<string?>);
        var source = type.GetInterface("IFace3a`1")!;

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("IFace3a<string>", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<string>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Parent_Nullable_ParentHostAttribute_BoundToReference_Wrapped()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace3c<IsNullable<string?>>);
        var source = type.GetInterface("IFace3a`1")!;

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("IFace3a<string?>", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<string?>", name);
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

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("IFace3a<T?>", name);

        options = new EasyTypeOptions(FULL);
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

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("IFace3a<bool?>", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<Yotei.Tools.IsNullable<bool?>>", name);
    }

    // Because the T in the base list is wrapped, what is persisted is the wrapped type itself,
    // which is interpreted by easy name in the right way. As been said many times, this is quite
    // a sub-optimal arrangement, but it may be useful somewhere.
    //[Enforced]
    [Fact]
    public static void Test_BaseListNullable_Wrapped_BoundToReference()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace3d<string?>);
        var source = type.GetInterface("IFace3a`1")!;

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("IFace3a", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("IFace3a<string?>", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace3a<string?>", name);
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

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("IFace4c", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("IFace4c<S, V>", name);

        options.GenericListOptions = new EasyTypeOptions() { PlaceHolder = true };
        name = source.EasyName(options); Assert.Equal("IFace4c<,>", name);

        options = new EasyTypeOptions(FULL);
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

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("IFace4c", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("IFace4c<long?, string>", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace4a<byte?, short?>.IFace4b<int?>.IFace4c<long?, string>", name);

        options.UseSpecialNames = false;
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

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("IFace4c", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("IFace4c<long?, string?>", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace4a<byte?, short?>.IFace4b<int?>.IFace4c<long?, string?>", name);

        options.UseSpecialNames = false;
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

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("IFace5c", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("IFace5c<S?, V?>", name);

        options.GenericListOptions = new EasyTypeOptions() { PlaceHolder = true };
        name = source.EasyName(options); Assert.Equal("IFace5c<,>", name);

        options = new EasyTypeOptions(FULL);
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

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("IFace5c", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("IFace5c<long?, string>", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace5a<byte?, short?>.IFace5b<int?>.IFace5c<long?, string>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Nested_NullableAttribute_Bound_WrappedReferences()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace5a<byte?, short?>.IFace5b<int?>.IFace5c<long?, IsNullable<string?>>);

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("IFace5c", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("IFace5c<long?, string?>", name);

        options = new EasyTypeOptions(FULL);
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace5a<byte?, short?>.IFace5b<int?>.IFace5c<long?, string?>", name);
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

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("Attribute", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("Attribute", name);

        options = new EasyTypeOptions(FULL);
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

        options = new EasyTypeOptions(EMPTY);
        name = source.EasyName(options); Assert.Equal("IsNullableAttribute", name);

        options = new EasyTypeOptions(DEFAULT);
        name = source.EasyName(options); Assert.Equal("IsNullable", name);

        options = new EasyTypeOptions(FULL);
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

        options = new EasyTypeOptions(DEFAULT) { NamespaceStyle = EasyNamespaceStyle.UseGlobal };
        name = source.EasyName(options);
        Assert.Equal("global::Yotei.Tools.IsNullable", name);

        options = new EasyTypeOptions(FULL) { NamespaceStyle = EasyNamespaceStyle.UseGlobal };
        name = source.EasyName(options);
        Assert.Equal("global::Yotei.Tools.IsNullableAttribute", name);
    }
}
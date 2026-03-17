namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_EasyType
{
    readonly static string NAMESPACE = "Yotei.Tools.Tests";
    readonly static string TESTHOST = "Test_EasyType";

    readonly static EasyTypeOptions EMPTY = EasyTypeOptions.Empty;
    readonly static EasyTypeOptions DEFAULT = EasyTypeOptions.Default;
    readonly static EasyTypeOptions FULL = EasyTypeOptions.Full;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Standard_ValueType()
    {
        EasyTypeOptions options;
        string name;
        var item = typeof(int);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Int32", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("int", name);

        options = DEFAULT with { UseNamespace = true };
        name = item.EasyName(options); Assert.Equal("int", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.Int32", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_ValueType_Nullable()
    {
        EasyTypeOptions options;
        string name;
        var item = typeof(int?);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Nullable", name);

        options = EMPTY with { GenericStyle = EasyGenericStyle.UseNames };
        name = item.EasyName(options); Assert.Equal("Nullable<Int32>", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("int?", name);

        options = DEFAULT with { UseNamespace = true };
        name = item.EasyName(options); Assert.Equal("int?", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.KeepWrappers };
        name = item.EasyName(options); Assert.Equal("Nullable<int>", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.Nullable<System.Int32>", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Standard_ReferenceType()
    {
        EasyTypeOptions options;
        string name;
        var item = typeof(string);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("String", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("string", name);

        options = DEFAULT with { UseNamespace = true };
        name = item.EasyName(options); Assert.Equal("string", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.String", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_ReferenceType_NullableByWrapper()
    {
        EasyTypeOptions options;
        string name;

        // var item = typeof(string?); // CS8639: typeof cannot be used on nullable reference type
        var item = typeof(IsNullable<string>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IsNullable", name);

        // Used in a context not allowed by the compiler...
        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("string?", name);

        // Used in a context not allowed by the compiler...
        options = DEFAULT with { UseNamespace = true };
        name = item.EasyName(options); Assert.Equal("string?", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.KeepWrappers };
        name = item.EasyName(options); Assert.Equal("IsNullable<string>", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("Yotei.Tools.IsNullable<System.String>", name);
    }

    // ----------------------------------------------------

    public interface IFace1A<[IsNullable] T> { }

    //[Enforced]
    [Fact]
    public static void Test_GenericNullable_ByAttribute_UnBound()
    {
        EasyTypeOptions options;
        string name;

        var item = typeof(IFace1A<>);
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace1A", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace1A<T?>", name);

        options = DEFAULT with { GenericStyle = EasyGenericStyle.PlaceHolders };
        name = item.EasyName(options); Assert.Equal("IFace1A<>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.None };
        name = item.EasyName(options); Assert.Equal("IFace1A<T>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.KeepWrappers };
        name = item.EasyName(options); Assert.Equal("IFace1A<T?>", name);

        options = DEFAULT with { UseNamespace = true };
        name = item.EasyName(options); Assert.Equal($"{NAMESPACE}.{TESTHOST}.IFace1A<T?>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_GenericNullable_ByAttribute_Bound_Nullable_Lost()
    {
        EasyTypeOptions options;
        string name;

        var item = typeof(IFace1A<string?>);
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace1A", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace1A<string>", name);

        options = DEFAULT with { GenericStyle = EasyGenericStyle.PlaceHolders };
        name = item.EasyName(options); Assert.Equal("IFace1A<>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.None };
        name = item.EasyName(options); Assert.Equal("IFace1A<string>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.KeepWrappers };
        name = item.EasyName(options); Assert.Equal("IFace1A<string>", name);

        options = DEFAULT with { UseNamespace = true };
        name = item.EasyName(options); Assert.Equal($"{NAMESPACE}.{TESTHOST}.IFace1A<string>", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal($"{NAMESPACE}.{TESTHOST}.IFace1A<System.String>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_GenericNullable_ByAttribute_Bound_NullableByAttribute()
    {
        EasyTypeOptions options;
        string name;

        var item = typeof(IFace1A<IsNullable<string?>>);
        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace1A", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace1A<string?>", name);

        options = DEFAULT with { GenericStyle = EasyGenericStyle.PlaceHolders };
        name = item.EasyName(options); Assert.Equal("IFace1A<>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.None };
        name = item.EasyName(options); Assert.Equal("IFace1A<IsNullable<string>>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.KeepWrappers };
        name = item.EasyName(options); Assert.Equal("IFace1A<IsNullable<string>>", name);

        options = DEFAULT with { UseNamespace = true };
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTHOST}.IFace1A<string?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTHOST}.IFace1A<Yotei.Tools.IsNullable<System.String>>", name);
    }

    // ----------------------------------------------------

    public interface IFace2A<T> { }
    public interface IFace2B<T> : IFace2A<T> { }

    //[Enforced]
    [Fact]
    public static void Test_NestedGeneric()
    {
        EasyTypeOptions options;
        string name;

        var type = typeof(IFace2B<>);
        var ifaces = type.GetInterfaces();
        var item = ifaces.Single(x => x.GetGenericTypeDefinition() == typeof(IFace2A<>));

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace2A", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace2A<T>", name);

        options = DEFAULT with { GenericStyle = EasyGenericStyle.PlaceHolders };
        name = item.EasyName(options); Assert.Equal("IFace2A<>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTHOST}.IFace2A<T>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_NestedGeneric_BoundNullable_NullabilityLost()
    {
        EasyTypeOptions options;
        string name;

        //var type = typeof(IFace2B<T?>); // T not known
        var type = typeof(IFace2B<string?>);
        var ifaces = type.GetInterfaces();
        var item = ifaces.Single(x => x.GetGenericTypeDefinition() == typeof(IFace2A<>));

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace2A", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace2A<string>", name);

        options = DEFAULT with { GenericStyle = EasyGenericStyle.PlaceHolders };
        name = item.EasyName(options); Assert.Equal("IFace2A<>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTHOST}.IFace2A<System.String>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_NestedGeneric_BoundNullable_ByWrapped()
    {
        EasyTypeOptions options;
        string name;

        var type = typeof(IFace2B<IsNullable<string?>>);
        var ifaces = type.GetInterfaces();
        var item = ifaces.Single(x => x.GetGenericTypeDefinition() == typeof(IFace2A<>));

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace2A", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace2A<string?>", name);

        options = DEFAULT with { GenericStyle = EasyGenericStyle.PlaceHolders };
        name = item.EasyName(options); Assert.Equal("IFace2A<>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.None };
        name = item.EasyName(options); Assert.Equal("IFace2A<IsNullable<string>>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.KeepWrappers };
        name = item.EasyName(options); Assert.Equal("IFace2A<IsNullable<string>>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTHOST}.IFace2A<Yotei.Tools.IsNullable<System.String>>", name);
    }

    // ----------------------------------------------------

    public interface IFace2C<[IsNullable] T> : IFace2A<T> { }

    //[Enforced]
    [Fact]
    public static void Test_NestedGeneric_ByAttribute_Unbound()
    {
        EasyTypeOptions options;
        string name;

        var type = typeof(IFace2C<>);
        var ifaces = type.GetInterfaces();
        var item = ifaces.Single(x => x.GetGenericTypeDefinition() == typeof(IFace2A<>));

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace2A", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace2A<T?>", name);

        options = DEFAULT with { GenericStyle = EasyGenericStyle.PlaceHolders };
        name = item.EasyName(options); Assert.Equal("IFace2A<>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.None };
        name = item.EasyName(options); Assert.Equal("IFace2A<T>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.KeepWrappers };
        name = item.EasyName(options); Assert.Equal("IFace2A<T?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTHOST}.IFace2A<T?>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_NestedGeneric_ByAttribute_Bound_Nullability_Lost()
    {
        EasyTypeOptions options;
        string name;

        var type = typeof(IFace2C<string?>);
        var ifaces = type.GetInterfaces();
        var item = ifaces.Single(x => x.GetGenericTypeDefinition() == typeof(IFace2A<>));

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace2A", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace2A<string>", name);

        options = DEFAULT with { GenericStyle = EasyGenericStyle.PlaceHolders };
        name = item.EasyName(options); Assert.Equal("IFace2A<>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.None };
        name = item.EasyName(options); Assert.Equal("IFace2A<string>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.KeepWrappers };
        name = item.EasyName(options); Assert.Equal("IFace2A<string>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTHOST}.IFace2A<System.String>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_NestedGeneric_ByAttribute_Bound_ByWrapper()
    {
        EasyTypeOptions options;
        string name;

        var type = typeof(IFace2C<IsNullable<string?>>);
        var ifaces = type.GetInterfaces();
        var item = ifaces.Single(x => x.GetGenericTypeDefinition() == typeof(IFace2A<>));

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace2A", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace2A<string?>", name);

        options = DEFAULT with { GenericStyle = EasyGenericStyle.PlaceHolders };
        name = item.EasyName(options); Assert.Equal("IFace2A<>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.None };
        name = item.EasyName(options); Assert.Equal("IFace2A<IsNullable<string>>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.KeepWrappers };
        name = item.EasyName(options); Assert.Equal("IFace2A<IsNullable<string>>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTHOST}.IFace2A<Yotei.Tools.IsNullable<System.String>>", name);
    }

    // ----------------------------------------------------

    public class Type4A<K, T>
    { public class Type4B<R> { public class Type4C<[IsNullable] S, [IsNullable] V> { } } }

    //[Enforced]
    [Fact]
    public static void Test_EmbeddedGeneric_Unbound()
    {
        EasyTypeOptions options;
        string name;

        var item = typeof(Type4A<,>.Type4B<>.Type4C<,>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type4C", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type4C<S?, V?>", name);

        options = DEFAULT with { GenericStyle = EasyGenericStyle.PlaceHolders };
        name = item.EasyName(options); Assert.Equal("Type4C<,>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.None };
        name = item.EasyName(options); Assert.Equal("Type4C<S, V>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.KeepWrappers };
        name = item.EasyName(options); Assert.Equal("Type4C<S?, V?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTHOST}.Type4A<K, T>.Type4B<R>.Type4C<S?, V?>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_EmbeddedGeneric_Bound_ReferenceNullabilityLost()
    {
        EasyTypeOptions options;
        string name;

        var item = typeof(Type4A<byte, short>.Type4B<int>.Type4C<long?, string?>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type4C", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type4C<long?, string>", name);

        options = DEFAULT with { GenericStyle = EasyGenericStyle.PlaceHolders };
        name = item.EasyName(options); Assert.Equal("Type4C<,>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.None };
        name = item.EasyName(options); Assert.Equal("Type4C<Nullable<long>, string>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.KeepWrappers };
        name = item.EasyName(options); Assert.Equal("Type4C<Nullable<long>, string>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTHOST}." +
            "Type4A<System.Byte, System.Int16>." +
            "Type4B<System.Int32>." +
            "Type4C<System.Nullable<System.Int64>, System.String>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_EmbeddedGeneric_Bound_ByWrapper()
    {
        EasyTypeOptions options;
        string name;

        var item = typeof(Type4A<byte, short>.Type4B<int>.Type4C<IsNullable<long?>, IsNullable<string?>>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type4C", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type4C<long?, string?>", name);

        options = DEFAULT with { GenericStyle = EasyGenericStyle.PlaceHolders };
        name = item.EasyName(options); Assert.Equal("Type4C<,>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.None };
        name = item.EasyName(options);
        Assert.Equal("Type4C<IsNullable<Nullable<long>>, IsNullable<string>>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.KeepWrappers };
        name = item.EasyName(options);
        Assert.Equal("Type4C<IsNullable<Nullable<long>>, IsNullable<string>>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTHOST}." +
            "Type4A<System.Byte, System.Int16>." +
            "Type4B<System.Int32>."+ 
            "Type4C<Yotei.Tools.IsNullable<System.Nullable<System.Int64>>, Yotei.Tools.IsNullable<System.String>>",
            name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Core_Attribute()
    {
        EasyTypeOptions options;
        string name;
        var item = typeof(Attribute);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Attribute", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Attribute", name);

        options = DEFAULT with { UseNamespace = true };
        name = item.EasyName(options); Assert.Equal("System.Attribute", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.Attribute", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Custom_Attribute()
    {
        EasyTypeOptions options;
        string name;
        var item = typeof(IsNullableAttribute);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IsNullableAttribute", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IsNullable", name);

        options = DEFAULT with { UseNamespace = true };
        name = item.EasyName(options); Assert.Equal("Yotei.Tools.IsNullable", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("Yotei.Tools.IsNullableAttribute", name);
    }
}
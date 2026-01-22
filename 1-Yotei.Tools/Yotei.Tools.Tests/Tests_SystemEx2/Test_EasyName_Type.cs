namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyName_Type
{
    static readonly string NAMESPACE = typeof(Test_EasyName_Type).Namespace!;
    static readonly string TESTNAME = typeof(Test_EasyName_Type).Name;

    readonly static EasyNameOptions EMPTY = EasyNameOptions.Empty;
    readonly static EasyNameOptions DEFAULT = EasyNameOptions.Default;
    readonly static EasyNameOptions FULL = EasyNameOptions.Full;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test0_System_NullableValueType()
    {
        EasyNameOptions options;
        string name;

        // Annotated...
        var item = typeof(int?);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Int32", name);

        options = EMPTY with { HideTypeName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Int32?", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.Nullable<System.Int32>", name);

        // Explicit...
        item = typeof(Nullable<int>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Int32", name);

        options = EMPTY with { HideTypeName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Int32?", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.Nullable<System.Int32>", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test0_System_NullableReferenceType()
    {
        EasyNameOptions options;
        string name;

        // CS8639: the typeof operator cannot be used on a nullable reference type...
        var item = typeof(string);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("String", name);

        options = EMPTY with { HideTypeName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("String", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.String", name);

        // Explicit...
        item = typeof(EasyNullable<string>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("String", name);

        options = EMPTY with { HideTypeName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("String?", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal("Yotei.Tools.EasyNullable<System.String>", name);
    }

    // ----------------------------------------------------

    // Using 'EasyNullable<T>' is not allowed in this context.
    // Using '[EasyNullable]' is ok because the type is a generic one.
    public interface IFace1<[EasyNullable] T> { }

    //[Enforced]
    [Fact]
    public static void Test1_Generic()
    {
        EasyNameOptions options;
        string name;

        // Unbound...
        var item = typeof(IFace1<>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace1", name);

        options = EMPTY with { HideTypeName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace1<T?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTNAME}.IFace1<T?>", name);

        // Bound (nullability lost)...
        item = typeof(IFace1<string?>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace1", name);

        options = EMPTY with { HideTypeName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace1<String>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTNAME}.IFace1<System.String>", name);

        // Bound (forced by wrapping, attribute not allowed with typeof)...
        item = typeof(IFace1<EasyNullable<string>>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace1", name);

        options = EMPTY with { HideTypeName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace1<String?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}.IFace1<Yotei.Tools.EasyNullable<System.String>>",
            name);
    }

    // ----------------------------------------------------

    public interface IFace2A<[EasyNullable] T> { }
    public interface IFace2B<T> : IFace2A<T> { }

    //[Enforced]
    [Fact]
    public static void Test2_Inherited_AsIs()
    {
        EasyNameOptions options;
        string name;

        // Unbound (nullability lost)...
        var type = typeof(IFace2B<>);
        var item = type.GetInterfaces().First();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace2A", name);

        options = EMPTY with { HideTypeName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace2A<T>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTNAME}.IFace2A<T>", name);

        // Bound (nullability lost)...
        type = typeof(IFace2B<string>);
        item = type.GetInterfaces().First();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace2A", name);

        options = EMPTY with { HideTypeName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace2A<String>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTNAME}.IFace2A<System.String>", name);

        // Bound (forced nullability)...
        type = typeof(IFace2B<EasyNullable<string>>);
        item = type.GetInterfaces().First();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace2A", name);

        options = EMPTY with { HideTypeName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace2A<String?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}.IFace2A<Yotei.Tools.EasyNullable<System.String>>",
            name);
    }

    // ----------------------------------------------------

    public interface IFace2C<T> : IFace2A<EasyNullable<T>> { }

    //[Enforced]
    [Fact]
    public static void Test2_Inherited_Enforced()
    {
        EasyNameOptions options;
        string name;

        // Unbound (wrapped nullability kept)...
        var type = typeof(IFace2C<>);
        var item = type.GetInterfaces().First();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace2A", name);

        options = EMPTY with { HideTypeName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace2A<T?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTNAME}.IFace2A<Yotei.Tools.EasyNullable<T>>", name);

        // Bound (wrapped nullability inherited, no need for annotation)...
        type = typeof(IFace2C<string>);
        item = type.GetInterfaces().First();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace2A", name);

        options = EMPTY with { HideTypeName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace2A<String?>", name);

        var xoptions = DEFAULT with { TypeNullableStyle = EasyNullableStyle.None };
        options = DEFAULT with { TypeGenericArgumentOptions = xoptions };
        name = item.EasyName(options); Assert.Equal("IFace2A<String>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}.IFace2A<Yotei.Tools.EasyNullable<System.String>>",
            name);
    }

    // ----------------------------------------------------

    public interface IFace3A<[EasyNullable] out T>
    { public interface IFace3B<[EasyNullable] in K> { } }

    //[Enforced]
    [Fact]
    public static void Test3_Nested()
    {
        EasyNameOptions options;
        string name;

        // Unbound...
        var item = typeof(IFace3A<>.IFace3B<>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace3B", name);

        options = EMPTY with { HideTypeName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace3B<K?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTNAME}.IFace3A<out T?>.IFace3B<in K?>", name);

        // Unbound (nullability lost)...
        item = typeof(IFace3A<int?>.IFace3B<string?>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace3B", name);

        options = EMPTY with { HideTypeName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace3B<String>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}." +
            "IFace3A<System.Nullable<System.Int32>>.IFace3B<System.String>",
            name);

        // Unbound (wrapped nullability enforced)...
        item = typeof(IFace3A<int?>.IFace3B<EasyNullable<string?>>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("IFace3B", name);

        options = EMPTY with { HideTypeName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace3B<String?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}." +
            "IFace3A<System.Nullable<System.Int32>>.IFace3B<Yotei.Tools.EasyNullable<System.String>>",
            name);
    }

    // ----------------------------------------------------

    public class Type4A<K, T>
    { public class Type4B<R> { public class Type4C<[EasyNullable] S, [EasyNullable] V> { } } }

    //[Enforced]
    [Fact]
    public static void Test4_DeepNested()
    {
        EasyNameOptions options;
        string name;

        // Unbound...
        var item = typeof(Type4A<,>.Type4B<>.Type4C<,>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type4C", name);

        options = EMPTY with { HideTypeName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type4C<S?, V?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTNAME}.Type4A<K, T>.Type4B<R>.Type4C<S?, V?>", name);

        // Unbound (nullability lost)...
        item = typeof(Type4A<byte, short>.Type4B<int>.Type4C<long?, string?>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type4C", name);

        options = EMPTY with { HideTypeName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type4C<Int64?, String>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}.Type4A<System.Byte, System.Int16>." +
            "Type4B<System.Int32>." +
            "Type4C<System.Nullable<System.Int64>, System.String>",
            name);

        // Unbound (wrapped nullability enforced)...
        item = typeof(Type4A<byte, short>.Type4B<int>.Type4C<long?, EasyNullable<string?>>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type4C", name);

        options = EMPTY with { HideTypeName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type4C<Int64?, String?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}.Type4A<System.Byte, System.Int16>." +
            "Type4B<System.Int32>." +
            "Type4C<System.Nullable<System.Int64>, Yotei.Tools.EasyNullable<System.String>>",
            name);
    }
}
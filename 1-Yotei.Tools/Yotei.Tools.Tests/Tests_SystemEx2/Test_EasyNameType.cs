using System.Xml.Schema;

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyNameType
{
    static readonly string NAMESPACE = typeof(Test_EasyNameType).Namespace!;
    static readonly string TESTNAME = typeof(Test_EasyNameType).Name;

    readonly static EasyNameType EMPTY = EasyNameType.Empty;
    readonly static EasyNameType DEFAULT = EasyNameType.Default;
    readonly static EasyNameType FULL = EasyNameType.Full;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test0_System_NullableValueType()
    {
        EasyNameType options;
        string name;

        // Annotated nullability...
        var item = typeof(int?);

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Int32", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.UseAnnotation };
        name = item.EasyName(options); Assert.Equal("Int32?", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.Nullable<System.Int32>", name);

        // Explicit nullability...
        item = typeof(IsNullable<int>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Int32", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.UseAnnotation };
        name = item.EasyName(options); Assert.Equal("Int32?", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("Yotei.Tools.IsNullable<System.Int32>", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test0_System_NullableReferenceType()
    {
        EasyNameType options;
        string name;

        // Cannot annotate a reference type in this context...
        // CA8639: the typeof operator cannot be used on a nullable reference type
        var item = typeof(string);

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("String", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.UseAnnotation };
        name = item.EasyName(options); Assert.Equal("String", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.String", name);

        // Explicit nullability...
        item = typeof(IsNullable<string>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("String", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.UseAnnotation };
        name = item.EasyName(options); Assert.Equal("String?", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("Yotei.Tools.IsNullable<System.String>", name);
    }

    // ----------------------------------------------------

    // Cannot use 'T' in this context, syntax error...
    // public interface IFace1<IsNullable<T>> { }

    public interface IFace1<[IsNullable] T> { } // Attribute is ok because 'T' is generic

    //[Enforced]
    [Fact]
    public static void Test1_Nullable_Generic()
    {
        EasyNameType options;
        string name;

        // Unbound generic...
        var item = typeof(IFace1<>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace1<T>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.UseAnnotation };
        name = item.EasyName(options); Assert.Equal("IFace1<T?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTNAME}.IFace1<T?>", name);

        // Bound generic, neither attribute nor annotation are preserved...
        item = typeof(IFace1<string?>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace1<String>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.UseAnnotation };
        name = item.EasyName(options); Assert.Equal("IFace1<String>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTNAME}.IFace1<System.String>", name);

        // Bound generic, forced nullability
        // item = typeof(IFace1<[IsNullable] string>); -- [IsNullable] is syntax error...
        item = typeof(IFace1<IsNullable<string>>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace1<String>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.UseAnnotation };
        name = item.EasyName(options); Assert.Equal("IFace1<String?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}.IFace1<Yotei.Tools.IsNullable<System.String>>",
            name);
    }

    // ----------------------------------------------------

    public interface IFace2A<[IsNullable] T> { }
    public interface IFace2B<[IsNullable] T> : IFace2A<T> { }

    //[Enforced]
    [Fact]
    public static void Test2_InheritedType()
    {
        EasyNameType options;
        string name;

        // Unbound generic...
        var type = typeof(IFace2B<>);
        var item = type.GetInterfaces().First();

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace2A<T>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.UseAnnotation };
        name = item.EasyName(options); Assert.Equal("IFace2A<T?>", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("IFace2A<T?>", name);

        // It happens type *is not* a generic type definition, so what follows rises an
        // invalid operation exception...
        // item = item.MakeGenericType(typeof(string));

        // Bounded, but nullable annotation not preserved...
        type = typeof(IFace2B<string?>);
        item = type.GetInterfaces().First();

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace2A<String>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.UseAnnotation };
        name = item.EasyName(options); Assert.Equal("IFace2A<String>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{TESTNAME}.IFace2A<System.String>", name);

        // Bounded, nullability enfoced...
        type = typeof(IFace2B<IsNullable<string>>);
        item = type.GetInterfaces().First();

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace2A<String>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.UseAnnotation };
        name = item.EasyName(options); Assert.Equal("IFace2A<String?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}.IFace2A<Yotei.Tools.IsNullable<System.String>>",
            name);
    }

    // ----------------------------------------------------

    public interface IFace3A<[IsNullable] out T>
    { public interface IFace3B<[IsNullable] in K> { } }

    //[Enforced]
    [Fact]
    public static void Test3_NestedType()
    {
        EasyNameType options;
        string name;

        // Unbound generic...
        var item = typeof(IFace3A<>.IFace3B<>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace3B<K>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.UseAnnotation };
        name = item.EasyName(options); Assert.Equal("IFace3B<K?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}.IFace3A<out T?>.IFace3B<in K?>",
            name);

        // Bounded, not preserved...
        item = typeof(IFace3A<int?>.IFace3B<string?>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace3B<String>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.UseAnnotation };
        name = item.EasyName(options); Assert.Equal("IFace3B<String>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}.IFace3A<System.Nullable<System.Int32>>.IFace3B<System.String>",
            name);

        // Bounded, forced...
        item = typeof(IFace3A<int?>.IFace3B<IsNullable<string?>>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace3B<String>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.UseAnnotation };
        name = item.EasyName(options); Assert.Equal("IFace3B<String?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}.IFace3A<System.Nullable<System.Int32>>." +
            "IFace3B<Yotei.Tools.IsNullable<System.String>>",
            name);
    }

    // ----------------------------------------------------

    public class Type4A<K, T>
    { public class Type4B<R> { public class Type4C<[IsNullable] S, [IsNullable] V> { } } }

    //[Enforced]
    [Fact]
    public static void Test4_DeepNestedType()
    {
        EasyNameType options;
        string name;

        // Unbound generic...
        var item = typeof(Type4A<,>.Type4B<>.Type4C<,>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type4C<S, V>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.UseAnnotation };
        name = item.EasyName(options); Assert.Equal("Type4C<S?, V?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}.Type4A<K, T>.Type4B<R>.Type4C<S?, V?>",
            name);

        // Bounded, not preserved...
        item = typeof(Type4A<byte, short>.Type4B<int>.Type4C<long?, string?>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type4C<Int64, String>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.UseAnnotation };
        name = item.EasyName(options); Assert.Equal("Type4C<Int64?, String>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}.Type4A<System.Byte, System.Int16>." +
            "Type4B<System.Int32>.Type4C<System.Nullable<System.Int64>, System.String>",
            name);

        // Bounded, forced...
        item = typeof(Type4A<byte, short>.Type4B<int>.Type4C<long?, IsNullable<string?>>);

        options = EMPTY;
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Empty(name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type4C<Int64, String>", name);

        options = DEFAULT with { NullableStyle = EasyNullableStyle.UseAnnotation };
        name = item.EasyName(options); Assert.Equal("Type4C<Int64?, String?>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{TESTNAME}." +
            "Type4A<System.Byte, System.Int16>." +
            "Type4B<System.Int32>." +
            "Type4C<System.Nullable<System.Int64>, Yotei.Tools.IsNullable<System.String>>",
            name);
    }
}
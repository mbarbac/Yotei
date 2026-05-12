#pragma warning disable CS8500, CA1822

namespace Yotei.Tools.Tests.EasyNames;

/* NOTES:
 * - Using reflection there is no way to determine if a type is a 'partial' one.
 * - Using reflection there is no way to determine if a type is a 'file' one.
 */

// ========================================================
//[Enforced]
public static class Test_EasyType
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyType";

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_WithRecursive()
    {
        EasyTypeOptions source, target;

        source = EasyTypeOptions.Empty; target = source.WithRecursive(); Assert.Same(source, target);
        source = EasyTypeOptions.Default; target = source.WithRecursive(); Assert.Same(source, target);
        source = EasyTypeOptions.Full; target = source.WithRecursive(); Assert.Same(source, target);

        source = EasyTypeOptions.Empty;
        target = source.WithRecursive(usePlaceHolder: true);
        Assert.NotSame(source, target);
        Assert.True(target.UsePlaceHolder);
        Assert.Null(target.GenericListOptions);

        source = EasyTypeOptions.Default;
        target = source.WithRecursive(usePlaceHolder: true);
        Assert.NotSame(source, target);
        Assert.True(target.UsePlaceHolder);
        Assert.NotSame(source.GenericListOptions, target.GenericListOptions);
        Assert.Same(target, target.GenericListOptions);

        source = EasyTypeOptions.Full;
        target = source.WithRecursive(usePlaceHolder: true);
        Assert.NotSame(source, target);
        Assert.True(target.UsePlaceHolder);
        Assert.NotSame(source.GenericListOptions, target.GenericListOptions);
        Assert.Same(target, target.GenericListOptions);

        source = EasyTypeOptions.Full with { GenericListOptions = EasyTypeOptions.Default };
        target = source.WithRecursive(usePlaceHolder: true);
        Assert.NotSame(source, target);
        Assert.True(target.UsePlaceHolder);
        Assert.NotSame(source.GenericListOptions, target.GenericListOptions);
        Assert.NotSame(target, target.GenericListOptions);
        Assert.True(target.GenericListOptions!.UsePlaceHolder);

        source = target;
        target = source.WithRecursive(usePlaceHolder: false);
        Assert.NotSame(source, target);
        Assert.False(target.UsePlaceHolder);
        Assert.NotSame(source.GenericListOptions, target.GenericListOptions);
        Assert.NotSame(target, target.GenericListOptions);
        Assert.False(target.GenericListOptions!.UsePlaceHolder);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Special_ValueType()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(int);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Int32", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("int", name);

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("int", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public struct System.Int32", name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("int?", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public struct System.Nullable<System.Int32>", name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("System.DateTime", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public struct System.DateTime", name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("System.DateTime?", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public struct System.Nullable<System.DateTime>", name);
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
        name = source.EasyName(options); Assert.Equal("String", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("string", name);

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("string", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public sealed class System.String", name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("string?", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public sealed class Yotei.Tools.IsNullable<System.String>", name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace1<T?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"private interface {PREFIX}.IFace1<T?>", name);

        options.GenericListOptions = EasyTypeOptions.Empty with { UsePlaceHolder = true };
        name = source.EasyName(options);
        Assert.Equal($"private interface {PREFIX}.IFace1<>", name);

        options.GenericListOptions = null;
        name = source.EasyName(options);
        Assert.Equal($"private interface {PREFIX}.IFace1", name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace1<bool?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"private interface {PREFIX}.IFace1<System.Nullable<System.Boolean>>", name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace1<string>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"private interface {PREFIX}.IFace1<System.String>", name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace1<string?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"private interface {PREFIX}.IFace1<Yotei.Tools.IsNullable<System.String>>",
            name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal($"System.Predicate<T?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public sealed Delegate System.Predicate<T?>", name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal($"System.Predicate<bool?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            "public sealed Delegate System.Predicate<System.Nullable<System.Boolean>>",
            name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal($"System.Predicate<string>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public sealed Delegate System.Predicate<System.String>", name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal($"System.Predicate<string?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            "public sealed Delegate System.Predicate<Yotei.Tools.IsNullable<System.String>>",
            name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace3a<T>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"private interface {PREFIX}.IFace3a<T>", name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace3a<T?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"private interface {PREFIX}.IFace3a<T?>", name);
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
        name = source.EasyName(options); Assert.Equal("IFace3a<bool?>", name);

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace3a<bool?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"private interface {PREFIX}.IFace3a<System.Nullable<System.Boolean>>",
            name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace3a<string>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"private interface {PREFIX}.IFace3a<System.String>", name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace3a<string?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"private interface {PREFIX}.IFace3a<Yotei.Tools.IsNullable<System.String>>",
            name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace3a<T?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"private interface {PREFIX}.IFace3a<Yotei.Tools.IsNullable<T>>", name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace3a<bool?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"private interface {PREFIX}." +
            "IFace3a<Yotei.Tools.IsNullable<System.Nullable<System.Boolean>>>",
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.IFace3a<string?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"private interface {PREFIX}.IFace3a<Yotei.Tools.IsNullable<System.String>>",
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace4a<K, T>.IFace4b<R>.IFace4c<S, V>", name);

        options = EasyTypeOptions.Default;
        options.GenericListOptions = new EasyTypeOptions() { UsePlaceHolder = true };
        name = source.EasyName(options); Assert.Equal("IFace4c<,>", name);

        // Interesting, the nested interface of an interface is a public one...
        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public interface {PREFIX}.IFace4a<K, T>.IFace4b<R>.IFace4c<S, V>",
            name);

        options.GenericListOptions = null;
        name = source.EasyName(options);
        Assert.Equal($"public interface {PREFIX}.IFace4a.IFace4b.IFace4c", name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace4a<byte?, short?>.IFace4b<int?>.IFace4c<long?, string>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public interface {PREFIX}." +
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace4a<byte?, short?>.IFace4b<int?>.IFace4c<long?, string?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public interface {PREFIX}." +
            "IFace4a<System.Nullable<System.Byte>, System.Nullable<System.Int16>>." +
            "IFace4b<System.Nullable<System.Int32>>." +
            "IFace4c<System.Nullable<System.Int64>, Yotei.Tools.IsNullable<System.String>>",
            name);
    }

    // ----------------------------------------------------

    interface IFace5a<K, T>
    { interface IFace5b<R> { interface IFace5c<[IsNullable] S, [IsNullable] V> { } } }

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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options);
        Assert.Equal(
            $"{PREFIX}.IFace5a<K, T>.IFace5b<R>.IFace5c<S?, V?>", name);

        options.GenericListOptions = new EasyTypeOptions() { UsePlaceHolder = true };
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace5a<,>.IFace5b<>.IFace5c<,>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"public interface {PREFIX}.IFace5a<K, T>.IFace5b<R>.IFace5c<S?, V?>", name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace5a<byte?, short?>.IFace5b<int?>.IFace5c<long?, string>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public interface {PREFIX}.IFace5a<System.Nullable<System.Byte>, System.Nullable<System.Int16>>." +
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace5a<byte?, short?>.IFace5b<int?>.IFace5c<long?, string?>", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public interface {PREFIX}." +
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options);
        Assert.Equal($"System.Attribute", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public abstract class System.Attribute", name);

        options.RemoveAttributeSuffix = true;
        name = source.EasyName(options);
        Assert.Equal("public abstract class System.Attribute", name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options);
        Assert.Equal($"Yotei.Tools.IsNullable", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public sealed class Yotei.Tools.IsNullableAttribute", name);
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

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options);
        Assert.Equal($"Yotei.Tools.IsNullable", name);

        options = EasyTypeOptions.Full with { NamespaceStyle = EasyNamespaceStyle.UseGlobal };
        name = source.EasyName(options);
        Assert.Equal("public sealed class global::Yotei.Tools.IsNullableAttribute", name);
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
    public static void Test_Array_ValueType_ArrayNullability_Lost()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(RType9a<>);
        var method = type.GetMethod("Foo1")!;
        var source = method.ReturnType;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Nullable[]", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("int?[]", name);

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("int?[]", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public sealed class System.Nullable<System.Int32>[]", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_ValueType_ArrayNullability_Wrapped()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(RType9a<>);
        var method = type.GetMethod("Foo2")!;
        var source = method.ReturnType;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IsNullable", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("int?[]?", name);

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("int?[]?", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            "public sealed class Yotei.Tools.IsNullable<Yotei.Tools.IsNullable<System.Int32>[]>",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_ValueType_ArrayNullability_Decorated_LostOnReturnType()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(RType9a<>);
        var method = type.GetMethod("Foo3")!;
        var source = method.ReturnType;

        // Syntax suggests that the return type is the decorated element, but in reality is the
        // method the one decorated.

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Nullable[]", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("int?[]", name);

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("int?[]", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            "public sealed class System.Nullable<System.Int32>[]",
            name);
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
    public static void Test_Array_ReferenceType_ArrayAndElementNullability_Lost()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(RType9b<>);
        var method = type.GetMethod("Foo1")!;
        var source = method.ReturnType;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("String[]", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("string[]", name);

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("string[]", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public sealed class System.String[]", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_ReferenceType_ArrayAndElementNullability_Wrapped()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(RType9b<>);
        var method = type.GetMethod("Foo2")!;
        var source = method.ReturnType;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IsNullable", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("string?[]?", name);

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("string?[]?", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            "public sealed class Yotei.Tools.IsNullable<Yotei.Tools.IsNullable<System.String>[]>",
            name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_ReferenceType_ArrayAndElementNullability_Decorated_LostOnReturnType()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(RType9b<>);
        var method = type.GetMethod("Foo3")!;
        var source = method.ReturnType;

        // Syntax suggests that the return type is the decorated element, but in reality is the
        // method the one decorated.

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IsNullable[]", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("string?[]", name);

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("string?[]", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            "public sealed class Yotei.Tools.IsNullable<System.String>[]",
            name);
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
    public static void Test_Array_GenericType_ArrayAndElementNullability_Lost()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(RType9c<>);
        var method = type.GetMethod("Foo1")!;
        var source = method.ReturnType;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("T[]", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("T[]", name);

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("T[]", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public sealed class T[]", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_GenericType_ArrayAndElementNullability_Wrapped()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(RType9c<>);
        var method = type.GetMethod("Foo2")!;
        var source = method.ReturnType;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IsNullable", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("T?[]?", name);

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("T?[]?", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public sealed class Yotei.Tools.IsNullable<Yotei.Tools.IsNullable<T>[]>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_GenericType_ArrayNullability_Decorated_LostOnReturnType()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(RType9c<>);
        var method = type.GetMethod("Foo3")!;
        var source = method.ReturnType;

        // Syntax suggests that the return type is the decorated element, but in reality is the
        // method the one decorated.

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IsNullable[]", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("T?[]", name);

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("T?[]", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            "public sealed class Yotei.Tools.IsNullable<T>[]",
            name);
    }

    // ----------------------------------------------------

    // Sentences like 'int*?' are syntax errors.
    unsafe class RType8a<T> { public int?* Foo1() => throw null!; }

    //[Enforced]
    [Fact]
    public static void Test_Pointer_ValueType()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(RType8a<>);
        var method = type.GetMethod("Foo1")!;
        var source = method.ReturnType;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Nullable*", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("int?*", name);

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("int?*", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public class System.Nullable<System.Int32>*", name);
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
        EasyTypeOptions options;
        string name;
        var type = typeof(RType8b<>);
        var method = type.GetMethod("Foo1")!;
        var source = method.ReturnType;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("String*", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("string*", name);

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("string*", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public class System.String*", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Pointer_ReferenceType_Nullability_Wrapped()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(RType8b<>);
        var method = type.GetMethod("Foo2")!;
        var source = method.ReturnType;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IsNullable*", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("string?*", name);

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("string?*", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public class Yotei.Tools.IsNullable<System.String>*", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Pointer_ReferenceType_Nullability_Decorated_LostOnReturnType()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(RType8b<>);
        var method = type.GetMethod("Foo3")!;
        var source = method.ReturnType;

        // Syntax suggests that the return type is the decorated element, but in reality is the
        // method the one decorated.

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("String*", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("string*", name);

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("string*", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public class System.String*", name);
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
        EasyTypeOptions options;
        string name;
        var type = typeof(RType8c<>);
        var method = type.GetMethod("Foo1")!;
        var source = method.ReturnType;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("T*", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("T*", name);

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("T*", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public class T*", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Pointer_GenericType_Nullability_Wrapped()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(RType8c<>);
        var method = type.GetMethod("Foo2")!;
        var source = method.ReturnType;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IsNullable*", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("T?*", name);

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("T?*", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public class Yotei.Tools.IsNullable<T>*", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Public()
    {
        var source = typeof(Test_EasyType);
        var options = EasyTypeOptions.Full;
        var name = source.EasyName(options);
        Assert.Equal($"public static class {PREFIX}", name);
    }

    // ----------------------------------------------------

    public class RType9a { }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Public()
    {
        var source = typeof(RType9a);
        var options = EasyTypeOptions.Full;
        var name = source.EasyName(options);
        Assert.Equal($"public class {PREFIX}.RType9a", name);
    }

    // ----------------------------------------------------

    protected class RType9b { }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Protected()
    {
        var source = typeof(RType9b);
        var options = EasyTypeOptions.Full;
        var name = source.EasyName(options);
        Assert.Equal($"protected class {PREFIX}.RType9b", name);
    }

    // ----------------------------------------------------

    protected internal class RType9c { }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Protected_Internal()
    {
        var source = typeof(RType9c);
        var options = EasyTypeOptions.Full;
        var name = source.EasyName(options);
        Assert.Equal($"protected internal class {PREFIX}.RType9c", name);
    }

    // ----------------------------------------------------

    private protected class RType9d { }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Private_Protected()
    {
        var source = typeof(RType9d);
        var options = EasyTypeOptions.Full;
        var name = source.EasyName(options);
        Assert.Equal($"private protected class {PREFIX}.RType9d", name);
    }

    // ----------------------------------------------------

    private class RType9e { }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Private()
    {
        var source = typeof(RType9e);
        var options = EasyTypeOptions.Full;
        var name = source.EasyName(options);
        Assert.Equal($"private class {PREFIX}.RType9e", name);
    }

    // ----------------------------------------------------

    internal class RType9f { }

    //[Enforced]
    [Fact]
    public static void Test_Nested_Internal()
    {
        var source = typeof(RType9f);
        var options = EasyTypeOptions.Full;
        var name = source.EasyName(options);
        Assert.Equal($"internal class {PREFIX}.RType9f", name);
    }

    // ----------------------------------------------------

    public enum EType1 { One, Two }

    //[Enforced]
    [Fact]
    public static void Test_Enum()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(EType1);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("EType1", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("EType1", name);

        options = options with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal($"{PREFIX}.EType1", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal($"public enum {PREFIX}.EType1", name);
    }
}
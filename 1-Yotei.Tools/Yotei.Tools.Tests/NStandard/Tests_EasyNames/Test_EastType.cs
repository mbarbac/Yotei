namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
public static class Test_EasyType
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyType";

    // ----------------------------------------------------

    [Fact]
    public static void Test_WithRecursive_Core()
    {
        // We'll use the fact that 'usePlaceHolder' is always false by default...
        EasyType source, target;

        source = EasyType.Empty; target = source.With(usePlaceHolder: true);
        Assert.True(target.UsePlaceHolder);
        Assert.Null(target.GenericListOptions);

        source = EasyType.Default; target = source.With(usePlaceHolder: true);
        Assert.True(target.UsePlaceHolder);
        Assert.True(target.GenericListOptions!.UsePlaceHolder);

        source = EasyType.Full; target = source.With(usePlaceHolder: true);
        Assert.True(target.UsePlaceHolder);
        Assert.True(target.GenericListOptions!.UsePlaceHolder);
    }

    [Fact]
    public static void Test_WithRecursive_Custom()
    {
        // We'll use the fact that 'usePlaceHolder' is always false by default...
        EasyType source, target;

        source = EasyType.Full with { GenericListOptions = EasyType.Default };        
        target = source.With(usePlaceHolder: true);
        Assert.True(target.UsePlaceHolder);
        Assert.True(target.GenericListOptions!.UsePlaceHolder);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_SpecialName_ValueType()
    {
        var source = typeof(int);
        var options = EasyType.Empty;
        var name = source.EasyName(options); Assert.Equal("Int32", name);

        options = EasyType.Default;
        name = source.EasyName(options); Assert.Equal("int", name);

        options = EasyType.Full;
        name = source.EasyName(options);
        Assert.Equal("public struct System.Int32", name);
    }

    [Fact]
    public static void Test_SpecialName_ValueType_Annotated()
    {
        var source = typeof(int?);
        var options = EasyType.Empty;
        var name = source.EasyName(options); Assert.Equal("Nullable", name);

        options = EasyType.Default;
        name = source.EasyName(options); Assert.Equal("int?", name);

        options = EasyType.Full;
        name = source.EasyName(options);
        Assert.Equal("public struct System.Nullable<System.Int32>", name);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_SpecialName_ReferenceType()
    {
        var source = typeof(string);
        var options = EasyType.Empty;
        var name = source.EasyName(options); Assert.Equal("String", name);

        options = EasyType.Default;
        name = source.EasyName(options); Assert.Equal("string", name);

        options = EasyType.Full;
        name = source.EasyName(options);
        Assert.Equal("public sealed class System.String", name);
    }

    [Fact]
    public static void Test_SpecialName_ReferenceType_Annotated()
    {
        // 'typeof(string?)' produces error CS8639: 'typeof operator cannot be used on a nullable
        // reference type'. This is one of the use cases the workaround types were created for.

        var source = typeof(IsNullable<string>);
        var options = EasyType.Empty;
        var name = source.EasyName(options); Assert.Equal("IsNullable", name);

        options = EasyType.Default;
        name = source.EasyName(options); Assert.Equal("string?", name);

        options = EasyType.Full;
        name = source.EasyName(options);
        Assert.Equal("public sealed class Yotei.Tools.IsNullable<System.String>", name);
    }

    // ----------------------------------------------------

    [Fact]
    public static void Test_ValueType()
    {
        var source = typeof(DateTime);
        var options = EasyType.Empty;
        var name = source.EasyName(options); Assert.Equal("DateTime", name);

        options = EasyType.Default;
        name = source.EasyName(options); Assert.Equal("DateTime", name);

        options = EasyType.Full;
        name = source.EasyName(options);
        Assert.Equal("public struct System.DateTime", name);
    }

    [Fact]
    public static void Test_ValueType_Annotated()
    {
        var source = typeof(DateTime?);
        var options = EasyType.Empty;
        var name = source.EasyName(options); Assert.Equal("Nullable", name);

        options = EasyType.Default;
        name = source.EasyName(options); Assert.Equal("DateTime?", name);

        options = EasyType.Full;
        name = source.EasyName(options);
        Assert.Equal("public struct System.Nullable<System.DateTime>", name);
    }

    // ----------------------------------------------------

    // 'IFace<T?>' produces error CS1003: 'syntax error'.
    // 'interface IFace2<IsNullable<T>>' also produces CS1003 plus a lot other ones.
    // This are one of the use cases the workaround types were created for.

    interface IFace1<[IsNullable] T> { }

    [Fact]
    public static void Test_GenericArgument_Unbound()
    {
        var source = typeof(IFace1<>);
        var options = EasyType.Empty;
        var name = source.EasyName(options); Assert.Equal("IFace1", name);

        options = EasyType.Default;
        name = source.EasyName(options); Assert.Equal("IFace1<T?>", name);

        options = options with
        { GenericListOptions = options.GenericListOptions! with { UsePlaceHolder = true } };
        name = source.EasyName(options);
        Assert.Equal("IFace1<>", name);

        options = EasyType.Full;
        name = source.EasyName(options);
        Assert.Equal($"private interface {PREFIX}.IFace1<T?>", name);

        options = options with { GenericListOptions = null };
        name = source.EasyName(options);
        Assert.Equal($"private interface {PREFIX}.IFace1", name);
    }

    [Fact]
    public static void Test_GenericArgument_BoundTo_Nullable_ValueType()
    {
        var source = typeof(IFace1<int?>);
        var options = EasyType.Empty;
        var name = source.EasyName(options); Assert.Equal("IFace1", name);

        options = EasyType.Default;
        name = source.EasyName(options); Assert.Equal("IFace1<int?>", name);

        options = options with
        { GenericListOptions = options.GenericListOptions! with { UsePlaceHolder = true } };
        name = source.EasyName(options);
        Assert.Equal("IFace1<>", name);

        options = EasyType.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"private interface {PREFIX}.IFace1<System.Nullable<System.Int32>>",
            name);

        options = options with { GenericListOptions = null };
        name = source.EasyName(options);
        Assert.Equal(
            $"private interface {PREFIX}.IFace1",
            name);
    }

    [Fact]
    public static void Test_GenericArgument_BoundTo_Nullable_ReferenceType_NullabilityLost()
    {
        var source = typeof(IFace1<string?>);
        var options = EasyType.Empty;
        var name = source.EasyName(options); Assert.Equal("IFace1", name);

        options = EasyType.Default;
        name = source.EasyName(options); Assert.Equal("IFace1<string>", name);

        options = options with
        { GenericListOptions = options.GenericListOptions! with { UsePlaceHolder = true } };
        name = source.EasyName(options);
        Assert.Equal("IFace1<>", name);

        options = EasyType.Full;
        name = source.EasyName(options);
        Assert.Equal($"private interface {PREFIX}.IFace1<System.String>", name);

        options = options with { GenericListOptions = null };
        name = source.EasyName(options);
        Assert.Equal($"private interface {PREFIX}.IFace1", name);
    }

    [Fact]
    public static void Test_GenericArgument_BoundTo_Nullable_ReferenceType_Wrapped()
    {
        // 'IFace1<[IsNullable] string>' produces error CS1031: 'type expected'.

        var source = typeof(IFace1<IsNullable<string>>);
        var options = EasyType.Empty;
        var name = source.EasyName(options); Assert.Equal("IFace1", name);

        options = EasyType.Default;
        name = source.EasyName(options); Assert.Equal("IFace1<string?>", name);

        options = options with
        { GenericListOptions = options.GenericListOptions! with { UsePlaceHolder = true } };
        name = source.EasyName(options);
        Assert.Equal("IFace1<>", name);

        options = EasyType.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"private interface {PREFIX}.IFace1<Yotei.Tools.IsNullable<System.String>>",
            name);

        options = options with { GenericListOptions = null };
        name = source.EasyName(options);
        Assert.Equal($"private interface {PREFIX}.IFace1", name);
    }

    // ----------------------------------------------------

    interface IFace2<[IsNullable] T> { void MyMethod(Predicate<T?> _); }

    [Fact]
    public static void Test_GenericParameter_Unbound()
    {
        var type = typeof(IFace2<>);
        var method = type.GetMethod("MyMethod")!;
        var source = method.GetParameters()[0].ParameterType;
        var options = EasyType.Empty;
        var name = source.EasyName(options); Assert.Equal("Predicate", name);

        options = EasyType.Default;
        name = source.EasyName(options); Assert.Equal("Predicate<T?>", name);

        options = options with
        { GenericListOptions = options.GenericListOptions! with { UsePlaceHolder = true } };
        name = source.EasyName(options);
        Assert.Equal("Predicate<>", name);

        options = EasyType.Full;
        name = source.EasyName(options);
        Assert.Equal($"public sealed Delegate System.Predicate<T?>", name);

        options = options with { GenericListOptions = null };
        name = source.EasyName(options);
        Assert.Equal($"public sealed Delegate System.Predicate", name);
    }

    [Fact]
    public static void Test_GenericParameter_BoundTo_Nullable_ValueType()
    {
        var type = typeof(IFace2<int?>);
        var method = type.GetMethod("MyMethod")!;
        var source = method.GetParameters()[0].ParameterType;
        var options = EasyType.Empty;
        var name = source.EasyName(options); Assert.Equal("Predicate", name);

        options = EasyType.Default;
        name = source.EasyName(options); Assert.Equal("Predicate<int?>", name);

        options = options with
        { GenericListOptions = options.GenericListOptions! with { UsePlaceHolder = true } };
        name = source.EasyName(options);
        Assert.Equal("Predicate<>", name);

        options = EasyType.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public sealed Delegate System.Predicate<System.Nullable<System.Int32>>",
            name);

        options = options with { GenericListOptions = null };
        name = source.EasyName(options);
        Assert.Equal($"public sealed Delegate System.Predicate", name);
    }

    [Fact]
    public static void Test_GenericParameter_BoundTo_Nullable_ReferenceType_NullabilityLost()
    {
        var type = typeof(IFace2<string?>);
        var method = type.GetMethod("MyMethod")!;
        var source = method.GetParameters()[0].ParameterType;
        var options = EasyType.Empty;
        var name = source.EasyName(options); Assert.Equal("Predicate", name);

        options = EasyType.Default;
        name = source.EasyName(options); Assert.Equal("Predicate<string>", name);

        options = options with
        { GenericListOptions = options.GenericListOptions! with { UsePlaceHolder = true } };
        name = source.EasyName(options);
        Assert.Equal("Predicate<>", name);

        options = EasyType.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public sealed Delegate System.Predicate<System.String>",
            name);

        options = options with { GenericListOptions = null };
        name = source.EasyName(options);
        Assert.Equal($"public sealed Delegate System.Predicate", name);
    }

    [Fact]
    public static void Test_GenericParameter_BoundTo_Nullable_ReferenceType_NullabilityLost_Wrapped()
    {
        var type = typeof(IFace2<IsNullable<string?>>);
        var method = type.GetMethod("MyMethod")!;
        var source = method.GetParameters()[0].ParameterType;
        var options = EasyType.Empty;
        var name = source.EasyName(options); Assert.Equal("Predicate", name);

        options = EasyType.Default;
        name = source.EasyName(options); Assert.Equal("Predicate<string?>", name);

        options = options with
        { GenericListOptions = options.GenericListOptions! with { UsePlaceHolder = true } };
        name = source.EasyName(options);
        Assert.Equal("Predicate<>", name);

        options = EasyType.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public sealed Delegate System.Predicate<Yotei.Tools.IsNullable<System.String>>",
            name);

        options = options with { GenericListOptions = null };
        name = source.EasyName(options);
        Assert.Equal($"public sealed Delegate System.Predicate", name);
    }
}
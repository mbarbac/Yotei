namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyParameter
{
    interface IFace1a { void Method(params int?[]? items); }

    //[Enforced]
    [Fact]
    public static void Test_Params_ValueType()
    {
        EasyParameterOptions options;
        string name;
        var type = typeof(IFace1a);
        var method = type.GetMethod("Method")!;
        var source = method.GetParameters()[0];

        options = EasyParameterOptions.Empty;
        name = source.EasyName(options); Assert.Equal("items", name);

        options = EasyParameterOptions.Default;
        name = source.EasyName(options); Assert.Equal("params int?[]?", name);

        options.TypeOptions = options.TypeOptions! with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("params int?[]?", name);

        options = EasyParameterOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("params System.Nullable<System.Int32>[]? items", name);
    }

    // ----------------------------------------------------

    interface IFace1b
    {
        void Method1(params string?[]? items);
        void Method2(params IsNullable<string?>[]? items);
        void Method3([IsNullable] params string?[]? items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Params_ReferenceType_ElementNullability_Lost()
    {
        EasyParameterOptions options;
        string name;
        var type = typeof(IFace1b);
        var method = type.GetMethod("Method1")!;
        var source = method.GetParameters()[0];

        options = EasyParameterOptions.Empty;
        name = source.EasyName(options); Assert.Equal("items", name);

        options = EasyParameterOptions.Default;
        name = source.EasyName(options); Assert.Equal("params string[]?", name);

        options.TypeOptions = options.TypeOptions! with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("params string[]?", name);

        options = EasyParameterOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("params System.String[]? items", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Params_ReferenceType_ElementNullabilityWrapped()
    {
        EasyParameterOptions options;
        string name;
        var type = typeof(IFace1b);
        var method = type.GetMethod("Method2")!;
        var source = method.GetParameters()[0];

        options = EasyParameterOptions.Empty;
        name = source.EasyName(options); Assert.Equal("items", name);

        options = EasyParameterOptions.Default;
        name = source.EasyName(options); Assert.Equal("params string?[]?", name);

        options.TypeOptions = options.TypeOptions! with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("params string?[]?", name);

        options = EasyParameterOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("params Yotei.Tools.IsNullable<System.String>[]? items", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Params_ReferenceType_ElementNullabilityDecorated_Lost()
    {
        EasyParameterOptions options;
        string name;
        var type = typeof(IFace1b);
        var method = type.GetMethod("Method3")!;
        var source = method.GetParameters()[0];

        options = EasyParameterOptions.Empty;
        name = source.EasyName(options); Assert.Equal("items", name);

        options = EasyParameterOptions.Default;
        name = source.EasyName(options); Assert.Equal("params string[]?", name);

        options.TypeOptions = options.TypeOptions! with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("params string[]?", name);

        options = EasyParameterOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("params System.String[]? items", name);
    }

    // ----------------------------------------------------

    interface IFace2 { void Method(scoped Span<int> items); }

    //[Enforced]
    [Fact]
    public static void Test_Scoped()
    {
        EasyParameterOptions options;
        string name;
        var type = typeof(IFace2);
        var method = type.GetMethod("Method")!;
        var source = method.GetParameters()[0];

        options = EasyParameterOptions.Empty;
        name = source.EasyName(options); Assert.Equal("items", name);

        options = EasyParameterOptions.Default;
        name = source.EasyName(options); Assert.Equal("scoped Span<int>", name);

        options.TypeOptions = options.TypeOptions! with { NamespaceStyle = EasyNamespaceStyle.Default };
        name = source.EasyName(options); Assert.Equal("scoped System.Span<int>", name);

        options = EasyParameterOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("scoped System.Span<System.Int32> items", name);
    }

    // ----------------------------------------------------

    interface IFace3 { void Method(byte one, out int? two, in string? three); }

    //[Enforced]
    [Fact]
    public static void Test_InOut()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace3);
        var source = type.GetMethod("Method")!;

        options.ParameterOptions = EasyParameterOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method(one, two, three)", name);

        options.ParameterOptions = EasyParameterOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method(byte, out int?, in string?)", name);

        options.ParameterOptions = EasyParameterOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            "Method(System.Byte one, out System.Nullable<System.Int32> two, in System.String? three)",
            name);
    }

    // ----------------------------------------------------

    interface IFace4 { void Method(ref int? one, ref string? two, ref readonly long? three); }

    //[Enforced]
    [Fact]
    public static void Test_Ref()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace4);
        var source = type.GetMethod("Method")!;

        options.ParameterOptions = EasyParameterOptions.Empty;
        name = source.EasyName(options);
        Assert.Equal("Method(one, two, three)", name);

        options.ParameterOptions = EasyParameterOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method(ref int?, ref string?, ref readonly long?)", name);

        options.ParameterOptions = EasyParameterOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            "Method(ref System.Nullable<System.Int32> one, " +
            "ref System.String? two, ref readonly System.Nullable<System.Int64> three)",
            name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_This()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(STypeA);
        var source = type.GetMethod("Method1")!;

        options.ParameterOptions = EasyParameterOptions.Empty;
        name = source.EasyName(options);
        Assert.Equal("Method1(item)", name);

        options.ParameterOptions = EasyParameterOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method1(this DateTime)", name);

        options.ParameterOptions = EasyParameterOptions.Full;
        name = source.EasyName(options); Assert.Equal("Method1(this System.DateTime item)", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_NotThis()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(STypeA);
        var source = type.GetMethod("Method2")!;

        options.ParameterOptions = EasyParameterOptions.Empty;
        name = source.EasyName(options);
        Assert.Equal("Method2(item)", name);

        options.ParameterOptions = EasyParameterOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method2(DateTime)", name);

        options.ParameterOptions = EasyParameterOptions.Full;
        name = source.EasyName(options); Assert.Equal("Method2(System.DateTime item)", name);
    }
}

// ========================================================
public static class STypeA
{
    public static void Method1(this DateTime item) { }
    public static void Method2(DateTime item) { }
}
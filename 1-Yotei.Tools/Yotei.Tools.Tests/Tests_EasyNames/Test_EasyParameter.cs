#pragma warning disable IDE0060

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyParameter
{
    interface IFace1a { void Method1(params int?[]? items); }

    //[Enforced]
    [Fact]
    public static void Test_Params_ValueType()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace1a);
        var source = type.GetMethod("Method1")!;

        options.ParameterOptions = EasyParameterOptions.Empty; // No arguments expected
        name = source.EasyName(options);
        Assert.Equal("Method1()", name);

        options.ParameterOptions = EasyParameterOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method1(params int?[]?)", name);

        options.ParameterOptions = EasyParameterOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal("Method1(params int?[]?)", name);

        options.ParameterOptions = EasyParameterOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("Method1(params System.Nullable<System.Int32>[]? items)", name);
    }

    // ----------------------------------------------------

    interface IFace1b
    {
        void Method1(params string?[]? items);
        void Method2(params IsNullable<string?>[]? items);
    }

    //[Enforced]
    [Fact]
    public static void Test_Params_ReferenceType_ElementNullabilityLost()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace1b);
        var source = type.GetMethod("Method1")!;

        options.ParameterOptions = EasyParameterOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method1(params string[]?)", name);

        options.ParameterOptions = EasyParameterOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal("Method1(params string[]?)", name);

        options.ParameterOptions = EasyParameterOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("Method1(params System.String[]? items)", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Params_ReferenceType_ElementNullabilityWrapped()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace1b);
        var source = type.GetMethod("Method2")!;

        options.ParameterOptions = EasyParameterOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method2(params string?[]?)", name);

        options.ParameterOptions = EasyParameterOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal("Method2(params string?[]?)", name);

        options.ParameterOptions = EasyParameterOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("Method2(params Yotei.Tools.IsNullable<System.String>[]? items)", name);
    }

    // ----------------------------------------------------

    interface IFace2 { void Method(scoped Span<int> items); }

    //[Enforced]
    [Fact]
    public static void Test_Scoped_Parameters()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace2);
        var source = type.GetMethod("Method")!;

        options.ParameterOptions = EasyParameterOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method(scoped Span<int>)", name);

        options.ParameterOptions = EasyParameterOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal("Method(scoped System.Span<int>)", name);

        options.ParameterOptions = EasyParameterOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("Method(scoped System.Span<System.Int32> items)", name);
    }

    // ----------------------------------------------------

    interface IFace3 { void Method(byte one, out int? two, in string? three); }

    //[Enforced]
    [Fact]
    public static void Test_InOut_Parameters()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace3);
        var source = type.GetMethod("Method")!;

        options.ParameterOptions = EasyParameterOptions.Empty;
        name = source.EasyName(options);
        Assert.Equal("Method(,,)", name);

        options.ParameterOptions = EasyParameterOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method(byte, out int?, in string?)", name);

        options.ParameterOptions = EasyParameterOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal("Method(byte, out int?, in string?)", name);

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
    public static void Test_Ref_Parameters()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(IFace4);
        var source = type.GetMethod("Method")!;

        options.ParameterOptions = EasyParameterOptions.Empty;
        name = source.EasyName(options);
        Assert.Equal("Method(,,)", name);

        options.ParameterOptions = EasyParameterOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method(ref int?, ref string?, ref readonly long?)", name);

        options.ParameterOptions = EasyParameterOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal("Method(ref int?, ref string?, ref readonly long?)", name);

        options.ParameterOptions = EasyParameterOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            "Method(ref System.Nullable<System.Int32> one, ref System.String? two, " +
            "ref readonly System.Nullable<System.Int64> three)",
            name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_This_Parameter()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(STypeA);
        var source = type.GetMethod("Method1")!;

        options.ParameterOptions = EasyParameterOptions.Empty;
        name = source.EasyName(options);
        Assert.Equal("Method1()", name);

        options.ParameterOptions = EasyParameterOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method1(this DateTime)", name);

        options.ParameterOptions = EasyParameterOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal("Method1(this System.DateTime)", name);

        options.ParameterOptions = EasyParameterOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("Method1(this System.DateTime item)", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_NotThis_Parameter()
    {
        EasyMethodOptions options = EasyMethodOptions.Empty;
        string name;
        var type = typeof(STypeA);
        var source = type.GetMethod("Method2")!;

        options.ParameterOptions = EasyParameterOptions.Empty;
        name = source.EasyName(options);
        Assert.Equal("Method2()", name);

        options.ParameterOptions = EasyParameterOptions.Default;
        name = source.EasyName(options);
        Assert.Equal("Method2(DateTime)", name);

        options.ParameterOptions = EasyParameterOptions.DefaultEx;
        name = source.EasyName(options);
        Assert.Equal("Method2(System.DateTime)", name);

        options.ParameterOptions = EasyParameterOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("Method2(System.DateTime item)", name);
    }
}

// ========================================================
public static class STypeA
{
    public static void Method1(this DateTime item) { }
    public static void Method2(DateTime item) { }
}
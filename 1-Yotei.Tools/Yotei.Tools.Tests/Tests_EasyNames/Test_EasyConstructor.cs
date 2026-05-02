#pragma warning disable IDE0060

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyConstructor
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyConstructor";

    // ----------------------------------------------------

    class RType1a { public RType1a(ref int? one) { } }

    //[Enforced]
    [Fact]
    public static void Test_Standard_Public()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType1a);
        var source = type.GetConstructors().Single();

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("RType1a", name);

        options = EasyMethodOptions.Default with
        {
            UseAccessibility = true,
            UseModifiers = true,
            ReturnTypeOptions = EasyTypeOptions.Default
        };
        name = source.EasyName(options);
        Assert.Equal("public RType1a(ref int?)", name);

        options.UseTechName = true;
        name = source.EasyName(options);
        Assert.Equal("public RType1a.ctor(ref int?)", name);

        options = EasyMethodOptions.Full with
        {
            UseAccessibility = true,
            UseModifiers = true,
            ReturnTypeOptions = EasyTypeOptions.Full with { UseSpecialNames = false },
            ParameterOptions = EasyParameterOptions.Full with
            { TypeOptions = EasyTypeOptions.Full with { UseSpecialNames = false } },
        };
        name = source.EasyName(options);
        Assert.Equal($"public {PREFIX}.RType1a.ctor(ref System.Nullable<int> one)", name);
    }

    // ----------------------------------------------------

    class RType1b { internal RType1b(ref int? one) { } }

    //[Enforced]
    [Fact]
    public static void Test_Standard_Internal()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType1b);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var source = type.GetConstructors(flags).Single();

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("RType1b", name);

        options = EasyMethodOptions.Default with
        {
            UseAccessibility = true,
            UseModifiers = true,
            ReturnTypeOptions = EasyTypeOptions.Default
        };
        name = source.EasyName(options);
        Assert.Equal("internal RType1b(ref int?)", name);

        options.UseTechName = true;
        name = source.EasyName(options);
        Assert.Equal("internal RType1b.ctor(ref int?)", name);

        options = EasyMethodOptions.Full with
        {
            UseAccessibility = true,
            UseModifiers = true,
            ReturnTypeOptions = EasyTypeOptions.Full with { UseSpecialNames = false },
            ParameterOptions = EasyParameterOptions.Full with
            { TypeOptions = EasyTypeOptions.Full with { UseSpecialNames = false } },
        };
        name = source.EasyName(options);
        Assert.Equal($"internal {PREFIX}.RType1b.ctor(ref System.Nullable<int> one)", name);
    }

    // ----------------------------------------------------

    class RType2 { static RType2() { } }

    //[Enforced]
    [Fact]
    public static void Test_Static()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(RType2);
        var flags = BindingFlags.Static | BindingFlags.NonPublic;
        var source = type.GetConstructors(flags).Single();

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("RType2", name);

        options = EasyMethodOptions.Default with
        {
            UseAccessibility = true,
            UseModifiers = true,
            ReturnTypeOptions = EasyTypeOptions.Default
        };
        name = source.EasyName(options);
        Assert.Equal("static RType2()", name);

        options.UseTechName = true;
        name = source.EasyName(options);
        Assert.Equal("static RType2.cctor()", name);

        options = EasyMethodOptions.Full with
        {
            UseAccessibility = true,
            UseModifiers = true,
            ReturnTypeOptions = EasyTypeOptions.Full with { UseSpecialNames = false },
            ParameterOptions = EasyParameterOptions.Full with
            { TypeOptions = EasyTypeOptions.Full with { UseSpecialNames = false } },
        };
        name = source.EasyName(options);
        Assert.Equal($"static {PREFIX}.RType2.cctor()", name);
    }
}
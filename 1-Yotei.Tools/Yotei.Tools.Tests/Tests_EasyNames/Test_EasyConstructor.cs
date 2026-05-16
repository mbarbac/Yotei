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

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("RType1a(ref int?)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("RType1a(ref System.Nullable<System.Int32> one)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("public RType1a(ref System.Nullable<System.Int32> one)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"public {PREFIX}.RType1a.ctor(ref System.Nullable<System.Int32> one)",
            name);
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

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("RType1b(ref int?)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("RType1b(ref System.Nullable<System.Int32> one)", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("internal RType1b(ref System.Nullable<System.Int32> one)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"internal {PREFIX}.RType1b.ctor(ref System.Nullable<System.Int32> one)",
            name);
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

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("RType2()", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("RType2()", name);

        options = options with
        { ReturnTypeOptions = EasyTypeOptions.Default, UseAccessibility = true, UseModifiers = true };
        name = source.EasyName(options);
        Assert.Equal("static RType2()", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"static {PREFIX}.RType2.cctor()",
            name);
    }
}
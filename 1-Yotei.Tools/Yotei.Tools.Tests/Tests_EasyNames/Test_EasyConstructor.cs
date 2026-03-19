#pragma warning disable IDE0060

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyConstructor
{
    readonly static string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    readonly static string TESTHOST = "Test_EasyConstructor";

    readonly static EasyMethodOptions EMPTY = EasyMethodOptions.Empty;
    readonly static EasyMethodOptions DEFAULT = EasyMethodOptions.Default;
    readonly static EasyMethodOptions FULL = EasyMethodOptions.Full;

    // ----------------------------------------------------

    class Type0A { public Type0A(ref int? one, out string? two) { two = default!; } }

    //[Enforced]
    [Fact]
    public static void Test_Standard_RefParameterNullabilityLost()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type0A);
        var item = type.GetConstructors().First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type0A", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type0A(ref int?, out string)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            "public Type0A.ctor(ref System.Nullable<System.Int32> one, out System.String two)",
            name);
    }

    // ----------------------------------------------------

    class Type0B { internal Type0B(ref int? one, [IsNullable] out string? two) { two = default!; } }

    //[Enforced]
    [Fact]
    public static void Test_Standard_NullabilityByAttribute()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type0B);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var item = type.GetConstructors(flags).First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type0B", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type0B(ref int?, out string?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            "internal Type0B.ctor(ref System.Nullable<System.Int32> one, out System.String? two)",
            name);
    }

    // ----------------------------------------------------

    class Type0C { protected Type0C(ref int? one, out IsNullable<string?> two) { two = default!; } }

    //[Enforced]
    [Fact]
    public static void Test_Standard_NullabilityByWrapper()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type0C);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var item = type.GetConstructors(flags).First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type0C", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type0C(ref int?, out string?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            "protected Type0C.ctor" +
            "(ref System.Nullable<System.Int32> one, out Yotei.Tools.IsNullable<System.String> two)",
            name);
    }

    // ----------------------------------------------------

    class Type0D { static Type0D() { } }

    //[Enforced]
    [Fact]
    public static void Test_Static()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type0D);
        var flags = BindingFlags.Static | BindingFlags.NonPublic;
        var item = type.GetConstructors(flags).First(); Assert.NotNull(item);

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type0D", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type0D()", name);

        options = FULL;
        name = item.EasyName(options);Assert.Equal("static Type0D.cctor()", name);
    }
}
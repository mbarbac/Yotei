#pragma warning disable CA1822, IDE0060

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyConstructor
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyConstructor";

    readonly static EasyMethodOptions EMPTY = EasyMethodOptions.Empty;
    readonly static EasyMethodOptions DEFAULT = EasyMethodOptions.Default;
    readonly static EasyMethodOptions FULL = EasyMethodOptions.Full;

    // ----------------------------------------------------

    class Type0a { public Type0a(ref int? one, out string? two) { two = default!; } }

    //[Enforced]
    [Fact]
    public static void Test_Standard()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type0a);
        var item = type.GetConstructors().Single();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type0a", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type0a(ref int?, out string?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"public {PREFIX}." +
            "Type0a.ctor(ref System.Nullable<System.Int32> one, out System.String? two)",
            name);
    }

    // ----------------------------------------------------

    class Type0b { internal Type0b(ref int? one, [IsNullable] out string? two) { two = default!; } }

    //[Enforced]
    [Fact]
    public static void Test_Standard_NullabilityByAttribute()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type0b);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var item = type.GetConstructors(flags).Single();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type0b", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type0b(ref int?, out string?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"internal {PREFIX}." +
            "Type0b.ctor(ref System.Nullable<System.Int32> one, out System.String? two)",
            name);
    }

    // ----------------------------------------------------

    class Type0c { protected Type0c(ref int? one, out IsNullable<string?> two) { two = default!; } }

    //[Enforced]
    [Fact]
    public static void Test_Standard_NullabilityByWrapper()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type0c);
        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var item = type.GetConstructors(flags).Single();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type0c", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type0c(ref int?, out string?)", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"protected {PREFIX}.Type0c.ctor" +
            "(ref System.Nullable<System.Int32> one, out Yotei.Tools.IsNullable<System.String> two)",
            name);
    }

    // ----------------------------------------------------

    class Type0d<T> { static Type0d() { } }

    //[Enforced]
    [Fact]
    public static void Test_Static()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(Type0d<>);
        var flags = BindingFlags.Static | BindingFlags.NonPublic;
        var item = type.GetConstructors(flags).Single();

        options = EMPTY;
        name = item.EasyName(options); Assert.Equal("Type0d", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Type0d()", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"private static {PREFIX}.Type0d<T>.cctor()", name);
    }
}
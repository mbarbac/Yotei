#pragma warning disable CA1822, CS8500

namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyType
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyType";

    // ----------------------------------------------------

    class IFace0a<T>
    {
        public int?[]? Method1() => throw null!;
        public IsNullable<IsNullable<int>[]>? Method2() => throw null!;
    }
    // HIGH: when EasyMethod is done
    // [return: IsNullable] public int?[]? Method3() => throw null!;

    //[Enforced]
    [Fact]
    public static void Test_Array_ValueType_Nullability_Lost()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace0a<>);
        var item = type.GetMethod("Method1")!;
        var source = item.ReturnType;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Nullable[]", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("int?[]", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("int?[]", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options); Assert.Equal("System.Nullable<System.Int32>[]", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_ValueType_Nullability_Wrapped()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace0a<>);
        var item = type.GetMethod("Method2")!;
        var source = item.ReturnType;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IsNullable", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("int?[]?", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("int?[]?", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("Yotei.Tools.IsNullable<Yotei.Tools.IsNullable<System.Int32>[]>", name);
    }

    // ----------------------------------------------------

    class IFace0b<T>
    {
        public string?[]? Method1() => throw null!;
        public IsNullable<IsNullable<string>[]>? Method2() => throw null!;
    }
    // HIGH: when EasyMethod is done
    // [return: IsNullable] public string?[]? Method3() => throw null!;

    //[Enforced]
    [Fact]
    public static void Test_Array_ReferenceType_Nullability_Lost()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace0b<>);
        var item = type.GetMethod("Method1")!;
        var source = item.ReturnType;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("string[]", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("string[]", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("string[]", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options); Assert.Equal("System.String[]", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Array_ReferenceType_Nullability_Wrapped()
    {
        EasyTypeOptions options;
        string name;
        var type = typeof(IFace0b<>);
        var item = type.GetMethod("Method2")!;
        var source = item.ReturnType;

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("IsNullable", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("string?[]?", name);

        options = EasyTypeOptions.DefaultEx;
        name = source.EasyName(options); Assert.Equal("string?[]?", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("Yotei.Tools.IsNullable<Yotei.Tools.IsNullable<System.String>[]>", name);
    }
}
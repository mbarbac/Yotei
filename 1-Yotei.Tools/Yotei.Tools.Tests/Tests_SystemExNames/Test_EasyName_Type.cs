namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
////[Enforced]
public static class Test_EasyName_Type
{
    const string NAMESPACE = "Yotei.Tools.Tests.EasyNames";
    const string CLASSNAME = nameof(Test_EasyName_Type);

    readonly static EasyNameTypeOptions DEFAULT = EasyNameTypeOptions.Default;
    readonly static EasyNameTypeOptions FULL = EasyNameTypeOptions.Full;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test0_System_NotNullable()
    {
        EasyNameTypeOptions options;
        string name;
        var item = typeof(string);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Equal("", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("String", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.String", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test0_System_NullableValueType()
    {
        EasyNameTypeOptions options;
        string name;
        var item = typeof(int?);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Equal("", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Int32?", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.Nullable<System.Int32>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test0_System_NullableValueType_Explicit()
    {
        EasyNameTypeOptions options;
        string name;
        var item = typeof(Nullable<int>);

        options = DEFAULT with { HideName = true };
        name = item.EasyName(options); Assert.Equal("", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("Int32?", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.Nullable<System.Int32>", name);
    }

    // ----------------------------------------------------

    public interface IFace1A<[IsNullable] K, in T> { }

    //[Enforced]
    [Fact]
    public static void Test1_NullableAttribute_Generic_Unbound()
    {
        EasyNameTypeOptions options;
        string name;
        var item = typeof(IFace1A<,>);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace1A", name);

        options = DEFAULT with { UseGenericArguments = true };
        name = item.EasyName(options); Assert.Equal("IFace1A<K?, T>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.IFace1A<K?, in T>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test1_NullableAttribute_Generic_Bound()
    {
        EasyNameTypeOptions options;
        string name;
        var item = typeof(IFace1A<string?, int?>);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace1A", name);

        // string? not persisted, [IsNullable] not allowed
        options = DEFAULT with { UseGenericArguments = true };
        name = item.EasyName(options); Assert.Equal("IFace1A<String, Int32?>", name);

        // variance modifiers lost when using typeof(...)
        options = FULL;
        name = item.EasyName(options);
        Assert.Equal(
            $"{NAMESPACE}.{CLASSNAME}." +
            "IFace1A<System.String, System.Nullable<System.Int32>>", name);
    }

    // ----------------------------------------------------

    public interface IFace2A<T> { }
    public interface IFace2B<T> : IFace2A<T?> { }

    //[Enforced]
    [Fact]
    public static void Test2_NullableAnnotation_Generic_Unbound()
    {
        EasyNameTypeOptions options;
        string name;
        var type = typeof(IFace2B<>);
        var item = type.GetInterface("IFace2A`1")!;

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace2A", name);

        // nullable annotation lost, as if the type was a reference one
        options = DEFAULT with { UseGenericArguments = true };
        name = item.EasyName(options); Assert.Equal("IFace2A<T>", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("IFace2A<T>", name);
    }

    //[Enforced]
    [Fact]
    public static void Test2_NullableAnnotation_Generic_Bound()
    {
        EasyNameTypeOptions options;
        string name;
        var type = typeof(IFace2B<string?>);
        var item = type.GetInterface("IFace2A`1")!;

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("IFace2A", name);

        // nullable annotation not persisted
        options = DEFAULT with { UseGenericArguments = true };
        name = item.EasyName(options); Assert.Equal("IFace2A<String>", name);

        options = FULL;
        name = item.EasyName(options);
        Assert.Equal($"{NAMESPACE}.{CLASSNAME}.IFace2A<System.String>", name);
    }

    // ----------------------------------------------------

    public interface IFace3A<T> { }
    public interface IFace3B<T> : IFace3A<IsNullable<T>> { }
}
namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyType
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyType";

    readonly static EasyTypeOptions EMPTY = EasyTypeOptions.Empty;
    readonly static EasyTypeOptions DEFAULT = EasyTypeOptions.Default;
    readonly static EasyTypeOptions FULL = EasyTypeOptions.Full;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Special_ValueType()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(int);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("int", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("int", name);

        options = FULL;
        name = source.EasyName(options); Assert.Equal("int", name);

        options = FULL with { UseSpecialNames = false };
        name = source.EasyName(options); Assert.Equal("System.Int32", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Special_ValueType_Annotated()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(int?);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("int?", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("int?", name);

        options = FULL;
        name = source.EasyName(options); Assert.Equal("int?", name);

        options = FULL with { UseSpecialNames = false };
        name = source.EasyName(options); Assert.Equal("System.Nullable<System.Int32>", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Other_ValueType()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(DateTime);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("DateTime", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("DateTime", name);

        options = FULL;
        name = source.EasyName(options); Assert.Equal("System.DateTime", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Other_ValueType_Annotated()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(DateTime?);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("DateTime?", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("DateTime?", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal("System.Nullable<System.DateTime>", name);
    }

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Special_ReferenceType()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(string);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("string", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("string", name);

        options = FULL;
        name = source.EasyName(options); Assert.Equal("string", name);

        options = FULL with { UseSpecialNames = false };
        name = source.EasyName(options); Assert.Equal("System.String", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Special_ReferenceType_Nullable()
    {
        EasyTypeOptions options;
        string name;
        //var source = typeof(string?); // CS8639: typeof cannot be use on nullable reference type
        var source = typeof(IsNullable<string>);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("string?", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("string?", name);

        options = FULL;
        name = source.EasyName(options); Assert.Equal("string?", name);

        options = FULL with { UseSpecialNames = false };
        name = source.EasyName(options);
        Assert.Equal("Yotei.Tools.IsNullable<System.String>", name);
    }

    // ----------------------------------------------------

    //interface IFace1<T?> { } // CS1003: Syntax error
    interface IFace1<[IsNullable] T> { }

    //[Enforced]
    [Fact]
    public static void Test_Generic_NullableAttribute_Unbound()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(IFace1<>);

        options = EMPTY;
        name = source.EasyName(options); Assert.Equal("IFace1", name);

        options = DEFAULT;
        name = source.EasyName(options); Assert.Equal("IFace1<T?>", name);

        options = FULL;
        name = source.EasyName(options);
        Assert.Equal($"{PREFIX}.IFace1<T?>", name);
    }
}
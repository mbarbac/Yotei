namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyType
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyType";

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Special_ValueType()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(int);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Int32", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("int", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public struct System.Int32", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Special_ValueType_Annotated()
    {
        EasyTypeOptions options;
        string name;
        var source = typeof(int?);

        options = EasyTypeOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Nullable", name);

        options = EasyTypeOptions.Default;
        name = source.EasyName(options); Assert.Equal("int?", name);

        options = EasyTypeOptions.Full;
        name = source.EasyName(options);
        Assert.Equal("public struct System.Nullable<System.Int32>", name);
    }
}
namespace Yotei.Tools.Tests;

// ========================================================
//[Enforced]
public static class Test_EasyType
{
    readonly static string NAMESPACE = typeof(Test_EasyType).GetType().Namespace ?? "";
    readonly static string TESTHOST = typeof(Test_EasyType).GetType().Name;

    readonly static EasyTypeOptions EMPTY = EasyTypeOptions.Empty;
    readonly static EasyTypeOptions DEFAULT = EasyTypeOptions.Default;
    readonly static EasyTypeOptions FULL = EasyTypeOptions.Full;

    // ----------------------------------------------------

    //[Enforced]
    [Fact]
    public static void Test_Standard_ValueType()
    {
        var item = typeof(int);
        var options = EMPTY;
        var name = item.EasyName(options); Assert.Equal("Int32", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("int", name);

        options = DEFAULT with { UseNamespace = true };
        name = item.EasyName(options); Assert.Equal("int", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.Int32", name);
    }

    //[Enforced]
    [Fact]
    public static void Test_Standard_ReferenceType()
    {
        var item = typeof(string);
        var options = EMPTY;
        var name = item.EasyName(options); Assert.Equal("String", name);

        options = DEFAULT;
        name = item.EasyName(options); Assert.Equal("string", name);

        options = DEFAULT with { UseNamespace = true };
        name = item.EasyName(options); Assert.Equal("string", name);

        options = FULL;
        name = item.EasyName(options); Assert.Equal("System.String", name);
    }
}
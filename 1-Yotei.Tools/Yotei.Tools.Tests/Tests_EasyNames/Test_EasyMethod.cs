namespace Yotei.Tools.Tests.EasyNames;

// ========================================================
//[Enforced]
public static class Test_EasyMethod
{
    const string PREFIX = "Yotei.Tools.Tests.EasyNames.Test_EasyMethod";

    // ----------------------------------------------------

    interface IFace1 { void Method(int? one); }

    //[Enforced]
    [Fact]
    public static void Test_Returns_Void()
    {
        EasyMethodOptions options;
        string name;
        var type = typeof(IFace1);
        var source = type.GetMethod("Method")!;

        options = EasyMethodOptions.Empty;
        name = source.EasyName(options); Assert.Equal("Method", name);

        options = EasyMethodOptions.Default;
        name = source.EasyName(options); Assert.Equal("Method(int?)", name);

        options = options with { ParameterOptions = EasyParameterOptions.Full };
        name = source.EasyName(options);
        Assert.Equal("Method(System.Nullable<System.Int32> one)", name);

        options = EasyMethodOptions.Full;
        name = source.EasyName(options);
        Assert.Equal(
            $"System.Void {PREFIX}.IFace1.Method(System.Nullable<System.Int32> one)",
            name);
    }
}